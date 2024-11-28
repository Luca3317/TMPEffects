using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

internal static class TMPEffectsTimelineEditorPrefsKeys
{
    internal const string DRAW_CURVES_EDITORPREFS_KEY = "TMPEffects.Timeline.DrawCurves.EditorPrefs";
}

[CustomTimelineEditor(typeof(TMPMeshModifierClip))]
public class TMPMeshModifierClipEditor : ClipEditor
{
    public override void OnClipChanged(TimelineClip clip)
    {
        var mClip = clip.asset as TMPMeshModifierClip;
        mClip.Step.Step.duration = (float)clip.duration;
        mClip.name = clip.displayName;
        
        if (mClip.Step.lastMovedEntry == 0)
        {
            mClip.Step.Step.entryDuration = Mathf.Clamp(mClip.Step.Step.entryDuration, 0f, (float)clip.duration);
            mClip.Step.Step.exitDuration = Mathf.Clamp(mClip.Step.Step.exitDuration, 0f,
                (float)clip.duration - mClip.Step.Step.entryDuration);
        }
        else
        {
            mClip.Step.Step.exitDuration = Mathf.Clamp(mClip.Step.Step.exitDuration, 0f, (float)clip.duration);
            mClip.Step.Step.entryDuration = Mathf.Clamp(mClip.Step.Step.entryDuration, 0f,
                (float)clip.duration - mClip.Step.Step.exitDuration);
        }
    }

    public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
    {
        var mClip = clip.asset as TMPMeshModifierClip;

        float leftWidth, rightWidth;

        leftWidth = (mClip.Step.Step.entryDuration / (float)clip.duration) * region.position.width;
        rightWidth = (mClip.Step.Step.exitDuration / (float)clip.duration) * region.position.width;

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

        var drawCurves = EditorPrefs.GetBool(TMPEffectsTimelineEditorPrefsKeys.DRAW_CURVES_EDITORPREFS_KEY);
        if (drawCurves)
        {
            DrawBackgroundWithCurve(mClip.Step.Step.entryCurve.curve, inRect, leftWidth, true);
            DrawBackgroundWithCurve(mClip.Step.Step.exitCurve.curve, outRect, rightWidth, false);
        }
        else
        {
            Handles.color = Color.black;
            Handles.DrawAAPolyLine(new Vector3(inRect.xMin, inRect.yMax), new Vector3(inRect.xMax, inRect.yMin));
            Handles.DrawAAPolyLine(outRect.min, outRect.max);
        }
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
        if ((int)(width * 5) <= 3)
            return;

        Rect rect = position;
        Vector3[] points = new Vector3[(int)(width * 5)];
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

        Handles.BeginGUI();
        Handles.color = Color.black;
        Handles.DrawAAPolyLine(points);
        Handles.EndGUI();
    }
}