using System.Collections;
using System.Collections.Generic;
using TMPEffects.Editor;
using TMPEffects.Timeline;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace TMPEffects.Editor
{
    [CustomTimelineEditor(typeof(TMPAnimationClip))]
    public class TMPAnimationClipEditor : ClipEditor
    {
        public override void OnClipChanged(TimelineClip clip)
        {
            var mClip = clip.asset as TMPAnimationClip;
            mClip.name = clip.displayName;

            // Clamp entry and exit based on total duration, given the last moved one precedence
            if (mClip.lastMovedEntry == 0)
            {
                mClip.entryDuration = Mathf.Clamp(mClip.entryDuration, 0f, (float)clip.duration);
                mClip.exitDuration = Mathf.Clamp(mClip.exitDuration, 0f,
                    (float)clip.duration - mClip.entryDuration);
            }
            else
            {
                mClip.exitDuration = Mathf.Clamp(mClip.exitDuration, 0f, (float)clip.duration);
                mClip.entryDuration = Mathf.Clamp(mClip.entryDuration, 0f,
                    (float)clip.duration - mClip.exitDuration);
            }
        }

        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            TMPAnimationClip modClip = clip.asset as TMPAnimationClip;
            TMPEffectsClipEditorUtility.DrawBackground(clip, region, modClip.entryDuration, modClip.exitDuration,
                modClip.entryCurve, modClip.exitCurve);
        }
    }
}
