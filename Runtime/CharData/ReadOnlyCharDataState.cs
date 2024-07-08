using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;

namespace TMPEffects.CharacterData
{
    /// <summary>
    /// Readonly version of <see cref="CharDataState"/>.<br/>
    /// Used to expose the current state of the <see cref="CharData"/> in <see cref="IAnimationContext"/>, allowing you to take it into account in your animations
    /// (ideally with late animations, see <see href="https://tmpeffects.luca3317.dev/docs/tmpanimator_gettingstarted.html#late-animations--second-pass">the docs</see>).
    /// </summary>
    public class ReadOnlyCharDataState : ICharDataState
    {
        List<Vector3> pivots = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();

        /// <inheritdoc/>
        public Vector3 positionDelta => state.positionDelta;
        /// <inheritdoc/>
        public Matrix4x4 scaleDelta => state.scaleDelta;
        /// <inheritdoc/>
        public IEnumerable<ValueTuple<Quaternion, Vector3>> Rotations
        {
            get
            {
                for (int i = 0; i < rotations.Count; i++)
                {
                    yield return new ValueTuple<Quaternion, Vector3>(rotations[i], pivots[i]);
                }
            }
        }

        /// <inheritdoc/>
        public Vector3 TL => state.TL;
        /// <inheritdoc/>
        public Vector3 TR => state.TR;
        /// <inheritdoc/>
        public Vector3 BR => state.BR;
        /// <inheritdoc/>
        public Vector3 BL => state.BL;

        /// <inheritdoc/>
        public Vector3 TLMax => state.TLMax;
        /// <inheritdoc/>
        public Vector3 TRMax => state.TRMax;
        /// <inheritdoc/>
        public Vector3 BRMax => state.BRMax;
        /// <inheritdoc/>
        public Vector3 BLMax => state.BLMax;

        /// <inheritdoc/>
        public Vector3 TLMin => state.TLMin;
        /// <inheritdoc/>
        public Vector3 TRMin => state.TRMin;
        /// <inheritdoc/>
        public Vector3 BRMin => state.BRMin;
        /// <inheritdoc/>
        public Vector3 BLMin => state.BLMin;

        /// <inheritdoc/>
        public Vector2 TL_UV => state.TL_UV;
        /// <inheritdoc/>
        public Vector2 TR_UV => state.TR_UV;
        /// <inheritdoc/>
        public Vector2 BR_UV => state.BR_UV;
        /// <inheritdoc/>
        public Vector2 BL_UV => state.BL_UV;

        /// <inheritdoc/>
        public Vector2 TL_UV2 => state.TL_UV2;
        /// <inheritdoc/>
        public Vector2 TR_UV2 => state.TR_UV2;
        /// <inheritdoc/>
        public Vector2 BR_UV2 => state.BR_UV2;
        /// <inheritdoc/>
        public Vector2 BL_UV2 => state.BL_UV2;

        /// <inheritdoc/>
        public Color32 TL_Color => state.TL_Color;
        /// <inheritdoc/>
        public Color32 TR_Color => state.TR_Color;
        /// <inheritdoc/>
        public Color32 BR_Color => state.BR_Color;
        /// <inheritdoc/>
        public Color32 BL_Color => state.BL_Color;

        /// <inheritdoc/>
        public Vector3 TL_Result => state.TL_Result;
        /// <inheritdoc/>
        public Vector3 TR_Result => state.TR_Result;
        /// <inheritdoc/>
        public Vector3 BR_Result => state.BR_Result;
        /// <inheritdoc/>
        public Vector3 BL_Result => state.BL_Result;

        public ReadOnlyCharDataState(CharDataState state)
        {
            this.state = state;
        }

        /// <inheritdoc/>
        public void CalculateVertexPositions()
        {
            state.CalculateVertexPositions();
        }

        private CharDataState state;
    }
}