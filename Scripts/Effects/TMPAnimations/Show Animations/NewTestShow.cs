using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;
using UnityEngine;
using static TMPEffects.EffectUtility;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new testshwo", menuName = "new testshwo")]
    public class NewTestShow : TMPShowAnimation
    {
        public override void Animate(CharData cData, IAnimationContext context)
        {
            ReadOnlyAnimatorContext ac = context.animatorContext;
            TestData data = (TestData)context.customData;

            float value = 1f;
            if (data.duration >= 0)
            {
                value = Mathf.Lerp(1f, 0f, (ac.PassedTime - ac.StateTime(cData)) / data.duration);
            }

            cData.SetPosition(cData.info.initialPosition + Vector3.up * value * 45f);

            if (value == 0)
            {
                if (cData.info.index == 0) Debug.Log("Finish animation test");
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
