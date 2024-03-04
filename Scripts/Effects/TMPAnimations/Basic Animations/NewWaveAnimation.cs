using System.Collections.Generic;
using UnityEngine;
using TMPEffects.TextProcessing;
using TMPEffects.Components.CharacterData;
using static TMPEffects.EffectUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new NewWaveAnimation", menuName = "TMPEffects/Animations/NewWave")]
    public class NewWaveAnimation : TMPAnimation
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float frequency = 0.1f;
        [SerializeField] private float amplitude = 10f;
        [SerializeField] private AnimationCurve curve = AnimationCurveUtility.EaseInOutSine();

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data data = (Data)context.customData;

            float xPos = cData.info.initialPosition.x;
            xPos /= (cData.info.referenceScale / 36f);

            float t = context.animatorContext.PassedTime * data.speed + xPos / 1000f * data.frequency;
            float t2 = Mathf.PingPong(t, 1);
            //t = Mathf.PingPong(context.animatorContext.PassedTime * data.speed + (xPos / (cData.info.referenceScale / 36f)) / 200 * data.frequency + Mathf.PI / 2, 1);
            float yOffset = data.amplitude * (curve.Evaluate(t2));
            cData.SetPosition(cData.info.initialPosition + Vector3.up * yOffset);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data data = (Data)customData;
            if (TryGetFloatParameter(out float val, parameters, "speed", speedAliases)) data.speed = val;
            if (TryGetFloatParameter(out val, parameters, "frequency", frequencyAliases)) data.frequency = val;
            if (TryGetFloatParameter(out val, parameters, "amplitude", amplitudeAliases)) data.amplitude = val;
            if (TryGetAnimCurveParameter(out AnimationCurve curve, parameters, "curve", curveAliases)) data.curve = curve;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;
            if (HasNonFloatParameter(parameters, "speed", speedAliases)) return false;
            if (HasNonFloatParameter(parameters, "frequency", frequencyAliases)) return false;
            if (HasNonFloatParameter(parameters, "amplitude", amplitudeAliases)) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", curveAliases)) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data() { amplitude = this.amplitude, frequency = this.frequency, speed = this.speed, curve = this.curve };
        }

        private readonly string[] speedAliases = new string[] { "s", "sp" };
        private readonly string[] frequencyAliases = new string[] { "f", "fq" };
        private readonly string[] amplitudeAliases = new string[] { "a", "amp" };
        private readonly string[] curveAliases = new string[] { "c", "crv" };

        private class Data
        {
            public float speed;
            public float frequency;
            public float amplitude;
            public AnimationCurve curve;
        }
    }
}