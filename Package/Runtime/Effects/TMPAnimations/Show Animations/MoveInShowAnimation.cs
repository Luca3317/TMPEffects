using TMPEffects.AutoParameters.Attributes;
using TMPEffects.Components.Animator;
using TMPEffects.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.Parameters.TMPParameterTypes;
using static TMPEffects.TMPAnimations.TMPAnimationUtility;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new MoveInShowAnimation",
        menuName = "TMPEffects/Animations/Show Animations/Built-in/MoveIn")]
    public partial class MoveInShowAnimation : TMPShowAnimation
    {
        [SerializeField, AutoParameter("duration", "dur", "d")]
        [Tooltip("How long the animation will take to fully show the character.\nAliases: duration, dur, d")]
        float duration = 1f;

        [SerializeField, AutoParameter("curve", "crv", "c")]
        [Tooltip(
            "The curve used for getting the t-value to interpolate between the start and target position.\nAliases: curve, crv, c")]
        AnimationCurve curve = AnimationCurveUtility.EaseOutElastic();

        [SerializeField, AutoParameter("startposition", "startpos", "start")]
        [Tooltip("The postion to move the character in from.\nAliases: startposition, startpos, start")]
        TypedVector3 startPosition = new TypedVector3(VectorType.Offset, Vector3.one * 100);

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            IAnimatorContext ac = context.AnimatorContext;
            float t = d.duration > 0 ? Mathf.Clamp01((ac.PassedTime - ac.StateTime(cData)) / d.duration) : 1f;
            float t2 = d.curve.Evaluate(t);

            Vector3 startPos;

            switch (d.startPosition.type)
            {
                case VectorType.Position:
                    startPos = d.startPosition.vector;
                    break;
                case VectorType.Anchor:
                    startPos = AnchorToPosition(d.startPosition.vector, cData);
                    break;
                case VectorType.Offset:
                    startPos = cData.InitialPosition + d.startPosition.vector;
                    break;

                default: throw new System.NotImplementedException(nameof(d.startPosition.type));
            }

            Vector3 pos = Vector3.LerpUnclamped(startPos, cData.InitialPosition, t2);

            cData.SetPosition(pos);

            if (t == 1) context.FinishAnimation(cData);
        }
    }
}