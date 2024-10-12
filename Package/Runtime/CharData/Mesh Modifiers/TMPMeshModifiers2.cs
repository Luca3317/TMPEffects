using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public struct TMPMeshModifiers2
{
    public Vector3 BL_Delta
    {
        get => bl_Delta;
        set
        {
            if (value == bl_Delta) return;
            bl_Delta = value;
            dirty |= DirtyFlags.Deltas;
        }
    }

    public Vector3 TL_Delta
    {
        get => tl_Delta;
        set
        {
            if (value == tl_Delta) return;
            tl_Delta = value;
            dirty |= DirtyFlags.Deltas;
        }
    }

    public Vector3 TR_Delta
    {
        get => tr_Delta;
        set
        {
            if (value == tr_Delta) return;
            tr_Delta = value;
            dirty |= DirtyFlags.Deltas;
        }
    }

    public Vector3 BR_Delta
    {
        get => br_Delta;
        set
        {
            if (value == br_Delta) return;
            br_Delta = value;
            dirty |= DirtyFlags.Deltas;
        }
    }

    public ColorOverride BL_Color
    {
        get { return bl_Color; }
        set
        {
            if (value.Equals(bl_Color)) return;
            bl_Color = value;
            dirty |= DirtyFlags.Colors;
        }
    }

    public ColorOverride TL_Color
    {
        get { return tl_Color; }
        set
        {
            if (value.Equals(tl_Color)) return;
            tl_Color = value;
            dirty |= DirtyFlags.Colors;
        }
    }

    public ColorOverride TR_Color
    {
        get { return tr_Color; }
        set
        {
            if (value.Equals(tr_Color)) return;
            tr_Color = value;
            dirty |= DirtyFlags.Colors;
        }
    }

    public ColorOverride BR_Color
    {
        get { return br_Color; }
        set
        {
            if (value.Equals(br_Color)) return;
            br_Color = value;
            dirty |= DirtyFlags.Colors;
        }
    }

    public UVOverride BL_UV0
    {
        get => bl_UV0;
        set
        {
            if (value == bl_UV0) return;
            bl_UV0 = value;
            dirty |= DirtyFlags.UVs;
        }
    }

    public UVOverride TL_UV0
    {
        get => tl_UV0;
        set
        {
            if (value == tl_UV0) return;
            tl_UV0 = value;
            dirty |= DirtyFlags.UVs;
        }
    }

    public UVOverride TR_UV0
    {
        get => tr_UV0;
        set
        {
            if (value == tr_UV0) return;
            tr_UV0 = value;
            dirty |= DirtyFlags.UVs;
        }
    }

    public UVOverride BR_UV0
    {
        get => br_UV0;
        set
        {
            if (value == br_UV0) return;
            br_UV0 = value;
            dirty |= DirtyFlags.UVs;
        }
    }

    public UVOverride BL_UV2
    {
        get => bl_UV2;
        set
        {
            if (value == bl_UV2) return;
            bl_UV2 = value;
            dirty |= DirtyFlags.UVs;
        }
    }

    public UVOverride TL_UV2
    {
        get => tl_UV2;
        set
        {
            if (value == tl_UV2) return;
            tl_UV2 = value;
            dirty |= DirtyFlags.UVs;
        }
    }

    public UVOverride TR_UV2
    {
        get => tr_UV2;
        set
        {
            if (value == tr_UV2) return;
            tr_UV2 = value;
            dirty |= DirtyFlags.UVs;
        }
    }

    public UVOverride BR_UV2
    {
        get => br_UV2;
        set
        {
            if (value == br_UV2) return;
            br_UV2 = value;
            dirty |= DirtyFlags.UVs;
        }
    }

    [SerializeField] private Vector3 bl_Delta;
    [SerializeField] private Vector3 tl_Delta;
    [SerializeField] private Vector3 tr_Delta;
    [SerializeField] private Vector3 br_Delta;

    [SerializeField] private ColorOverride bl_Color;
    [SerializeField] private ColorOverride tl_Color;
    [SerializeField] private ColorOverride tr_Color;
    [SerializeField] private ColorOverride br_Color;

    [SerializeField] private UVOverride bl_UV0;
    [SerializeField] private UVOverride tl_UV0;
    [SerializeField] private UVOverride tr_UV0;
    [SerializeField] private UVOverride br_UV0;

    [SerializeField] private UVOverride bl_UV2;
    [SerializeField] private UVOverride tl_UV2;
    [SerializeField] private UVOverride tr_UV2;
    [SerializeField] private UVOverride br_UV2;

    public DirtyFlags Dirty => dirty;
    [SerializeField] private DirtyFlags dirty;

    [Serializable]
    public struct UVOverride
    {
        public Vector3 OverrideValue => _overrideValue;
        public bool Override => _override;

        private bool _override;
        private Vector3 _overrideValue;

        public static UVOverride Default = new UVOverride(null);
        public static UVOverride GetDefault => Default;

        public UVOverride(Vector3? overrideValue = null)
        {
            if (overrideValue.HasValue)
            {
                _overrideValue = overrideValue.Value;
                _override = true;
            }
            else
            {
                _overrideValue = Vector3.zero;
                _override = false;
            }
        }

        public Vector3 GetValue(Vector3 fallback)
        {
            if (Override) return OverrideValue;
            return fallback;
        }

        public static bool operator ==(UVOverride a, UVOverride b)
        {
            return a.Override == b.Override && a.OverrideValue == b.OverrideValue;
        }

        public static bool operator !=(UVOverride a, UVOverride b)
        {
            return a.Override != b.Override || a.OverrideValue != b.OverrideValue;
        }

        public static UVOverride operator +(UVOverride a, UVOverride b)
        {
            if (a.Override)
            {
                if (b.Override)
                    return new UVOverride(a.OverrideValue + b.OverrideValue);

                return new UVOverride(a.OverrideValue);
            }

            if (b.Override)
                return new UVOverride(b.OverrideValue);

            return new UVOverride();
        }
    } 
    
    public TMPMeshModifiers2(TMPMeshModifiers2 original)
    {
        bl_Delta = original.bl_Delta;
        tl_Delta = original.tl_Delta;
        tr_Delta = original.tr_Delta;
        br_Delta = original.br_Delta;

        bl_UV0 = original.bl_UV0;
        tl_UV0 = original.tl_UV0;
        tr_UV0 = original.tr_UV0;
        br_UV0 = original.br_UV0;

        bl_UV2 = original.bl_UV2;
        tl_UV2 = original.tl_UV2;
        tr_UV2 = original.tr_UV2;
        br_UV2 = original.br_UV2;

        bl_Color = original.bl_Color;
        tl_Color = original.tl_Color;
        tr_Color = original.tr_Color;
        br_Color = original.br_Color;

        dirty = original.Dirty;
    }

    public void Reset()
    {
        bl_Delta = Vector3.zero;
        tl_Delta = Vector3.zero;
        tr_Delta = Vector3.zero;
        br_Delta = Vector3.zero;

        bl_Color = new ColorOverride();
        tl_Color = new ColorOverride();
        tr_Color = new ColorOverride();
        br_Color = new ColorOverride();

        bl_UV0 = UVOverride.GetDefault; //new UVOverride(null);
        tl_UV0 = UVOverride.GetDefault; //new UVOverride(null);
        tr_UV0 = UVOverride.GetDefault; //new UVOverride(null);
        br_UV0 = UVOverride.GetDefault; //new UVOverride(null);

        bl_UV2 = UVOverride.GetDefault; //new UVOverride(null);
        tl_UV2 = UVOverride.GetDefault; //new UVOverride(null);
        tr_UV2 = UVOverride.GetDefault; //new UVOverride(null);
        br_UV2 = UVOverride.GetDefault; //new UVOverride(null);

        dirty = 0;
    }

    public void Combine(TMPMeshModifiers2 other, bool useFlags = true)
    {
        if (!useFlags)
        {
            BL_Delta += other.BL_Delta;
            TL_Delta += other.TL_Delta;
            TR_Delta += other.TR_Delta;
            BR_Delta += other.BR_Delta;
            
            BL_Color += other.BL_Color;
            TL_Color += other.TL_Color;
            TR_Color += other.TR_Color;
            BR_Color += other.BR_Color;
            
            BL_UV0 += other.BL_UV0;
            TL_UV0 += other.TL_UV0;
            TR_UV0 += other.TR_UV0;
            BR_UV0 += other.BR_UV0;
            
            BL_UV2 += other.BL_UV2;
            TL_UV2 += other.TL_UV2;
            TR_UV2 += other.TR_UV2;
            BR_UV2 += other.BR_UV2;
            
            return;
        }
        
        if (other.dirty.HasFlag(DirtyFlags.Deltas))
        {
            BL_Delta += other.BL_Delta;
            TL_Delta += other.TL_Delta;
            TR_Delta += other.TR_Delta;
            BR_Delta += other.BR_Delta;
        }

        if (other.dirty.HasFlag(DirtyFlags.Colors))
        {
            BL_Color += other.BL_Color; // TODO does thiswork
            TL_Color += other.TL_Color;
            TR_Color += other.TR_Color;
            BR_Color += other.BR_Color;
        }

        if (other.dirty.HasFlag(DirtyFlags.UVs))
        {
            BL_UV0 += other.BL_UV0;
            TL_UV0 += other.TL_UV0;
            TR_UV0 += other.TR_UV0;
            BR_UV0 += other.BR_UV0;
            
            BL_UV2 += other.BL_UV2;
            TL_UV2 += other.TL_UV2;
            TR_UV2 += other.TR_UV2;
            BR_UV2 += other.BR_UV2;
        }

        dirty |= other.dirty;
    }

    public static TMPMeshModifiers2 operator +(TMPMeshModifiers2 lhs, TMPMeshModifiers2 rhs)
    {
        if (rhs.dirty.HasFlag(DirtyFlags.Deltas))
        {
            lhs.BL_Delta += rhs.BL_Delta;
            lhs.TL_Delta += rhs.TL_Delta;
            lhs.TR_Delta += rhs.TR_Delta;
            lhs.BR_Delta += rhs.BR_Delta;
        }

        if (rhs.dirty.HasFlag(DirtyFlags.Colors))
        {
            lhs.BL_Color += rhs.BL_Color; // TODO does thiswork
            lhs.TL_Color += rhs.TL_Color;
            lhs.TR_Color += rhs.TR_Color;
            lhs.BR_Color += rhs.BR_Color;
        }

        if (rhs.dirty.HasFlag(DirtyFlags.UVs))
        {
            lhs.BL_UV0 += rhs.BL_UV0;
            lhs.TL_UV0 += rhs.TL_UV0;
            lhs.TR_UV0 += rhs.TR_UV0;
            lhs.BR_UV0 += rhs.BR_UV0;
        }
        return lhs;
    }

    [Flags]
    public enum DirtyFlags : byte
    {
        None = 0,
        Deltas = 1,
        Colors = 1 << 1,
        UVs = 1 << 2,

        All = byte.MaxValue
    }
}


