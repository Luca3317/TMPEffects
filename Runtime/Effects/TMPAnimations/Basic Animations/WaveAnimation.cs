using System.Collections.Generic;
using UnityEngine;
using TMPEffects.CharacterData;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new WaveAnimation", menuName = "TMPEffects/Animations/Wave")]
    public class WaveAnimation : TMPAnimation
    {
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\nFor more information about Wave, see the section on it in the documentation.")]
        [SerializeField] Wave wave = new Wave(AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 0.5f, 0.5f, 1f, 1f, 0.2f);
        [Tooltip("The way the offset for the wave is calculated.\nFor more information about Wave, see the section on it in the documentation.\nAliases: waveoffset, woffset, waveoff, woff")]
        [SerializeField] WaveOffsetType waveOffsetType = WaveOffsetType.XPos;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data data = (Data)context.CustomData;

            // Evaluate the wave based on time and offset
            float eval = data.wave.Evaluate(context.AnimatorContext.PassedTime, GetWaveOffset(cData, context, data.waveOffsetType)).Item1;

            // Move the character up based on the wave evaluation
            cData.SetPosition(cData.InitialPosition + Vector3.up * eval);
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data data = (Data)customData;
            if (TryGetWaveOffsetParameter(out var wot, parameters, "waveoffset", WaveOffsetAliases)) data.waveOffsetType = wot;
            data.wave = CreateWave(this.wave, GetWaveParameters(parameters));
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;
            if (HasNonWaveOffsetParameter(parameters, "waveoffset", WaveOffsetAliases)) return false;
            return ValidateWaveParameters(parameters);
        }

        public override object GetNewCustomData()
        {
            return new Data() { wave = this.wave, waveOffsetType = this.waveOffsetType };
        }

        private class Data
        {
            public Wave wave;
            public WaveOffsetType waveOffsetType;
        }
    }
}