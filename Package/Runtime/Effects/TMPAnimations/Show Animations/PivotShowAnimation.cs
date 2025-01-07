using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using UnityEngine;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using static TMPEffects.Parameters.TMPParameterUtility;
using static TMPEffects.Parameters.TMPParameterTypes;
using static TMPEffects.TMPAnimations.TMPAnimationUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new PivotShowAnimation",
        menuName = "TMPEffects/Animations/Show Animations/Built-in/Pivot")]
    public partial class PivotShowAnimation : TMPShowAnimation
    {
        [SerializeField, AutoParameter("duration", "dur", "d")]
        [Tooltip("How long the animation will take to fully hide the character.\nAliases: duration, dur, d")]
        private float duration = 1f;

        [SerializeField, AutoParameter("curve", "crv", "c")]
        [Tooltip("The curve used for getting the t-value to interpolate between the angles.\nAliases: curve, crv, c")]
        private AnimationCurve curve = AnimationCurveUtility.EaseOutBack();

        [SerializeField, AutoParameter("pivot", "pv", "p")]
        [Tooltip("The pivot position of the rotation.\nAliases: pivot, pv, p")]
        private TypedVector2 pivot = new TypedVector2(VectorType.Anchor, Vector3.zero);

        [SerializeField, AutoParameter("startangle", "start")]
        [Tooltip("The start euler angles.\nAliases: startangle, start")]
        private Vector3 startAngle = new Vector3(0, 0, 210);

        [SerializeField, AutoParameter("targetangle", "target")]
        [Tooltip("The start euler angles.\nAliases: targetangle, target")]
        private Vector3 targetAngle = Vector3.zero;

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            float t = Mathf.Lerp(0, 1,
                (context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData)) / d.duration);

            if (t >= 1)
            {
                context.FinishAnimation(cData);
            }

            float t2 = d.curve.Evaluate(t);
            Vector3 angle = Vector3.LerpUnclamped(d.startAngle, d.targetAngle, t2);
            cData.AddRotation(angle, d.pivot.ToPosition(cData, context));
        }
    }
}