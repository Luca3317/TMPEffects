using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using UnityEngine;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using static TMPEffects.Parameters.ParameterUtility;
using static TMPEffects.Parameters.ParameterTypes;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Extensions;
using TMPEffects.Parameters;
using TMPro;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new WaveAnimation", menuName = "TMPEffects/Animations/Basic Animations/Built-in/Wave")]
    [AutoParameters]
    public partial class WaveAnimation : TMPAnimation
    {
        [SerializeField]
        [AutoParameterBundle("")]
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\n" +
                 "For more information about Wave, see the section on it in the documentation.")]
        Wave wave = new Wave(AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 0.5f, 0.5f,
            1f, 1f, 0.2f);

        [SerializeField]
        [AutoParameter("waveoffset")]
        [Tooltip(
            "The way the offset for the wave is calculated.\n" +
            "For more information about Wave, see the section on it in the documentation.\nAliases: waveoffset, woffset, waveoff, woff")]
        OffsetTypePowerEnum waveOffsetType = OffsetType.XPos;
        
        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {
            // Evaluate the wave based on time and offset
            float eval = data.wave.Evaluate(context.AnimatorContext.PassedTime,
                data.waveOffsetType.GetOffset(cData, context)).Value;

            // Move the character up based on the wave evaluation
            cData.PositionDelta = Vector3.up * eval;
        }
    }
}