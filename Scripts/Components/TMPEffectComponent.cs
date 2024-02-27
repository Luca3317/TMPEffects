using TMPro;
using UnityEngine;
using TMPEffects.Components.Mediator;

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
            TMPMediatorManager.Unsubscribe(GetComponent<TMP_Text>(), obj);
            mediator = null;
        }

        protected void UpdateMediator()
        {
            if (mediator == null)
            {
                TMPMediatorManager.Subscribe(GetComponent<TMP_Text>(), obj);
                mediator = TMPMediatorManager.GetMediator(gameObject);
            }
        }
    }
}

