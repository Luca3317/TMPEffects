using TMPro;
using UnityEngine;
using TMPEffects.Components.Mediator;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.Components
{
    /// <summary>
    /// Base class for TMPAnimator and TMPWriter.
    /// </summary>
    public abstract class TMPEffectComponent : MonoBehaviour
    {
        /// <summary>
        /// The associated <see cref="TMP_Text"/> component.
        /// </summary>
        public TMP_Text TextComponent => Mediator.Text;

        /// <summary>
        /// Set the text of the associated <see cref="TMP_Text"/> component.
        /// </summary>
        /// <param name="text">The new text.</param>
        public void SetText(string text)
        {
            Mediator.Text.SetText(text);
        } 


        public void Show(int start, int length, bool skipShowProcess = false)
        {
            Mediator.SetVisibilityState(start, length, skipShowProcess ? VisibilityState.Shown : VisibilityState.Showing);
        }

        public void Hide(int start, int length, bool skipHideProcess = false)
        {
            Mediator.SetVisibilityState(start, length, skipHideProcess ? VisibilityState.Hidden : VisibilityState.Hiding);
        }

        [System.NonSerialized] private readonly object obj = new();
        [System.NonSerialized] private TMPMediator mediator = null;

        protected TMPMediator Mediator
        {
            get
            {
                return mediator;
            }
        }

        protected void FreeMediator()
        {
            TMP_Text text = GetComponent<TMP_Text>();
            TMPMediatorManager.Unsubscribe(text, obj);
        }

        protected void UpdateMediator()
        {
            TMP_Text text = GetComponent<TMP_Text>();
            TMPMediatorManager.Subscribe(text, obj);
            mediator = TMPMediatorManager.GetMediator(text);
        }
    }
}

