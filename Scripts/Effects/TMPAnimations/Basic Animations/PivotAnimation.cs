using System.Collections.Generic;
using UnityEngine;
using TMPEffects.CharacterData;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new PivotAnimation", menuName = "TMPEffects/Animations/Pivot")]
    public class PivotAnimation : TMPAnimation
    {
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\nFor more information about Wave, see the section on it in the documentation.")]
        [SerializeField] Wave wave;
        [Tooltip("The way the offset for the wave is calculated.\nFor more information about Wave, see the section on it in the documentation.\nAliases: waveoffset, woffset, waveoff, woff")]
        [SerializeField] WaveOffsetType waveOffsetType;

        [Tooltip("The pivot position of the rotation.\nAliases: pivot, pv, p")]
        [SerializeField] private TypedVector3 pivot = new TypedVector3(VectorType.Anchor, Vector3.zero);
        [Tooltip("The axis to rotate around.\nAliases: rotationaxis, axis, a")]
        [SerializeField] private Vector3 rotationAxis = Vector3.right;

        [Tooltip("The maximum angle of the rotation.\nAliases: maxangle, maxa, max")]
        [SerializeField] private float maxAngleLimit = 180;
        [Tooltip("The minimum angle of the rotation.\nAliases: minangle, mina, min")]
        [SerializeField] private float minAngleLimit = -180f;

        public override void Animate(CharData cData, IAnimationContext context)
        {
            LimitedRotation(cData, context);
        }

        private void LimitedRotation(CharData cData, IAnimationContext context)
        {
            Data d = context.CustomData as Data;

            // Evaluate the wave based on time and offset
            (float, int) result = d.wave.Evaluate(context.AnimatorContext.PassedTime, GetWaveOffset(cData, context, d.waveOffset));

            // Calculate the angle based on the evaluate wave
            float angle = Mathf.LerpUnclamped(d.minAngleLimit, d.maxAngleLimit, result.Item1);

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
            if (TryGetFloatParameter(out float val, parameters, "maxangle", maxAngleAliases)) d.maxAngleLimit = val;
            if (TryGetFloatParameter(out val, parameters, "minangle", minAngleAliases)) d.minAngleLimit = val;
            if (TryGetVector3Parameter(out Vector3 v3, parameters, "rotationaxis", rotationAxisAliases)) d.rotationAxis = v3;
            if (TryGetWaveOffsetParameter(out WaveOffsetType type, parameters, "waveoffset", WaveOffsetAliases)) d.waveOffset = type;
            if (TryGetTypedVector3Parameter(out var tv3, parameters, "pivot", pivotAliases)) d.pivot = tv3;
            d.wave = CreateWave(wave, GetWaveParameters(parameters));
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return true;

            if (HasNonFloatParameter(parameters, "maxangle", maxAngleAliases)) return false;
            if (HasNonFloatParameter(parameters, "minangle", minAngleAliases)) return false;
            if (HasNonVector3Parameter(parameters, "rotationaxis", rotationAxisAliases)) return false;
            if (HasNonWaveOffsetParameter(parameters, "waveoffset", WaveOffsetAliases)) return false;
            if (HasNonTypedVector3Parameter(parameters, "pivot", pivotAliases)) return false;
            return ValidateWaveParameters(parameters);
        }

        public override object GetNewCustomData()
        {
            return new Data
            {
                wave = this.wave, 
                waveOffset = this.waveOffsetType,
                pivot = this.pivot,
                maxAngleLimit = this.maxAngleLimit,
                minAngleLimit = this.minAngleLimit,
                rotationAxis = this.rotationAxis,
            };
        }

        private readonly string[] pivotAliases = new string[] { "p", "pv" };
        private readonly string[] maxAngleAliases = new string[] { "max", "maxa" };
        private readonly string[] minAngleAliases = new string[] { "min", "mina" };
        private readonly string[] rotationAxisAliases = new string[] { "axis", "a" };

        private class Data
        {
            public Wave wave;
            public WaveOffsetType waveOffset;

            public TypedVector3 pivot;
            public float maxAngleLimit;
            public float minAngleLimit;
            public Vector3 rotationAxis;
        }
    }
}