using System;
using System.Collections.Generic;
using System.Linq;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;

[Serializable]
public struct Rotation
{
    public Vector3 pivot;
    public Vector3 eulerAngles;

    public Rotation(Vector3 eulerAngles, Vector3 pivot)
    {
        this.eulerAngles = eulerAngles;
        this.pivot = pivot;
    }
}

[Serializable]
public class TMPCharDataModifiers
{
    public TMPMeshModifiers2 meshModifiers;
    public TMPCharacterMeshModifiers characterMeshModifiers;

    public Vector3 BL_Result { get; private set; }
    public Vector3 TL_Result { get; private set; }
    public Vector3 TR_Result { get; private set; }
    public Vector3 BR_Result { get; private set; }

    public TMPCharDataModifiers()
    {
        meshModifiers = new TMPMeshModifiers2();
        characterMeshModifiers = new TMPCharacterMeshModifiers();
    }

    public void CalculateVertexPositions(CharData cData, IAnimatorContext context)
    {
         // Apply vertex transformations
        Vector3 vbl = cData.InitialMesh.BL_Position + meshModifiers.BL_Delta;
        Vector3 vtl = cData.InitialMesh.TL_Position + meshModifiers.TL_Delta;
        Vector3 vtr = cData.InitialMesh.TR_Position + meshModifiers.TR_Delta;
        Vector3 vbr = cData.InitialMesh.BR_Position + meshModifiers.BR_Delta;

        if (cData.InitialMesh.BL_Position != vbl) Debug.LogWarning("NOT EQWUAL: DELTA: " + meshModifiers.BL_Delta);

        // TODO Clamp
        // For now only the vertex offsets are clamped to min/max of each individual animation, as otherwise stacked animations are likely to deform the character
        // vtl = new Vector3(Mathf.Clamp(vtl.x, TLMin.x, TLMax.x), Mathf.Clamp(vtl.y, TLMin.y, TLMax.y), Mathf.Clamp(vtl.z, TLMin.z, TLMax.z));
        // vtr = new Vector3(Mathf.Clamp(vtr.x, TRMin.x, TRMax.x), Mathf.Clamp(vtr.y, TRMin.y, TRMax.y), Mathf.Clamp(vtr.z, TRMin.z, TRMax.z));
        // vbr = new Vector3(Mathf.Clamp(vbr.x, BRMin.x, BRMax.x), Mathf.Clamp(vbr.y, BRMin.y, BRMax.y), Mathf.Clamp(vbr.z, BRMin.z, BRMax.z));
        // vbl = new Vector3(Mathf.Clamp(vbl.x, BLMin.x, BLMax.x), Mathf.Clamp(vbl.y, BLMin.y, BLMax.y), Mathf.Clamp(vbl.z, BLMin.z, BLMax.z));

        // Apply scale
        vtl = characterMeshModifiers.ScaleDelta.MultiplyPoint3x4(vtl - cData.InitialPosition) + cData.InitialPosition;
        vtr = characterMeshModifiers.ScaleDelta.MultiplyPoint3x4(vtr - cData.InitialPosition) + cData.InitialPosition;
        vbr = characterMeshModifiers.ScaleDelta.MultiplyPoint3x4(vbr - cData.InitialPosition) + cData.InitialPosition;
        vbl = characterMeshModifiers.ScaleDelta.MultiplyPoint3x4(vbl - cData.InitialPosition) + cData.InitialPosition;

        // Apply rotation
        Vector3 pivot;
        Matrix4x4 matrix;
        foreach (var rot in characterMeshModifiers.Rotations)
        {
            pivot = rot.pivot;
            matrix = Matrix4x4.Rotate(Quaternion.Euler(rot.eulerAngles));
            // Debug.LogWarning("After rot pos with eulerangles " + rot.eulerAngles + ", vtl is " + (matrix.MultiplyPoint3x4(vtl - pivot) + pivot) + "; was " + vtl);

            vbl = matrix.MultiplyPoint3x4(vbl - pivot) + pivot;
            vtl = matrix.MultiplyPoint3x4(vtl - pivot) + pivot;
            vtr = matrix.MultiplyPoint3x4(vtr - pivot) + pivot;
            vbr = matrix.MultiplyPoint3x4(vbr - pivot) + pivot;
        }

        // Apply transformation
        var scaled = AnimationUtility.ScaleVector(characterMeshModifiers.PositionDelta, cData, context);
        vtl += scaled;
        vtr += scaled;
        vbr += scaled;
        vbl += scaled;

        BL_Result = vbl;
        TL_Result = vtl;
        TR_Result = vtr;
        BR_Result = vbr;
    }

