using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

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
    public TMPMeshModifiers meshModifiers;
    public TMPCharacterMeshModifiers characterMeshModifiers;

    public Vector3 BL_Result { get; private set; }
    public Vector3 TL_Result { get; private set; }
    public Vector3 TR_Result { get; private set; }
    public Vector3 BR_Result { get; private set; }

    public TMPCharDataModifiers()
    {
        meshModifiers = new TMPMeshModifiers();
        characterMeshModifiers = new TMPCharacterMeshModifiers();
    }

    public TMPCharDataModifiers(TMPCharDataModifiers original)
    {
        meshModifiers = new TMPMeshModifiers(original.meshModifiers);
        characterMeshModifiers = new TMPCharacterMeshModifiers(original.characterMeshModifiers);
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
            vbl += AnimationUtility.ScaleVector(meshModifiers.BL_Delta, cData, context);;
            vtl += AnimationUtility.ScaleVector(meshModifiers.TL_Delta, cData, context);;
            vtr += AnimationUtility.ScaleVector(meshModifiers.TR_Delta, cData, context);;
            vbr += AnimationUtility.ScaleVector(meshModifiers.BR_Delta, cData, context);;
        }

        // TODO Clamp
        // For now only the vertex offsets are clamped to min/max of each individual animation, as otherwise stacked animations are likely to deform the character
        // If i want to do that again, ill need to update those in combine.
        // vtl = new Vector3(Mathf.Clamp(vtl.x, TLMin.x, TLMax.x), Mathf.Clamp(vtl.y, TLMin.y, TLMax.y), Mathf.Clamp(vtl.z, TLMin.z, TLMax.z));
        // vtr = new Vector3(Mathf.Clamp(vtr.x, TRMin.x, TRMax.x), Mathf.Clamp(vtr.y, TRMin.y, TRMax.y), Mathf.Clamp(vtr.z, TRMin.z, TRMax.z));
        // vbr = new Vector3(Mathf.Clamp(vbr.x, BRMin.x, BRMax.x), Mathf.Clamp(vbr.y, BRMin.y, BRMax.y), Mathf.Clamp(vbr.z, BRMin.z, BRMax.z));
        // vbl = new Vector3(Mathf.Clamp(vbl.x, BLMin.x, BLMax.x), Mathf.Clamp(vbl.y, BLMin.y, BLMax.y), Mathf.Clamp(vbl.z, BLMin.z, BLMax.z));

        // Apply scale
        if (characterMeshModifiers.Modifier.HasFlag(TMPCharacterMeshModifiers.ModifierFlags.Scale))
        {
            vtl = characterMeshModifiers.ScaleDelta.MultiplyPoint3x4(vtl - cData.InitialPosition) +
                  cData.InitialPosition;
            vtr = characterMeshModifiers.ScaleDelta.MultiplyPoint3x4(vtr - cData.InitialPosition) +
                  cData.InitialPosition;
            vbr = characterMeshModifiers.ScaleDelta.MultiplyPoint3x4(vbr - cData.InitialPosition) +
                  cData.InitialPosition;
            vbl = characterMeshModifiers.ScaleDelta.MultiplyPoint3x4(vbl - cData.InitialPosition) +
                  cData.InitialPosition;
        }

        // Apply rotation
        if (characterMeshModifiers.Modifier.HasFlag(TMPCharacterMeshModifiers.ModifierFlags.Rotations))
        {
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
        }

        // Apply transformation
        if (characterMeshModifiers.Modifier.HasFlag(TMPCharacterMeshModifiers.ModifierFlags.Position))
        {
            var pos = characterMeshModifiers.Position.GetValue(cData.InitialPosition);
            var delta = pos - cData.InitialPosition;
            vbl += delta;
            vtl += delta;
            vtr += delta;
            vbr += delta;
        }
        
        if (characterMeshModifiers.Modifier.HasFlag(TMPCharacterMeshModifiers.ModifierFlags.PositionDelta))
        {
            var scaled = AnimationUtility.ScaleVector(characterMeshModifiers.PositionDelta, cData, context);
            vtl += scaled;
            vtr += scaled;
            vbr += scaled;
            vbl += scaled;
        }

        BL_Result = vbl;
        TL_Result = vtl;
        TR_Result = vtr;
        BR_Result = vbr;
    }

    public static TMPCharDataModifiers LerpUnclamped(CharData cData, TMPCharDataModifiers modifiers, float t)
    {
        TMPCharDataModifiers result = new TMPCharDataModifiers();

        if (modifiers.characterMeshModifiers.Modifier.HasFlag(TMPCharacterMeshModifiers.ModifierFlags.PositionDelta))
        {
            result.characterMeshModifiers.PositionDelta =
                modifiers.characterMeshModifiers.PositionDelta * t;
        }

        if (modifiers.characterMeshModifiers.Modifier.HasFlag(TMPCharacterMeshModifiers.ModifierFlags.Rotations))
        {
            for (int i = 0; i < modifiers.characterMeshModifiers.Rotations.Count; i++)
            {
                var rot = modifiers.characterMeshModifiers.Rotations[i];
                result.characterMeshModifiers.AddRotation(
                    new Rotation(Vector3.LerpUnclamped(cData.InitialRotation.eulerAngles, rot.eulerAngles, t),
                        rot.pivot));
            }
        }

        if (modifiers.characterMeshModifiers.Modifier.HasFlag(TMPCharacterMeshModifiers.ModifierFlags.Scale))
        {
            Vector3 startScale = cData.InitialScale;
            Vector3 endScale = modifiers.characterMeshModifiers.ScaleDelta.lossyScale;
            Vector3 lerpedScale = Vector3.LerpUnclamped(startScale, endScale, t);
            result.characterMeshModifiers.ScaleDelta = Matrix4x4.Scale(lerpedScale);
        }

        result.meshModifiers = LerpMeshModifiersUnclamped(cData, modifiers.meshModifiers, t);

        return result;
    }

    private static TMPMeshModifiers LerpMeshModifiersUnclamped(CharData cData, TMPMeshModifiers modifiers, float t)
    {
        if (t <= 0) return new TMPMeshModifiers();

        TMPMeshModifiers result = new TMPMeshModifiers();

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

        return result;
    }

    public void Reset()
    {
        meshModifiers.ClearModifiers();
        characterMeshModifiers.ClearModifierFlags();
    }
}

