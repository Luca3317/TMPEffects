using TMPEffects.AutoParameters.Attributes;
using UnityEngine;
using TMPEffects.CharacterData;
using TMPEffects.Parameters;
using static TMPEffects.Parameters.TMPParameterTypes;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new PivotAnimation",
        menuName = "TMPEffects/Animations/Basic Animations/Built-in/Pivot")]
    public partial class PivotAnimation : TMPAnimation
    {
        [SerializeField, AutoParameterBundle("")]
        [Tooltip(
            "The wave that defines the behavior of this animation. No prefix.\nFor more information about Wave, see the section on it in the documentation.")]
        Wave wave;

        [SerializeField, AutoParameterBundle("")]
        [Tooltip(
            "The way the offset for the wave is calculated.\nFor more information about Wave, see the section on it in the documentation.\nAliases: waveoffset, woffset, waveoff, woff")]
        OffsetBundle waveOffsetType;

        [SerializeField, AutoParameter("pivot", "pv", "p")]
        [Tooltip("The pivot position of the rotation.\nAliases: pivot, pv, p")]
        private TypedVector3 pivot = new TypedVector3(VectorType.Anchor, Vector3.zero);

        [SerializeField, AutoParameter("rotationaxis", "axis", "a")]
        [Tooltip("The axis to rotate around.\nAliases: rotationaxis, axis, a")]
        private Vector3 rotationAxis = Vector3.right;

        [SerializeField, AutoParameter("maxangle", "maxa", "max")]
        [Tooltip("The maximum angle of the rotation.\nAliases: maxangle, maxa, max")]
        private float maxAngleLimit = 180;

        [SerializeField, AutoParameter("minangle", "mina", "min")]
        [Tooltip("The minimum angle of the rotation.\nAliases: minangle, mina, min")]
        private float minAngleLimit = -180f;

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            // Evaluate the wave based on time and offset
            (float, int) result = d.wave.Evaluate(context.AnimatorContext.PassedTime,
                d.waveOffsetType.GetOffset(cData, context));

            // Calculate the angle based on the evaluate wave
            float angle = Mathf.LerpUnclamped(d.minAngleLimit, d.maxAngleLimit, result.Item1);
            cData.AddRotation(Quaternion.AngleAxis(angle, d.rotationAxis).eulerAngles,
                d.pivot.ToPosition(cData, context));
        }
    }
}