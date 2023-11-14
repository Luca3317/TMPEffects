using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Attribute that marks a class as a TMProEffect
 * 
 * TODO
 * Either: leave as is
 * Or: Only take tag as input and have a populate method on TMProEffectUtility
 * 
 * Once C#11 support; generic attribute is best
 */

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TMProEffectAttribute : Attribute
{
    public string Tag => tag;
    private string tag;
    public TMProEffectAttribute(string tag)
    {
        this.tag = tag;
    }
}