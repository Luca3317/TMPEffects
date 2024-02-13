using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TMPEffects.TextProcessing;
using TMPEffects.Extensions;
using System.Collections;
using System.Collections.ObjectModel;
using System;

namespace TMPEffects.Components
{
    public static class TMPMediatorManager
    {
        private static Dictionary<GameObject, ValueTuple<TMPMediator, List<object>>> mediators = new();

        public static void Subscribe(TMP_Text text, object obj)
        {
            List<object> list;
            if (!mediators.ContainsKey(text.gameObject))
            {
                Debug.Log("Created mediator for " + text.name);
                TMPMediator mediator = new TMPMediator(text);
                list = new List<object>() { obj };
                mediators.Add(text.gameObject, new ValueTuple<TMPMediator, List<object>>(mediator, list));
            }
            else
            {
                Debug.Log("Added sub for " + text.name);
                list = mediators[text.gameObject].Item2;
                if (!list.Contains(obj))
                {
                    list.Add(obj);
                }
            }
        }

        public static void Unsubscribe(TMP_Text text, object obj)
        {
            if (!mediators.TryGetValue(text.gameObject, out ValueTuple<TMPMediator, List<object>> tuple))
            {
                return;
            }

            Debug.Log("Removed sub for " + text.name);
            tuple.Item2.Remove(obj);

            if (tuple.Item2.Count == 0)
            {
                Debug.Log("Deleted mediator for " + text.name);
                mediators.Remove(text.gameObject);
            }
        }

        public static TMPMediator GetMediator(GameObject go) => mediators[go].Item1;

        public static bool TryGetMediator(GameObject go, out TMPMediator mediator)
        {
            mediator = null;
            if (!mediators.TryGetValue(go, out var tuple))
            {
                return false;
            }
            mediator = tuple.Item1;
            return true;
        }
    }

    /*
    * A mediator class for TMPAnimator and TMPWriter (and potential additions, if any).
    * Handles the pre- and postprocessing of the text, as well as maintaining information
    * about it in the form of a CharData collection.
    */
    public class TMPMediator
    {
        /// <summary>
        /// List containing all the current charData.
        /// You can rely on this never being reassigned (therefore any wrappers
        /// you may create, e.g. Collection<CharData>, can be relied on as well.
        /// </summary>
        public List<CharData> CharData { get; private set; }
        internal TMPTextProcessor Processor { get; private set; }
        public TMP_Text Text { get; private set; }

        public delegate void EmptyEventHandler();
        public delegate void RangeEventHandler(int start, int lenght);
        public delegate void VisibilityEventHandler(int index, CharData.VisibilityState previous);
        public event EmptyEventHandler TextChanged;
        public event RangeEventHandler ForcedUpdate;
        public event EmptyEventHandler CharDataPopulated;
        public event VisibilityEventHandler OnVisibilityStateUpdated;

        List<object> subscribers;
        public ReadOnlyCollection<object> Subscribers;

        public TMPMediator(TMP_Text text)
        {
            Text = text;

            CharData = new List<CharData>();
            Processor = new TMPTextProcessor(Text);

            subscribers = new List<object>();
            Subscribers = new ReadOnlyCollection<object>(subscribers);

            SetPreprocessor();
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        }

        public void ForceReprocess()
        {
            Text.ForceMeshUpdate(true, true);
        }

        public void ForceUpdate(int start, int length)
        {
            ForcedUpdate?.Invoke(start, length);
        }

        void OnTextChanged(UnityEngine.Object obj)
        {
            if ((obj as TMP_Text) == Text)
            {
                TextChangedProcedure();
            }
        }

        // TODO IDK if i like e.g. animator relying on e.g. writer calling this
        public void VisibilityStateUpdated(int index, CharData.VisibilityState previous)
        {
            OnVisibilityStateUpdated?.Invoke(index, previous);
        }

        void TextChangedProcedure()
        {
            Processor.AdjustIndices(Text.textInfo);
            PopulateCharData();

            TextChanged?.Invoke();
        }

        void SetPreprocessor()
        {
            Text.textPreprocessor = Processor;
        }

        void UnsetPreprocessor()
        {
            if (Text.textPreprocessor == Processor)
                Text.textPreprocessor = null;
        }

        void PopulateCharData()
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