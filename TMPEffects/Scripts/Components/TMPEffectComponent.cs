using System.Collections.Generic;
using TMPEffects.Tags;
using TMPro;
using UnityEngine;

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


        public int CharacterCount => mediator.CharData.Count;
        //public string ProcessedText
        //{
        //    get
        //}



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

        protected void InsertElement<T>(List<T> list, T tag) where T : TMPEffectTag
        {
            int largerIndex = list.FindIndex(x => x.startIndex > tag.startIndex);
            if (largerIndex >= 0) list.Insert(largerIndex, tag);
            else list.Add(tag);
        }
        protected void InsertElement<T>(List<T> list, T element, System.Predicate<T> match)
        {
            int largerIndex = list.FindIndex(match);
            if (largerIndex >= 0) list.Insert(largerIndex, element);
            else list.Add(element);
        }
    }
}

