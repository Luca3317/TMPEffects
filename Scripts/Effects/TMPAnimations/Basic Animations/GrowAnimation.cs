using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;
using static TMPEffects.ParameterUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName ="new GrowAnimation", menuName ="TMPEffects/Animations/Grow")]
    public class GrowAnimation : TMPAnimation
    {
        [SerializeField] float maxScale = 1.25f;
        [SerializeField] float minScale = 1.0f;

        [SerializeField]
        Wave wave = new Wave(AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 0.3f, 0.3f, 1f, 1f, 0f, 1f, 0.04f);
        [SerializeField] WaveOffsetType waveOffsetType = WaveOffsetType.XPos;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;

            (float, int) result = d.wave.Evaluate(context.animatorContext.PassedTime, GetWaveOffset(cData, context, d.waveOffsetType));
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
            if (TryGetFloatParameter(out float f, parameters, "maxScale", "maxScl", "max")) d.maxScale = f;
            if (TryGetFloatParameter(out f, parameters, "minScale", "minScl", "min")) d.minScale = f;
            if (TryGetWaveOffsetParameter(out var offset, parameters, "waveoffset", WaveOffsetAliases)) d.waveOffsetType = offset;

            d.wave = CreateWave(wave, GetWaveParameters(parameters));
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "maxScale", "maxScl", "max")) return false;
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
