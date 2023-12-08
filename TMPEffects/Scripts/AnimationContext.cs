using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationContext
{
    public bool scaleAnimations = true;
    public bool useScaledTime = true;
    [System.NonSerialized, HideInInspector] public float deltaTime;
    [System.NonSerialized, HideInInspector] public float passedTime;
}
