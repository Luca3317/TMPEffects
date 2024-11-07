using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using UnityEngine;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using static TMPEffects.Parameters.ParameterUtility;
using static TMPEffects.Parameters.ParameterTypes;
using static TMPEffects.TMPAnimations.AnimationUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new ContPivotAnimation",
        menuName = "TMPEffects/Animations/Basic Animations/Built-in/ContPivot")]
    public partial class ContPivotAnimation : TMPAnimation
    {
        [SerializeField, AutoParameter("speed", "sp", "s")]
        [Tooltip("The speed of the rotation, in rotations per second.\nAliased: speed, sp, s")]
        private float speed;

        [SerializeField, AutoParameter("pivot", "pv", "p")]
        [Tooltip("The pivot position of the rotation.\nAliases: pivot, pv, p")] 
        private TypedVector3 pivot = new TypedVector3(VectorType.Anchor, Vector3.zero);

        [SerializeField, AutoParameter("rotationaxis", "axis", "a")]
        [Tooltip("The axis to rotate around.\nAliases: rotationaxis, axis, a")]
        private Vector3 rotationAxis = Vector3.right;

        private partial void Animate(CharData cData, AutoParametersData d, IAnimationContext context)
        {
            // Calculate the angle based on the evaluate wave
            float angle = (context.AnimatorContext.PassedTime * d.speed * 360) % 360;
            
            // Set the pivot depending on its type
            switch (d.pivot.type)
            {
                case VectorType.Position:
                    cData.AddRotation(Quaternion.AngleAxis(angle, d.rotationAxis).eulerAngles,
                        d.pivot.IgnoreScaling(cData, context).ToPosition(cData));
                    break;
                case VectorType.Offset:
                    Debug.LogWarning("Delta is " + d.pivot.ToDelta(cData) + " ( / " + d.pivot.vector+ ")");
                    cData.AddRotation(Quaternion.AngleAxis(angle, d.rotationAxis).eulerAngles,
                        cData.InitialPosition + d.pivot.ToDelta(cData));
                    break;
                case VectorType.Anchor:
                    cData.AddRotation(Quaternion.AngleAxis(angle, d.rotationAxis).eulerAngles,
                        d.pivot.ToPosition(cData));
                    break;
            }
        }
    }
}