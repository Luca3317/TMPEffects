using System.Collections.Generic;
using UnityEngine;
using TMPEffects.TextProcessing;

namespace TMPEffects.Animations
{
    [CreateAssetMenu(fileName = "new WaveTMProEffect", menuName = "TMPEffects/Effects/Wave")]
    public class WaveTMPEffect : TMPAnimation
    {
        [SerializeField] private float initialSpeed;
        [SerializeField] private float initialFrequency;
        [SerializeField] private float initialAmplitude;

        private float frequency;
        private float amplitude;
        private float speed;

        public override void Animate(ref CharData cData, AnimationContext context)
        {
            float scale = cData.pointSize / 36f;

            for (int i = 0; i < 4; i++)
            {
                float xPos = (cData.initialMesh.vertex_TL.position.x + cData.initialMesh.vertex_TR.position.x) / 2;
                float yOffset = amplitude * (Mathf.Sin((context.passedTime) * speed + (xPos / (200 * (context.scaleAnimations ? scale : 1)))/*  cData.index*/ * frequency + Mathf.PI / 2) + 1) * (context.scaleAnimations ? scale : 1);
                cData.currentMesh.SetPosition(i, cData.initialMesh.GetPosition(i) + Vector3.up * yOffset);
            }
        }

        public override void SetParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null) return;

            foreach (var kvp in parameters)
            {
                switch (kvp.Key)
                {
                    case "s":
                    case "sp":
                    case "speed": ParsingUtility.StringToFloat(kvp.Value, out speed); break;

                    case "f":
                    case "fq":
                    case "frequency": ParsingUtility.StringToFloat(kvp.Value, out frequency); break;

                    case "a":
                    case "amp":
                    case "amplitude": ParsingUtility.StringToFloat(kvp.Value, out amplitude); break;
                }
            }
        }

        public override void ResetParameters()
        {
            frequency = initialFrequency;
            amplitude = initialAmplitude;
            speed = initialSpeed;
        }

        public override bool ValidateParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null)
                return true;

            foreach (var kvp in parameters)
            {
                Debug.Log("Trying to parse " + kvp.Key + " value: " + kvp.Value);
                if (!ParsingUtility.StringToFloat(kvp.Value, out _))
                {
                    Debug.Log("Will fail");
                }
                else Debug.Log("succes");
                switch (kvp.Key)
                {
                    case "s":
                    case "sp":
                    case "speed":
                    case "f":
                    case "fq":
                    case "frequency":
                    case "a":
                    case "amp":
                    case "amplitude": if (!ParsingUtility.StringToFloat(kvp.Value, out _)) return false; break;
                }
            }

            return true;
        }
    }
}