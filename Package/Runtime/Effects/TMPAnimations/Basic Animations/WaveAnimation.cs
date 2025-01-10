using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using UnityEngine;
using TMPEffects.Extensions;
using TMPEffects.CharacterData;
using TMPEffects.Databases;
using TMPEffects.Parameters;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new WaveAnimation", menuName = "TMPEffects/Animations/Basic Animations/Built-in/Wave")]
    public partial class WaveAnimation : TMPAnimation
    {
        [Tooltip("The timing offsets used by this animation. No prefix.\n" +
                 "For more information about it, see the section on OffsetProviders in the documentation.")]
        [SerializeField, AutoParameterBundle("")]
        private OffsetBundle offsetProvider = new OffsetBundle();

        [Tooltip("The wave that defines the behavior of this animation. No prefix.\n" +
                 "For more information about it, see the section on Waves in the documentation.")]
        [SerializeField, AutoParameterBundle("")]
        private Wave wave = new Wave(AnimationCurveUtility.EaseInOutSine(),
            AnimationCurveUtility.EaseInOutSine(), 0.5f, 0.5f, 1f, 0f, 0f);

        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {
            // Evaluate the wave based on time and offset
            float eval = data.wave
                .Evaluate(context.AnimatorContext.PassedTime, data.offsetProvider.GetOffset(cData, context)).Value;

            // Move the character up based on the wave evaluation
            cData.PositionDelta = Vector3.up * eval;
        }
    }
}