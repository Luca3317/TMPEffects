using System;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[Serializable]
public class CharDataModifiers
{
    public TMPMeshModifiers MeshModifiers
    {
        get => meshModifiers;
        set => meshModifiers = value;
    }

    public TMPCharacterModifiers CharacterModifiers
    {
        get => characterModifiers;
        set => characterModifiers = value;
    }

    [SerializeField] private TMPMeshModifiers meshModifiers;
    [SerializeField] private TMPCharacterModifiers characterModifiers;

    public Vector3 BL_Result { get; private set; }
    public Vector3 TL_Result { get; private set; }
    public Vector3 TR_Result { get; private set; }
    public Vector3 BR_Result { get; private set; }

    public CharDataModifiers()
    {
        meshModifiers = new TMPMeshModifiers();
        characterModifiers = new TMPCharacterModifiers();
    }

    public CharDataModifiers(CharDataModifiers original)
    {
        meshModifiers = new TMPMeshModifiers(original.meshModifiers);
        characterModifiers = new TMPCharacterModifiers(original.characterModifiers);
    }

    public void Combine(CharDataModifiers other)
    {
        meshModifiers.Combine(other.meshModifiers);
        characterModifiers.Combine(other.characterModifiers);
    }

    public void CalculateVertexPositions(CharData cData, IAnimatorContext context)
    {
        // Apply vertex transformations
        Vector3 vbl = cData.InitialMesh.BL_Position;
        Vector3 vtl = cData.InitialMesh.TL_Position;
        Vector3 vtr = cData.InitialMesh.TR_Position;
        Vector3 vbr = cData.InitialMesh.BR_Position;

        if (meshModifiers.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.Deltas))
        {
            vbl += AnimationUtility.ScaleVector(meshModifiers.BL_Delta, cData, context);
            vtl += AnimationUtility.ScaleVector(meshModifiers.TL_Delta, cData, context);
            vtr += AnimationUtility.ScaleVector(meshModifiers.TR_Delta, cData, context);
            vbr += AnimationUtility.ScaleVector(meshModifiers.BR_Delta, cData, context);
        }

        // TODO Clamp
        // For now only the vertex offsets are clamped to min/max of each individual animation, as otherwise stacked animations are likely to deform the character
        // If i want to do that again, ill need to update those in combine.
        // vtl = new Vector3(Mathf.Clamp(vtl.x, TLMin.x, TLMax.x), Mathf.Clamp(vtl.y, TLMin.y, TLMax.y), Mathf.Clamp(vtl.z, TLMin.z, TLMax.z));
        // vtr = new Vector3(Mathf.Clamp(vtr.x, TRMin.x, TRMax.x), Mathf.Clamp(vtr.y, TRMin.y, TRMax.y), Mathf.Clamp(vtr.z, TRMin.z, TRMax.z));
        // vbr = new Vector3(Mathf.Clamp(vbr.x, BRMin.x, BRMax.x), Mathf.Clamp(vbr.y, BRMin.y, BRMax.y), Mathf.Clamp(vbr.z, BRMin.z, BRMax.z));
        // vbl = new Vector3(Mathf.Clamp(vbl.x, BLMin.x, BLMax.x), Mathf.Clamp(vbl.y, BLMin.y, BLMax.y), Mathf.Clamp(vbl.z, BLMin.z, BLMax.z));

