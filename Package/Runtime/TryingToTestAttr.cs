using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.Databases;
using TMPEffects.ParameterUtilityGenerator.Attributes;
using UnityEngine;

[TMPParameterType("SomeShit", typeof(TryingToTestAttr2), typeof(TryingToTestAttr2), typeof(TryingToTestAttr2), true)]
public partial class TryingToTestAttr2
{
    public static partial bool StringToSomeShit(string str, out TryingToTestAttr2 result, ITMPKeywordDatabase db = null)
    {
        result = null;
        return false; 
    }

    public float a;
    public float a1;
    public float a2;
    public float a3;
}