    public void Reset()
    {
        meshModifiers.Reset();
        characterMeshModifiers.Reset();
    }
}

[Serializable]
public class TMPCharacterMeshModifiers
{
    public DirtyFlags Dirty => dirty;

    public Vector3 PositionDelta
    {
        get => positionDelta;
        set
        {
            if (value == positionDelta) return;
            positionDelta = value;
            dirty |= DirtyFlags.PositionDelta;
        }
    }

    public Matrix4x4 ScaleDelta
    {
        get => scaleDelta;
        set
        {
            if (value == scaleDelta) return;
            scaleDelta = value;
            dirty |= DirtyFlags.Scale;
        }
    }

    // TODO Either: Implement it so that you can add / remove rotations
    // and the dirty flag is set accordingly; OR, just dont allow setting
    // scale and positiondelta either
    public IEnumerable<Rotation> Rotations
    {
        get
        {
            for (int i = 0; i < rotations.Count; i++)
            {
                yield return rotations[i];
            }
        }
        set
        {
            rotations.Clear();
            rotations.AddRange(value);
            dirty |= DirtyFlags.Rotations;
        }
    }

    [SerializeField] private Vector3 positionDelta = Vector3.zero;
    [SerializeField] private Matrix4x4 scaleDelta = Matrix4x4.Scale(Vector3.one);
    [SerializeField] private List<Rotation> rotations = new List<Rotation>();
    private DirtyFlags dirty;

    public TMPCharacterMeshModifiers()
    {
    }

    public TMPCharacterMeshModifiers(TMPCharacterMeshModifiers original)
    {
        positionDelta = original.positionDelta;
        scaleDelta = original.scaleDelta;
        rotations = new List<Rotation>(original.rotations);
        dirty = 0;
    }

    public void Reset()
    {
        positionDelta = Vector3.zero;
        scaleDelta = Matrix4x4.Scale(Vector3.one);
        rotations.Clear();
    }

    public static TMPCharacterMeshModifiers operator +(TMPCharacterMeshModifiers lhs, TMPCharacterMeshModifiers rhs)
    {
        TMPCharacterMeshModifiers result = new TMPCharacterMeshModifiers();
        result.positionDelta = lhs.positionDelta + rhs.positionDelta;
        result.scaleDelta = lhs.ScaleDelta * rhs.ScaleDelta;
        result.rotations = lhs.rotations.Concat(rhs.rotations).ToList();
        return result;
    }

    [Flags]
    public enum DirtyFlags
    {
        PositionDelta = 1,
        Rotations = 1 << 1,
        Scale = 1 << 2
    }

    public void ClearDirtyFlags()
    {
        dirty = 0;
    }
}

[Serializable]
public class CharDataModifiers
{
    public Vector3 TL_Result { get; private set; }
    public Vector3 TR_Result { get; private set; }
    public Vector3 BR_Result { get; private set; }
    public Vector3 BL_Result { get; private set; }

    public TMPCharacterMeshModifiers CharacterMeshModifiers => characterMeshModifiers;
    public TMPMeshModifiers2 MeshModifiers => meshModifiers;

    [SerializeField] private TMPCharacterMeshModifiers characterMeshModifiers = new TMPCharacterMeshModifiers();
    [SerializeField] private TMPMeshModifiers2 meshModifiers = new TMPMeshModifiers2();

    private CharData cData;
    private IAnimatorContext context;

    public CharDataModifiers()
    {
        // positionDelta = Vector3.zero;
        // scaleDelta = Matrix4x4.Scale(Vector3.one);
        // rotations.Clear();
        // meshModifiers = new TMPMeshModifiers2();
    }

