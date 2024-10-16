using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[CustomTimelineEditor(typeof(TMPMeshModifierClip))]
public class TMPMeshModifierClipEditor : ClipEditor
{
    public override void OnClipChanged(TimelineClip clip)
    {
        var mClip = clip.asset as TMPMeshModifierClip;
        mClip.Step.duration = (float)clip.duration;
        mClip.name = clip.displayName;

        if (mClip.Step.lastMovedEntry == 0)
        {
            mClip.Step.entryDuration = Mathf.Clamp(mClip.Step.entryDuration, 0f, (float)clip.duration);
            mClip.Step.exitDuration = Mathf.Clamp(mClip.Step.exitDuration, 0f, (float)clip.duration - mClip.Step.entryDuration);
        }
        else 
        {
            mClip.Step.exitDuration = Mathf.Clamp(mClip.Step.exitDuration, 0f, (float)clip.duration);
            mClip.Step.entryDuration = Mathf.Clamp(mClip.Step.entryDuration, 0f, (float)clip.duration - mClip.Step.exitDuration);
        }
    }

    public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
    {
        var mClip = clip.asset as TMPMeshModifierClip;

        float leftWidth, rightWidth;
        leftWidth = (mClip.Step.entryDuration / (float)clip.duration) * region.position.width;
        rightWidth = (mClip.Step.exitDuration / (float)clip.duration) * region.position.width;

        //     Debug.Log(leftWidth + "   " + mClip.Step.entryDuration + "   " + clip.duration );
        // // Calculate the blend in/out visual areas
        // Rect blendInRect = new Rect(region.position.x, region.position.y, leftWidth, region.position.height);
        // Rect blendOutRect = new Rect(region.position.x + region.position.width - rightWidth, region.position.y, rightWidth, region.position.height);
        //
        // // Draw custom blend in/out visuals
        // EditorGUI.DrawRect(blendInRect, new Color(0.2f, 0.6f, 1f, 0.3f)); // Blend In
        // EditorGUI.DrawRect(blendOutRect, new Color(0.2f, 0.6f, 1f, 0.3f)); // Blend Out
        //

        var rect = region.position;
        if (TimelineEditor.selectedClips.Contains(clip))
        {
            rect.yMin += 1f;
            rect.yMax -= 0f;
        }
        else
        {
            rect.yMin -= 0.75f;
            rect.yMax += 2.5f;
        }

        var inRect = new Rect(rect.x, rect.y, leftWidth, rect.height);
        var outRect = new Rect(rect.x + rect.width - rightWidth, rect.y, rightWidth, rect.height);

        EditorGUI.DrawRect(inRect, new Color(0.2f, 0.6f, 1f, 0.3f));
        EditorGUI.DrawRect(outRect, new Color(0.2f, 0.6f, 1f, 0.3f));

        DrawBackgroundWithCurve(mClip.Step.entryCurve, inRect, leftWidth, true);
        DrawBackgroundWithCurve(mClip.Step.exitCurve, outRect, rightWidth, false);
    }

    public override ClipDrawOptions GetClipOptions(TimelineClip clip)
    {
        return new ClipDrawOptions()
        {
            tooltip = "My tooltip :)",
            hideScaleIndicator = true,
        };
    }

    void DrawBackgroundWithCurve(AnimationCurve curve, Rect position, float width, bool blendin)
    {
        if (curve == null)
            return;
        if ((int)(width * 50) <= 3)
            return;

        Rect rect = position;
        Handles.BeginGUI();

        Vector3[] points = new Vector3[(int)(width * 50)];
        points[0] = new Vector3(rect.xMin + width, rect.yMin, 0);
        points[1] = new Vector3(rect.xMin + width, rect.yMax, 0);
        points[2] = new Vector3(rect.xMin, rect.yMax, 0);

        for (int i = 0; i < points.Length - 3; i++)
        {
            float t = (float)i / (points.Length - 4);

            float x = Mathf.Lerp(rect.xMin, rect.xMin + width, t);
            float y = rect.yMax - curve.Evaluate(blendin ? t : 1 - t) * rect.height;
            points[i + 3] = new Vector3(x, y, 0);
        }

        Handles.color = Color.black;
        // Handles.DrawAAConvexPolygon(points);
        // DrawAAConcavePolygon2(points, position.yMax);
        Handles.color = Color.black;
        Handles.DrawAAPolyLine(points);
        Handles.EndGUI();
    }

    private void DrawAAConcavePolygon(Vector3[] points)
    {
        var sum = Vector3.zero;
        for (int i = 0; i < points.Length; i++)
        {
            sum += points[i];
        }

        var avg = sum / points.Length;

        Vector3[] tmp = new Vector3[3];
        tmp[0] = avg;
        for (int i = 0; i < points.Length - 1; i++)
        {
            tmp[1] = points[i];
            tmp[2] = points[i + 1];
            Handles.DrawAAConvexPolygon(tmp);
        }
    }

    private void DrawAAConcavePolygon2(Vector3[] points, float ymin)
    {
        var topRight = points[0];
        var bottomRight = points[1];
        Vector3 currentPoint;

        for (int i = points.Length - 1; i >= 3; i--)
        {
            currentPoint = points[i];
            var currPointBttm = currentPoint;
            currPointBttm.y = ymin;

            Handles.DrawAAConvexPolygon(currentPoint, topRight, bottomRight, currPointBttm);

            topRight = currentPoint;
            bottomRight = topRight;
            bottomRight.y = ymin;
        }
    }
}