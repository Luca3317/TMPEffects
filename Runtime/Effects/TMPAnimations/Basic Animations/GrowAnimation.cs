using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;
using static TMPEffects.ParameterUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new GrowAnimation", menuName = "TMPEffects/Animations/Grow")]
    public class GrowAnimation : TMPAnimation
    {
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\nFor more information about Wave, see the section on it in the documentation.")]
        [SerializeField] Wave wave = new Wave(AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 0.3f, 0.3f, 1f, 0f, 1f, 0.04f);
        [Tooltip("The way the offset for the wave is calculated.\nFor more information about Wave, see the section on it in the documentation.\nAliases: waveoffset, woffset, waveoff, woff")]
        [SerializeField] WaveOffsetType waveOffsetType = WaveOffsetType.XPos;

        [Tooltip("The maximum scale to grow to.\nAliases: maxscale, maxscl, max")]
        [SerializeField] float maxScale = 1.25f;
        [Tooltip("The minimum scale to shrink to.\nAliases: minscale, minscl, min")]
        [SerializeField] float minScale = 1.0f;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.CustomData as Data;

            // Evaluate the wave based on time and offsets
            (float, int) result = d.wave.Evaluate(context.AnimatorContext.PassedTime, GetWaveOffset(cData, context, d.waveOffsetType));

            // Calculate the current scale and set it
            float scale = Mathf.LerpUnclamped(d.minScale, d.maxScale, result.Item1);
            cData.SetScale(Vector3.one * scale);
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                maxScale = this.maxScale,
                minScale = this.minScale,
                wave = this.wave,
                waveOffsetType = this.waveOffsetType
            };
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float f, parameters, "maxscale", "maxscl", "max")) d.maxScale = f;
            if (TryGetFloatParameter(out f, parameters, "minscale", "minscl", "min")) d.minScale = f;
            if (TryGetWaveOffsetParameter(out var offset, parameters, "waveoffset", WaveOffsetAliases)) d.waveOffsetType = offset;

            d.wave = CreateWave(wave, GetWaveParameters(parameters));
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "maxscale", "maxscl", "max")) return false;
            if (HasNonFloatParameter(parameters, "minScale", "minScl", "min")) return false;
            if (HasNonWaveOffsetParameter(parameters, "waveoffset", WaveOffsetAliases)) return false;

            return ValidateWaveParameters(parameters);
        }

        private class Data
        {
            public float maxScale;
            public float minScale;
            public Wave wave;
            public WaveOffsetType waveOffsetType;
        }
    }
}
