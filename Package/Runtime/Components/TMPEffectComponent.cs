using TMPro;
using UnityEngine;
using TMPEffects.Components.Mediator;
using TMPEffects.CharacterData;
using UnityEngine.XR;

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
        public TMP_Text TextComponent => Mediator != null ? Mediator.Text : text != null ? text : (text = GetComponent<TMP_Text>());

        /// <summary>
        /// Set the text of the associated <see cref="TMP_Text"/> component.
        /// </summary>
        /// <param name="text">The new text.</param>
        public void SetText(string text)
        {
            if (Mediator == null) TextComponent.SetText(text);
            else Mediator.SetText(text);
        }

        /// <summary>
        /// Show a subset of the text's characters.
        /// </summary>
        /// <param name="start">First character index to show.</param>
        /// <param name="length">Amount of characters to show.</param>
        /// <param name="skipShowProcess">Whether to skip the show process.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void Show(int start, int length, bool skipShowProcess = false)
        {
            if (Mediator == null)
            {
                throw new System.InvalidOperationException("Component is not enabled!");
            }

            VisibilityState visibility = skipShowProcess ? VisibilityState.Shown : VisibilityState.Showing;
            Mediator.SetVisibilityState(start, length, visibility);
        }

        /// <summary>
        /// Hide a subset of the text's characters.
        /// </summary>
        /// <param name="start">First character index to hide.</param>
        /// <param name="length">Amount of characters to hide.</param>
        /// <param name="skipHideProcess">Whether to skip the hide process.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void Hide(int start, int length, bool skipHideProcess = false)
        {
            if (Mediator == null)
            {
                throw new System.InvalidOperationException("Component is not enabled!");
            }

            VisibilityState visibility = skipHideProcess ? VisibilityState.Hidden : VisibilityState.Hiding;
            Mediator.SetVisibilityState(start, length, visibility);
        }

        /// <summary>
        /// Show all of the text's character.
        /// </summary>
        /// <param name="skipShowProcess">Whether to skip the show process.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void ShowAll(bool skipShowProcess = false)
        {
            if (Mediator == null)
            {
                throw new System.InvalidOperationException("Component is not enabled!");
            }

            VisibilityState visibility = skipShowProcess ? VisibilityState.Shown : VisibilityState.Showing;
            Mediator.SetVisibilityState(0, Mediator.Text.textInfo.characterCount, visibility);
        }

        /// <summary>
        /// Hide all of the text's character.
        /// </summary>
        /// <param name="skipShowProcess">Whether to skip the hide process.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void HideAll(bool skipHideProcess = false)
        {
            if (Mediator == null)
            {
                throw new System.InvalidOperationException("Component is not enabled!");
            }

            VisibilityState visibility = skipHideProcess ? VisibilityState.Hidden : VisibilityState.Hiding;
            Mediator.SetVisibilityState(0, Mediator.Text.textInfo.characterCount, visibility);
        }

        [System.NonSerialized] private readonly object obj = new();
        [System.NonSerialized] private TMPMediator mediator = null;
        [System.NonSerialized] private TMP_Text text = null;

        protected TMPMediator Mediator
        {
            get
            {
                //if (mediator == null) UpdateMediator();
                return mediator;
            }
        }

        protected void FreeMediator()
        {
            TMP_Text text = Mediator.Text;
            TMPMediatorManager.Unsubscribe(text, obj);
            mediator = null;
        }

        protected void UpdateMediator()
        {
            TMP_Text text = GetComponent<TMP_Text>();
            TMPMediatorManager.Subscribe(text, obj);
            mediator = TMPMediatorManager.GetMediator(text);
        }
    }
}

