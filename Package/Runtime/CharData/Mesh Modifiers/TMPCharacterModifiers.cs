
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[Serializable]
public class TMPCharacterModifiers
{
    public ModifierFlags Modifier => modifier;

    // public Vector3Override Position
    // {
    //     get => position;
    //     set
    //     {
    //         if (value == position) return;
    //         if (!value.Override)
    //         {
    //             ClearScale();
    //             return;
    //         }
    //         position = value;
    //         modifier |= ModifierFlags.Position;
    //     }
    // }

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
        if (rotations.Count > 100) throw new System.Exception("Cannot add more than 100 rotations.");
        rotations.Add(rotation);
        modifier |= ModifierFlags.Rotations;
    }

    public void RemoveRotation(int index)
    {
        rotations.RemoveAt(index);
        if (rotations.Count == 0) ClearRotations();
    }

    // [SerializeField] private Vector3Override position = Vector3Override.GetDefault;
    [SerializeField] private Vector3 positionDelta = Vector3.zero;
    [SerializeField] private Matrix4x4 scaleDelta = Matrix4x4.Scale(Vector3.one);
    [SerializeField] private List<Rotation> rotations = new List<Rotation>();
    [SerializeField] private ModifierFlags modifier;
    private ReadOnlyCollection<Rotation> rotationsReadOnly;

    public TMPCharacterModifiers()
    {
    }

    public TMPCharacterModifiers(TMPCharacterModifiers original)
    {
        // position = original.position;
        positionDelta = original.positionDelta;
        scaleDelta = original.scaleDelta;
        rotations = new List<Rotation>(original.rotations);
        modifier = original.Modifier;
    }
    
    public void Combine(TMPCharacterModifiers other)
    {
        // if (other.Position.Override)
        // {
        //     position = other.position;
        // }

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
        // Position = 1,
        PositionDelta = 1 << 1,
        Rotations = 1 << 2,
        Scale = 1 << 3
    }

    public void ClearModifierFlags()
    {
        // ClearPosition();
        ClearPositionDelta();
        ClearRotations();
        ClearScale();
    }

    public void ClearModifierFlags(ModifierFlags flags)
    {
        var both = modifier & flags;
        
        // if (both.HasFlag(ModifierFlags.Position))
        //     ClearPosition();
        
        if (both.HasFlag(ModifierFlags.PositionDelta))
            ClearPositionDelta();

        if (both.HasFlag(ModifierFlags.Rotations))
            ClearRotations();
      
        if (both.HasFlag(ModifierFlags.Scale))
            ClearScale();
    }

    // private void ClearPosition()
    // {
    //     modifier &= ~ModifierFlags.Position;
    //     position = Vector3Override.GetDefault;
    // }

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