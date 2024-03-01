using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;
using TMPEffects.TextProcessing;
using UnityEngine;
using static TMPEffects.EffectUtility;
namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new fade", menuName = "new fade")]
    public class NewFadeShowAnimation : TMPShowAnimation
    {
        public override void Animate(CharData cData, IAnimationContext context)
        {
            ReadOnlyAnimatorContext ac = context.animatorContext;
            TestData td = (TestData)context.customData;

            float value = 1f;
            if (td.duration >= 0)
            {
                value = Mathf.Lerp(0f, 1f, (ac.PassedTime - ac.StateTime(cData)) / td.duration);
            }

            //Debug.Log("for " + cData.info.index + $"; ({ac.PassedTime} - {ac.StateTime(cData)}) / {currentDuration} = {(ac.PassedTime - ac.StateTime(cData)) / currentDuration}");

            for (int i = 0; i < 4; i++)
            {
                Color32 color = cData.mesh.GetColor(i);
                color.a = (byte)(value * color.a);
                cData.mesh.SetColor(i, color);
            }

            if (value == 1)
            {
                if (cData.info.index == 0) Debug.Log("Finish animation fade");
                context.FinishAnimation(cData.info.index);
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            TestData td = (TestData)customData;

            float tmpDuration;
            if (TryGetFloatParameter("d", parameters, out tmpDuration))
            {
                td.duration = tmpDuration;
            }
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (!HasFloatParameter("d", parameters)) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new TestData() { duration = this.duration };
        }

        private class TestData
        {
            public float duration;
        }
    }
}
