using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;
using static TMPEffects.ParameterUtility;
using TMPEffects.TextProcessing;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new PaletteAnimation", menuName = "TMPEffects/Animations/Palette")]
    public class PaletteAnimation : TMPAnimation
    {
        [SerializeField] Wave wave;
        [SerializeField] WaveOffsetType waveOffset;

        [SerializeField] Color[] colors;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;

            (float, int) result = d.wave.Evaluate(context.animatorContext.PassedTime, GetWaveOffset(cData, context, d.waveOffset));

            float index = Mathf.Abs((d.colors.Length) * (d.wave.Amplitude == 0 ? 0 : result.Item1 / d.wave.Amplitude));
            int intIndex = (int)index;

            float t;
            Color color0;
            Color color1;
            Color color;

            // Handle edge cases for exact crest / trough
            if (index == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    cData.mesh.SetColor(i, d.colors[0]);
                }

                return;
            }
            if (index == d.colors.Length)
            {
                for (int i = 0; i < 4; i++)
                {
                    cData.mesh.SetColor(i, d.colors[0]);
                }

                return;
            }

            if (result.Item2 == 1)
            {
                t = index % 1f;

                color0 = d.colors[intIndex];
                if (intIndex == d.colors.Length - 1)
                {
                    color1 = d.colors[0];
                }
                else
                {
                    color1 = d.colors[intIndex + 1];
                }

                color = Color.Lerp(color0, color1, t);
            }
            else if (result.Item2 == -1)
            {
                t = index % 1;
                color0 = d.colors[d.colors.Length - 1 - intIndex];

                if (intIndex == 0)
                {
                    color1 = d.colors[0];
                }
                else
                {
                    color1 = d.colors[d.colors.Length - intIndex];
                }

                color = Color.Lerp(color0, color1, 1f-t);
            }
            else throw new System.Exception("Shouldnt be possible");

            for (int i = 0; i < 4; i++)
            {
                cData.mesh.SetColor(i, color);
            }
        }

        public override object GetNewCustomData()
        {
            return new Data()
            {
                colors = this.colors,
                wave = this.wave,
                waveOffset = this.waveOffset,
            };
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = (Data)customData;
            if (TryGetWaveOffsetParameter(out var offset, parameters, "waveoffset", WaveOffsetAliases)) d.waveOffset = offset;
            if (TryGetArrayParameter<Color>(out var array, parameters, ParsingUtility.StringToColor, "colors", "clrs")) d.colors = array.ToArray();

            if (d.colors == null || d.colors.Length == 0)
            {
                d.colors = new Color[1] { Color.black };
            }

            d.wave = CreateWave(wave, GetWaveParameters(parameters));
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonWaveOffsetParameter(parameters, "waveoffset", WaveOffsetAliases)) return false;
            if (HasNonArrayParameter<Color>(parameters, ParsingUtility.StringToColor, "colors", "clrs")) return false;

            return ValidateWaveParameters(parameters);
        }

        private class Data
        {
            public Wave wave;
            public WaveOffsetType waveOffset;
            public Color[] colors;
        }
    }
}

