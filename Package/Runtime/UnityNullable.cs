using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
// [Serializable]
// public struct ColorNullable : UnityNullable<Color> {}

[Serializable]
public class UnityNullable<T> : IEquatable<UnityNullable<T>>
{
    public bool HasValue
    {
        get => hasValue;
        set
        {
            hasValue = value;
            if (!value) this.value = default(T);
        }
    }
    
    public T Value
    {
        get
        {
            if (!hasValue) return default(T);
            return value;
        }
        
        set
        {
            hasValue = true;
            this.value = value;
        }
    }

    [SerializeField] private T value;
    [SerializeField] private bool hasValue;

    public bool Equals(UnityNullable<T> other)
    {
        if (other == null) return false;
        return EqualityComparer<T>.Default.Equals(value, other.value) && hasValue == other.hasValue;
    }

    public override bool Equals(object obj)
    {
        return obj is UnityNullable<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(value, hasValue);
    }
}
