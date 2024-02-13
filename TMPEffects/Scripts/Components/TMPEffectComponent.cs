using TMPro;
using UnityEngine;

namespace TMPEffects.Components
{
    /// <summary>
    /// Base class for TMPAnimator and TMPWriter.
    /// </summary>
    public abstract class TMPEffectComponent : MonoBehaviour
    {
        public TMP_Text TextComponent => Mediator.Text;
        public TMP_TextInfo TextInfo => Mediator.Text.textInfo;

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

        public int CharacterCount => Mediator.CharData.Count;

        public void SetText(string text)
        {
            Mediator.Text.SetText(text);
        }

        public void UpdateMeshes(TMPro.TMP_VertexDataUpdateFlags flags = TMPro.TMP_VertexDataUpdateFlags.All)
        {
            Mediator.Text.UpdateVertexData(flags);
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