// [Serializable]
// public class TMPMeshModifiers2
// {
//     public Vector3 BL_Delta
//     {
//         get => bl_Delta;
//         set
//         {
//             if (value == bl_Delta) return;
//             bl_Delta = value;
//             Dirty |= DirtyFlags.Deltas;
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
//             Dirty |= DirtyFlags.Deltas;
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
//             Dirty |= DirtyFlags.Deltas;
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
//             Dirty |= DirtyFlags.Deltas;
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
//     public UVOverride BL_UV0
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
//     public UVOverride TL_UV0
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
//     public UVOverride TR_UV0
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
//     public UVOverride BR_UV0
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
//     public UVOverride BL_UV2
//     {
//         get => bl_UV2;
//         set
//         {
//             if (value == bl_UV2) return;
//             bl_UV2 = value;
//             Dirty |= DirtyFlags.UVs;
//         }
//     }
//
//     public UVOverride TL_UV2
//     {
//         get => tl_UV2;
//         set
//         {
//             if (value == tl_UV2) return;
//             tl_UV2 = value;
//             Dirty |= DirtyFlags.UVs;
//         }
//     }
//
//     public UVOverride TR_UV2
//     {
//         get => tr_UV2;
//         set
//         {
//             if (value == tr_UV2) return;
//             tr_UV2 = value;
//             Dirty |= DirtyFlags.UVs;
//         }
//     }
//
//     public UVOverride BR_UV2
//     {
//         get => br_UV2;
//         set
//         {
//             if (value == br_UV2) return;
//             br_UV2 = value;
//             Dirty |= DirtyFlags.UVs;
//         }
//     }
//
//     [SerializeField] private Vector3 bl_Delta;
//     [SerializeField] private Vector3 tl_Delta;
//     [SerializeField] private Vector3 tr_Delta;
//     [SerializeField] private Vector3 br_Delta;
//
//     [SerializeField] private ColorOverride bl_Color;
//     [SerializeField] private ColorOverride tl_Color;
//     [SerializeField] private ColorOverride tr_Color;
//     [SerializeField] private ColorOverride br_Color;
//
//     [SerializeField] private UVOverride bl_UV0;
//     [SerializeField] private UVOverride tl_UV0;
//     [SerializeField] private UVOverride tr_UV0;
//     [SerializeField] private UVOverride br_UV0;
//
//     [SerializeField] private UVOverride bl_UV2;
//     [SerializeField] private UVOverride tl_UV2;
//     [SerializeField] private UVOverride tr_UV2;
//     [SerializeField] private UVOverride br_UV2;
//
//     private DirtyFlags Dirty;
//
//     [Serializable]
//     public struct UVOverride
//     {
//         public Vector3 OverrideValue => _overrideValue;
//         public bool Override => _override;
//
//         private bool _override;
//         private Vector3 _overrideValue;
//
//         public UVOverride(Vector3? overrideValue = null)
//         {
//             if (overrideValue.HasValue)
//             {
//                 _overrideValue = overrideValue.Value;
//                 _override = true;
//             }
//             else
//             {
//                 _overrideValue = Vector3.zero;
//                 _override = false;
//             }
//         }
//
//         public Vector3 GetValue(Vector3 fallback)
//         {
//             if (Override) return OverrideValue;
//             return fallback;
//         }
//
//         public static bool operator ==(UVOverride a, UVOverride b)
//         {
//             return a.Override == b.Override && a.OverrideValue == b.OverrideValue;
//         }
//
//         public static bool operator !=(UVOverride a, UVOverride b)
//         {
//             return a.Override != b.Override || a.OverrideValue != b.OverrideValue;
//         }
//
//         public static UVOverride operator +(UVOverride a, UVOverride b)
//         {
//             if (a.Override)
//             {
//                 if (b.Override)
//                     return new UVOverride(a.OverrideValue + b.OverrideValue);
//
//                 return new UVOverride(a.OverrideValue);
//             }
//
//             if (b.Override)
//                 return new UVOverride(b.OverrideValue);
//
//             return new UVOverride();
//         }
//     }
//
//     public TMPMeshModifiers2()
//     {
//     }
//
//     public TMPMeshModifiers2(TMPMeshModifiers2 original)
//     {
//         bl_Delta = original.bl_Delta;
//         tl_Delta = original.tl_Delta;
//         tr_Delta = original.tr_Delta;
//         br_Delta = original.br_Delta;
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
//     public void Reset()
//     {
//         bl_Delta = Vector3.zero;
//         tl_Delta = Vector3.zero;
//         tr_Delta = Vector3.zero;
//         br_Delta = Vector3.zero;
//         
//         bl_Color = new ColorOverride();
//         tl_Color = new ColorOverride();
//         tr_Color = new ColorOverride();
//         br_Color = new ColorOverride();
//         
//         bl_UV0 = new UVOverride(null);
//         tl_UV0 = new UVOverride(null);
//         tr_UV0 = new UVOverride(null);
//         br_UV0 = new UVOverride(null);
//         
//         bl_UV2 = new UVOverride(null);
//         tl_UV2 = new UVOverride(null);
//         tr_UV2 = new UVOverride(null);
//         br_UV2 = new UVOverride(null);
//     }
//
//     public static TMPMeshModifiers2 operator +(TMPMeshModifiers2 lhs, TMPMeshModifiers2 rhs)
//     {
//         TMPMeshModifiers2 result = new TMPMeshModifiers2();
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
//     [Flags]
//     public enum DirtyFlags : byte
//     {
//         None = 0,
//         Deltas = 1,
//         Colors = 1 << 1,
//         UVs = 1 << 2,
//
//         All = byte.MaxValue
//     }
// }