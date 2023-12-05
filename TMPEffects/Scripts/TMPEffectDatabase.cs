using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class TMPEffectDatabase<T> : ScriptableObject
{
    public abstract bool Contains(string name);
    public abstract T GetEffect(string name);
}