using System.Collections.Generic;
using TMPEffects.Tags;
using UnityEngine;

public struct SegmentData
{
    public int startIndex;
    public int length;
    public int index;

    public Vector3 max;
    public Vector3 min;

    public SegmentData(TMPAnimationTag tag, List<CharData> cData)
    {
        startIndex = tag.startIndex;
        length = tag.length;
        index = 0;

        max = Vector3.negativeInfinity;
        min = Vector3.positiveInfinity;
        Vector3 leftTop, bottomRight;
        for (int i = startIndex; i < startIndex + length; i++)
        {
            if (!cData[i].info.isVisible) continue;

            leftTop = cData[i].info.initialMesh.vertex_TL.position;
            bottomRight = cData[i].info.initialMesh.vertex_BR.position;
            max = new Vector3(Mathf.Max(max.x, leftTop.x, bottomRight.x), Mathf.Max(max.y, leftTop.y, bottomRight.y), Mathf.Max(max.z, leftTop.z, bottomRight.z));
            min = new Vector3(Mathf.Min(min.x, leftTop.x, bottomRight.x), Mathf.Min(min.y, leftTop.y, bottomRight.y), Mathf.Min(min.z, leftTop.z, bottomRight.z));
        }
    }
}