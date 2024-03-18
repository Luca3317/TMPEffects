using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRangeAttribute : PropertyAttribute 
{
    public float min;
    public float max;

    public NewRangeAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}
