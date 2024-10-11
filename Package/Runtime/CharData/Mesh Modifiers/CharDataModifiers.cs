using System;
using System.Collections.Generic;
using System.Linq;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;

// TODO implement israw for position + for vertex deltas in tmpmeshmodifiers2
[Serializable]
public class CharDataModifiers
{
    public Vector3 PositionDelta => positionDelta;
    public Matrix4x4 ScaleDelta => scaleDelta;

    public IEnumerable<Rotation> Rotations
    {
        get
        {
            for (int i = 0; i < rotations.Count; i++)
            {
                yield return rotations[i];
            }
        }
    }

    [Serializable]
    public struct Rotation
    {
        public Vector3 pivot;
        public Quaternion rotation;

        public Rotation(Quaternion rotation, Vector3 pivot)
        {
            this.rotation = rotation;
            this.pivot = pivot;
        }
    }

    [SerializeField] private Vector3 positionDelta;
    [SerializeField] private Matrix4x4 scaleDelta;
    [SerializeField] private List<Rotation> rotations = new List<Rotation>();
    [SerializeField] private TMPMeshModifiers2 meshModifiers = new TMPMeshModifiers2();

    private CharData cData;
    private IAnimatorContext context;

    public Vector3 TL_Result { get; private set; }
    public Vector3 TR_Result { get; private set; }
    public Vector3 BR_Result { get; private set; }
    public Vector3 BL_Result { get; private set; }

    public CharDataModifiers()
    {
        positionDelta = Vector3.zero;
        scaleDelta = Matrix4x4.Scale(Vector3.one);
        rotations.Clear();
    }

    public CharDataModifiers(CharDataModifiers original)
    {
        positionDelta = original.PositionDelta;
        rotations = new List<Rotation>(original.rotations);
        scaleDelta = original.ScaleDelta;

        meshModifiers = new TMPMeshModifiers2(original.meshModifiers);
    }

    public static CharDataModifiers operator +(CharDataModifiers lhs, CharDataModifiers rhs)
    {
        CharDataModifiers result = new CharDataModifiers();

        result.positionDelta = lhs.PositionDelta + rhs.PositionDelta;
        result.scaleDelta = lhs.ScaleDelta * rhs.ScaleDelta;
        result.rotations = lhs.rotations.Concat(rhs.rotations).ToList();

        result.meshModifiers = lhs.meshModifiers + rhs.meshModifiers;
        return result;
    }

