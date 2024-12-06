using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using UnityEngine;
using static TMPEffects.TMPAnimations.TMPAnimationUtility;
using static TMPEffects.Parameters.TMPParameterUtility;
using static TMPEffects.Parameters.TMPParameterTypes;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.HideAnimations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new SpreadHideAnimation",
        menuName = "TMPEffects/Animations/Hide Animations/Built-in/Spread")]
    public partial class SpreadHideAnimation : TMPHideAnimation
    {
        [SerializeField, AutoParameter("duration", "dur", "d")]
        [Tooltip("How long the animation will take to fully hide the character.\nAliases: duration, dur, d")]
        float duration = 1;

        [SerializeField, AutoParameter("curve", "crv", "c")]
        [Tooltip(
            "The curve used for getting the t-value to interpolate between the percentages.\nAliases: curve, crv, c")]
        AnimationCurve curve = AnimationCurveUtility.EaseOutElastic();

        [SerializeField, AutoParameter("anchor", "anc", "a")]
        [Tooltip("The anchor from where the character spreads.\nAliases: anchor, anc, a")]
        TypedVector2 anchor = new TypedVector2(VectorType.Anchor, Vector2.zero);

        [SerializeField, AutoParameter("direction", "dir")]
        [Tooltip("The direction in which the character spreads.\nAliases: direction, dir")]
        Vector3 direction = Vector3.up;

        [SerializeField, AutoParameter("startpercentage", "start")]
        [Tooltip("The start percentage of the spread, 0 being fully hidden.\nAliases: startpercentage, start")]
        float startPercentage = 1;

        [SerializeField, AutoParameter("targetpercentage", "target")]
        [Tooltip("The target percentage of the spread, 1 being fully shown.\nAliases: targetpercentage, target")]
        float targetPercentage = 0;

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            float t = Mathf.Lerp(d.startPercentage, d.targetPercentage,
                (context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData)) / d.duration);
            float t2 = d.curve.Evaluate(1 - t);

            float l = Mathf.Lerp(0f, 1f,
                (context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData)) / d.duration);
            if (l >= 1)
                context.FinishAnimation(cData);

            Grow(cData, context, d, t2);
        }

        private void Grow(CharData cData, IAnimationContext context, AutoParametersData d, float t)
        {
            float percentage = Mathf.LerpUnclamped(d.startPercentage, d.targetPercentage, t);

            Vector2 actualDir = new Vector2(-d.direction.y, d.direction.x);

            Vector3 lineStart, lineEnd;

            switch (d.anchor.type)
            {
                case VectorType.Offset:
                    lineStart = cData.InitialPosition + (Vector3)(d.anchor.vector - actualDir * 2);
                    lineEnd = cData.InitialPosition + (Vector3)(d.anchor.vector + actualDir * 2);
                    break;
                case VectorType.Anchor:
                    lineStart = AnchorToPosition(d.anchor.vector - actualDir * 2, cData);
                    lineEnd = AnchorToPosition(d.anchor.vector + actualDir * 2, cData);
                    break;
                case VectorType.Position:
                    lineStart = d.anchor.vector - actualDir * 2;
                    lineEnd = d.anchor.vector + actualDir * 2;
                    break;

                default: throw new System.NotImplementedException(nameof(anchor.type));
            }

            for (int i = 0; i < 4; i++)
            {
                Vector3 startPos = ClosestPointOnLine(lineStart, lineEnd, cData.mesh.initial.GetPosition(i));
                Vector3 pos = Vector3.LerpUnclamped(startPos, cData.mesh.initial.GetPosition(i), percentage);

                SetVertexRaw(i, pos, cData, context);
            }
        }
    }
}