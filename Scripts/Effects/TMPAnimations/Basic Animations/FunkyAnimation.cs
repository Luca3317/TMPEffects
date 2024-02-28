using System;
using System.Collections.Generic;
using TMPEffects;
using TMPEffects.TextProcessing;
using UnityEngine;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new FunkyAnimation", menuName = "TMPEffects/Animations/Funky")]
    public class FunkyAnimation : TMPAnimation
    {
        [SerializeField] float speed;
        [SerializeField] float squeezeFactor;
        [SerializeField] float amplitude;
        [SerializeField] float snapSpeed;

        [System.NonSerialized] float currentSpeed;
        [System.NonSerialized] float currentSnapSpeed;
        [System.NonSerialized] float currentAmplitude;
        [System.NonSerialized] float currentSqueezeFactor;

        public override void Animate(CharData cData, IAnimationContext context)
        {

            float t = Mathf.Sin(context.animatorContext.PassedTime * currentSpeed) / 2 + 0.5f;
            bool movingUp = Mathf.Cos(context.animatorContext.PassedTime * currentSpeed) > 0;       //(Mathf.Sin(context.AnimatorContext.passedTime + 0.0001f) / 2 + 0.5f) < t;


            var delta0 = cData.mesh.initial.vertex_TL.position - cData.mesh.initial.vertex_BL.position;
            var delta1 = cData.mesh.initial.vertex_TR.position - cData.mesh.initial.vertex_BR.position;


            Vector3 keyframe0_1 = cData.mesh.initial.GetPosition(0);
            Vector3 keyframe0_2 = cData.mesh.initial.GetPosition(3);
            keyframe0_1 = AnimationUtility.GetRawVertex(1, keyframe0_1 + delta0 * currentSqueezeFactor, cData, ref context);
            keyframe0_2 = AnimationUtility.GetRawVertex(2, keyframe0_2 + delta1 * currentSqueezeFactor, cData, ref context);

            Vector3 keyframe1_1 = cData.mesh.initial.GetPosition(1);
            Vector3 keyframe1_2 = cData.mesh.initial.GetPosition(2);
            keyframe1_1 = AnimationUtility.GetRawVertex(1, keyframe1_1, cData, ref context) + Vector3.left * currentAmplitude;
            keyframe1_2 = AnimationUtility.GetRawVertex(2, keyframe1_2, cData, ref context) + Vector3.left * currentAmplitude;

            Vector3 keyframe2_1 = cData.mesh.initial.GetPosition(1);
            Vector3 keyframe2_2 = cData.mesh.initial.GetPosition(2);
            keyframe2_1 = AnimationUtility.GetRawVertex(1, keyframe2_1, cData, ref context) + Vector3.right * currentAmplitude;
            keyframe2_2 = AnimationUtility.GetRawVertex(2, keyframe2_2, cData, ref context) + Vector3.right * currentAmplitude;

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

            cData.SetVertex(1, p1);
            cData.SetVertex(2, p2);
        }

        public override void ResetParameters()
        {
            currentSpeed = speed;
            currentSnapSpeed = snapSpeed;
            currentAmplitude = amplitude;
            currentSqueezeFactor = squeezeFactor;
        }

        public override void SetParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            foreach (KeyValuePair<string, string> kvp in parameters)
            {
                switch (kvp.Key)
                {
                    case "s":
                    case "speed":
                    case "sp":
                        ParsingUtility.StringToFloat(kvp.Value, out speed);
                        break;

                    case "sq":
                    case "squeeze":
                    case "squeezefactor":
                        ParsingUtility.StringToFloat(kvp.Value, out currentSqueezeFactor);
                        break;

                    case "a":
                    case "amp":
                    case "amplitude":
                        ParsingUtility.StringToFloat(kvp.Value, out currentAmplitude);
                        break;

                    case "sn":
                    case "snap":
                    case "snapspeed":
                        ParsingUtility.StringToFloat(kvp.Value, out currentSnapSpeed);
                        break;
                }
            }
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            foreach (KeyValuePair<string, string> kvp in parameters)
            {
                switch (kvp.Key)
                {
                    case "s":
                    case "speed":
                    case "sp":
                    case "sq":
                    case "squeeze":
                    case "a":
                    case "amp":
                    case "amplitude":
                    case "squeezefactor":
                    case "sn":
                    case "snap":
                    case "snapspeed":
                        if (!ParsingUtility.StringToFloat(kvp.Value, out _)) return false;
                        break;
                }
            }

            return true;
        }
    }
}
