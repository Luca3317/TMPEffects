using System.Collections;
using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;
using static TMPEffects.Parameters.ParameterUtility;
using static TMPEffects.Parameters.ParameterTypes;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new GrowAnimation", menuName = "TMPEffects/Animations/Basic Animations/Built-in/Grow")]
    public partial class GrowAnimation : TMPAnimation
    {
        [SerializeField, AutoParameterBundle("")] 
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\nFor more information about Wave, see the section on it in the documentation.")]
        Wave wave = new Wave(AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 0.3f, 0.3f, 1f, 0f, 1f);
        
        [SerializeField, AutoParameter("waveoffset", "woffset", "waveoff", "woff")]
        [Tooltip("The way the offset for the wave is calculated.\nFor more information about Wave, see the section on it in the documentation.\nAliases: waveoffset, woffset, waveoff, woff")]
        OffsetTypePowerEnum waveOffsetType = OffsetType.XPos;

        [SerializeField, AutoParameter("maxscale", "maxscl", "max")]
        [Tooltip("The maximum scale to grow to.\nAliases: maxscale, maxscl, max")]
        float maxScale = 1.25f;
        [SerializeField, AutoParameter("minscale", "minscl", "min")]
        [Tooltip("The minimum scale to shrink to.\nAliases: minscale, minscl, min")]
        float minScale = 1.0f;
        
        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            // Evaluate the wave based on time and offsets
            (float, int) result = d.wave.Evaluate(context.AnimatorContext.PassedTime, d.waveOffsetType.GetOffset(cData, context));

            // Calculate the current scale and set it
            float scale = Mathf.LerpUnclamped(d.minScale, d.maxScale, result.Item1);
            cData.SetScale(Vector3.one * scale);
        }
    }
}
