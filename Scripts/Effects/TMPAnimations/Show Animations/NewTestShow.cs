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
        private float currentDuration;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            ReadOnlyAnimatorContext ac = context.animatorContext;

            float value = 1f;
            if (currentDuration >= 0)
            {
                value = Mathf.Lerp(1f, 0f, (ac.PassedTime - ac.StateTime(cData)) / currentDuration);
            }

            cData.SetPosition(cData.info.initialPosition + Vector3.up * value * 45f);

            if (value == 0)
            {
                if (cData.info.index == 0) Debug.Log("Finish animation test");
                context.FinishAnimation(cData.info.index);
            }
        }

        public override void ResetParameters()
        {
            currentDuration = duration;
        }

        public override void SetParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            float tmpDuration;
            if (TryGetFloatParameter("d", parameters, out tmpDuration))
            {
                currentDuration = tmpDuration;
            }
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (!HasFloatParameter("d", parameters)) return false;
            return true;
        }
    }
}
