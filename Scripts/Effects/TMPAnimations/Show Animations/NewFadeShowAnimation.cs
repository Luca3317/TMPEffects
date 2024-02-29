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
        [SerializeField] float duration;

        private float currentDuration;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            ReadOnlyAnimatorContext ac = context.animatorContext;

            float value = 1f;
            if (currentDuration != 0)
            {
                value = Mathf.Lerp(0f, 1f, (ac.PassedTime - ac.StateTime(cData)) / currentDuration);
            }

            //Debug.Log("called w " + value + " t =  " + (ac.PassedTime - ac.StateTime(cData)) / currentDuration);
            if (value == 1)
            {
                if (cData.info.character == 'w')
                    Debug.LogWarning("NOW");
                // TODO Likely remove these methods from chardata
                // Probably getvisibility state too
                // Add these to animatorcontext as well
                // Because: seems weird to still have these in chardata now that decoupled mostly
                // And will prevent you from fucking with this by setting e.g. from showing to hidden.
                // then again maybe thats a neat thing to be able to do
                cData.SetVisibilityState(VisibilityState.Shown);
            }

            for (int i = 0; i < 4; i++)
            {
                Color32 color = cData.mesh.GetColor(i);
                color.a = (byte)(value * color.a);
                cData.mesh.SetColor(i, color);
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
