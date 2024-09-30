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
    [Serializable]
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

        public UnityNullable<Color32> BL_Color
        {
            get
            {
                bl_Color ??= new UnityNullable<Color32>();
                return bl_Color;
            }
            set
            {
                if (value.Equals(bl_Color)) return;
                bl_Color = value;
                Dirty |= DirtyFlags.Colors;
            }
        }

        public UnityNullable<Color32> TL_Color
        {
            get
            {
                tl_Color ??= new UnityNullable<Color32>();
                return tl_Color;
            }
            set
            {
                if (value.Equals(tl_Color)) return;
                tl_Color = value;
                Dirty |= DirtyFlags.Colors;
            }
        }

        public UnityNullable<Color32> TR_Color
        {
            get
            {
                tr_Color ??= new UnityNullable<Color32>();
                return tr_Color;
            }
            set
            {
                if (value.Equals(tr_Color)) return;
                tr_Color = value;
                Dirty |= DirtyFlags.Colors;
            }
        }

        public UnityNullable<Color32> BR_Color
        {
            get
            {
                br_Color ??= new UnityNullable<Color32>();
                return br_Color;
            }
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

        [SerializeField] private Vector3 positionDelta = Vector3.zero;
        [SerializeField] private Matrix4x4 scaleDelta = Matrix4x4.identity;
        [SerializeField] private Quaternion rotationDelta = Quaternion.identity;
        // TODO RotationPivot? Not needed for keyframe editor likely, but this should be generic

        [SerializeField] private Vector3 bl_Delta;
        [SerializeField] private Vector3 tl_Delta;
        [SerializeField] private Vector3 tr_Delta;
        [SerializeField] private Vector3 br_Delta;

        [SerializeField] public UnityNullable<Color32> bl_Color;
        [SerializeField] private UnityNullable<Color32> tl_Color;
        [SerializeField] private UnityNullable<Color32> tr_Color;
        [SerializeField] private UnityNullable<Color32> br_Color;

        [SerializeField] private Vector3 bl_UV0;
        [SerializeField] private Vector3 tl_UV0;
        [SerializeField] private Vector3 tr_UV0;
        [SerializeField] private Vector3 br_UV0;

        private Vector3 BLMax, BLMin;
        private Vector3 TLMax, TLMin;
        private Vector3 TRMax, TRMin;
        private Vector3 BRMax, BRMin;

        public static TMPMeshModifiers operator +(TMPMeshModifiers lhs, TMPMeshModifiers rhs)
        {
            TMPMeshModifiers result = new TMPMeshModifiers();
            result.PositionDelta = lhs.PositionDelta + rhs.PositionDelta;
            // result.ScaleDelta = lhs.ScaleDelta * rhs.ScaleDelta;
            result.RotationDelta = lhs.RotationDelta * rhs.RotationDelta;

            result.BL_Delta = lhs.BL_Delta + rhs.BL_Delta;
            result.TL_Delta = lhs.TL_Delta + rhs.TL_Delta;
            result.TR_Delta = lhs.TR_Delta + rhs.TR_Delta;
            result.BR_Delta = lhs.BR_Delta + rhs.BR_Delta;

            result.BL_Color =
                rhs.BL_Color.HasValue ? rhs.BL_Color : lhs.BL_Color; // Use rhs color if set, otherwise use lhs
            result.TL_Color = rhs.TL_Color.HasValue ? rhs.TL_Color : lhs.TL_Color;
            result.TR_Color = rhs.TR_Color.HasValue ? rhs.TR_Color : lhs.TR_Color;
            result.BR_Color = rhs.BR_Color.HasValue ? rhs.BR_Color : lhs.BR_Color;

            result.BL_UV0 = lhs.BL_UV0 + rhs.BL_UV0;
            result.TL_UV0 = lhs.TL_UV0 + rhs.TL_UV0;
            result.TR_UV0 = lhs.TR_UV0 + rhs.TR_UV0;
            result.BR_UV0 = lhs.BR_UV0 + rhs.BR_UV0;
            return result;
        }

        public static TMPMeshModifiers LerpUnclamped(CharData cData, TMPMeshModifiers modifiers, float t)
        {
            TMPMeshModifiers result = new TMPMeshModifiers();

            if (modifiers.PositionDelta != Vector3.zero)
                result.PositionDelta = modifiers.PositionDelta * t;

            if (modifiers.RotationDelta != Quaternion.identity)
                result.RotationDelta = Quaternion.LerpUnclamped(cData.InitialRotation, modifiers.RotationDelta, t);

            // TODO I want to be able to use simple V3 scale -- why didnt i originally? some shit with order of applying?
            // if (step.Modifiers.ScaleDelta != Vector3.one)

            if (modifiers.BL_Delta != Vector3.zero)
                result.BL_Delta = modifiers.BL_Delta * t;

            if (modifiers.TL_Delta != Vector3.zero)
                result.TL_Delta = modifiers.TL_Delta * t;

            if (modifiers.TR_Delta != Vector3.zero)
                result.TR_Delta = modifiers.TR_Delta * t;

            if (modifiers.BR_Delta != Vector3.zero)
                result.BR_Delta = modifiers.BR_Delta * t;

            if (modifiers.BL_Color.HasValue)
                result.BL_Color.Value = Color32.LerpUnclamped(cData.initialMesh.BL_Color, modifiers.BL_Color.Value, t);

            if (modifiers.TL_Color.HasValue)
                result.TL_Color.Value = Color32.LerpUnclamped(cData.initialMesh.TL_Color, modifiers.TL_Color.Value, t);

            if (modifiers.TR_Color.HasValue)
                result.TR_Color.Value = Color32.LerpUnclamped(cData.initialMesh.TR_Color, modifiers.TR_Color.Value, t);

            if (modifiers.BR_Color.HasValue)
                result.BR_Color.Value = Color32.LerpUnclamped(cData.initialMesh.BR_Color, modifiers.BR_Color.Value, t);

            if (modifiers.BL_UV0 != Vector3.zero)
                result.BL_UV0 = modifiers.BL_UV0 * t;

            if (modifiers.TL_UV0 != Vector3.zero)
                result.TL_UV0 = modifiers.TL_UV0 * t;

            if (modifiers.TR_UV0 != Vector3.zero)
                result.TR_UV0 = modifiers.TR_UV0 * t;

            if (modifiers.BR_UV0 != Vector3.zero)
                result.BR_UV0 = modifiers.BR_UV0 * t;
            
            return result;

            // TMPMeshModifiers result = new TMPMeshModifiers();
            // result.PositionDelta = Vector3.LerpUnclamped(a.PositionDelta, b.PositionDelta, t);
            // // TODO scale
            // result.RotationDelta = Quaternion.LerpUnclamped(a.RotationDelta, b.RotationDelta, t);
            //
            // // TODO This actually doesnt make THAT much sense, since if you dont override delta etc
            // // you dont want it to be dirty (and therefore used)
            // result.BL_Delta = Vector3.LerpUnclamped(a.BL_Delta, b.BL_Delta, t);
            // result.TL_Delta = Vector3.LerpUnclamped(a.TL_Delta, b.TL_Delta, t);
            // result.TR_Delta = Vector3.LerpUnclamped(a.TR_Delta, b.TR_Delta, t);
            // result.BR_Delta = Vector3.LerpUnclamped(a.BR_Delta, b.BR_Delta, t);
            //
            // result.BL_Color.Value = b.BL_Color.HasValue
            //     ? (a.BL_Color.HasValue
            //         ? Color32.LerpUnclamped(a.BL_Color.Value, b.BL_Color.Value, t)
            //         : Color32.LerpUnclamped(cData.initialMesh.BL_Color, b.BL_Color.Value, t))
            //     
            //     : a.BL_Color.HasValue ? Color32.LerpUnclamped(a.BL_Color.Value, cData.initialMesh.BL_Color, t) : default;
        }

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

        // TODO The applier (TMPMeshModifierApplier or so) should be reused within the actual animator
        // (/ in the chardatastate). Does this method still make sense then?
        public void ApplyToCharData(CharData cData, TMPMeshModifiers modifiers)
        {
            if (modifiers.PositionDelta != Vector3.zero)
                cData.SetPosition(cData.InitialPosition + modifiers.PositionDelta);

            if (modifiers.RotationDelta != Quaternion.identity)
                cData.SetRotation(cData.InitialRotation * modifiers.RotationDelta);

            // TODO I want to be able to use simple V3 scale -- why didnt i originally? some shit with order of applying?
            // if (step.Modifiers.ScaleDelta != Vector3.one)

            if (modifiers.BL_Delta != Vector3.zero)
                cData.SetVertex(0, cData.initialMesh.BL_Position + modifiers.BL_Delta);

            if (modifiers.TL_Delta != Vector3.zero)
                cData.SetVertex(1, cData.initialMesh.BL_Position + modifiers.TL_Delta);

            if (modifiers.TR_Delta != Vector3.zero)
                cData.SetVertex(2, cData.initialMesh.BL_Position + modifiers.TR_Delta);

            if (modifiers.BR_Delta != Vector3.zero)
                cData.SetVertex(3, cData.initialMesh.BL_Position + modifiers.BR_Delta);

            if (modifiers.BL_Color.HasValue)
                cData.mesh.SetColor(0, modifiers.BL_Color.Value);

            if (modifiers.TL_Color.HasValue)
                cData.mesh.SetColor(1, modifiers.TL_Color.Value);

            if (modifiers.TR_Color.HasValue)
                cData.mesh.SetColor(2, modifiers.TR_Color.Value);

            if (modifiers.BR_Color.HasValue)
                cData.mesh.SetColor(3, modifiers.BR_Color.Value);

            if (modifiers.BL_UV0 != Vector3.zero)
                cData.mesh.SetUV0(0, modifiers.BL_UV0);

            if (modifiers.TL_UV0 != Vector3.zero)
                cData.mesh.SetUV0(1, modifiers.TL_UV0);

            if (modifiers.TR_UV0 != Vector3.zero)
                cData.mesh.SetUV0(2, modifiers.TR_UV0);

            if (modifiers.BR_UV0 != Vector3.zero)
                cData.mesh.SetUV0(3, modifiers.BR_UV0);
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
                modifiers.BL_Color.HasValue ? modifiers.BL_Color.Value : cData.initialMesh.BL_Color,
                modifiers.TL_Color.HasValue ? modifiers.TL_Color.Value : cData.initialMesh.TL_Color,
                modifiers.TR_Color.HasValue ? modifiers.TR_Color.Value : cData.initialMesh.TR_Color,
                modifiers.BR_Color.HasValue ? modifiers.BR_Color.Value : cData.initialMesh.BR_Color
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