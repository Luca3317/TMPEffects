using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using UnityEngine;
using static TMPEffects.TMPAnimations.TMPAnimationUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new FunkyAnimation", menuName = "TMPEffects/Animations/Basic Animations/Built-in/Funky")]
    public partial class FunkyAnimation : TMPAnimation
    {
        [SerializeField, AutoParameter("speed", "sp", "s")] 
        [Tooltip("The speed at which the animation plays.\nAliases: speed, sp, s")]
        float speed;
        [SerializeField, AutoParameter("squeezefactor", "squeeze", "sqz")] 
        [Tooltip("The percentage of its original size the text is squeezed to.\nAliases: squeezefactor, squeeze, sqz")]
        float squeezeFactor;
        [SerializeField, AutoParameter("amplitude", "amp")] 
        [Tooltip("The amplitude the text pushes to the left / right.\nAliases: amplitude, amp")]
        float amplitude;

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            float t = Mathf.Sin(context.AnimatorContext.PassedTime * d.speed) / 2 + 0.5f;
            bool movingUp = Mathf.Cos(context.AnimatorContext.PassedTime * d.speed) > 0;

            var delta0 = cData.InitialMesh.TL_Position - cData.mesh.initial.BL_Position;
            var delta1 = cData.InitialMesh.TR_Position - cData.mesh.initial.BR_Position;

            Vector3 keyframe0_1 = cData.mesh.initial.GetPosition(0);
            Vector3 keyframe0_2 = cData.mesh.initial.GetPosition(3);
            keyframe0_1 = GetRawVertex(1, keyframe0_1 + delta0 * d.squeezeFactor, cData, context);
            keyframe0_2 = GetRawVertex(2, keyframe0_2 + delta1 * d.squeezeFactor, cData, context);

            Vector3 keyframe1_1 = cData.mesh.initial.GetPosition(1);
            Vector3 keyframe1_2 = cData.mesh.initial.GetPosition(2);
            keyframe1_1 = GetRawVertex(1, keyframe1_1, cData, context) + Vector3.left * d.amplitude;
            keyframe1_2 = GetRawVertex(2, keyframe1_2, cData, context) + Vector3.left * d.amplitude;

            Vector3 keyframe2_1 = cData.mesh.initial.GetPosition(1);
            Vector3 keyframe2_2 = cData.mesh.initial.GetPosition(2);
            keyframe2_1 = GetRawVertex(1, keyframe2_1, cData, context) + Vector3.right * d.amplitude;
            keyframe2_2 = GetRawVertex(2, keyframe2_2, cData, context) + Vector3.right * d.amplitude;

            Vector3 p1 = cData.mesh.initial.GetPosition(1), p2 = cData.mesh.initial.GetPosition(2);
            if (movingUp)
            {
                if (t <= 0.9)
                { 
                    p2 = Vector3.Lerp(keyframe1_2, keyframe0_2, t / 0.9f);
                    p1 = Vector3.Lerp(keyframe1_1, keyframe0_1, t / 0.9f);
                }
                else
                {
                    p1 = Vector3.Lerp(keyframe0_1, keyframe2_1, (t - 0.9f) / 0.1f);
                    p2 = Vector3.Lerp(keyframe0_2, keyframe2_2, (t - 0.9f) / 0.1f);
                }
            }

            else
            {
                if (t >= 0.1)
                {
                    p1 = Vector3.Lerp(keyframe0_1, keyframe2_1, (t - 0.1f) / 0.9f);
                    p2 = Vector3.Lerp(keyframe0_2, keyframe2_2, (t - 0.1f) / 0.9f);
                }
                else
                {
                    p1 = Vector3.Lerp(keyframe1_1, keyframe0_1, t / 0.1f);
                    p2 = Vector3.Lerp(keyframe1_2, keyframe0_2, t / 0.1f);
                }
            }

            cData.mesh.SetPosition(1, p1);
            cData.mesh.SetPosition(2, p2);
        }
    }
}
