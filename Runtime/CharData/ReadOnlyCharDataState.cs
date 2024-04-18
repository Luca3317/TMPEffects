using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.Animator;
using UnityEngine;

namespace TMPEffects.CharacterData
{
    public class ReadOnlyCharDataState
    {
        List<Vector3> pivots = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();

        public Vector3 positionDelta => state.positionDelta;
        public Matrix4x4 scaleDelta => state.scaleDelta;

        public Vector3 TL => state.TL;
        public Vector3 TR => state.TR;
        public Vector3 BR => state.BR;
        public Vector3 BL => state.BL;

        public Vector3 TLMax => state.TLMax;
        public Vector3 TRMax => state.TRMax;
        public Vector3 BRMax => state.BRMax;
        public Vector3 BLMax => state.BLMax;

        public Vector3 TLMin => state.TLMin;
        public Vector3 TRMin => state.TRMin;
        public Vector3 BRMin => state.BRMin;
        public Vector3 BLMin => state.BLMin;

        public Vector2 TL_UV => state.TL_UV;
        public Vector2 TR_UV => state.TR_UV;
        public Vector2 BR_UV => state.BR_UV;
        public Vector2 BL_UV => state.BL_UV;

        public Vector2 TL_UV2 => state.TL_UV2;
        public Vector2 TR_UV2 => state.TR_UV2;
        public Vector2 BR_UV2 => state.BR_UV2;
        public Vector2 BL_UV2 => state.BL_UV2;

        public Color32 TL_Color => state.TL_Color;
        public Color32 TR_Color => state.TR_Color;
        public Color32 BR_Color => state.BR_Color;
        public Color32 BL_Color => state.BL_Color;

        public Vector3 TL_Result => state.TL_Result;
        public Vector3 TR_Result => state.TR_Result;
        public Vector3 BR_Result => state.BR_Result;
        public Vector3 BL_Result => state.BL_Result;

        public ReadOnlyCharDataState(CharDataState state)
        {
            this.state = state;
        }

        public void CalculateVertexPositions()
        {
            state.CalculateVertexPositions();
        }

        private CharDataState state;
    }
}