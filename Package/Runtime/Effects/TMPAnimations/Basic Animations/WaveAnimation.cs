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
        [SerializeField] [AutoParameterBundle("")]
        OffsetBundle offsetProvider = new OffsetBundle();

        [Tooltip("The wave that defines the behavior of this animation. No prefix.\n" +
                 "For more information about Wave, see the section on it in the documentation.")]
        [SerializeField]
        [AutoParameterBundle("")]
        private Wave wave = new Wave(AnimationCurveUtility.EaseInOutSine(),
            AnimationCurveUtility.EaseInOutSine(), 0.5f, 0.5f, 1f, 0f, 0f);

        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {
            // Evaluate the wave based on time and offset
            float offset = data.offsetProvider.GetOffset(cData, context);
            float eval = data.wave
                .Evaluate(context.AnimatorContext.PassedTime, offset).Value;

            // Move the character up based on the wave evaluation
            cData.PositionDelta = Vector3.up * eval;
        }
    }
}