[Serializable]
public class TMPCharacterMeshModifiers
{
    public ModifierFlags Modifier => modifier;

    public Vector3Override Position
    {
        get => position;
        set
        {
            if (value == position) return;
            if (!value.Override)
            {
                ClearScale();
                return;
            }
            position = value;
            modifier |= ModifierFlags.Position;
        }
    }

    public Vector3 PositionDelta
    {
        get => positionDelta;
        set
        {
            if (value == positionDelta) return;
            if (value == Vector3.zero)
            {
                ClearPositionDelta();
                return;
            }
            positionDelta = value;
            modifier |= ModifierFlags.PositionDelta;
        }
    }

    public Matrix4x4 ScaleDelta
    {
        get => scaleDelta;
        set
        {
            if (value == scaleDelta) return;
            if (value == Matrix4x4.identity)
            {
                ClearScale();
                return;
            }
            scaleDelta = value;
            modifier |= ModifierFlags.Scale;
        }
    }

    public ReadOnlyCollection<Rotation> Rotations
    {
        get
        {
            if (rotationsReadOnly == null)
                rotationsReadOnly = new ReadOnlyCollection<Rotation>(rotations);
            return rotationsReadOnly;
        }
    }

    public void InsertRotation(int index, Rotation rotation)
    {
        rotations.Insert(index, rotation);
        modifier |= ModifierFlags.Rotations;
    }

    public void AddRotation(Rotation rotation)
    {
        rotations.Add(rotation);
        modifier |= ModifierFlags.Rotations;
    }

    public void RemoveRotation(int index)
    {
        rotations.RemoveAt(index);
        if (rotations.Count == 0) ClearRotations();
    }

    [SerializeField] private Vector3Override position = Vector3Override.GetDefault;
    [SerializeField] private Vector3 positionDelta = Vector3.zero;
    [SerializeField] private Matrix4x4 scaleDelta = Matrix4x4.Scale(Vector3.one);
    [SerializeField] private List<Rotation> rotations = new List<Rotation>();
    [SerializeField] private ModifierFlags modifier;
    private ReadOnlyCollection<Rotation> rotationsReadOnly;

    public TMPCharacterMeshModifiers()
    {
    }

    public TMPCharacterMeshModifiers(TMPCharacterMeshModifiers original)
    {
        position = original.position;
        positionDelta = original.positionDelta;
        scaleDelta = original.scaleDelta;
        rotations = new List<Rotation>(original.rotations);
        modifier = original.Modifier;
    }
    
    public void Combine(TMPCharacterMeshModifiers other)
    {
        if (other.Position.Override)
        {
            position = other.position;
        }

        if (other.Modifier.HasFlag(ModifierFlags.PositionDelta))
        {
            positionDelta += other.positionDelta;
        }

        if (other.Modifier.HasFlag(ModifierFlags.Scale))
        {
            scaleDelta *= other.ScaleDelta;
        }

        if (other.Modifier.HasFlag(ModifierFlags.Rotations))
        {
            for (int i = 0; i < other.rotations.Count; i++)
            {
                rotations.Add(other.rotations[i]);
            }
        }

        modifier |= other.Modifier;
    }

    [Flags]
    public enum ModifierFlags : int
    {
        Position = 1,
        PositionDelta = 1 << 1,
        Rotations = 1 << 2,
        Scale = 1 << 3
    }

    public void ClearModifierFlags()
    {
        ClearPosition();
        ClearPositionDelta();
        ClearRotations();
        ClearScale();
    }

    public void ClearModifierFlags(ModifierFlags flags)
    {
        var both = modifier & flags;
        
        if (both.HasFlag(ModifierFlags.Position))
            ClearPosition();
        
        if (both.HasFlag(ModifierFlags.PositionDelta))
            ClearPositionDelta();

        if (both.HasFlag(ModifierFlags.Rotations))
            ClearRotations();
      
        if (both.HasFlag(ModifierFlags.Scale))
            ClearScale();
    }

    private void ClearPosition()
    {
        modifier &= ~ModifierFlags.Position;
        position = Vector3Override.GetDefault;
    }

    private void ClearRotations()
    {
        modifier &= ~ModifierFlags.Rotations;
        rotations.Clear();
    }
    
    private void ClearPositionDelta()
    {
        modifier &= ~ModifierFlags.PositionDelta;
        positionDelta = Vector3.zero;
    }
    
    private void ClearScale()
    {
        modifier &= ~ModifierFlags.Scale;
        scaleDelta = Matrix4x4.identity;
    }
}