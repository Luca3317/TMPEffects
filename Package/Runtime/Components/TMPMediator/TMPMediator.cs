using System.Collections.Generic;
using TMPro;
using TMPEffects.TextProcessing;
using System.Collections.ObjectModel;
using TMPEffects.CharacterData;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace TMPEffects.Components.Mediator
{
    /// <summary>
    /// A mediator class for TMPAnimator and TMPWriter (and potential additions, if any).<br/>
    /// Handles the pre- and postprocessing of the text, as well as maintaining information
    /// about it in the form of a <see cref="CharData"/> and <see cref="VisibilityState"/> collection.
    /// </summary>
    public class TMPMediator : IDisposable
    {
        /// <summary>
        /// Collection containing all the current <see cref="VisibilityState"/> states of the <see cref="CharData"/> collection.<br/>
        /// The mapping of <see cref="VisibilityState"/> to <see cref="CharData"/> is index based, i.e. the n-th <see cref="VisibilityState"/>
        /// is associated with the n-th <see cref="CharData"/>.<br/>
        /// You can rely on this never being reassigned (therefore any wrappers
        /// you may create, e.g. Collection<VisibilityData>, can be relied on as well.
        /// </summary>
        public readonly ReadOnlyCollection<VisibilityState> VisibilityStates;
        /// <summary>
        /// Collection containing all the current <see cref="CharData"/>.<br/>
        /// You can rely on this never being reassigned (therefore any wrappers
        /// you may create, e.g. Collection<CharData>, can be relied on as well.
        /// </summary>
        public readonly ReadOnlyCollection<CharData> CharData;
        /// <summary>
        /// The <see cref="TMPTextProcessor"/> used by the associated <see cref="TMP_Text"/> component.
        /// </summary>
        public readonly TMPTextProcessor Processor;
        /// <summary>
        /// The associated <see cref="TMP_Text"/> component.
        /// </summary>
        public readonly TMP_Text Text;

        public delegate void VisibilityEventHandler(int index, VisibilityState previous);
        public delegate void TextChangedEarlyEventHandler(bool textContentChanged, ReadOnlyCollection<CharData> oldCharData);
        public delegate void TextChangedLateEventHandler(bool textContentChanged, ReadOnlyCollection<CharData> oldCharData, ReadOnlyCollection<VisibilityState> oldVisibilities);

        /// <summary>
        /// Raised when the associated <see cref="TMP_Text"/> component raises its TEXT_CHANGED_EVENT, before <see cref="TextChanged_Late"/>.<br/>
        /// You should use this callback for initialization, e.g. processing tags. Don't modify data that might mess with the initialization of other
        /// listeners, such as setting visibilities or char data. For such operations, use <see cref="TextChanged_Late"/>.
        /// </summary>
        public event TextChangedEarlyEventHandler TextChanged_Early;
        /// <summary>
        /// Raised when the associated <see cref="TMP_Text"/> component raises its TEXT_CHANGED_EVENT, after <see cref="TextChanged_Early"/>.<br/>
        /// </summary>
        public event TextChangedLateEventHandler TextChanged_Late;
        /// <summary>
        /// Raised when the <see cref="VisibilityState"/> of one of the contained <see cref="CharData"/> is updated. 
        /// </summary>
        public event VisibilityEventHandler VisibilityStateUpdated;


        /// <summary>
        /// Create a new instance of TMPMediator.
        /// </summary>
        /// <param name="text">The associated <see cref="TMP_Text"/> component.</param>
        internal TMPMediator(TMP_Text text)
        {
            if (text == null)
            {
                throw new System.ArgumentNullException(nameof(text));
            }

            Text = text;

            charData = new List<CharData>();
            CharData = new ReadOnlyCollection<CharData>(charData);
            Processor = new TMPTextProcessor(Text);

            visibilityStates = new List<VisibilityState>();
            VisibilityStates = new ReadOnlyCollection<VisibilityState>(visibilityStates);

            SetPreprocessor();
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        }

        /// <summary>
        /// Forces a mesh update on the associated <see cref="TMP_Text"/> component.<br/>
        /// Will reprocess the text.
        /// </summary>
        public void ForceReprocess()
        {
            if (Text != null) Text.ForceMeshUpdate(false, true);
        }

        /// <summary>
        /// Dispose the TMPMediator instance.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                Debug.LogError("Tried to dispose TMPMediator multiple times; Bug");
                return;
            }

            UnsetPreprocessor();
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        }

        /// <summary>
        /// Register as the visibility processor of this TMPMediator.<br/>
        /// There may be at most one visibility processor registered at any given time.<br/>
        /// This makes you responsible of applying any actual changes to the mesh of
        /// the character when its associated <see cref="VisibilityState"/> is updated.<br/>
        /// Subscribe to the <see cref="VisibilityStateUpdated"/> event to get a callback whenever
        /// an <see cref="VisibilityState"/> is updated.
        /// </summary>
        /// <param name="obj">The object to identify the subscriber.</param>
        /// <returns>true if registration was successful; otherwise false.</returns>
        public bool RegisterVisibilityProcessor(object obj)
        {
            if (visibilityProcessor != null) return false;
            if (obj == null) return false;
            visibilityProcessor = obj;
            return true;
        }

        /// <summary>
        /// Unregister as the visibility processor of this TMPMediator.
        /// </summary>
        /// <param name="obj">The object to identify the subscriber.</param>
        /// <returns>true if registration was successful; otherwise false.</returns>
        public bool UnregisterVisibilityProcessor(object obj)
        {
            if (visibilityProcessor != obj) return false;
            visibilityProcessor = null;
            return true;
        }

        /// <summary>
        /// Get the associated <see cref="VisibilityState"/> of the given <see cref="CharData"/>.
        /// </summary>
        /// <remarks>
        /// This uses the index property of <see cref="CharData.info"/> to get the associated <see cref="VisibilityState"/>.<br/>
        /// Therefore, the responsiblity is on you to ensure <paramref name="cData"/> is valid, i.e. belongs to this TMPMediator instance.
        /// </remarks>
        /// <param name="cData">The <see cref="CharData"/> to get the <see cref="VisibilityState"/> of.</param>
        /// <returns>The associated <see cref="VisibilityState"/> of the given <see cref="CharData"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.NullReferenceException"></exception>
        public VisibilityState GetVisibilityState(CharData cData)
        {
            return visibilityStates[cData.info.index];
        }

        /// <summary>
        /// Set the <see cref="VisibilityState"/> associated with the <see cref="CharData"/> at the given indices.
        /// </summary>
        /// <param name="startIndex">The first index of the to-be-updated <see cref="VisibilityState"/>.</param>
        /// <param name="length">The amount of to-be-updated <see cref="VisibilityState"/>.</param>
        /// <param name="state">The <see cref="VisibilityState"/> to set to.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void SetVisibilityState(int startIndex, int length, VisibilityState state)
        {
            if (startIndex < 0 || length < 0 || startIndex + length > Text.textInfo.characterCount)
            {
                throw new System.ArgumentOutOfRangeException("Invalid input: Start = " + startIndex + "; Length = " + length + "; Length of string: " + Text.textInfo.characterCount);
            }

            VisibilityState newState = state;
            bool processor = visibilityProcessor != null;
            if (!processor)
            {
                if (state == VisibilityState.Showing) newState = VisibilityState.Shown;
                if (state == VisibilityState.Hiding) newState = VisibilityState.Hidden;
            }

            for (int i = startIndex; i < startIndex + length; i++)
            {
                VisibilityState previous = visibilityStates[i];
                if (newState == previous) continue;

                if (!processor)
                {
                    switch (newState)
                    {
                        case VisibilityState.Shown:
                            Show(i);
                            break;
                        case VisibilityState.Hidden:
                            Hide(i);
                            break;

                        default: throw new System.ArgumentException(nameof(state));
                    }
                }

                visibilityStates[i] = newState;
                VisibilityStateUpdated?.Invoke(i, previous);
            }

            if (!processor && Text.mesh != null)
            {
                Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            }
        }

        /// <summary>
        /// Set the <see cref="VisibilityState"/> associated with the given <see cref="CharData"/>.
        /// </summary>
        /// <remarks>
        /// This uses the index property of <see cref="CharData.info"/> to get the associated <see cref="VisibilityState"/>.<br/>
        /// Therefore, the responsiblity is on you to ensure <paramref name="cData"/> is valid, i.e. belongs to this TMPMediator instance.
        /// </remarks>
        /// <param name="cData">The <see cref="CharData"/> to set the associated <see cref="VisibilityState"/> of.</param>
        /// <param name="state">The <see cref="VisibilityState"/> to set to.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void SetVisibilityState(CharData cData, VisibilityState state)
            => SetVisibilityState(cData.info.index, 1, state);

        /// <summary>
        /// Set the <see cref="VisibilityState"/> associated with the <see cref="CharData"/> at the given index.
        /// </summary>
        /// <param name="index">The index of the to-be-updated <see cref="VisibilityState"/>.</param>
        /// <param name="state">The <see cref="VisibilityState"/> to set to.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void SetVisibilityState(int index, VisibilityState state)
        {
            SetVisibilityState(index, 1, state);
        }

        /// <summary>
        /// Set the text of the associated <see cref="TMP_Text"/> component.
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            settingText = true;
            Text.SetText(text);
        }

        /// <summary>
        /// Apply the mesh of the <see cref="CharData"/>.
        /// </summary>
        /// <param name="cData"></param>
        public void ApplyMesh(CharData cData)
        {
            int index = cData.info.index;
            TMP_TextInfo info = Text.textInfo;

            TMP_CharacterInfo cInfo = info.characterInfo[cData.info.index];
            int vIndex = cInfo.vertexIndex, mIndex = cInfo.materialReferenceIndex;
            Color32[] colors = info.meshInfo[mIndex].colors32;
            Vector3[] verts = info.meshInfo[mIndex].vertices;

#if UNITY_2023_2_OR_NEWER
            Vector4[] uvs0 = info.meshInfo[mIndex].uvs0;
#else
            Vector2[] uvs0 = info.meshInfo[mIndex].uvs0;
#endif

            Vector2[] uvs2 = info.meshInfo[mIndex].uvs2;

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = CharData[index].mesh.GetPosition(j);
                colors[vIndex + j] = CharData[index].mesh.GetColor(j);

#if UNITY_2023_2_OR_NEWER
                Vector4 current = uvs0[vIndex + j];
                Vector2 charUV0 = CharData[index].mesh.GetUV0(j);
                current.x = charUV0.x;
                current.y = charUV0.y;
                uvs0[vIndex + j] = current;
#else
                uvs0[vIndex + j] = CharData[index].mesh.GetUV0(j);
#endif

                uvs2[vIndex + j] = CharData[index].mesh.GetUV2(j);
            }
        }

        private readonly List<VisibilityState> visibilityStates;
        private readonly List<CharData> charData;
        private object visibilityProcessor = null;
        private bool disposed = false;
        bool settingText = false;

        private void OnTextChanged(UnityEngine.Object obj)
        {

            if (Text == null) return;

            if ((obj as TMP_Text) == Text)
            {
                TextChangedProcedure();
            }
        }

        private void TextChangedProcedure()
        {
            // Adjust the indiecs of all processed tags
            Processor.AdjustIndices();

            // Cache the old chardata before repopulating with new chardata
            var oldchardata = new ReadOnlyCollection<CharData>(new List<CharData>(charData));
            PopulateCharData();

            // Check whether there was textual changes (excluding processed tags)
            bool changed = settingText || CompareCharData(oldchardata);

            settingText = false;

            // Invoke textchanged events
            TextChanged_Early?.Invoke(changed, oldchardata);

            // Cache the old visibility states
            var oldvisibility = new ReadOnlyCollection<VisibilityState>(new List<VisibilityState>(VisibilityStates));

            // If there was no actual textual change, restore prior visibilities
            // TODO This arguably should be handled in the visibility processor, if one is present
            if (!changed)
            {
                for (int i = 0; i < oldvisibility.Count; i++)
                {
                    if (oldvisibility[i] == VisibilityState.Hidden)
                    {
                        visibilityStates[i] = VisibilityState.Shown;
                        SetVisibilityState(i, VisibilityState.Hidden);
                    }
                    else
                    {
                        visibilityStates[i] = oldvisibility[i];
                    }
                }
            }
            // Else, reset visibility states
            else
            {
                ResetVisibilityStates();
            }

            // Invoke textchanged events
            TextChanged_Late?.Invoke(changed, oldchardata, oldvisibility);
        }

        private bool CompareCharData(ReadOnlyCollection<CharData> oldData)
        {
            if (oldData.Count == CharData.Count)
            {
                for (int i = 0; i < oldData.Count; i++)
                {
                    if (oldData[i].info.character != CharData[i].info.character) return true;
                }

                return false;
            }

            return true;
        }

        private void SetPreprocessor()
        {
            Text.textPreprocessor = Processor;
        }

        private void UnsetPreprocessor()
        {
            if (Text.textPreprocessor == Processor)
                Text.textPreprocessor = null;
        }

        private void PopulateCharData()
        {
            charData.Clear();

            int wordIndex;
            TMP_TextInfo info = Text.textInfo;
            CharData data;
            TMP_WordInfo? wordInfo;
            for (int i = 0; i < info.characterCount; i++)
            {
                var cInfo = info.characterInfo[i];
                wordInfo = null;
                wordIndex = -1;

                if (cInfo.isVisible)
                {
                    for (int j = 0; j < info.wordCount; j++)
                    {
                        wordInfo = info.wordInfo[j];
                        if (wordInfo.Value.firstCharacterIndex <= i && wordInfo.Value.lastCharacterIndex >= i)
                        {
                            wordIndex = j;
                            break;
                        }
                    }
                }

                data = wordInfo == null ? new CharData(i, cInfo, wordIndex) : new CharData(i, cInfo, wordIndex, wordInfo.Value);
                charData.Add(data);
            }

            charData.TrimExcess();
        }

        private void ResetVisibilityStates()
        {
            visibilityStates.Clear();

            for (int i = 0; i < Text.textInfo.characterCount; i++)
            {
                visibilityStates.Add(VisibilityState.Shown);
            }

        }

        private void Hide(int index)
        {
            CharData cData = charData[index];

            if (!cData.info.isVisible) return;

            // Set the current mesh's vertices all to the initial mesh values
            for (int j = 0; j < 4; j++)
            {
                cData.SetVertex(j, Vector3.zero);// cData.initialPosition);
            }

            // Apply the new vertices to the vertex array
            ApplyMesh(cData);
        }

        private void Show(int index)
        {
            CharData cData = charData[index];

            if (!cData.info.isVisible) return;

            // Set the current mesh's vertices all to the initial mesh values
            for (int j = 0; j < 4; j++)
            {
                cData.SetVertex(j, cData.mesh.initial.GetPosition(j));
            }

            // Apply the new vertices to the vertex array
            ApplyMesh(cData);
        }

    }
}