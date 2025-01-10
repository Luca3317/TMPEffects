using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using UnityEngine;
using static TMPEffects.TMPAnimations.TMPAnimationUtility;
using TMPEffects.Extensions;
using TMPEffects.Parameters;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new GrowAnimation", menuName = "TMPEffects/Animations/Basic Animations/Built-in/Grow")]
    public partial class GrowAnimation : TMPAnimation
    {
        [SerializeField, AutoParameterBundle("")]
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\n" +
                 "For more information about it, see the section on Waves in the documentation.")]
        private Wave wave = new Wave(AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 0.3f,
            0.3f, 1f, 0f, 1f);

        [SerializeField, AutoParameterBundle("")] 
        [Tooltip("The timing offsets used by this animation. No prefix.\n" +
                 "For more information about it, see the section on OffsetProviders in the documentation.")]
        private OffsetBundle offsetProvider;

        [SerializeField, AutoParameter("maxscale", "maxscl", "max")]
        [Tooltip("The maximum scale to grow to.\nAliases: maxscale, maxscl, max")]
        private float maxScale = 1.25f;

        [SerializeField, AutoParameter("minscale", "minscl", "min")]
        [Tooltip("The minimum scale to shrink to.\nAliases: minscale, minscl, min")]
        private float minScale = 1.0f;

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            // Evaluate the wave based on time and offsets
            (float, int) result = d.wave.Evaluate(context.AnimatorContext.PassedTime,
                d.offsetProvider.GetOffset(cData, context));

            // Calculate the current scale and set it
            float scale = Mathf.LerpUnclamped(d.minScale, d.maxScale, result.Item1);
            cData.SetScale(Vector3.one * scale);
        }
    }
}