        // Apply scale
        if (characterModifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.Scale))
        {
            vbl = characterModifiers.ScaleDelta.MultiplyPoint3x4(vbl - cData.InitialPosition) +
                  cData.InitialPosition;
            vtl = characterModifiers.ScaleDelta.MultiplyPoint3x4(vtl - cData.InitialPosition) +
                  cData.InitialPosition;
            vtr = characterModifiers.ScaleDelta.MultiplyPoint3x4(vtr - cData.InitialPosition) +
                  cData.InitialPosition;
            vbr = characterModifiers.ScaleDelta.MultiplyPoint3x4(vbr - cData.InitialPosition) +
                  cData.InitialPosition;
        }

        // Apply rotation
        if (characterModifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.Rotations))
        {
            Vector3 pivot;
            Matrix4x4 matrix;

            for (int i = 0; i < characterModifiers.Rotations.Count; i++)
            {
                var rot = characterModifiers.Rotations[i];
                
                if (rot.eulerAngles == Vector3.zero) continue;
                
                pivot = cData.InitialPosition +
                        AnimationUtility.ScaleVector(rot.pivot - cData.InitialPosition, cData, context);

                matrix = Matrix4x4.Rotate(Quaternion.Euler(rot.eulerAngles));

                vbl = matrix.MultiplyPoint3x4(vbl - pivot) + pivot;
                vtl = matrix.MultiplyPoint3x4(vtl - pivot) + pivot;
                vtr = matrix.MultiplyPoint3x4(vtr - pivot) + pivot;
                vbr = matrix.MultiplyPoint3x4(vbr - pivot) + pivot;
            }
        }

        if (characterModifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.PositionDelta))
        {
            var scaled = AnimationUtility.ScaleVector(characterModifiers.PositionDelta, cData, context);
            vbl += scaled;
            vtl += scaled;
            vtr += scaled;
            vbr += scaled;
        }

        BL_Result = vbl;
        TL_Result = vtl;
        TR_Result = vtr;
        BR_Result = vbr;
    }

    static Vector3 GetPreciseScale(Matrix4x4 matrix)
    {
        return new Vector3(
            matrix.GetColumn(0).magnitude * Mathf.Sign(matrix.m00),
            matrix.GetColumn(1).magnitude * Mathf.Sign(matrix.m11),
            matrix.GetColumn(2).magnitude * Mathf.Sign(matrix.m22)
        );
    }

    public static void LerpUnclamped(CharData cData, IAnimatorContext ctx, CharDataModifiers start,
        CharDataModifiers end, float t,
        CharDataModifiers result)
    {
        LerpCharacterModifiersUnclamped(cData, start.CharacterModifiers, end.CharacterModifiers, t,
            result.CharacterModifiers);
        LerpMeshModifiersUnclamped(cData, ctx, start.MeshModifiers, end.MeshModifiers, t, result.MeshModifiers);
    }

    public static void LerpCharacterModifiersUnclamped(CharData cData, TMPCharacterModifiers start,
        TMPCharacterModifiers end, float t, TMPCharacterModifiers result)
    {
        result.ClearModifierFlags();

        var combinedFlags = end.Modifier | start.Modifier;

        if (combinedFlags.HasFlag(TMPCharacterModifiers.ModifierFlags.PositionDelta))
        {
            result.PositionDelta =
                Vector3.LerpUnclamped(start.PositionDelta, end.PositionDelta, t);
        }

        if (combinedFlags.HasFlag(TMPCharacterModifiers.ModifierFlags.Scale))
        {
            Vector3 endScale = GetPreciseScale(end.ScaleDelta);
            Vector3 startScale = GetPreciseScale(start.ScaleDelta);
            Vector3 lerpedScale = Vector3.LerpUnclamped(startScale, endScale, t);

            result.ScaleDelta = Matrix4x4.Scale(lerpedScale);
        }

        if (combinedFlags.HasFlag(TMPCharacterModifiers.ModifierFlags.Rotations))
        {
            // TODO THis probably isnt right
            try
            {
                for (int i = 0; i < start.Rotations.Count; i++)
                {
                    var rot = start.Rotations[i];
                    result.AddRotation(
                        new Rotation(Vector3.LerpUnclamped(rot.eulerAngles, cData.InitialRotation.eulerAngles, t),
                            rot.pivot));
                }

                for (int i = 0; i < end.Rotations.Count; i++)
                {
                    var rot = end.Rotations[i];
                    result.AddRotation(
                        new Rotation(Vector3.LerpUnclamped(cData.InitialRotation.eulerAngles, rot.eulerAngles, t),
                            rot.pivot));
                }
            }
            catch
            {
                Debug.LogError("Tried to add to many with " + end.Rotations.Count);
            }
        }
    }

    public static void LerpMeshModifiersUnclamped(CharData cData, IAnimatorContext ctx, TMPMeshModifiers start,
        TMPMeshModifiers end,
        float t,
        TMPMeshModifiers result)
    {
        result.ClearModifiers();

        var combinedFlags = start.Modifier | end.Modifier;

        if (combinedFlags.HasFlag(TMPMeshModifiers.ModifierFlags.Deltas))
        {
            result.BL_Delta = Vector3.LerpUnclamped(start.BL_Delta, end.BL_Delta, t);
            result.TL_Delta = Vector3.LerpUnclamped(start.TL_Delta, end.TL_Delta, t);
            result.TR_Delta = Vector3.LerpUnclamped(start.TR_Delta, end.TR_Delta, t);
            result.BR_Delta = Vector3.LerpUnclamped(start.BR_Delta, end.BR_Delta, t);
        }

        if (end.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.Colors))
        {
            if (start.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.Colors))
            {
                result.BL_Color = ColorOverride.LerpUnclamped(start.BL_Color, end.BL_Color, t);
                result.TL_Color = ColorOverride.LerpUnclamped(start.TL_Color, end.TL_Color, t);
                result.TR_Color = ColorOverride.LerpUnclamped(start.TR_Color, end.TR_Color, t);
                result.BR_Color = ColorOverride.LerpUnclamped(start.BR_Color, end.BR_Color, t);
            }
            else if (cData.MeshModifiers.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.Colors))
            {
                result.BL_Color = ColorOverride.LerpUnclamped(cData.MeshModifiers.BL_Color, end.BL_Color, t);
                result.TL_Color = ColorOverride.LerpUnclamped(cData.MeshModifiers.TL_Color, end.TL_Color, t);
                result.TR_Color = ColorOverride.LerpUnclamped(cData.MeshModifiers.TR_Color, end.TR_Color, t);
                result.BR_Color = ColorOverride.LerpUnclamped(cData.MeshModifiers.BR_Color, end.BR_Color, t);
            }
            else if (ctx.Modifiers.MeshModifiers.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.Colors))
            {
                result.BL_Color = ColorOverride.LerpUnclamped(ctx.Modifiers.MeshModifiers.BL_Color, end.BL_Color, t);
                result.TL_Color = ColorOverride.LerpUnclamped(ctx.Modifiers.MeshModifiers.TL_Color, end.TL_Color, t);
                result.TR_Color = ColorOverride.LerpUnclamped(ctx.Modifiers.MeshModifiers.TR_Color, end.TR_Color, t);
                result.BR_Color = ColorOverride.LerpUnclamped(ctx.Modifiers.MeshModifiers.BR_Color, end.BR_Color, t);
            }
            else
            {
                result.BL_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.BL_Color, end.BL_Color, t);
                result.TL_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.TL_Color, end.TL_Color, t);
                result.TR_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.TR_Color, end.TR_Color, t);
                result.BR_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.BR_Color, end.BR_Color, t);
            }
        }

        if (combinedFlags.HasFlag(TMPMeshModifiers.ModifierFlags.UVs))
        {
            var startVector = start.BL_UV0.GetValue(cData.InitialMesh.BL_UV0);
            var endVector = end.BL_UV0.GetValue(cData.InitialMesh.BL_UV0);
            result.BL_UV0 =
                new Vector3Override(Vector3.LerpUnclamped(startVector, endVector, t));

            startVector = start.TL_UV0.GetValue(cData.InitialMesh.TL_UV0);
            endVector = end.TL_UV0.GetValue(cData.InitialMesh.TL_UV0);
            result.TL_UV0 =
                new Vector3Override(Vector3.LerpUnclamped(startVector, endVector, t));

            startVector = start.TR_UV0.GetValue(cData.InitialMesh.TR_UV0);
            endVector = end.TR_UV0.GetValue(cData.InitialMesh.TR_UV0);
            result.TR_UV0 =
                new Vector3Override(Vector3.LerpUnclamped(startVector, endVector, t));

            startVector = start.BR_UV0.GetValue(cData.InitialMesh.BR_UV0);
            endVector = end.BR_UV0.GetValue(cData.InitialMesh.BR_UV0);
            result.BR_UV0 =
                new Vector3Override(Vector3.LerpUnclamped(startVector, endVector, t));
        }
    }

    public static CharDataModifiers LerpUnclamped(CharData cData, CharDataModifiers modifiers, float t)
    {
        CharDataModifiers storage = new CharDataModifiers();
        LerpUnclamped(cData, modifiers, t, storage);
        return storage;
    }

    public static void LerpUnclamped(CharData cData, CharDataModifiers modifiers, float t,
        CharDataModifiers result)
    {
        LerpCharacterModifiersUnclamped(cData, modifiers.CharacterModifiers, t, result.CharacterModifiers);
        LerpMeshModifiersUnclamped(cData, modifiers.MeshModifiers, t, result.MeshModifiers);
    }

    public static void LerpCharacterModifiersUnclamped(CharData cData, TMPCharacterModifiers modifiers, float t,
        TMPCharacterModifiers result)
    {
        // result.ClearModifierFlags();

        if (modifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.PositionDelta))
        {
            result.PositionDelta =
                modifiers.PositionDelta * t;
        }

        if (modifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.Rotations))
        {
            try
            {
                for (int i = 0; i < modifiers.Rotations.Count; i++)
                {
                    var rot = modifiers.Rotations[i];
                    result.AddRotation(
                        new Rotation(Vector3.LerpUnclamped(cData.InitialRotation.eulerAngles, rot.eulerAngles, t),
                            rot.pivot));
                }
            }
            catch
            {
                Debug.LogError("Tried to add to many with " + modifiers.Rotations.Count);
            }
        }

        if (modifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.Scale))
        {
            // TODO
            // LossyScale is more lossy than I thought, so this is not an option
            // For example: Matrix4x4.Scale(1, -1.5, 1).lossyScale = (-1, 1.5, 0)
            // Vector3 endScale = modifiers.characterModifiers.ScaleDelta.lossyScale;

            // For now; GetPreciseScale. Probably fuggin slow though
            // Alternatively; store list of vector3s that is combined when needed?
            // Could cache that result. Hm

            Vector3 endScale = GetPreciseScale(modifiers.ScaleDelta);
            Vector3 startScale = cData.InitialScale;
            Vector3 lerpedScale = Vector3.LerpUnclamped(startScale, endScale, t);

            result.ScaleDelta = Matrix4x4.Scale(lerpedScale);
        }
    }

    public static void LerpMeshModifiersUnclamped(CharData cData, TMPMeshModifiers modifiers, float t,
        TMPMeshModifiers result)
    {
        // result.ClearModifiers();

        if (modifiers.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.Deltas))
        {
            result.BL_Delta = modifiers.BL_Delta * t;
            result.TL_Delta = modifiers.TL_Delta * t;
            result.TR_Delta = modifiers.TR_Delta * t;
            result.BR_Delta = modifiers.BR_Delta * t;
        }

        if (modifiers.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.Colors))
        {
            result.BL_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.BL_Color, modifiers.BL_Color, t);
            result.TL_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.TL_Color, modifiers.TL_Color, t);
            result.TR_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.TR_Color, modifiers.TR_Color, t);
            result.BR_Color = ColorOverride.LerpUnclamped(cData.InitialMesh.BR_Color, modifiers.BR_Color, t);
        }

        if (modifiers.Modifier.HasFlag(TMPMeshModifiers.ModifierFlags.UVs))
        {
            var vector = modifiers.BL_UV0.GetValue(cData.InitialMesh.BL_UV0);
            result.BL_UV0 =
                new Vector3Override(Vector3.LerpUnclamped(cData.InitialMesh.BL_UV0, vector, t));

            vector = modifiers.TL_UV0.GetValue(cData.InitialMesh.TL_UV0);
            result.TL_UV0 =
                new Vector3Override(Vector3.LerpUnclamped(cData.InitialMesh.TL_UV0, vector, t));

            vector = modifiers.TR_UV0.GetValue(cData.InitialMesh.TR_UV0);
            result.TR_UV0 =
                new Vector3Override(Vector3.LerpUnclamped(cData.InitialMesh.TR_UV0, vector, t));

            vector = modifiers.BR_UV0.GetValue(cData.InitialMesh.BR_UV0);
            result.BR_UV0 =
                new Vector3Override(Vector3.LerpUnclamped(cData.InitialMesh.BR_UV0, vector, t));
        }
    }

    public void Reset()
    {
        meshModifiers.ClearModifiers();
        characterModifiers.ClearModifierFlags();
    }
}