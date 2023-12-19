using IntervalTree;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace TMPEffects.Components
{
    /// <summary>
    /// Base class for TMPAnimator and TMPWriter.
    /// </summary>
    public abstract class TMPEffectComponent : MonoBehaviour
    {
        public TMP_Text TextComponent => mediator.Text;
        public TMP_TextInfo TextInfo => mediator.Text.textInfo;

        [System.NonSerialized] internal TMPMediator mediator;

        public void SetText(string text)
        {
            mediator.Text.SetText(text);
        }

        public void UpdateMeshes(TMPro.TMP_VertexDataUpdateFlags flags = TMPro.TMP_VertexDataUpdateFlags.All)
        {
            mediator.Text.UpdateVertexData(flags);
        }

        protected void UpdateMediator()
        {
            mediator = TMPMediator.Create(gameObject);
        }
    }
}

