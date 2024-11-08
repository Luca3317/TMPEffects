using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [CreateAssetMenu(fileName = "new CurveShowAnimation", menuName = "TMPEffects/Animations/Show Animations/Curve")]
    public class CurveShowAnimation : TMPShowAnimation
    {
        public TMPAnimation animation;
        public AnimationCurve curve;
        public float duration;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            if (animation == null) return;

            float passed = context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData);
            float t = curve.Evaluate(passed / duration);
            if (passed > duration)
            {
                context.FinishAnimation(cData);
                return;
            }

            animation.Animate(cData, context);
            CharDataModifiers.LerpCharacterModifiersUnclamped(cData, cData._CharacterModifiers, 1 - t,
                cData._CharacterModifiers);
            CharDataModifiers.LerpMeshModifiersUnclamped(cData, cData.MeshModifiers, 1 - t, cData.MeshModifiers);
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters, IAnimatorContext context)
        {
            if (animation == null) return false;
            return animation.ValidateParameters(parameters, context);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters,
            IAnimationContext context)
        {
            if (animation == null) return;
            animation.SetParameters(customData, parameters, context);
        }

        public override object GetNewCustomData(IAnimationContext context)
        {
            if (animation == null) return null;
            return animation.GetNewCustomData(context);
        }
    }
}