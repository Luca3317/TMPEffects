using System.Collections.Generic;
using UnityEngine;
using TMPEffects.TextProcessing;
using TMPEffects.Components.CharacterData;
using static TMPEffects.EffectUtility;

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
            if (TryGetFloatParameter(out float val, parameters, "speed", speedAliases)) data.speed = val;
            if (TryGetFloatParameter(out val, parameters, "frequency", frequencyAliases)) data.frequency = val;
            if (TryGetFloatParameter(out val, parameters, "amplitude", amplitudeAliases)) data.amplitude = val;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;
            if (HasNonFloatParameter(parameters, "speed", speedAliases)) return false;
            if (HasNonFloatParameter(parameters, "frequency", frequencyAliases)) return false;
            if (HasNonFloatParameter(parameters, "amplitude", amplitudeAliases)) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data() { amplitude = this.amplitude, frequency = this.frequency, speed = this.speed };
        }

        private readonly string[] speedAliases = new string[] { "s", "sp" };
        private readonly string[] frequencyAliases = new string[] { "f", "fq" };
        private readonly string[] amplitudeAliases = new string[] { "a", "amp" };

        private class Data
        {
            public float speed;
            public float frequency;
            public float amplitude;
        }
    }
}