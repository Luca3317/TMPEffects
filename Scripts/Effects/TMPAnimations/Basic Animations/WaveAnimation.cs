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

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data data = (Data)context.customData;
            float xPos = (cData.mesh.initial.vertex_TL.position.x + cData.mesh.initial.vertex_TR.position.x) / 2;
            float yOffset = data.amplitude * (Mathf.Sin((context.animatorContext.PassedTime) * data.speed + (xPos / (cData.info.referenceScale / 36f)) / 200 * data.frequency + Mathf.PI / 2) + 1);
            cData.SetPosition(cData.info.initialPosition + Vector3.up * yOffset);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data data = (Data)customData;

            foreach (var kvp in parameters)
            { 
                switch (kvp.Key)
                {
                    case "s":
                    case "sp":
                    case "speed": ParsingUtility.StringToFloat(kvp.Value, out data.speed); break;

                    case "f":
                    case "fq":
                    case "frequency": ParsingUtility.StringToFloat(kvp.Value, out data.frequency); break;

                    case "a":
                    case "amp":
                    case "amplitude": ParsingUtility.StringToFloat(kvp.Value, out data.amplitude); break;
                }
            }
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

        public override object GetNewCustomData()
        {
            return new Data() { amplitude = this.amplitude, frequency = this.frequency, speed = this.speed };
        }

        private class Data
        {
            public float speed;
            public float frequency;
            public float amplitude;
        }
    }
}