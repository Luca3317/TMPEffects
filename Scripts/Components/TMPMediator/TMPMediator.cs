using System.Collections.Generic;
using TMPro;
using TMPEffects.TextProcessing;
using System.Collections.ObjectModel;
using TMPEffects.Components.CharacterData;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace TMPEffects.Components.Mediator
{
    /// <summary>
    /// A mediator class for TMPAnimator and TMPWriter (and potential additions, if any).<br/>
    /// Handles the pre- and postprocessing of the text, as well as maintaining information
    /// about it in the form of a CharData collection.
    /// </summary>
    public class TMPMediator
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

        public delegate void EmptyEventHandler();
        public delegate void RangeEventHandler(int start, int lenght);
        public delegate void VisibilityEventHandler(int index, VisibilityState previous);

        /// <summary>
        /// Raised when the text of the associated <see cref="TMP_Text"/> component changed.
        /// </summary>
        public event EmptyEventHandler TextChanged;
        /// <summary>
        /// Raised when the <see cref="CharData"/> property is fully populated.<br/>
        /// The property is populated whenever the text of the associated <see cref="TMP_Text"/> property changes.
        /// </summary>
        public event EmptyEventHandler CharDataPopulated;
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
            Text.ForceMeshUpdate(true, true);
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
            Debug.Log("Register 1");
            if (visibilityProcessor != null) return false;
            Debug.Log("Register 2 and shit null: " + (obj == null));
            if (obj == null) return false;
            Debug.Log("Register 3");
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
            Debug.Log("Unregister 1");
            if (visibilityProcessor != obj) return false;
            Debug.Log("Unregister 2");
            visibilityProcessor = null;
            Debug.Log("Unregister 3");
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
                            Show(i, true);
                            break;
                        case VisibilityState.Hidden:
                            Hide(i, true);
                            break;

                        default: throw new System.ArgumentException(nameof(state));
                    }
                }

                visibilityStates[i] = newState;
                VisibilityStateUpdated?.Invoke(i, previous);
            }

            if (!processor && Text.mesh != null)
                Text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
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

        private readonly List<VisibilityState> visibilityStates;
        private readonly List<CharData> charData;
        private object visibilityProcessor = null;

        private void OnTextChanged(UnityEngine.Object obj)
        { 
            if ((obj as TMP_Text) == Text)
            {
                TextChangedProcedure();
            }
        }

        private void TextChangedProcedure()
        {
            Processor.AdjustIndices();
            PopulateCharData();

            TextChanged?.Invoke();
        }

        private void SetPreprocessor()
        {
            Text.textPreprocessor = Processor;
        }

        private void PopulateCharData()
        {
            charData.Clear();
            visibilityStates.Clear();

            TMP_TextInfo info = Text.textInfo;
            CharData data;
            TMP_WordInfo? wordInfo;
            for (int i = 0; i < info.characterCount; i++)
            {
                var cInfo = info.characterInfo[i];
                wordInfo = null;

                if (cInfo.isVisible)
                {
                    for (int j = 0; j < info.wordCount; j++)
                    {
                        wordInfo = info.wordInfo[j];
                        if (wordInfo.Value.firstCharacterIndex <= i && wordInfo.Value.lastCharacterIndex >= i)
                        {
                            break;
                        }
                    }
                }

                data = wordInfo == null ? new CharData(i, cInfo, this) : new CharData(i, cInfo, this, wordInfo.Value);
                charData.Add(data);
                visibilityStates.Add(VisibilityState.Shown);
            }

            charData.TrimExcess();

            CharDataPopulated?.Invoke();
        }

        private void Hide(int startIndex, int length, bool skipHideProcess)
        {
            if (startIndex < 0 || length < 0 || startIndex + length > Text.textInfo.characterCount)
            {
                throw new System.ArgumentOutOfRangeException("Invalid input: Start = " + startIndex + "; Length = " + length + "; Length of string: " + Text.textInfo.characterCount);
            }

            for (int i = startIndex; i < startIndex + length; i++)
            {
                Hide(i, skipHideProcess);
            }
        }

        private void Show(int startIndex, int length, bool skipShowProcess)
        {
            if (startIndex < 0 || length < 0 || startIndex + length > Text.textInfo.characterCount)
            {
                throw new System.ArgumentOutOfRangeException("Invalid input: Start = " + startIndex + "; Length = " + length + "; Length of string: " + Text.textInfo.characterCount);
            }

            for (int i = startIndex; i < startIndex + length; i++)
            {
                Show(i, skipShowProcess);
            }
        }

        private void Hide(int index, bool skipHideProcess)
        {
            TMP_TextInfo info = Text.textInfo;
            CharData cData;
            TMP_CharacterInfo cInfo;
            Vector3[] verts;
            Color32[] colors;
            Vector2[] uvs0;
            Vector2[] uvs2;
            int vIndex, mIndex;

            cData = charData[index];

            if (!cData.info.isVisible) return;

            // Set the current mesh's vertices all to the initial mesh values
            for (int j = 0; j < 4; j++)
            {
                cData.SetVertex(j, Vector3.zero);// cData.info.initialPosition);
            }

            // Apply the new vertices to the vertex array
            cInfo = info.characterInfo[index];
            vIndex = cInfo.vertexIndex;
            mIndex = cInfo.materialReferenceIndex;

            colors = info.meshInfo[mIndex].colors32;
            verts = info.meshInfo[mIndex].vertices;
            uvs0 = info.meshInfo[mIndex].uvs0;
            uvs2 = info.meshInfo[mIndex].uvs2;

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = cData.mesh[j].position;
                colors[vIndex + j] = cData.mesh[j].color;
                uvs0[vIndex + j] = cData.mesh[j].uv;
                uvs2[vIndex + j] = cData.mesh[j].uv2;
            }
        }

        private void Show(int index, bool skipShowProcess)
        {
            TMP_TextInfo info = Text.textInfo;
            CharData cData;
            TMP_CharacterInfo cInfo;
            Vector3[] verts;
            Color32[] colors;
            Vector2[] uvs0;
            Vector2[] uvs2;
            int vIndex, mIndex;

            cData = charData[index];

            if (!cData.info.isVisible) return;

            // Set the current mesh's vertices all to the initial mesh values
            for (int j = 0; j < 4; j++)
            {
                cData.SetVertex(j, cData.mesh.initial.GetVertex(j));
            }

            // Apply the new vertices to the vertex array
            cInfo = info.characterInfo[index];
            vIndex = cInfo.vertexIndex;
            mIndex = cInfo.materialReferenceIndex;

            colors = info.meshInfo[mIndex].colors32;
            verts = info.meshInfo[mIndex].vertices;
            uvs0 = info.meshInfo[mIndex].uvs0;
            uvs2 = info.meshInfo[mIndex].uvs2;

            for (int j = 0; j < 4; j++)
            {
                verts[vIndex + j] = cData.mesh.initial.GetVertex(j);
                colors[vIndex + j] = cData.mesh.initial.GetColor(j);
                uvs0[vIndex + j] = cData.mesh.initial.GetUV0(j);
                uvs2[vIndex + j] = cData.mesh.initial.GetUV2(j);
            }
        }
    }
}