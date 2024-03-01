using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using UnityEngine;
using static TMPEffects.EffectUtility;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new FadeShowAnimation", menuName = "TMPEffects/Show Animations/Fade")]
    public class FadeShowAnimation : TMPShowAnimation
    {
        public override void Animate(CharData cData, IAnimationContext context)
        {
            var ac = context.animatorContext;
            var data = context.customData as Data;

            float t = data.duration > 0 ? (ac.PassedTime - ac.StateTime(cData)) / data.duration : 1f;
            float value = Mathf.Lerp(0f, 1f, t);

            for (int i = 0; i < 4; i++)
            {
                Color32 color = cData.mesh.GetColor(i);
                color.a = (byte)(value * color.a);
                cData.mesh.SetColor(i, color);
            }

            if (t == 1)
            {
                context.FinishAnimation(cData);
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float val, parameters, "d", durationAlias)) d.duration = val;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;
            if (HasNonFloatParameter(parameters, "d", durationAlias)) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data() { duration = this.duration };
        }

        private readonly string[] durationAlias = new string[] { "d", "dur" };

        private class Data
        {
            public float duration;
        }
    }
}
