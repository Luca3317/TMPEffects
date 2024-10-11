    // // TODO
    // // This should store all types of modifications made to a mesh; vertex positions, colors, etc
    // // That is *very* close to what VertexData does, except this stores deltas and not raw values 
    // // => how to combine
    //
    // using System;
    // using TMPEffects.CharacterData;
    // using UnityEngine;
    //
    // [Serializable]
    // public class TMPMeshModifier
    // {
    //     public Vector3 PositionDelta
    //     {
    //         get => positionDelta;
    //         set
    //         {
    //             if (value == positionDelta) return;
    //             positionDelta = value;
    //             Dirty |= DirtyFlags.Position;
    //         }
    //     }
    //
    //     public bool PositionDeltaIsRaw
    //     {
    //         get => positionDeltaIsRaw;
    //         set
    //         {
    //             if (value == positionDeltaIsRaw) return;
    //             positionDeltaIsRaw = value;
    //             Dirty |= DirtyFlags.Position;
    //         }
    //     }
    //
    //     public Matrix4x4 ScaleDelta
    //     {
    //         get => scaleDelta;
    //         set
    //         {
    //             if (value == scaleDelta) return;
    //             scaleDelta = value;
    //             Dirty |= DirtyFlags.Scale;
    //         }
    //     }
    //
    //     public Quaternion RotationDelta
    //     {
    //         get => rotationDelta;
    //         set
    //         {
    //             if (value == rotationDelta) return;
    //             rotationDelta = value;
    //             Dirty |= DirtyFlags.Rotation;
    //         }
    //     }
    //
    //     public Vector3 BL_Delta
    //     {
    //         get => bl_Delta;
    //         set
    //         {
    //             if (value == bl_Delta) return;
    //             bl_Delta = value;
    //             Dirty |= DirtyFlags.Vertices;
    //         }
    //     }
    //
    //     public bool BL_DeltaIsRaw
    //     {
    //         get => bl_DeltaIsRaw;
    //         set
    //         {
    //             if (value == bl_DeltaIsRaw) return;
    //             bl_DeltaIsRaw = value;
    //             Dirty |= DirtyFlags.Vertices;
    //         }
    //     }
    //
    //     public Vector3 TL_Delta
    //     {
    //         get => tl_Delta;
    //         set
    //         {
    //             if (value == tl_Delta) return;
    //             tl_Delta = value;
    //             Dirty |= DirtyFlags.Vertices;
    //         }
    //     }
    //
    //     public bool TL_DeltaIsRaw
    //     {
    //         get => tl_DeltaIsRaw;
    //         set
    //         {
    //             if (value == tl_DeltaIsRaw) return;
    //             tl_DeltaIsRaw = value;
    //             Dirty |= DirtyFlags.Vertices;
    //         }
    //     }
    //
    //     public Vector3 TR_Delta
    //     {
    //         get => tr_Delta;
    //         set
    //         {
    //             if (value == tr_Delta) return;
    //             tr_Delta = value;
    //             Dirty |= DirtyFlags.Vertices;
    //         }
    //     }
    //
    //     public bool TR_DeltaIsRaw
    //     {
    //         get => tr_DeltaIsRaw;
    //         set
    //         {
    //             if (value == tr_DeltaIsRaw) return;
    //             tr_DeltaIsRaw = value;
    //             Dirty |= DirtyFlags.Vertices;
    //         }
    //     }
    //
    //     public Vector3 BR_Delta
    //     {
    //         get => br_Delta;
    //         set
    //         {
    //             if (value == br_Delta) return;
    //             br_Delta = value;
    //             Dirty |= DirtyFlags.Vertices;
    //         }
    //     }
    //
    //     public bool BR_DeltaIsRaw
    //     {
    //         get => br_DeltaIsRaw;
    //         set
    //         {
    //             if (value == br_DeltaIsRaw) return;
    //             br_DeltaIsRaw = value;
    //             Dirty |= DirtyFlags.Vertices;
    //         }
    //     }
    //
    //     public ColorOverride BL_Color
    //     {
    //         get { return bl_Color; }
    //         set
    //         {
    //             if (value.Equals(bl_Color)) return;
    //             bl_Color = value;
    //             Dirty |= DirtyFlags.Colors;
    //         }
    //     }
    //
    //     public ColorOverride TL_Color
    //     {
    //         get { return tl_Color; }
    //         set
    //         {
    //             if (value.Equals(tl_Color)) return;
    //             tl_Color = value;
    //             Dirty |= DirtyFlags.Colors;
    //         }
    //     }
    //
    //     public ColorOverride TR_Color
    //     {
    //         get { return tr_Color; }
    //         set
    //         {
    //             if (value.Equals(tr_Color)) return;
    //             tr_Color = value;
    //             Dirty |= DirtyFlags.Colors;
    //         }
    //     }
    //
    //     public ColorOverride BR_Color
    //     {
    //         get { return br_Color; }
    //         set
    //         {
    //             if (value.Equals(br_Color)) return;
    //             br_Color = value;
    //             Dirty |= DirtyFlags.Colors;
    //         }
    //     }
    //
    //     public Vector3 BL_UV0
    //     {
    //         get => bl_UV0;
    //         set
    //         {
    //             if (value == bl_UV0) return;
    //             bl_UV0 = value;
    //             Dirty |= DirtyFlags.UVs;
    //         }
    //     }
    //
    //     public Vector3 TL_UV0
    //     {
    //         get => tl_UV0;
    //         set
    //         {
    //             if (value == tl_UV0) return;
    //             tl_UV0 = value;
    //             Dirty |= DirtyFlags.UVs;
    //         }
    //     }
    //
    //     public Vector3 TR_UV0
    //     {
    //         get => tr_UV0;
    //         set
    //         {
    //             if (value == tr_UV0) return;
    //             tr_UV0 = value;
    //             Dirty |= DirtyFlags.UVs;
    //         }
    //     }
    //
    //     public Vector3 BR_UV0
    //     {
    //         get => br_UV0;
    //         set
    //         {
    //             if (value == br_UV0) return;
    //             br_UV0 = value;
    //             Dirty |= DirtyFlags.UVs;
    //         }
    //     }
    //
    //     [SerializeField] private Vector3 positionDelta = Vector3.zero;
    //     [SerializeField] private bool positionDeltaIsRaw = false;
    //
    //     [SerializeField] private Matrix4x4 scaleDelta = Matrix4x4.identity;
    //     [SerializeField] private Quaternion rotationDelta = Quaternion.identity;
    //     // TODO RotationPivot? Not needed for keyframe editor likely, but this should be generic
    //
    //     [SerializeField] private Vector3 bl_Delta;
    //     [SerializeField] private bool bl_DeltaIsRaw = false;
    //     [SerializeField] private Vector3 tl_Delta;
    //     [SerializeField] private bool tl_DeltaIsRaw = false;
    //     [SerializeField] private Vector3 tr_Delta;
    //     [SerializeField] private bool tr_DeltaIsRaw = false;
    //     [SerializeField] private Vector3 br_Delta;
    //     [SerializeField] private bool br_DeltaIsRaw = false;
    //
    //     [SerializeField] private ColorOverride bl_Color;
    //     [SerializeField] private ColorOverride tl_Color;
    //     [SerializeField] private ColorOverride tr_Color;
    //     [SerializeField] private ColorOverride br_Color;
    //
    //     [SerializeField] private Vector3 bl_UV0;
    //     [SerializeField] private Vector3 tl_UV0;
    //     [SerializeField] private Vector3 tr_UV0;
    //     [SerializeField] private Vector3 br_UV0;
    //
    //     private Vector3 BLMax, BLMin;
    //     private Vector3 TLMax, TLMin;
    //     private Vector3 TRMax, TRMin;
    //     private Vector3 BRMax, BRMin;
    //
    //     public TMPMeshModifiers()
    //     {
    //     }
    //
    //     public TMPMeshModifiers(TMPMeshModifiers original)
    //     {
    //         positionDelta = original.PositionDelta;
    //         rotationDelta = original.RotationDelta;
    //         scaleDelta = original.ScaleDelta;
    //
    //         positionDeltaIsRaw = original.positionDeltaIsRaw;
    //
    //         bl_Delta = original.bl_Delta;
    //         tl_Delta = original.tl_Delta;
    //         tr_Delta = original.tr_Delta;
    //         br_Delta = original.br_Delta;
    //
    //         bl_DeltaIsRaw = original.bl_DeltaIsRaw;
    //         tl_DeltaIsRaw = original.tl_DeltaIsRaw;
    //         tr_DeltaIsRaw = original.tr_DeltaIsRaw;
    //         br_DeltaIsRaw = original.br_DeltaIsRaw;
    //
    //         bl_UV0 = original.bl_UV0;
    //         tl_UV0 = original.tl_UV0;
    //         tr_UV0 = original.tr_UV0;
    //         br_UV0 = original.br_UV0;
    //
    //         bl_Color = original.bl_Color;
    //         tl_Color = original.tl_Color;
    //         tr_Color = original.tr_Color;
    //         br_Color = original.br_Color;
    //
    //         Dirty = 0;
    //     }
    //
    //     public static TMPMeshModifiers operator +(TMPMeshModifiers lhs, TMPMeshModifiers rhs)
    //     {
    //         TMPMeshModifiers result = new TMPMeshModifiers();
    //
    //         result.PositionDelta = lhs.PositionDelta + rhs.PositionDelta;
    //         // result.ScaleDelta = lhs.ScaleDelta * rhs.ScaleDelta;
    //         result.RotationDelta = lhs.RotationDelta * rhs.RotationDelta;
    //
    //         result.BL_Delta = lhs.BL_Delta + rhs.BL_Delta;
    //         result.TL_Delta = lhs.TL_Delta + rhs.TL_Delta;
    //         result.TR_Delta = lhs.TR_Delta + rhs.TR_Delta;
    //         result.BR_Delta = lhs.BR_Delta + rhs.BR_Delta;
    //
    //         result.BL_Color = lhs.BL_Color + rhs.BL_Color; // TODO does thiswork
    //         result.TL_Color = lhs.TL_Color + rhs.TL_Color;
    //         result.TR_Color = lhs.TR_Color + rhs.TR_Color;
    //         result.BR_Color = lhs.BR_Color + rhs.BR_Color;
    //
    //         result.BL_UV0 = lhs.BL_UV0 + rhs.BL_UV0;
    //         result.TL_UV0 = lhs.TL_UV0 + rhs.TL_UV0;
    //         result.TR_UV0 = lhs.TR_UV0 + rhs.TR_UV0;
    //         result.BR_UV0 = lhs.BR_UV0 + rhs.BR_UV0;
    //         return result;
    //     }
    //
    //     // TODO Should be rewritable to lerp between two meshmodifiers?
    //     // Since all values are deltas (except colors) should be equivalent to
    //     // LerpUnclamped(modifiersWithEverythingZeroOrNull, targetModifier, t)
    //     public static TMPMeshModifiers LerpUnclamped(CharData cData, TMPMeshModifiers modifiers, float t)
    //     {
    //         TMPMeshModifiers result = new TMPMeshModifiers(modifiers);
    //
    //         if (modifiers.PositionDelta != Vector3.zero)
    //             result.PositionDelta = modifiers.PositionDelta * t;
    //
    //         if (modifiers.RotationDelta != Quaternion.identity)
    //             result.RotationDelta = Quaternion.LerpUnclamped(cData.InitialRotation, modifiers.RotationDelta, t);
    //
    //         // TODO I want to be able to use simple V3 scale -- why didnt i originally? some shit with order of applying?
    //         // if (step.Modifiers.ScaleDelta != Vector3.one)
    //
    //         if (modifiers.BL_Delta != Vector3.zero)
    //             result.BL_Delta = modifiers.BL_Delta * t;
    //
    //         if (modifiers.TL_Delta != Vector3.zero)
    //             result.TL_Delta = modifiers.TL_Delta * t;
    //
    //         if (modifiers.TR_Delta != Vector3.zero)
    //             result.TR_Delta = modifiers.TR_Delta * t;
    //
    //         if (modifiers.BR_Delta != Vector3.zero)
    //             result.BR_Delta = modifiers.BR_Delta * t;
    //
    //         if (modifiers.BL_Color.Override != ColorOverride.OverrideMode.None)
    //             result.BL_Color = ColorOverride.LerpUnclamped(cData.initialMesh.BL_Color, modifiers.BL_Color, t);
    //
    //         if (modifiers.TL_Color.Override != ColorOverride.OverrideMode.None)
    //             result.TL_Color = ColorOverride.LerpUnclamped(cData.initialMesh.TL_Color, modifiers.TL_Color, t);
    //
    //         if (modifiers.TR_Color.Override != ColorOverride.OverrideMode.None)
    //             result.TR_Color = ColorOverride.LerpUnclamped(cData.initialMesh.TR_Color, modifiers.TR_Color, t);
    //
    //         if (modifiers.BR_Color.Override != ColorOverride.OverrideMode.None)
    //             result.BR_Color = ColorOverride.LerpUnclamped(cData.initialMesh.BR_Color, modifiers.BR_Color, t);
    //
    //         if (modifiers.BL_UV0 != Vector3.zero)
    //             result.BL_UV0 = modifiers.BL_UV0 * t;
    //
    //         if (modifiers.TL_UV0 != Vector3.zero)
    //             result.TL_UV0 = modifiers.TL_UV0 * t;
    //
    //         if (modifiers.TR_UV0 != Vector3.zero)
    //             result.TR_UV0 = modifiers.TR_UV0 * t;
    //
    //         if (modifiers.BR_UV0 != Vector3.zero)
    //             result.BR_UV0 = modifiers.BR_UV0 * t;
    //
    //         return result;
    //
    //         // TMPMeshModifiers result = new TMPMeshModifiers();
    //         // result.PositionDelta = Vector3.LerpUnclamped(a.PositionDelta, b.PositionDelta, t);
    //         // // TODO scale
    //         // result.RotationDelta = Quaternion.LerpUnclamped(a.RotationDelta, b.RotationDelta, t);
    //         //
    //         // // TODO This actually doesnt make THAT much sense, since if you dont override delta etc
    //         // // you dont want it to be dirty (and therefore used)
    //         // result.BL_Delta = Vector3.LerpUnclamped(a.BL_Delta, b.BL_Delta, t);
    //         // result.TL_Delta = Vector3.LerpUnclamped(a.TL_Delta, b.TL_Delta, t);
    //         // result.TR_Delta = Vector3.LerpUnclamped(a.TR_Delta, b.TR_Delta, t);
    //         // result.BR_Delta = Vector3.LerpUnclamped(a.BR_Delta, b.BR_Delta, t);
    //         //
    //         // result.BL_Color.Value = b.BL_Color.HasValue
    //         //     ? (a.BL_Color.HasValue
    //         //         ? Color32.LerpUnclamped(a.BL_Color.Value, b.BL_Color.Value, t)
    //         //         : Color32.LerpUnclamped(cData.initialMesh.BL_Color, b.BL_Color.Value, t))
    //         //     
    //         //     : a.BL_Color.HasValue ? Color32.LerpUnclamped(a.BL_Color.Value, cData.initialMesh.BL_Color, t) : default;
    //     }
    //
    //     [Flags]
    //     public enum DirtyFlags : byte
    //     {
    //         None = 0,
    //         Vertices = 1,
    //         Colors = 1 << 1,
    //         UVs = 1 << 2,
    //         Position = 1 << 3,
    //         Scale = 1 << 4,
    //         Rotation = 1 << 5,
    //
    //         All = byte.MaxValue
    //     }
    //
    //     public DirtyFlags Dirty { get; private set; }
    //
    //     public void ClearDirtyFlags()
    //     {
    //         Dirty = DirtyFlags.None;
    //     }
    // }