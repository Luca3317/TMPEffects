using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using TMPEffects.Extensions;
using UnityEngine;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new FunkyAnimation", menuName = "TMPEffects/Animations/Funky")]
    public class FunkyAnimation : TMPAnimation
    {
        [SerializeField] float speed;
        [SerializeField] float squeezeFactor;
        [SerializeField] float amplitude;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;

            float t = Mathf.Sin(context.animatorContext.PassedTime * d.speed) / 2 + 0.5f;
            bool movingUp = Mathf.Cos(context.animatorContext.PassedTime * d.speed) > 0;

            var delta0 = cData.mesh.initial.vertex_TL.position - cData.mesh.initial.vertex_BL.position;
            var delta1 = cData.mesh.initial.vertex_TR.position - cData.mesh.initial.vertex_BR.position;

            Vector3 keyframe0_1 = cData.mesh.initial.GetVertex(0);
            Vector3 keyframe0_2 = cData.mesh.initial.GetVertex(3);
            keyframe0_1 = GetRawVertex(1, keyframe0_1 + delta0 * d.squeezeFactor, cData, ref context);
            keyframe0_2 = GetRawVertex(2, keyframe0_2 + delta1 * d.squeezeFactor, cData, ref context);

            Vector3 keyframe1_1 = cData.mesh.initial.GetVertex(1);
            Vector3 keyframe1_2 = cData.mesh.initial.GetVertex(2);
            keyframe1_1 = GetRawVertex(1, keyframe1_1, cData, ref context) + Vector3.left * d.amplitude;
            keyframe1_2 = GetRawVertex(2, keyframe1_2, cData, ref context) + Vector3.left * d.amplitude;

            Vector3 keyframe2_1 = cData.mesh.initial.GetVertex(1);
            Vector3 keyframe2_2 = cData.mesh.initial.GetVertex(2);
            keyframe2_1 = GetRawVertex(1, keyframe2_1, cData, ref context) + Vector3.right * d.amplitude;
            keyframe2_2 = GetRawVertex(2, keyframe2_2, cData, ref context) + Vector3.right * d.amplitude;

            Vector3 p1 = cData.mesh.initial.GetVertex(1), p2 = cData.mesh.initial.GetVertex(2);
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

            cData.SetVertex(1, p1);
            cData.SetVertex(2, p2);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "speed", SpeedAliases)) d.speed = f;
            if (TryGetFloatParameter(out f, parameters, "amplitude", AmplitudeAliases)) d.amplitude = f;
            if (TryGetFloatParameter(out f, parameters, "squeezeFactor", "squeeze", "sqz")) d.squeezeFactor = f;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "speed", SpeedAliases)) return false;
            if (HasNonFloatParameter(parameters, "amplitude", AmplitudeAliases)) return false;
            if (HasNonFloatParameter(parameters, "squeezeFactor", "squeeze", "sqz")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                amplitude = this.amplitude,
                speed = this.speed,
                squeezeFactor = this.squeezeFactor
            };
        }

        private class Data
        {
            public float speed;
            public float squeezeFactor;
            public float amplitude;
        }
    }
}
