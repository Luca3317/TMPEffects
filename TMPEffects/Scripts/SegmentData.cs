using System.Collections.Generic;
using TMPEffects.Components;
using TMPEffects.Tags;
using UnityEngine;

public class SegmentData
{
    public readonly int startIndex;
    public readonly int length;

    public readonly int firstVisibleIndex;
    public readonly int lastVisibleIndex;

    public readonly int firstAnimationIndex;
    public readonly int lastAnimationIndex;

    public readonly int firstShowAnimationIndex;
    public readonly int lastShowAnimationIndex;

    public readonly int firstHideAnimationIndex;
    public readonly int lastHideAnimationIndex;

    public Vector3 max;
    public Vector3 min;

    internal SegmentData(TMPAnimator animator, TMPAnimationTag tag, List<CharData> cData)
    {
        startIndex = tag.startIndex;
        length = tag.length;
        firstVisibleIndex = -1;
        lastVisibleIndex = -1;
        firstAnimationIndex = -1;
        lastAnimationIndex = -1;
        firstShowAnimationIndex = -1;
        lastShowAnimationIndex = -1;
        firstHideAnimationIndex = -1;
        lastHideAnimationIndex = -1;

        max = Vector3.negativeInfinity;
        min = Vector3.positiveInfinity;
        Vector3 leftTop, bottomRight;
        for (int i = startIndex; i < startIndex + length; i++)
        {
            if (!cData[i].info.isVisible) continue;

            leftTop = cData[i].mesh.initial.vertex_TL.position;
            bottomRight = cData[i].mesh.initial.vertex_BR.position;
            max = new Vector3(Mathf.Max(max.x, leftTop.x, bottomRight.x), Mathf.Max(max.y, leftTop.y, bottomRight.y), Mathf.Max(max.z, leftTop.z, bottomRight.z));
            min = new Vector3(Mathf.Min(min.x, leftTop.x, bottomRight.x), Mathf.Min(min.y, leftTop.y, bottomRight.y), Mathf.Min(min.z, leftTop.z, bottomRight.z));

            if (firstVisibleIndex == -1) firstVisibleIndex = i;
            lastVisibleIndex = i;

            if (!animator.IsExcludedBasic(cData[i].info.character))
            {
                if (firstAnimationIndex == -1) firstAnimationIndex = i;
                lastAnimationIndex = i;
            }
            if (!animator.IsExcludedShow(cData[i].info.character))
            {
                if (firstAnimationIndex == -1) firstShowAnimationIndex = i;
                lastShowAnimationIndex = i;
            }
            if (!animator.IsExcludedHide(cData[i].info.character))
            {
                if (firstAnimationIndex == -1) firstHideAnimationIndex = i;
                lastHideAnimationIndex = i;
            }
        }
    }
}