using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.Components.Animator;
using TMPEffects.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.Parameters.TMPParameterUtility;
using static TMPEffects.TMPAnimations.TMPAnimationUtility;

namespace TMPEffects.TMPAnimations.ShowAnimations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new FadeShowAnimation",
        menuName = "TMPEffects/Animations/Show Animations/Built-in/Fade")]
    public partial class FadeShowAnimation : TMPShowAnimation
    {
        [SerializeField, AutoParameter("duration", "dur", "d")]
        [Tooltip("How long the animation will take to fully show the character.\nAliases: duration, dur, d")]
        float duration = 1;

        [SerializeField, AutoParameter("curve", "crv", "c")]
        [Tooltip("The curve used for fading in.\nAliases: curve, crv, c")] 
        AnimationCurve curve = AnimationCurveUtility.EaseInSine();

        [SerializeField, AutoParameter("startopacity", "startop", "start")]
        [Tooltip("The opacity that is faded in from.\nAliases: startopacity, startop, start")]
        float startOpacity = 0;

        [SerializeField, AutoParameter("anchor", "anc", "a")]
        [Tooltip("The anchor that is faded in from.\nAliases: anchor, anc, a")]
        Vector3 anchor = Vector3.zero;

        [SerializeField, AutoParameter("direction", "dir")]
        [Tooltip("The direction used for fading in.\nAliases: direction, dir")] 
        Vector3 direction = Vector3.up;

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            IAnimatorContext ac = context.AnimatorContext;

            float t = d.duration > 0 ? Mathf.Clamp01((ac.PassedTime - ac.StateTime(cData)) / d.duration) : 1f;
            float t2 = d.curve.Evaluate(t);

            if (t == 1) context.FinishAnimation(cData);

            FadeIn(cData, context, d, t2);
        }

        private void FadeIn(CharData cData, IAnimationContext context, AutoParametersData d, float t)
        {
            Vector2 anchor = d.anchor;
            FixAnchor(ref anchor);

            if (anchor == Vector2.zero)
            {
                for (int i = 0; i < 4; i++)
                {
                    float eval = Mathf.Lerp(d.startOpacity, cData.info.color.a, t);

                    Color32 c;
                    c = cData.mesh.initial.GetColor(i);
                    c.a = (byte)((eval / 255f) * c.a);

                    cData.mesh.SetAlpha(i, (byte)((eval / 255f) * c.a));
                }

                return;
            }

            Vector2 direction = new Vector2(-anchor.x, -anchor.y);
            Vector2 opposite = direction;

            Vector3 anchorPosition = AnchorToPosition(anchor, cData);
            Vector3 oppositePosition = AnchorToPosition(opposite, cData);
            float dist = (anchorPosition - oppositePosition).magnitude;

            for (int i = 0; i < 4; i++)
            {
                Vector3 vec = cData.mesh.initial.GetPosition(i) - anchorPosition;
                vec.x *= direction.x;
                vec.y *= direction.y;

                float currDist = (vec.magnitude / dist);

                float eval = Mathf.Lerp(d.startOpacity, cData.info.color.a, t * (2 - currDist));

                Color32 c;
                c = cData.mesh.initial.GetColor(i);
                c.a = (byte)((eval / 255f) * c.a);

                cData.mesh.SetAlpha(i, (byte)((eval / 255f) * c.a));
            }
        }

        private void FixAnchor(ref Vector2 v)
        {
            if (v.x != 0)
            {
                if (v.x > 0) v.x = 1;
                else v.x = -1;
            }

            if (v.y != 0)
            {
                if (v.y > 0) v.y = 1;
                else v.y = -1;
            }
        }
    }
}