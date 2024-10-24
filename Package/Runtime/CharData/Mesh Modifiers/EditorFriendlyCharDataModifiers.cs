using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEngine;

[System.Serializable]
public class EditorFriendlyRotation
{
    public Vector3 eulerAngles = Vector3.zero;

    public ParameterTypes.TypedVector3 pivot =
        new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

    public EditorFriendlyRotation()
    {
    }

    public EditorFriendlyRotation(Vector3 eulerAngles, ParameterTypes.TypedVector3 pivot)
    {
        this.eulerAngles = eulerAngles;
        this.pivot = pivot;
    }
}

[System.Serializable]
public class EditorFriendlyCharDataModifiers
{
    public ParameterTypes.TypedVector3 Position =
        new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

    public Vector3 Scale = Vector3.one;

    public List<EditorFriendlyRotation> Rotations =
        new List<EditorFriendlyRotation>();

    public ParameterTypes.TypedVector3 BL_Position =
        new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

    public ParameterTypes.TypedVector3 TL_Position =
        new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

    public ParameterTypes.TypedVector3 TR_Position =
        new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

    public ParameterTypes.TypedVector3 BR_Position =
        new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

    public ColorOverride BL_Color = new ColorOverride();
    public ColorOverride TL_Color = new ColorOverride();
    public ColorOverride TR_Color = new ColorOverride();
    public ColorOverride BR_Color = new ColorOverride();

    public ParameterTypes.TypedVector3 BL_UV0 =
        new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

    public ParameterTypes.TypedVector3 TL_UV0 =
        new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

    public ParameterTypes.TypedVector3 TR_UV0 =
        new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);

    public ParameterTypes.TypedVector3 BR_UV0 =
        new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero);


    private bool dirty = false;
    
    public void ToCharDataModifiers(CharData cData, IAnimationContext ctx, CharDataModifiers result)
        => ToCharDataModifiers(cData, ctx.AnimatorContext, result);
    
    public void ToCharDataModifiers(CharData cData, IAnimatorContext ctx, CharDataModifiers result)
    {
        // Position(Delta)
        if (Position.type == ParameterTypes.VectorType.Position)
        {
            Vector3 position = Position.IgnoreScaling(cData, ctx).ToDelta(cData);
            if (position != Vector3.zero) result.CharacterModifiers.PositionDelta = position;
        }
        else if (Position.vector != Vector3.zero)
        {
            Vector3 posDelta = Position.ToDelta(cData);
            if (posDelta != Vector3.zero) result.CharacterModifiers.PositionDelta = posDelta;
        }

        // Scale
        if (Scale != Vector3.one)
        {
            result.CharacterModifiers.ScaleDelta = Matrix4x4.Scale(Scale);
        }

        // Rotations
        if (Rotations.Count > 0)
        {
            for (int i = 0; i < Rotations.Count; i++)
            {
                var rot = Rotations[i];
                if (rot.eulerAngles != Vector3.zero)
                {
                    Vector3 pivot = Vector3.zero;
                    if (rot.pivot.type == ParameterTypes.VectorType.Offset)
                    {
                        Vector3 posDelta = rot.pivot.ToPosition(cData);
                        pivot = posDelta;
                    }
                    else
                    {
                        Vector3 position = rot.pivot /*.IgnoreScaling(cData, ctx)*/
                            .ToPosition(cData);
                        pivot = position;
                    }

                    result.CharacterModifiers.AddRotation(new Rotation(rot.eulerAngles, pivot));
                }
            }
        }

        // Vertex colors
        result.MeshModifiers.BL_Color = BL_Color;
        result.MeshModifiers.TL_Color = TL_Color;
        result.MeshModifiers.TR_Color = TR_Color;
        result.MeshModifiers.BR_Color = BR_Color;

        // Vertex deltas
        if (BL_Position.type == ParameterTypes.VectorType.Offset)
        {
            Vector3 posDelta = BL_Position.ToDelta(cData, cData.InitialMesh.BL_Position);
            if (posDelta != Vector3.zero) result.MeshModifiers.BL_Delta = posDelta;
        }
        else
        {
            Vector3 position = BL_Position.IgnoreScaling(cData, ctx)
                .ToDelta(cData, cData.InitialMesh.BL_Position);
            result.MeshModifiers.BL_Delta = position;
        }

        if (TL_Position.type == ParameterTypes.VectorType.Position)
        {
            Vector3 position = TL_Position.IgnoreScaling(cData, ctx)
                .ToDelta(cData, cData.InitialMesh.TL_Position);
            result.MeshModifiers.TL_Delta = position;
        }
        else
        {
            Vector3 posDelta = TL_Position.ToDelta(cData, cData.InitialMesh.TL_Position);
            if (posDelta != Vector3.zero) result.MeshModifiers.TL_Delta = posDelta;
        }

        if (TR_Position.type == ParameterTypes.VectorType.Position)
        {
            Vector3 position = TR_Position.IgnoreScaling(cData, ctx)
                .ToDelta(cData, cData.InitialMesh.TR_Position);
            result.MeshModifiers.TR_Delta = position;
        }
        else
        {
            Vector3 posDelta = TR_Position.ToDelta(cData, cData.InitialMesh.TR_Position);
            if (posDelta != Vector3.zero) result.MeshModifiers.TR_Delta = posDelta;
        }

        if (BR_Position.type == ParameterTypes.VectorType.Position)
        {
            Vector3 position = BR_Position.IgnoreScaling(cData, ctx)
                .ToDelta(cData, cData.InitialMesh.BR_Position);
            result.MeshModifiers.BR_Delta = position;
        }
        else
        {
            Vector3 posDelta = BR_Position.ToDelta(cData, cData.InitialMesh.BR_Position);
            result.MeshModifiers.BR_Delta = posDelta;
        }
    }
}