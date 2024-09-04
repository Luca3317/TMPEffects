using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMPEffects.CharacterData
{
    // TODO
    // This should store all types of modifications made to a mesh; vertex positions, colors, etc
    // That is *very* close to what VertexData does, except this stores deltas and not raw values 
    // => how to combine
    public class TMPMeshModifiers
    {
        public Vector3 PositionDelta
        {
            get => positionDelta;
            set
            {
                if (value == positionDelta) return;
                positionDelta = value;
                Dirty |= DirtyFlags.Position;
            }
        }

        public Matrix4x4 ScaleDelta
        {
            get => scaleDelta;
            set
            {
                if (value == scaleDelta) return;
                scaleDelta = value;
                Dirty |= DirtyFlags.Scale;
            }
        }

        public Quaternion RotationDelta
        {
            get => rotationDelta;
            set
            {
                if (value == rotationDelta) return;
                rotationDelta = value;
                Dirty |= DirtyFlags.Rotation;
            }
        }

        public Vector3 BL_Delta
        {
            get => bl_Delta;
            set
            {
                if (value == bl_Delta) return;
                bl_Delta = value;
                Dirty |= DirtyFlags.Vertices;
            }
        }

        public Vector3 TL_Delta
        {
            get => tl_Delta;
            set
            {
                if (value == tl_Delta) return;
                tl_Delta = value;
                Dirty |= DirtyFlags.Vertices;
            }
        }

        public Vector3 TR_Delta
        {
            get => tr_Delta;
            set
            {
                if (value == tr_Delta) return;
                tr_Delta = value;
                Dirty |= DirtyFlags.Vertices;
            }
        }

        public Vector3 BR_Delta
        {
            get => br_Delta;
            set
            {
                if (value == br_Delta) return;
                br_Delta = value;
                Dirty |= DirtyFlags.Vertices;
            }
        }

        public Color32? BL_Color
        {
            get => bl_Color;
            set
            {
                if (value.Equals(bl_Color)) return;
                bl_Color = value;
                Dirty |= DirtyFlags.Colors;
            }
        }

        public Color32? TL_Color
        {
            get => tl_Color;
            set
            {
                if (value.Equals(tl_Color)) return;
                tl_Color = value;
                Dirty |= DirtyFlags.Colors;
            }
        }

        public Color32? TR_Color
        {
            get => tr_Color;
            set
            {
                if (value.Equals(tr_Color)) return;
                tr_Color = value;
                Dirty |= DirtyFlags.Colors;
            }
        }

        public Color32? BR_Color
        {
            get => br_Color;
            set
            {
                if (value.Equals(br_Color)) return;
                br_Color = value;
                Dirty |= DirtyFlags.Colors;
            }
        }

        public Vector3 BL_UV0
        {
            get => bl_UV0;
            set
            {
                if (value == bl_UV0) return;
                bl_UV0 = value;
                Dirty |= DirtyFlags.UVs;
            }
        }

        public Vector3 TL_UV0
        {
            get => tl_UV0;
            set
            {
                if (value == tl_UV0) return;
                tl_UV0 = value;
                Dirty |= DirtyFlags.UVs;
            }
        }

        public Vector3 TR_UV0
        {
            get => tr_UV0;
            set
            {
                if (value == tr_UV0) return;
                tr_UV0 = value;
                Dirty |= DirtyFlags.UVs;
            }
        }

        public Vector3 BR_UV0
        {
            get => br_UV0;
            set
            {
                if (value == br_UV0) return;
                br_UV0 = value;
                Dirty |= DirtyFlags.UVs;
            }
        }

        private Vector3 positionDelta = Vector3.zero;
        private Matrix4x4 scaleDelta = Matrix4x4.identity;
        private Quaternion rotationDelta = Quaternion.identity;
        // TODO RotationPivot? Not needed for keyframe editor likely, but this should be generic

        private Vector3 bl_Delta;
        private Vector3 tl_Delta;
        private Vector3 tr_Delta;
        private Vector3 br_Delta;

        private Color32? bl_Color;
        private Color32? tl_Color;
        private Color32? tr_Color;
        private Color32? br_Color;

        private Vector3 bl_UV0;
        private Vector3 tl_UV0;
        private Vector3 tr_UV0;
        private Vector3 br_UV0;

        private Vector3 BLMax, BLMin;
        private Vector3 TLMax, TLMin;
        private Vector3 TRMax, TRMin;
        private Vector3 BRMax, BRMin;

        [Flags]
        public enum DirtyFlags : byte
        {
            None = 0,
            Vertices = 1,
            Colors = 1 << 1,
            UVs = 1 << 2,
            Position = 1 << 3,
            Scale = 1 << 4,
            Rotation = 1 << 5,

            All = byte.MaxValue
        }

        public DirtyFlags Dirty { get; private set; }

        public void ClearDirtyFlags()
        {
            Dirty = DirtyFlags.None;
        }
    }

    public class SmthThatAppliesModifiers
    {
        private Vector3 BLMin, BLMax;
        private Vector3 TLMin, TLMax;
        private Vector3 TRMin, TRMax;
        private Vector3 BRMin, BRMax;

        private (Vector3 BL, Vector3 TL, Vector3 TR, Vector3 BR)? prevResult = null;

        public SmthThatAppliesModifiers()
        {
        }

        // TODO shouldnt have to pass in cdata; make some interface or w/e
        public (Vector3 BL, Vector3 TL, Vector3 TR, Vector3 BR) CalculateVertexPositions(CharData cData,
            TMPMeshModifiers modifiers)
        {
            if (modifiers.Dirty == TMPMeshModifiers.DirtyFlags.None && prevResult.HasValue)
            {
                return prevResult.Value;
            }

            Vector3 vbl = cData.initialMesh.BL_Position; //+ modifiers.BL_Delta;
            Vector3 vtl = cData.initialMesh.TL_Position; // + modifiers.TL_Delta;
            Vector3 vtr = cData.initialMesh.TR_Position; // + modifiers.TR_Delta;
            Vector3 vbr = cData.initialMesh.BR_Position; // + modifiers.BR_Delta;

            // Apply scale
            vbl = modifiers.ScaleDelta.MultiplyPoint3x4(vbl - cData.InitialPosition) + cData.InitialPosition;
            vtl = modifiers.ScaleDelta.MultiplyPoint3x4(vtl - cData.InitialPosition) + cData.InitialPosition;
            vtr = modifiers.ScaleDelta.MultiplyPoint3x4(vtr - cData.InitialPosition) + cData.InitialPosition;
            vbr = modifiers.ScaleDelta.MultiplyPoint3x4(vbr - cData.InitialPosition) + cData.InitialPosition;

            // Apply rotation TODO
            // Vector3 pivot;
            Matrix4x4 matrix = Matrix4x4.Rotate(modifiers.RotationDelta);
            vtl = matrix.MultiplyPoint3x4(vtl);
            vtr = matrix.MultiplyPoint3x4(vtr);
            vbr = matrix.MultiplyPoint3x4(vbr);
            vbl = matrix.MultiplyPoint3x4(vbl);

            // TODO Moved those after scaling; since its, well a transformation too. Might have to do the same in CHarDataState
            // TODO Actually it might not be that simple; this does mess with the appearance more than i assumed; maybe separate pre/post modifications? idk
            // Apply vertex transformations
            vbl += modifiers.BL_Delta;
            vtl += modifiers.TL_Delta;
            vtr += modifiers.TR_Delta;
            vbr += modifiers.BR_Delta;

            // Apply transformation
            var scaled = modifiers.PositionDelta;
            vbl += scaled;
            vtl += scaled;
            vtr += scaled;
            vbr += scaled;

            modifiers.ClearDirtyFlags();
            prevResult = (vbl, vtl, vtr, vbr);
            return prevResult.Value;
        }

        // TODO same as above + need to have some default value for colors (=> not set); probably nullable
        public (Color32 BL, Color32 TL, Color32 TR, Color32 BR) CalculateVertexColors(CharData cData,
            TMPMeshModifiers modifiers)
        {
            return
            (
                modifiers.BL_Color ?? cData.initialMesh.BL_Color, 
                modifiers.TL_Color ?? cData.initialMesh.TL_Color, 
                modifiers.TR_Color ?? cData.initialMesh.TR_Color, 
                modifiers.BR_Color ?? cData.initialMesh.BR_Color
            );
        }

        // TODO same as above + need to have some default value for colors (=> not set); probably nullable
        public (Vector3 BL, Vector3 TL, Vector3 TR, Vector3 BR) CalculateVertexUVs(CharData cData,
            TMPMeshModifiers modifiers)
        {
            return
            (
                cData.initialMesh.BL_UV0,
                cData.initialMesh.TL_UV0,
                cData.initialMesh.TR_UV0,
                cData.initialMesh.BR_UV0
            );
        }
    }
}