using Codice.CM.Common;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPEffects.Components;
using TMPEffects.Tags;
using UnityEngine;

// TODO!
// Reinitialize every time excludeshow/hide/basic or punctuation changed

public class SegmentData
{
    public readonly int startIndex;
    public readonly int length;

    public readonly int firstVisibleIndex;
    public readonly int lastVisibleIndex;

    public readonly int firstAnimationIndex;
    public readonly int lastAnimationIndex;

    public Vector3 max;
    public Vector3 min;

    internal SegmentData(EffectTagIndices indices, IList<CharData> cData, Predicate<char> animates)
    {
        startIndex = indices.StartIndex;
        length = indices.Length;
        firstVisibleIndex = -1;
        lastVisibleIndex = -1;
        firstAnimationIndex = -1;
        lastAnimationIndex = -1;

        max = Vector3.negativeInfinity;
        min = Vector3.positiveInfinity;
        Vector3 leftTop, bottomRight;
        for (int i = startIndex; i < startIndex + length; i++)
        {
            // TODO This is for testing purposes while changing the index concept;
            if (i >= cData.Count) break;

            if (!cData[i].info.isVisible) continue;

            leftTop = cData[i].mesh.initial.vertex_TL.position;
            bottomRight = cData[i].mesh.initial.vertex_BR.position;
            max = new Vector3(Mathf.Max(max.x, leftTop.x, bottomRight.x), Mathf.Max(max.y, leftTop.y, bottomRight.y), Mathf.Max(max.z, leftTop.z, bottomRight.z));
            min = new Vector3(Mathf.Min(min.x, leftTop.x, bottomRight.x), Mathf.Min(min.y, leftTop.y, bottomRight.y), Mathf.Min(min.z, leftTop.z, bottomRight.z));

            if (firstVisibleIndex == -1) firstVisibleIndex = i;
            lastVisibleIndex = i;

            if (!animates(cData[i].info.character))
            {
                if (firstAnimationIndex == -1) firstAnimationIndex = i;
                lastAnimationIndex = i;
            }
        }
    }
}