    public CharDataModifiers(CharDataModifiers original)
    {
        characterMeshModifiers = new TMPCharacterMeshModifiers(original.characterMeshModifiers);
        meshModifiers = new TMPMeshModifiers2(original.meshModifiers);
        cData = original.cData;
        context = original.context;
    }

    // TODO This has to be called before doing anything else to ensure cdata set
    // Probably change that somehow or just doucument very well
    public void Reset(CharData cData, IAnimatorContext context)
    {
        this.cData = cData;
        this.context = context;
        ClearCharModifiers();
        ClearMeshModifiers();
    }

    public static CharDataModifiers operator +(CharDataModifiers lhs, CharDataModifiers rhs)
    {
        CharDataModifiers result = new CharDataModifiers(lhs)
        {
            characterMeshModifiers = lhs.characterMeshModifiers + rhs.characterMeshModifiers,
            meshModifiers = lhs.meshModifiers + rhs.meshModifiers
        };
        return result;
    }

    public static CharDataModifiers LerpUnclamped(CharData cData, CharDataModifiers modifiers, float t)
    {
        CharDataModifiers result = new CharDataModifiers(modifiers);

        if (modifiers.characterMeshModifiers.PositionDelta != Vector3.zero)
            result.characterMeshModifiers.PositionDelta = modifiers.characterMeshModifiers.PositionDelta * t;

        List<Rotation> rotations = new List<Rotation>();
        foreach (var rot in modifiers.characterMeshModifiers.Rotations)
        {
            rotations.Add(
                // new Rotation(Quaternion.LerpUnclamped(cData.InitialRotation, rot.rotation, t),
                new Rotation(Vector3.LerpUnclamped(cData.InitialRotation.eulerAngles, rot.eulerAngles, t),
                    rot.pivot));
        }

        result.characterMeshModifiers.Rotations = rotations;

        Vector3 startScale = cData.InitialScale;
        Vector3 endScale = modifiers.characterMeshModifiers.ScaleDelta.lossyScale;
        Vector3 lerpedScale = Vector3.LerpUnclamped(startScale, endScale, t);
        result.characterMeshModifiers.ScaleDelta = Matrix4x4.Scale(lerpedScale);

        result.meshModifiers = LerpMeshModifiersUnclamped(cData, modifiers.meshModifiers, t);

        return result;
    }

    private static TMPMeshModifiers2 LerpMeshModifiersUnclamped(CharData cData, TMPMeshModifiers2 target, float t)
    {
        TMPMeshModifiers2 result = new TMPMeshModifiers2();

        if (target.BL_Delta != Vector3.zero)
            result.BL_Delta = target.BL_Delta * t;

        if (target.TL_Delta != Vector3.zero)
            result.TL_Delta = target.TL_Delta * t;

        if (target.TR_Delta != Vector3.zero)
            result.TR_Delta = target.TR_Delta * t;

        if (target.BR_Delta != Vector3.zero)
            result.BR_Delta = target.BR_Delta * t;

        if (target.BL_Color.Override != ColorOverride.OverrideMode.None)
            result.BL_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.BL_Color, target.BL_Color, t);

