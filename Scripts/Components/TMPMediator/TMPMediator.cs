using System.Collections.Generic;
using TMPro;
using TMPEffects.TextProcessing;
using System.Collections.ObjectModel;
using TMPEffects.Components.CharacterData;

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
        /// List containing all the current <see cref="CharData"/>.<br/>
        /// You can rely on this never being reassigned (therefore any wrappers
        /// you may create, e.g. Collection<CharData>, can be relied on as well.
        /// </summary>
        public List<CharData> CharData { get; private set; }
        /// <summary>
        /// The <see cref="TMPTextProcessor"/> used by the associated <see cref="TMP_Text"/> component.
        /// </summary>
        public TMPTextProcessor Processor { get; private set; }
        /// <summary>
        /// The associated <see cref="TMP_Text"/> component.
        /// </summary>
        public TMP_Text Text { get; private set; }

        public delegate void EmptyEventHandler();
        public delegate void RangeEventHandler(int start, int lenght);
        public delegate void VisibilityEventHandler(int index, VisibilityState previous);
        /// <summary>
        /// Raised when the text of the associated <see cref="TMP_Text"/> component changed.
        /// </summary>
        public event EmptyEventHandler TextChanged;
        /// <summary>
        /// Raised when the <see cref="ForceUpdate(int, int)"/> method of this TMPMediator instance is called.<br/>
        /// The <see cref="ForceUpdate(int, int)"/> method serves to notify all subscribers that something changed, and they should update themselves accordingly.
        /// </summary>
        public event RangeEventHandler ForcedUpdate;
        /// <summary>
        /// Raised when the <see cref="CharData"/> property is fully populated.<br/>
        /// The property is populated whenever the text of the associated <see cref="TMP_Text"/> property changes.
        /// </summary>
        public event EmptyEventHandler CharDataPopulated;
        /// <summary>
        /// Raised when the <see cref="VisibilityStateUpdated(int, TMPEffects.CharData.VisibilityState)"/> method is called.
        /// </summary>
        public event VisibilityEventHandler OnVisibilityStateUpdated;

        /// <summary>
        /// List containing the current subscribers of this instance.
        /// TODO Remove? Why allow access to this
        /// </summary>
        public ReadOnlyCollection<object> Subscribers;
        private readonly List<object> subscribers;

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

            CharData = new List<CharData>();
            Processor = new TMPTextProcessor(Text);

            subscribers = new List<object>();
            Subscribers = new ReadOnlyCollection<object>(subscribers);

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
        /// Force an update.<br/>
        /// This notifies all subscribers of the <see cref="ForcedUpdate"/> event that something changed, and they should update themselves accordingly.
        /// </summary>
        /// <param name="start">The first index of the text of the associated <see cref="TMP_Text"/> component that the update is relevant to.</param>
        /// <param name="length">
        /// The length of the text segment of the associated <see cref="TMP_Text"/> component that the update is relevant to, starting from <paramref name="start"/>.<br/>
        /// Leave at default to include whole text.
        /// </param>
        public void ForceUpdate(int start = 0, int length = -1)
        {
            if (start < 0 || start >= CharData.Count) throw new System.ArgumentOutOfRangeException(nameof(start));

            if (length == -1)
            {
                length = CharData.Count - start;
            }

            if (length < 0 || (length - start) > CharData.Count) throw new System.ArgumentOutOfRangeException(nameof(length));

            ForcedUpdate?.Invoke(start, length);
        }

        // TODO IDK if i like e.g. animator relying on e.g. writer calling this
        /// <summary>
        /// Calling this method noifies all subscribers of the <see cref="OnVisibilityStateUpdated"/> event that the visibility state of a <see cref="CharData"/> changed.
        /// </summary>
        /// <param name="index">The index of the <see cref="CharData"/>.</param>
        /// <param name="previous">The previous <see cref="CharData.VisibilityState"/>.</param>
        public void VisibilityStateUpdated(int index, VisibilityState previous)
        {
            OnVisibilityStateUpdated?.Invoke(index, previous);
        }

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
            CharData.Clear();

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

                data = wordInfo == null ? new CharData(i, cInfo) : new CharData(i, cInfo, wordInfo.Value);
                CharData.Add(data);
            }

            CharData.TrimExcess();
            CharDataPopulated?.Invoke();
        }
    }
}