using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Empty sub-class to make custom editor for animation steps used in the timeline
// (couldnt work into normal editor because of reliance on timeline package)
[System.Serializable]
public class TimelineAnimationStep
{
    public AnimationStep Step;
#if UNITY_EDITOR
    public int lastMovedEntry = 0;
#endif
}