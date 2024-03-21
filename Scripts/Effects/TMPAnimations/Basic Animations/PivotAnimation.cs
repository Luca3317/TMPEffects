using System.Collections.Generic;
using UnityEngine;
using TMPEffects.CharacterData;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new PivotAnimation", menuName = "TMPEffects/Animations/Pivot")]
    public class PivotAnimation : TMPAnimation
    {
        [Tooltip("How fast the characters rotate, in rotations per second")]
        [SerializeField] private float speed = 1f;
        //[Tooltip("Whether to consider the pivot vector as an offset relative to the characters position, or a raw position.")]
        //[SerializeField] private bool offsetDelta = true;
        [Tooltip("Whether to consider the pivot vector as an offset relative to the characters position, a raw position, or as an anchor.")]
        [SerializeField] private VectorType pivotType = VectorType.Offset;
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
        [Tooltip("The wave to use for the rotation. Ignored if limitRotation is false.")]
        [SerializeField] Wave wave;
        [Tooltip("The offset to use for the wave. Ignored if limitRotation is false.")]
        [SerializeField] WaveOffsetType waveOffsetType;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;
            if (d.limitRotation)
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

            (float, int) result = d.wave.Evaluate(context.animatorContext.PassedTime, GetWaveOffset(cData, context, d.waveOffset));
            float angle = Mathf.LerpUnclamped(d.minAngleLimit, d.maxAngleLimit, result.Item1);
            var rotate = Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.right, (cData.mesh.initial.GetVertex(3) - cData.mesh.initial.GetVertex(0)).normalized));

            cData.SetRotation(Quaternion.AngleAxis(angle, rotate.MultiplyPoint3x4(d.rotationAxis)));

            switch (d.pivotType)
            {
                case VectorType.Position: SetPivotRaw(d.pivot, cData, ref context); break;
                case VectorType.Offset: cData.SetPivot(cData.info.initialPosition + new Vector3(d.pivot.x, d.pivot.y, 0f)); break;
                case VectorType.Anchor: SetPivotRaw(AnchorToPosition(d.pivot, cData), cData, ref context); break;
            }
        }

        private void ContinuousRotation(CharData cData, IAnimationContext context)
        {
            Data d = context.customData as Data;

            float angle = (context.animatorContext.PassedTime * d.speed * 360) % 360;
            var rotate = Matrix4x4.Rotate(Quaternion.FromToRotation(Vector3.right, (cData.mesh.initial.GetVertex(3) - cData.mesh.initial.GetVertex(0)).normalized));
            cData.SetRotation(Quaternion.AngleAxis(angle, rotate.MultiplyPoint3x4(d.rotationAxis)));

            switch (d.pivotType)
            {
                case VectorType.Position: SetPivotRaw(d.pivot, cData, ref context); break;
                case VectorType.Offset: cData.SetPivot(cData.info.initialPosition + new Vector3(d.pivot.x, d.pivot.y, 0f)); break;
                case VectorType.Anchor: SetPivotRaw(AnchorToPosition(d.pivot, cData), cData, ref context); break;
            }
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            if (parameters == null) return;

            Data d = customData as Data;
            if (TryGetFloatParameter(out float val, parameters, "speed", speedAliases)) d.speed = val;
            if (TryGetFloatParameter(out val, parameters, "maxAngle", maxAngleAliases)) d.maxAngleLimit = val;
            if (TryGetFloatParameter(out val, parameters, "minAngle", minAngleAliases)) d.minAngleLimit = val;
            if (TryGetBoolParameter(out bool bVal, parameters, "limitRotation", limitRotationAliases)) d.limitRotation = bVal;
            if (TryGetVector3Parameter(out Vector3 v3, parameters, "axis", rotationAxisAliases)) d.rotationAxis = v3;
            if (TryGetWaveOffsetParameter(out WaveOffsetType type, parameters, "waveoffset", WaveOffsetAliases)) d.waveOffset = type;

            if (TryGetAnyVector3Parameter(out v3, out VectorType vType, parameters, "pivot", pivotAliases))
            {
                d.pivot = v3;
                d.pivotType = vType;
            }

            d.wave = CreateWave(wave, GetWaveParameters(parameters));
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "speed", speedAliases)) return false;
            if (HasNonFloatParameter(parameters, "maxAngle", maxAngleAliases)) return false;
            if (HasNonFloatParameter(parameters, "minAngle", minAngleAliases)) return false;
            if (HasNonVector3Parameter(parameters, "axis", rotationAxisAliases)) return false;
            if (HasNonBoolParameter(parameters, "limitRotation", limitRotationAliases)) return false;
            if (HasNonWaveOffsetParameter(parameters, "waveoffset", WaveOffsetAliases)) return false;
            if (HasNonAnyVector3Parameter(parameters, "pivot", pivotAliases)) return false;

            return ValidateWaveParameters(parameters);
        }

        public override object GetNewCustomData()
        {
            return new Data
            {
                speed = this.speed,
                pivot = this.pivot,
                pivotType = this.pivotType,
                maxAngleLimit = this.maxAngleLimit,
                minAngleLimit = this.minAngleLimit,
                rotationAxis = this.rotationAxis,
                limitRotation = this.limitRotation
            };
        }

        private readonly string[] speedAliases = new string[] { "s", "sp" };
        private readonly string[] pivotAliases = new string[] { "p", "pv" };
        private readonly string[] offsetDeltaAliases = new string[] { "od", "odelta" };
        private readonly string[] limitRotationAliases = new string[] { "limit", "limitRot", "lRot" };
        private readonly string[] maxAngleAliases = new string[] { "max", "maxA" };
        private readonly string[] minAngleAliases = new string[] { "min", "minA" };
        private readonly string[] rotationAxisAliases = new string[] { "a" };


        private class Data
        {
            public Wave wave;
            public WaveOffsetType waveOffset;

            public float speed;
            public VectorType pivotType;
            public Vector3 pivot;
            //public bool offsetDelta;
            public bool limitRotation;
            public float maxAngleLimit;
            public float minAngleLimit;
            public Vector3 rotationAxis;
        }
    }
}