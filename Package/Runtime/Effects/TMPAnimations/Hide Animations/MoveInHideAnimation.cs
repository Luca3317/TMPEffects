using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.Components.Animator;
using TMPEffects.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.Parameters.TMPParameterUtility;
using static TMPEffects.Parameters.TMPParameterTypes;
using static TMPEffects.TMPAnimations.TMPAnimationUtility;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new MoveInHideAnimation",
        menuName = "TMPEffects/Animations/Hide Animations/Built-in/MoveIn")]
    public partial class MoveInHideAnimation : TMPHideAnimation
    {
        [SerializeField, AutoParameter("duration", "dur", "d")]
        [Tooltip("How long the animation will take to fully hide the character.\nAliases: duration, dur, d")]
        float duration = 1f;

        [SerializeField, AutoParameter("curve", "crv", "c")]
        [Tooltip(
            "The curve used for getting the t-value to interpolate between the start and target position.\nAliases: curve, crv, c")]
        AnimationCurve curve = AnimationCurveUtility.EaseInBack();

        [SerializeField, AutoParameter("targetposition", "targetpos", "target")]
        [Tooltip("The postion to move the character to.\nAliases: targetposition, targetpos, target")] 
        TypedVector3 targetPosition = new TypedVector3(VectorType.Offset, new Vector3(0, 1250, 0));

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            IAnimatorContext ac = context.AnimatorContext;

            float t = d.duration > 0 ? Mathf.Clamp01((ac.PassedTime - ac.StateTime(cData)) / d.duration) : 1f;
            float t2 = d.curve.Evaluate(t);          
            
            if (t >= 1)
            {
                context.FinishAnimation(cData);
                return;
            }

            Vector3 targetPos;

            switch (d.targetPosition.type)
            {
                case VectorType.Position:
                    targetPos = d.targetPosition.vector;
                    break;
                case VectorType.Anchor:
                    targetPos = AnchorToPosition(d.targetPosition.vector, cData);
                    break;
                case VectorType.Offset:
                    targetPos = cData.InitialPosition + d.targetPosition.vector;
                    break;

                default: throw new System.NotImplementedException(nameof(d.targetPosition.type));
            }

            Vector3 pos = Vector3.LerpUnclamped(cData.InitialPosition, targetPos, t2);

            cData.SetPosition(pos);
        }
    }
}