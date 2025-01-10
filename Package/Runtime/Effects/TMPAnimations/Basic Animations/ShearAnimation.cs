using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Parameters;
using UnityEngine;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new ShearAnimation",
        menuName = "TMPEffects/Animations/Basic Animations/Built-in/Shear")]
    public partial class ShearAnimation : TMPAnimation
    {
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\n" +
                 "For more information about it, see the section on Waves in the documentation.")]
        [AutoParameterBundle(""), SerializeField]
        private Wave wave;

        [Tooltip("The timing offsets used by this animation. No prefix.\n" +
                 "For more information about it, see the section on OffsetProviders in the documentation.")]
        [AutoParameterBundle(""), SerializeField]
        private OffsetBundle offset;

        private partial void Animate(CharData cData, AnimData data, IAnimationContext context)
        {
            float value = data.wave.Evaluate(context.AnimatorContext.PassedTime, data.offset.GetOffset(cData, context))
                .Value;

            value -= data.wave.Amplitude / 2f;

            cData.mesh.BL_Position = cData.InitialMesh.BL_Position + Vector3.right * value;
            cData.mesh.BR_Position = cData.InitialMesh.BR_Position + Vector3.right * value;
            cData.mesh.TL_Position = cData.InitialMesh.TL_Position - Vector3.right * value;
            cData.mesh.TR_Position = cData.InitialMesh.TR_Position - Vector3.right * value;
        }

        [AutoParametersStorage]
        private partial class AnimData
        {
        }
    }
}