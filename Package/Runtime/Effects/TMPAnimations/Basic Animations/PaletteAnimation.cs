using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Parameters;
using UnityEngine;
using static TMPEffects.TMPAnimations.TMPAnimationUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new PaletteAnimation",
        menuName = "TMPEffects/Animations/Basic Animations/Built-in/Palette")]
    public partial class PaletteAnimation : TMPAnimation
    {
        [SerializeField, AutoParameterBundle("")]
        [Tooltip(
            "The wave that defines the behavior of this animation. No prefix.\nFor more information about Wave, see the section on it in the documentation.")]
        Wave wave;

        [SerializeField, AutoParameterBundle("")]
        [Tooltip(
            "The way the offset for the wave is calculated.\nFor more information about Wave, see the section on it in the documentation.\nAliases: waveoffset, woffset, waveoff, woff")]
        OffsetBundle waveOffset;

        [SerializeField, AutoParameter("colors", "clrs")] [Tooltip("The colors to cycle through.\nAliases: colors, clrs")]
        Color[] colors;

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            // Evaluate the wave based on time and offset
            (float, int) result =
                d.wave.Evaluate(context.AnimatorContext.PassedTime, d.waveOffset.GetOffset(cData, context));

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
    }
}