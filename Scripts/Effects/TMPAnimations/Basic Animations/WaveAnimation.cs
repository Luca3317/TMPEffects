using System.Collections.Generic;
using UnityEngine;
using TMPEffects.TextProcessing;
using TMPEffects.Components.CharacterData;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new WaveAnimation", menuName = "TMPEffects/Animations/Wave")]
    public class WaveAnimation : TMPAnimation
    {
        [SerializeField] private float speed;
        [SerializeField] private float frequency;
        [SerializeField] private float amplitude;

        private float currentFrequency;
        private float currentAmplitude;
        private float currentSpeed;

        public override void Animate(ref CharData cData, IAnimationContext context)
        {
            float xPos = (cData.mesh.initial.vertex_TL.position.x + cData.mesh.initial.vertex_TR.position.x) / 2;
            float yOffset = currentAmplitude * (Mathf.Sin((context.animatorContext.PassedTime) * currentSpeed +/* cData.index*/ (xPos / (cData.info.referenceScale /*(cData.ascender - cData.descender)*/ / 36f)) / 200 * currentFrequency + Mathf.PI / 2) + 1) * (/*context.scaleAnimations ? scale :*/ 1);
            cData.SetPosition(cData.info.initialPosition + Vector3.up * yOffset);
        }

        public override void SetParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            foreach (var kvp in parameters)
            { 
                switch (kvp.Key)
                {
                    case "s":
                    case "sp":
                    case "speed": ParsingUtility.StringToFloat(kvp.Value, out currentSpeed); break;

                    case "f":
                    case "fq":
                    case "frequency": ParsingUtility.StringToFloat(kvp.Value, out currentFrequency); break;

                    case "a":
                    case "amp":
                    case "amplitude": ParsingUtility.StringToFloat(kvp.Value, out currentAmplitude); break;
                }
            }
        }

        public override void ResetParameters()
        { 
            currentFrequency = frequency;
            currentAmplitude = amplitude;
            currentSpeed = speed;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null)
                return true;

            foreach (var kvp in parameters)
            {
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
                    case "amplitude":
                        if (!ParsingUtility.StringToFloat(kvp.Value, out _)) return false;
                        break;
                }
            }

            return true;
        }
    }
}