        if (target.TL_Color.Override != ColorOverride.OverrideMode.None)
            result.TL_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.TL_Color, target.TL_Color, t);

        if (target.TR_Color.Override != ColorOverride.OverrideMode.None)
            result.TR_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.TR_Color, target.TR_Color, t);

        if (target.BR_Color.Override != ColorOverride.OverrideMode.None)
            result.BR_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.BR_Color, target.BR_Color, t);

        if (target.BL_UV0.Override)
            result.BL_UV0 = new TMPMeshModifiers2.UVOverride(target.BL_UV0.OverrideValue * t);

        if (target.TL_UV0.Override)
            result.TL_UV0 = new TMPMeshModifiers2.UVOverride(target.TL_UV0.OverrideValue * t);

        if (target.TR_UV0.Override)
            result.TR_UV0 = new TMPMeshModifiers2.UVOverride(target.TR_UV0.OverrideValue * t);

        if (target.BR_UV0.Override)
            result.BR_UV0 = new TMPMeshModifiers2.UVOverride(target.BR_UV0.OverrideValue * t);

        if (target.BL_UV2.Override)
            result.BL_UV2 = new TMPMeshModifiers2.UVOverride(target.BL_UV2.OverrideValue * t);

        if (target.TL_UV2.Override)
            result.TL_UV2 = new TMPMeshModifiers2.UVOverride(target.TL_UV2.OverrideValue * t);

        if (target.TR_UV2.Override)
            result.TR_UV2 = new TMPMeshModifiers2.UVOverride(target.TR_UV2.OverrideValue * t);

        if (target.BR_UV2.Override)
            result.BR_UV2 = new TMPMeshModifiers2.UVOverride(target.BR_UV2.OverrideValue * t);

        return result;
    }

    public void CalculateVertexPositions()
    {
        // Apply vertex transformations
        Vector3 vbl = cData.InitialMesh.BL_Position + meshModifiers.BL_Delta;
        Vector3 vtl = cData.InitialMesh.TL_Position + meshModifiers.TL_Delta;
        Vector3 vtr = cData.InitialMesh.TR_Position + meshModifiers.TR_Delta;
        Vector3 vbr = cData.InitialMesh.BR_Position + meshModifiers.BR_Delta;

        if (cData.InitialMesh.BL_Position != vbl) Debug.LogWarning("NOT EQWUAL: DELTA: " + meshModifiers.BL_Delta);

        // TODO Clamp
        // For now only the vertex offsets are clamped to min/max of each individual animation, as otherwise stacked animations are likely to deform the character
        // vtl = new Vector3(Mathf.Clamp(vtl.x, TLMin.x, TLMax.x), Mathf.Clamp(vtl.y, TLMin.y, TLMax.y), Mathf.Clamp(vtl.z, TLMin.z, TLMax.z));
        // vtr = new Vector3(Mathf.Clamp(vtr.x, TRMin.x, TRMax.x), Mathf.Clamp(vtr.y, TRMin.y, TRMax.y), Mathf.Clamp(vtr.z, TRMin.z, TRMax.z));
        // vbr = new Vector3(Mathf.Clamp(vbr.x, BRMin.x, BRMax.x), Mathf.Clamp(vbr.y, BRMin.y, BRMax.y), Mathf.Clamp(vbr.z, BRMin.z, BRMax.z));
        // vbl = new Vector3(Mathf.Clamp(vbl.x, BLMin.x, BLMax.x), Mathf.Clamp(vbl.y, BLMin.y, BLMax.y), Mathf.Clamp(vbl.z, BLMin.z, BLMax.z));

        // Apply scale
        vtl = characterMeshModifiers.ScaleDelta.MultiplyPoint3x4(vtl - cData.InitialPosition) + cData.InitialPosition;
        vtr = characterMeshModifiers.ScaleDelta.MultiplyPoint3x4(vtr - cData.InitialPosition) + cData.InitialPosition;
        vbr = characterMeshModifiers.ScaleDelta.MultiplyPoint3x4(vbr - cData.InitialPosition) + cData.InitialPosition;
        vbl = characterMeshModifiers.ScaleDelta.MultiplyPoint3x4(vbl - cData.InitialPosition) + cData.InitialPosition;

        // Apply rotation
        Vector3 pivot;
        Matrix4x4 matrix;
        foreach (var rot in characterMeshModifiers.Rotations)
        {
            pivot = rot.pivot;
            matrix = Matrix4x4.Rotate(Quaternion.Euler(rot.eulerAngles));
            // Debug.LogWarning("After rot pos with eulerangles " + rot.eulerAngles + ", vtl is " + (matrix.MultiplyPoint3x4(vtl - pivot) + pivot) + "; was " + vtl);

            vbl = matrix.MultiplyPoint3x4(vbl - pivot) + pivot;
            vtl = matrix.MultiplyPoint3x4(vtl - pivot) + pivot;
            vtr = matrix.MultiplyPoint3x4(vtr - pivot) + pivot;
            vbr = matrix.MultiplyPoint3x4(vbr - pivot) + pivot;
        }

        // Apply transformation
        var scaled = AnimationUtility.ScaleVector(characterMeshModifiers.PositionDelta, cData, context);
        vtl += scaled;
        vtr += scaled;
        vbr += scaled;
        vbl += scaled;

        BL_Result = vbl;
        TL_Result = vtl;
        TR_Result = vtr;
        BR_Result = vbr;
    }

    /*
     * TODO
     * ApplyToCharData is used by TMPAnimator to set the CharData to use the state of the modifiers; apply the animation essentially
     * ApplyToCharData2 is used by GenericAnimation to set the CharData to use this as well, but not directly to the vertices
     *
     * Update got rid of applytochardata (and renamed 2)
     * Might not have to split class further?
     */
    public void ApplyToCharData()
    {
        cData.SetPosition(cData.InitialPosition + characterMeshModifiers.PositionDelta);
        cData.SetScale(characterMeshModifiers.ScaleDelta.lossyScale);

        Quaternion finalRotation = Quaternion.identity;
        Vector3 finalPivot = Vector3.zero;
        Matrix4x4 matrix;

        // TODO There is no way to combine rotations with different pivots
        // TODO => Update CharData to use a list for rotations as well!
        foreach (var rot in characterMeshModifiers.Rotations)
        {
            finalPivot += rot.pivot;
            finalRotation = Quaternion.Euler(rot.eulerAngles) * finalRotation;
        }

        cData.SetPivot(finalPivot);
        cData.SetRotation(finalRotation);

        cData.mesh.SetPosition(0, cData.InitialMesh.BL_Position + meshModifiers.BL_Delta);
        cData.mesh.SetPosition(1, cData.InitialMesh.TL_Position + meshModifiers.TL_Delta);
        cData.mesh.SetPosition(2, cData.InitialMesh.TR_Position + meshModifiers.TR_Delta);
        cData.mesh.SetPosition(3, cData.InitialMesh.BR_Position + meshModifiers.BR_Delta);

        cData.mesh.SetColor(0, meshModifiers.BL_Color.GetValue(cData.InitialMesh.GetColor(0)));
        cData.mesh.SetColor(1, meshModifiers.TL_Color.GetValue(cData.InitialMesh.GetColor(1)));
        cData.mesh.SetColor(2, meshModifiers.TR_Color.GetValue(cData.InitialMesh.GetColor(2)));
        cData.mesh.SetColor(3, meshModifiers.BR_Color.GetValue(cData.InitialMesh.GetColor(3)));

        cData.mesh.SetUV0(0, meshModifiers.BL_UV0.GetValue(cData.InitialMesh.GetUV0(0)));
        cData.mesh.SetUV0(1, meshModifiers.TL_UV0.GetValue(cData.InitialMesh.GetUV0(1)));
        cData.mesh.SetUV0(2, meshModifiers.TR_UV0.GetValue(cData.InitialMesh.GetUV0(2)));
        cData.mesh.SetUV0(3, meshModifiers.BR_UV0.GetValue(cData.InitialMesh.GetUV0(3)));

        cData.mesh.SetUV2(0, meshModifiers.BL_UV2.GetValue(cData.InitialMesh.GetUV2(0)));
        cData.mesh.SetUV2(1, meshModifiers.TL_UV2.GetValue(cData.InitialMesh.GetUV2(1)));
        cData.mesh.SetUV2(2, meshModifiers.TR_UV2.GetValue(cData.InitialMesh.GetUV2(2)));
        cData.mesh.SetUV2(3, meshModifiers.BR_UV2.GetValue(cData.InitialMesh.GetUV2(3)));
    }

    public void PushCharacterMeshModifiersIntoVertexMeshModifiers()
    {
        CalculateVertexPositions();
        meshModifiers.BL_Delta = BL_Result - cData.InitialMesh.BL_Position;
        meshModifiers.TL_Delta = TL_Result - cData.InitialMesh.TL_Position;
        meshModifiers.TR_Delta = TR_Result - cData.InitialMesh.TR_Position;
        meshModifiers.BR_Delta = BR_Result - cData.InitialMesh.BR_Position;
        ClearCharModifiers();
    }

    private void ClearCharModifiers()
    {
        characterMeshModifiers.PositionDelta = Vector3.zero;
        characterMeshModifiers.ScaleDelta = Matrix4x4.Scale(Vector3.one);
        characterMeshModifiers.Rotations = new List<Rotation>();
        // characterMeshModifiers.ClearDirtyFlags();
    }

    private void ClearMeshModifiers()
    {
        meshModifiers = new TMPMeshModifiers2();

        meshModifiers.BL_UV0 = new TMPMeshModifiers2.UVOverride(cData.InitialMesh.GetUV0(0));
        meshModifiers.TL_UV0 = new TMPMeshModifiers2.UVOverride(cData.InitialMesh.GetUV0(1));
        meshModifiers.TR_UV0 = new TMPMeshModifiers2.UVOverride(cData.InitialMesh.GetUV0(2));
        meshModifiers.BR_UV0 = new TMPMeshModifiers2.UVOverride(cData.InitialMesh.GetUV0(3));

        meshModifiers.BL_UV2 = new TMPMeshModifiers2.UVOverride(cData.InitialMesh.GetUV2(0));
        meshModifiers.TL_UV2 = new TMPMeshModifiers2.UVOverride(cData.InitialMesh.GetUV2(1));
        meshModifiers.TR_UV2 = new TMPMeshModifiers2.UVOverride(cData.InitialMesh.GetUV2(2));
        meshModifiers.BR_UV2 = new TMPMeshModifiers2.UVOverride(cData.InitialMesh.GetUV2(3));
        // meshModifiers.ClearDirtyFlags();
    }

    // Update values based on current chardata's state
    public void UpdateFromCharDataState()
    {
        if (cData.positionDirty)
            characterMeshModifiers.PositionDelta += (cData.Position - cData.InitialPosition);

        if (cData.scaleDirty)
            characterMeshModifiers.ScaleDelta *= Matrix4x4.Scale(cData.Scale);

        if (cData.rotationDirty)
        {
            if (cData.Rotation != Quaternion.identity || cData.Rotation.eulerAngles != Vector3.zero)
            {
                var scaled = cData.InitialPosition +
                             AnimationUtility.ScaleVector((cData.RotationPivot - cData.InitialPosition), cData,
                                 context);
                Rotation rot = new Rotation(cData.Rotation.eulerAngles, scaled);

                // TODO this is not okay lol
                characterMeshModifiers.Rotations =
                    characterMeshModifiers.Rotations.Concat(new List<Rotation>() { rot }).ToList();
                // rotations.Add(new Rotation(cData.Rotation.eulerAngles, scaled));
            }
        }

        if (cData.verticesDirty)
        {
            Vector3 deltaTL;
            Vector3 deltaTR;
            Vector3 deltaBR;
            Vector3 deltaBL;

            deltaTL = AnimationUtility.ScaleVector(cData.mesh.TL_Position - cData.mesh.initial.TL_Position, cData,
                context);
            deltaTR = AnimationUtility.ScaleVector(cData.mesh.TR_Position - cData.mesh.initial.TR_Position, cData,
                context);
            deltaBR = AnimationUtility.ScaleVector(cData.mesh.BR_Position - cData.mesh.initial.BR_Position, cData,
                context);
            deltaBL = AnimationUtility.ScaleVector(cData.mesh.BL_Position - cData.mesh.initial.BL_Position, cData,
                context);

            meshModifiers.BL_Delta += deltaBL;
            meshModifiers.TL_Delta += deltaTL;
            meshModifiers.TR_Delta += deltaTR;
            meshModifiers.BR_Delta += deltaBR;

            // TLMax = new Vector3(Mathf.Max(cData.mesh.initial.TL_Position.x + deltaTL.x, TLMax.x), Mathf.Max(cData.mesh.initial.TL_Position.y + deltaTL.y, TLMax.y), Mathf.Max(cData.mesh.initial.TL_Position.z + deltaTL.z, TLMax.z));
            // TRMax = new Vector3(Mathf.Max(cData.mesh.initial.TR_Position.x + deltaTR.x, TRMax.x), Mathf.Max(cData.mesh.initial.TR_Position.y + deltaTR.y, TRMax.y), Mathf.Max(cData.mesh.initial.TR_Position.z + deltaTR.z, TRMax.z));
            // BRMax = new Vector3(Mathf.Max(cData.mesh.initial.BR_Position.x + deltaBR.x, BRMax.x), Mathf.Max(cData.mesh.initial.BR_Position.y + deltaBR.y, BRMax.y), Mathf.Max(cData.mesh.initial.BR_Position.z + deltaBR.z, BRMax.z));
            // BLMax = new Vector3(Mathf.Max(cData.mesh.initial.BL_Position.x + deltaBL.x, BLMax.x), Mathf.Max(cData.mesh.initial.BL_Position.y + deltaBL.y, BLMax.y), Mathf.Max(cData.mesh.initial.BL_Position.z + deltaBL.z, BLMax.z));
            //
            // TLMin = new Vector3(Mathf.Min(cData.mesh.initial.TL_Position.x + deltaTL.x, TLMin.x), Mathf.Min(cData.mesh.initial.TL_Position.y + deltaTL.y, TLMin.y), Mathf.Min(cData.mesh.initial.TL_Position.z + deltaTL.z, TLMin.z));
            // TRMin = new Vector3(Mathf.Min(cData.mesh.initial.TR_Position.x + deltaTR.x, TRMin.x), Mathf.Min(cData.mesh.initial.TR_Position.y + deltaTR.y, TRMin.y), Mathf.Min(cData.mesh.initial.TR_Position.z + deltaTR.z, TRMin.z));
            // BRMin = new Vector3(Mathf.Min(cData.mesh.initial.BR_Position.x + deltaBR.x, BRMin.x), Mathf.Min(cData.mesh.initial.BR_Position.y + deltaBR.y, BRMin.y), Mathf.Min(cData.mesh.initial.BR_Position.z + deltaBR.z, BRMin.z));
            // BLMin = new Vector3(Mathf.Min(cData.mesh.initial.BL_Position.x + deltaBL.x, BLMin.x), Mathf.Min(cData.mesh.initial.BL_Position.y + deltaBL.y, BLMin.y), Mathf.Min(cData.mesh.initial.BL_Position.z + deltaBL.z, BLMin.z));
        }

        // General TODO
        // You should be able to only set color on vertex
        // => Dirty flags for each individual vertx in VertexData
        if (cData.colorsDirty)
        {
            ColorOverride.OverrideMode mode = ColorOverride.OverrideMode.Color;

            if (cData.alphasDirty)
                mode |= ColorOverride.OverrideMode.Alpha;

            meshModifiers.BL_Color = new ColorOverride(cData.mesh.GetColor(0), mode);
            meshModifiers.TL_Color = new ColorOverride(cData.mesh.GetColor(1), mode);
            meshModifiers.TR_Color = new ColorOverride(cData.mesh.GetColor(2), mode);
            meshModifiers.BR_Color = new ColorOverride(cData.mesh.GetColor(3), mode);
        }
        else if (cData.alphasDirty)
        {
            ColorOverride.OverrideMode mode = ColorOverride.OverrideMode.Alpha;
            meshModifiers.BL_Color = new ColorOverride(cData.mesh.GetColor(0), mode);
            meshModifiers.TL_Color = new ColorOverride(cData.mesh.GetColor(1), mode);
            meshModifiers.TR_Color = new ColorOverride(cData.mesh.GetColor(2), mode);
            meshModifiers.BR_Color = new ColorOverride(cData.mesh.GetColor(3), mode);
        }

        if (cData.uvsDirty)
        {
            meshModifiers.BL_UV0 = new TMPMeshModifiers2.UVOverride(cData.mesh.GetUV0(0));
            meshModifiers.TL_UV0 = new TMPMeshModifiers2.UVOverride(cData.mesh.GetUV0(1));
            meshModifiers.TR_UV0 = new TMPMeshModifiers2.UVOverride(cData.mesh.GetUV0(2));
            meshModifiers.BR_UV0 = new TMPMeshModifiers2.UVOverride(cData.mesh.GetUV0(3));

            meshModifiers.BL_UV2 = new TMPMeshModifiers2.UVOverride(cData.mesh.GetUV2(0));
            meshModifiers.TL_UV2 = new TMPMeshModifiers2.UVOverride(cData.mesh.GetUV2(1));
            meshModifiers.TR_UV2 = new TMPMeshModifiers2.UVOverride(cData.mesh.GetUV2(2));
            meshModifiers.BR_UV2 = new TMPMeshModifiers2.UVOverride(cData.mesh.GetUV2(3));
        }
    }
}