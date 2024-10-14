using System;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using UnityEngine;
using UnityEngine.Serialization;

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
            vtl = characterModifiers.ScaleDelta.MultiplyPoint3x4(vtl - cData.InitialPosition) +
                  cData.InitialPosition;
            vtr = characterModifiers.ScaleDelta.MultiplyPoint3x4(vtr - cData.InitialPosition) +
                  cData.InitialPosition;
            vbr = characterModifiers.ScaleDelta.MultiplyPoint3x4(vbr - cData.InitialPosition) +
                  cData.InitialPosition;
            vbl = characterModifiers.ScaleDelta.MultiplyPoint3x4(vbl - cData.InitialPosition) +
                  cData.InitialPosition;
        }

        // Apply rotation
        if (characterModifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.Rotations))
        {
            Vector3 pivot;
            Matrix4x4 matrix;
            foreach (var rot in characterModifiers.Rotations)
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
        if (characterModifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.Position))
        {
            var pos = characterModifiers.Position.GetValue(cData.InitialPosition);
            var delta = pos - cData.InitialPosition;
            vbl += delta;
            vtl += delta;
            vtr += delta;
            vbr += delta;
        }

        if (characterModifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.PositionDelta))
        {
            var scaled = AnimationUtility.ScaleVector(characterModifiers.PositionDelta, cData, context);
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

    public static CharDataModifiers LerpUnclamped(CharData cData, CharDataModifiers modifiers, float t)
    {
        return LerpUnclamped(cData, modifiers, t, new CharDataModifiers());
    }

    public static CharDataModifiers LerpUnclamped(CharData cData, CharDataModifiers modifiers, float t,
        CharDataModifiers result)
    {
        if (modifiers.characterModifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.PositionDelta))
        {
            result.characterModifiers.PositionDelta =
                modifiers.characterModifiers.PositionDelta * t;
        }

        if (modifiers.characterModifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.Rotations))
        {
            for (int i = 0; i < modifiers.characterModifiers.Rotations.Count; i++)
            {
                var rot = modifiers.characterModifiers.Rotations[i];
                result.characterModifiers.AddRotation(
                    new Rotation(Vector3.LerpUnclamped(cData.InitialRotation.eulerAngles, rot.eulerAngles, t),
                        rot.pivot));
            }
        }

        if (modifiers.characterModifiers.Modifier.HasFlag(TMPCharacterModifiers.ModifierFlags.Scale))
        {
            Vector3 startScale = cData.InitialScale;
            Vector3 endScale = modifiers.characterModifiers.ScaleDelta.lossyScale;
            Vector3 lerpedScale = Vector3.LerpUnclamped(startScale, endScale, t);
            result.characterModifiers.ScaleDelta = Matrix4x4.Scale(lerpedScale);
        }

        LerpMeshModifiersUnclamped(cData, modifiers.meshModifiers, t, result.MeshModifiers);

        return result;
    }
    
    private static void LerpMeshModifiersUnclamped(CharData cData, TMPMeshModifiers modifiers, float t, TMPMeshModifiers result)
    {
        result.ClearModifiers();
        if (t <= 0) return;
        
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