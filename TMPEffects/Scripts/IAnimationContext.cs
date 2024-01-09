using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimationContext
{
    public AnimatorContext animatorContext { get; set; }
    public SegmentData segmentData { get; set; }

    public void ResetContext() { }
}
