using System.Collections.Generic;
using UnityEngine;
using TMPEffects.CharacterData;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new ContPivotAnimation", menuName = "TMPEffects/Animations/ContPivot")]
    public class ContPivotAnimation : TMPAnimation
    {
        [Tooltip("The speed of the rotation, in rotations per second.\nAliased: speed, sp, s")]
        [SerializeField] private float speed;
        [Tooltip("The pivot position of the rotation.\nAliases: pivot, pv, p")]
        [SerializeField] private TypedVector3 pivot = new TypedVector3(VectorType.Anchor, Vector3.zero);
        [Tooltip("The axis to rotate around.\nAliases: rotationaxis, axis, a")]
        [SerializeField] private Vector3 rotationAxis = Vector3.right;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            ContinuousRotation(cData, context);
        }

        private void ContinuousRotation(CharData cData, IAnimationContext context)
        {
            Data d = context.CustomData as Data;

            // Calculate the angle based on the evaluate wave
            float angle = (context.AnimatorContext.PassedTime * d.speed * 360) % 360;

            // Set the rotation using the rotationaxis and current angle
            cData.SetRotation(Quaternion.AngleAxis(angle, d.rotationAxis));

            // Set the pivot depending on its type
            switch (d.pivot.type)
            {
                case VectorType.Position: SetPivotRaw(d.pivot.vector, cData, context); break;
                case VectorType.Offset: cData.SetPivot(cData.InitialPosition + new Vector3(d.pivot.vector.x, d.pivot.vector.y, 0f)); break;
                case VectorType.Anchor: SetPivotRaw(AnchorToPosition(d.pivot.vector, cData), cData, context); break;
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetVector3Parameter(out Vector3 v3, parameters, "rotationaxis", rotationAxisAliases)) d.rotationAxis = v3;
            if (TryGetTypedVector3Parameter(out var tv3, parameters, "pivot", pivotAliases)) d.pivot = tv3;
            if (TryGetFloatParameter(out var speed, parameters, "speed", "sp", "s")) d.speed = speed;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonVector3Parameter(parameters, "rotationaxis", rotationAxisAliases)) return false;
            if (HasNonTypedVector3Parameter(parameters, "pivot", pivotAliases)) return false;
            if (HasNonFloatParameter(parameters, "speed", "sp", "s")) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data
            {
                speed = this.speed,
                pivot = this.pivot,
                rotationAxis = this.rotationAxis,
            };
        }

        private readonly string[] pivotAliases = new string[] { "p", "pv" };
        private readonly string[] rotationAxisAliases = new string[] { "axis", "a" };

        private class Data
        {
            public float speed;
            public TypedVector3 pivot;
            public Vector3 rotationAxis;
        }
    }
}