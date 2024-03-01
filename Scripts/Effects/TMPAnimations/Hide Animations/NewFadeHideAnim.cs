using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.Components.CharacterData;
using TMPEffects.TextProcessing;
using UnityEngine;
using static TMPEffects.EffectUtility;
namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new fade hide", menuName = "new fade hide")]
    public class NewFadeHideAnim : TMPHideAnimation
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

            Debug.Log("for " + cData.info.index + $"; ({ac.PassedTime} - {ac.StateTime(cData)}) / {currentDuration} = {(ac.PassedTime - ac.StateTime(cData)) / currentDuration}");

            for (int i = 0; i < 4; i++)
            {
                Color32 color = cData.mesh.GetColor(i);
                color.a = (byte)(value * color.a);
                cData.mesh.SetColor(i, color);

                //cData.SetPosition(cData.info.initialPosition + Vector3.up * value * 25);
            }

            if (value == 0)
            {
                Debug.Log($"Finishing animation for {cData.info.character} at {cData.info.index}");
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