    public static CharDataModifiers LerpUnclamped(CharData cData, CharDataModifiers modifiers, float t)
    {
        CharDataModifiers result = new CharDataModifiers(modifiers);

        if (modifiers.positionDelta != Vector3.zero)
            result.positionDelta = modifiers.positionDelta * t;

        for (int i = 0; i < modifiers.rotations.Count; i++)
        {
            var rot = modifiers.rotations[i];
            result.rotations.Add(new Rotation(Quaternion.LerpUnclamped(cData.InitialRotation, rot.rotation, t),
                rot.pivot));
        }

        Vector3 startScale = cData.InitialScale;
        Vector3 endScale = modifiers.scaleDelta.lossyScale;
        Vector3 lerpedScale = Vector3.LerpUnclamped(startScale, endScale, t);
        result.scaleDelta = Matrix4x4.Scale(lerpedScale);

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
            result.BL_Color = ColorOverride.LerpUnclamped(cData.initialMesh.BL_Color, target.BL_Color, t);

        if (target.TL_Color.Override != ColorOverride.OverrideMode.None)
            result.TL_Color = ColorOverride.LerpUnclamped(cData.initialMesh.TL_Color, target.TL_Color, t);

        if (target.TR_Color.Override != ColorOverride.OverrideMode.None)
            result.TR_Color = ColorOverride.LerpUnclamped(cData.initialMesh.TR_Color, target.TR_Color, t);

        if (target.BR_Color.Override != ColorOverride.OverrideMode.None)
            result.BR_Color = ColorOverride.LerpUnclamped(cData.initialMesh.BR_Color, target.BR_Color, t);

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

    public void SetContext(CharData cData, IAnimatorContext context)
    {
        this.context = context;
        this.cData = cData;
    }


    public void Reset()
    {
        Init();
    }


    private void Init()
    {
        positionDelta = Vector3.zero;
        scaleDelta = Matrix4x4.Scale(Vector3.one);
        rotations.Clear();

        ResetMeshModifiers();
    }

    private void ResetMeshModifiers()
    {
        meshModifiers = new TMPMeshModifiers2();

        meshModifiers.BL_UV0 = new TMPMeshModifiers2.UVOverride(cData.initialMesh.GetUV0(0));
        meshModifiers.TL_UV0 = new TMPMeshModifiers2.UVOverride(cData.initialMesh.GetUV0(1));
        meshModifiers.TR_UV0 = new TMPMeshModifiers2.UVOverride(cData.initialMesh.GetUV0(2));
        meshModifiers.BR_UV0 = new TMPMeshModifiers2.UVOverride(cData.initialMesh.GetUV0(3));

        meshModifiers.BL_UV2 = new TMPMeshModifiers2.UVOverride(cData.initialMesh.GetUV2(0));
        meshModifiers.TL_UV2 = new TMPMeshModifiers2.UVOverride(cData.initialMesh.GetUV2(1));
        meshModifiers.TR_UV2 = new TMPMeshModifiers2.UVOverride(cData.initialMesh.GetUV2(2));
        meshModifiers.BR_UV2 = new TMPMeshModifiers2.UVOverride(cData.initialMesh.GetUV2(3));
    }

    public void Apply()
    {
        CalculateVertexPositions();

        cData.mesh.SetPosition(0, BL_Result);
        cData.mesh.SetPosition(1, TL_Result);
        cData.mesh.SetPosition(2, TR_Result);
        cData.mesh.SetPosition(3, BR_Result);

        cData.mesh.SetColor(0, meshModifiers.BL_Color.GetValue(cData.initialMesh.GetColor(0)));
        cData.mesh.SetColor(1, meshModifiers.TL_Color.GetValue(cData.initialMesh.GetColor(1)));
        cData.mesh.SetColor(2, meshModifiers.TR_Color.GetValue(cData.initialMesh.GetColor(2)));
        cData.mesh.SetColor(3, meshModifiers.BR_Color.GetValue(cData.initialMesh.GetColor(3)));

        cData.mesh.SetUV0(0, meshModifiers.BL_UV0.GetValue(cData.initialMesh.GetUV0(0)));
        cData.mesh.SetUV0(1, meshModifiers.TL_UV0.GetValue(cData.initialMesh.GetUV0(1)));
        cData.mesh.SetUV0(2, meshModifiers.TR_UV0.GetValue(cData.initialMesh.GetUV0(2)));
        cData.mesh.SetUV0(3, meshModifiers.BR_UV0.GetValue(cData.initialMesh.GetUV0(3)));

        cData.mesh.SetUV2(0, meshModifiers.BL_UV2.GetValue(cData.initialMesh.GetUV2(0)));
        cData.mesh.SetUV2(1, meshModifiers.TL_UV2.GetValue(cData.initialMesh.GetUV2(1)));
        cData.mesh.SetUV2(2, meshModifiers.TR_UV2.GetValue(cData.initialMesh.GetUV2(2)));
        cData.mesh.SetUV2(3, meshModifiers.BR_UV2.GetValue(cData.initialMesh.GetUV2(3)));
    }

    public void CalculateVertexPositions()
    {
        // Apply vertex transformations
        Vector3 vbl = cData.initialMesh.BL_Position + meshModifiers.BL_Delta;
        Vector3 vtl = cData.initialMesh.TL_Position + meshModifiers.TL_Delta;
        Vector3 vtr = cData.initialMesh.TR_Position + meshModifiers.TR_Delta;
        Vector3 vbr = cData.initialMesh.BR_Position + meshModifiers.BR_Delta;

        // TODO Clamp
        // For now only the vertex offsets are clamped to min/max of each individual animation, as otherwise stacked animations are likely to deform the character
        // vtl = new Vector3(Mathf.Clamp(vtl.x, TLMin.x, TLMax.x), Mathf.Clamp(vtl.y, TLMin.y, TLMax.y), Mathf.Clamp(vtl.z, TLMin.z, TLMax.z));
        // vtr = new Vector3(Mathf.Clamp(vtr.x, TRMin.x, TRMax.x), Mathf.Clamp(vtr.y, TRMin.y, TRMax.y), Mathf.Clamp(vtr.z, TRMin.z, TRMax.z));
        // vbr = new Vector3(Mathf.Clamp(vbr.x, BRMin.x, BRMax.x), Mathf.Clamp(vbr.y, BRMin.y, BRMax.y), Mathf.Clamp(vbr.z, BRMin.z, BRMax.z));
        // vbl = new Vector3(Mathf.Clamp(vbl.x, BLMin.x, BLMax.x), Mathf.Clamp(vbl.y, BLMin.y, BLMax.y), Mathf.Clamp(vbl.z, BLMin.z, BLMax.z));

        // Apply scale
        if (scaleDelta != Matrix4x4.identity)
        {
            vtl = ScaleDelta.MultiplyPoint3x4(vtl - cData.InitialPosition) + cData.InitialPosition;
            vtr = ScaleDelta.MultiplyPoint3x4(vtr - cData.InitialPosition) + cData.InitialPosition;
            vbr = ScaleDelta.MultiplyPoint3x4(vbr - cData.InitialPosition) + cData.InitialPosition;
            vbl = ScaleDelta.MultiplyPoint3x4(vbl - cData.InitialPosition) + cData.InitialPosition;
        }

        // Apply rotation
        Vector3 pivot;
        Matrix4x4 matrix;
        for (int i = 0; i < rotations.Count; i++)
        {
            var rot = rotations[i];
            pivot = rot.pivot;
            matrix = Matrix4x4.Rotate(rot.rotation);
            Debug.LogWarning("Scale in rot: " + matrix.lossyScale);

            vtl = matrix.MultiplyPoint3x4(vtl - pivot) + pivot;
            vtr = matrix.MultiplyPoint3x4(vtr - pivot) + pivot;
            vbr = matrix.MultiplyPoint3x4(vbr - pivot) + pivot;
            vbl = matrix.MultiplyPoint3x4(vbl - pivot) + pivot;
        }

        // Apply transformation
        var scaled = AnimationUtility.ScaleVector(PositionDelta, cData, context);
        vtl += scaled;
        vtr += scaled;
        vbr += scaled;
        vbl += scaled;

        BL_Result = vbl;
        TL_Result = vtl;
        TR_Result = vtr;
        BR_Result = vbr;
    }

    public void Update()
    {
        if (cData.positionDirty)
            positionDelta += (cData.Position - cData.InitialPosition);

        if (cData.scaleDirty)
            scaleDelta *= Matrix4x4.Scale(cData.Scale);

        if (cData.rotationDirty)
        {
            if (cData.Rotation != Quaternion.identity || cData.Rotation.eulerAngles == Vector3.zero)
            {
                var scaled = cData.InitialPosition +
                             AnimationUtility.ScaleVector((cData.RotationPivot - cData.InitialPosition), cData,
                                 context);
                rotations.Add(new Rotation(cData.Rotation, scaled));
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