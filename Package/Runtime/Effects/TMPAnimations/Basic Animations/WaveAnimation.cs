using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using UnityEngine;
using static TMPEffects.Parameters.ParameterTypes;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Extensions;
using TMPEffects.CharacterData;
using TMPEffects.Databases;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new WaveAnimation", menuName = "TMPEffects/Animations/Basic Animations/Built-in/Wave")]
    public partial class WaveAnimation : TMPAnimation
    {
        [SerializeField]
        [AutoParameterBundle("")]
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\n" +
                 "For more information about Wave, see the section on it in the documentation.")]
        Wave wave = new Wave(AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 0.5f, 0.5f,
            1f, 1f, 0.2f);

        [SerializeField] [AutoParameterBundle("")]
        OffsetBundle offsetProvider = new OffsetBundle();

        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {
            // Evaluate the wave based on time and offset
            float eval = data.wave.Evaluate(context.AnimatorContext.PassedTime,
                data.offsetProvider.GetOffset(cData, context), data.offsetProvider.GetUniformity(context)).Value;

            // Move the character up based on the wave evaluation
            cData.PositionDelta = Vector3.up * eval;
        }
    }
}