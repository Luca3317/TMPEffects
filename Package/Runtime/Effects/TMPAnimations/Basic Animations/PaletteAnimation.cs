using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Parameters;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;
using static TMPEffects.Parameters.ParameterUtility;
using static TMPEffects.Parameters.ParameterTypes;
using TMPEffects.TextProcessing;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new PaletteAnimation", menuName = "TMPEffects/Animations/Basic Animations/Built-in/Palette")]
    public class PaletteAnimation : TMPAnimation
    {
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\nFor more information about Wave, see the section on it in the documentation.")]
        [SerializeField] Wave wave;
        [Tooltip("The way the offset for the wave is calculated.\nFor more information about Wave, see the section on it in the documentation.\nAliases: waveoffset, woffset, waveoff, woff")]
        [SerializeField] WaveOffsetType waveOffset;

        [Tooltip("The colors to cycle through.\nAliases: colors, clrs")]
        [SerializeField] Color[] colors;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.CustomData as Data;

            // Evaluate the wave based on time and offset
            (float, int) result = d.wave.Evaluate(context.AnimatorContext.PassedTime, GetWaveOffset(cData, context, d.waveOffset));

            // Calculate the index to be used for the colors array
            float index = Mathf.Abs((d.colors.Length) * (d.wave.Amplitude == 0 ? 0 : result.Item1 / d.wave.Amplitude));
            int intIndex = (int)index;

            float t;
            Color color0;
            Color color1;
            Color color;

            // Handle edge case for exact trough
            if (index == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    cData.mesh.SetColor(i, d.colors[0], true);
                }

                return;
            }

            // Handle edge case for exact crest
            if (index == d.colors.Length)
            {
                for (int i = 0; i < 4; i++)
                {
                    cData.mesh.SetColor(i, d.colors[0], true);
                }

                return;
            }

            // If the wave is moving up
            if (result.Item2 == 1)
            {
                // Calculate interpolation value between the two current colors
                t = index % 1f;

                // Set the two current colors
                color0 = d.colors[intIndex];
                if (intIndex == d.colors.Length - 1)
                {
                    color1 = d.colors[0];
                }
                else
                {
                    color1 = d.colors[intIndex + 1];
                }

                // Calculate color
                color = Color.Lerp(color0, color1, t);
            }
            else if (result.Item2 == -1)
            {
                // Calculate interpolation value between the two current colors
                t = index % 1;

                // Set the two current colors
                color0 = d.colors[d.colors.Length - 1 - intIndex];
                if (intIndex == 0)
                {
                    color1 = d.colors[0];
                }
                else
                {
                    color1 = d.colors[d.colors.Length - intIndex];
                }

                // Calculate color
                color = Color.Lerp(color0, color1, 1f - t);
            }
            else throw new System.Exception("Shouldnt be possible");

            // Set the color for each vertex of the character
            // (without overriding the alpha channel)
            for (int i = 0; i < 4; i++)
            {
                cData.mesh.SetColor(i, color, true);
            }
        }

        public override object GetNewCustomData(IAnimationContext context)
        {
            return new Data()
            {
                colors = this.colors,
                wave = this.wave,
                waveOffset = this.waveOffset,
            };
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters,
            IAnimationContext context)
        {
            if (parameters == null) return;

            Data d = (Data)customData;
            
            if (TryGetArrayParameter(out var array, parameters, ParameterParsing.StringToColor, context.AnimatorContext.KeywordDatabase.ColorKeywords, "colors", "clrs")) d.colors = array;
            if (TryGetWaveOffsetParameter(out var offset, parameters, "waveoffset", WaveOffsetAliases)) d.waveOffset = offset;

            // TODO Should have some quick and easy way to do this in ParameterUtility/parsing
            // if (TryGetArrayParameter<TypedVector2>(out TypedVector2[] l, parameters,
            //         (string a, out TypedVector2 b, IDictionary<string, TypedVector2> c) =>
            //             ParameterParsing.StringToTypedVector2(a, out b, context.AnimatorContext.KeywordDatabase.Vector3Keywords, context.AnimatorContext.KeywordDatabase.AnchorKeywords),
            //         ""))
            // {
            // }
            
            if (d.colors == null || d.colors.Length == 0)
            {
                d.colors = new Color[1] { Color.black };
            }

            d.wave = CreateWave(wave, GetWaveParameters(parameters));
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters, IAnimatorContext context)
        {
            if (parameters == null) return true;

            if (HasNonWaveOffsetParameter(parameters, "waveoffset", WaveOffsetAliases)) return false;
            if (HasNonArrayParameter<Color>(parameters, ParameterParsing.StringToColor, context.KeywordDatabase.ColorKeywords,  "colors", "clrs")) return false;

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

