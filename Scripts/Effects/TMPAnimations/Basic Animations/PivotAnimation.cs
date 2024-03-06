using System.Collections.Generic;
using UnityEngine;
using TMPEffects.Components.CharacterData;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new PivotAnimation", menuName = "TMPEffects/Animations/Pivot")]
    public class PivotAnimation : TMPAnimation
    {
        [Tooltip("How fast the characters rotate, in rotations per second")]
        [SerializeField] private float speed = 1f;
        [Tooltip("Whether to consider the pivot vector as an offset relative to the characters position, or a raw position.")]
        [SerializeField] private bool offsetDelta = true;
        [Tooltip("The pivot position. Depending on the value of offsetDelta, this is either an offset relative to the characters position, or a raw position.")]
        [SerializeField] private Vector3 pivot = Vector2.zero;
        [Tooltip("The axis of rotation.")]
        [SerializeField] private Vector3 rotationAxis = Vector3.right;

        [Header("Limited rotations")]
        [Tooltip("Whether to limit the rotation to the given angles.")]
        [SerializeField] private bool limitRotation = false;
        [Tooltip("The maximum angle of the rotation. Ignored if limitRotation is false.")]
        [SerializeField] private float maxAngleLimit = 180;
        [Tooltip("The minimum angle of the rotation. Ignored if limitRotation is false.")]
        [SerializeField] private float minAngleLimit = -180f;
        [Tooltip("The curve to use for the rotation. Ignored if limitRotation is false.")]
        [SerializeField] private AnimationCurve curve = AnimationCurveUtility.EaseInOutCubic();

        public override void Animate(CharData cData, IAnimationContext context)
        {
            if (limitRotation)
            {
                LimitedRotation(cData, context);
            }
            else
            {
                ContinuousRotation(cData, context);
            }
        }

        private void LimitedRotation(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;

            float t = (context.animatorContext.PassedTime * d.speed);
            float t2 = Mathf.PingPong(t, 1);

            float eval = GetValue(d.curve, WrapMode.PingPong, context, t2, cData);
            float angle = Mathf.LerpUnclamped(d.minAngleLimit, d.maxAngleLimit, eval);

            var rotate = Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.right, (cData.mesh.initial.GetVertex(3) - cData.mesh.initial.GetVertex(0)).normalized));

            cData.SetRotation(Quaternion.AngleAxis(angle, rotate.MultiplyPoint3x4(d.rotationAxis)));

            if (offsetDelta)
            {
                cData.SetPivot(cData.info.initialPosition + new Vector3(d.pivot.x, d.pivot.y, 0f));
            }
            else
            {
                cData.SetPivot(new Vector3(d.pivot.x, d.pivot.y, 0f));
            }
        }

        private void ContinuousRotation(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;


            float angle = (context.animatorContext.PassedTime * d.speed * 360) % 360;
            var rotate = Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.right, (cData.mesh.initial.GetVertex(3) - cData.mesh.initial.GetVertex(0)).normalized));
            cData.SetRotation(Quaternion.AngleAxis(angle, rotate.MultiplyPoint3x4(d.rotationAxis)));
            if (offsetDelta)
            {
                cData.SetPivot(cData.info.initialPosition + new Vector3(d.pivot.x, d.pivot.y, 0f));
            }
            else
            {
                cData.SetPivot(new Vector3(d.pivot.x, d.pivot.y, 0f));
            } 
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float val, parameters, "speed", speedAliases)) d.speed = val;
            if (TryGetFloatParameter(out val, parameters, "maxAngle", maxAngleAliases)) d.maxAngleLimit = val;
            if (TryGetFloatParameter(out val, parameters, "minAngle", minAngleAliases)) d.minAngleLimit = val;
            if (TryGetBoolParameter(out bool bVal, parameters, "offsetDelta", offsetDeltaAliases)) d.offsetDelta = bVal;
            if (TryGetBoolParameter(out bVal, parameters, "limitRotation", limitRotationAliases)) d.limitRotation = bVal;
            if (TryGetVector3Parameter(out Vector3 v3, parameters, "pivot", pivotAliases)) d.pivot = v3;
            if (TryGetVector3Parameter(out v3, parameters, "axis", rotationAxisAliases)) d.rotationAxis = v3;
            if (TryGetAnimCurveParameter(out var cv, parameters, "curve", curveAliases)) d.curve = cv;
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "speed", speedAliases)) return false;
            if (HasNonFloatParameter(parameters, "maxAngle", maxAngleAliases)) return false;
            if (HasNonFloatParameter(parameters, "minAngle", minAngleAliases)) return false;
            if (HasNonVector3Parameter(parameters, "pivot", pivotAliases)) return false;
            if (HasNonBoolParameter(parameters, "offsetDelta", offsetDeltaAliases)) return false;
            if (HasNonVector3Parameter(parameters, "axis", rotationAxisAliases)) return false;
            if (HasNonAnimCurveParameter(parameters, "curve", curveAliases)) return false;
            if (HasNonBoolParameter(parameters, "limitRotation", limitRotationAliases)) return false;
            return true;
        }

        public override object GetNewCustomData()
        {
            return new Data
            {
                speed = this.speed,
                pivot = this.pivot,
                offsetDelta = this.offsetDelta,
                maxAngleLimit = this.maxAngleLimit,
                minAngleLimit = this.minAngleLimit,
                rotationAxis = this.rotationAxis,
                limitRotation = this.limitRotation,
                curve = this.curve
            };
        }

        private readonly string[] speedAliases = new string[] { "s", "sp" };
        private readonly string[] pivotAliases = new string[] { "p", "pv" };
        private readonly string[] offsetDeltaAliases = new string[] { "od", "odelta" };
        private readonly string[] limitRotationAliases = new string[] { "limit", "limitRot", "lRot" };
        private readonly string[] maxAngleAliases = new string[] { "max", "maxA" };
        private readonly string[] minAngleAliases = new string[] { "min", "minA" };
        private readonly string[] rotationAxisAliases = new string[] { "a" };
        private readonly string[] curveAliases = new string[] { "c", "crv" };


        private class Data
        {
            public float speed;
            public Vector3 pivot;
            public bool offsetDelta;
            public bool limitRotation;
            public float maxAngleLimit;
            public float minAngleLimit;
            public Vector3 rotationAxis;
            public AnimationCurve curve;
        }
    }
}