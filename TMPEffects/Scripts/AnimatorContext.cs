using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimatorContext
{
    public bool scaleAnimations = true;
    public bool useScaledTime = true;
    [System.NonSerialized, HideInInspector] public float deltaTime;
    [HideInInspector] public float passedTime { get => passed; set { passed = value; } }

    private float passed;

    public TimeData timings = new();

    [System.Serializable]
    public class TimeData
    {
        public float useScaledTime;
        [System.NonSerialized, HideInInspector]public float passedTime;
        [System.NonSerialized, HideInInspector]public float deltaTime;

        public void ResetTime()
        {
            passedTime = 0;
            deltaTime = 0;
        }
    }
}
