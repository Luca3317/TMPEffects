using UnityEngine;
using UnityEditor;
using TMPEffects.TMPAnimations;
using TMPEffects.Components;

namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPShowAnimation), true)]
    internal class TMPShowAnimationEditor : TMPAnimationEditorBase
    {
        protected float restartDelay = 2f;
        protected float timeDone = -1f;

        protected TMPWriter writer;

        private static readonly GUIContent restartGUI = new GUIContent("Restart Delay", "The delay before restarting the animation once done.");
        private static readonly GUIContent delayGUI = new GUIContent("Delay", "The delay of the writer.");

        protected override void OnEnable()
        {
            base.OnEnable();
            writer = targetText.gameObject.AddComponent<TMPWriter>();
            writer.enabled = true;
            writer.StartWriter();
        }

        protected override void OnChange(object anim)
        {
            base.OnChange(anim);
            animator.ResetTime();
            timeDone = -1f;

            var delay = writer.CurrentDelays.delay;
            writer.RestartWriter();
            writer.CurrentDelays.delay = delay;

            this.anim.context = null;
        }

        protected override void DrawPreviewBar()
        {
            base.DrawPreviewBar();

            var prev = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 80;
            EditorGUILayout.BeginHorizontal();
            restartDelay = EditorGUILayout.Slider(new GUIContent("Restart delay"), restartDelay, 0, 10, GUILayout.Width(EditorGUIUtility.currentViewWidth / 2f));
            EditorGUIUtility.labelWidth = 40;
            writer.CurrentDelays.delay = EditorGUILayout.Slider(new GUIContent("Delay"), writer.CurrentDelays.delay, 0, 1, GUILayout.Width(EditorGUIUtility.currentViewWidth / 2f));
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = prev;

            if (timeDone != -1f)
            {
                if (Time.time - timeDone >= restartDelay / 2f)
                {
                    animator.HideAll(true);
                }

                if (Time.time - timeDone >= restartDelay)
                {
                    timeDone = -1f;
                    OnChange(target);
                }
            }
            else if (anim.context != null && anim.context.Finished(anim.context.SegmentData.lastAnimationIndex))
            {
                timeDone = Time.time;
            }
        }
    }
}