using UnityEngine;
using UnityEditor;
using TMPEffects.TMPAnimations;
namespace TMPEffects.Editor
{
    [CustomEditor(typeof(TMPHideAnimation), true)]
    internal class TMPHideAnimationEditor : TMPAnimationEditorBase
    {
        protected float restartDelay = 2f;
        protected float timeDone;
        protected float startTime = -1f;
        protected float hideTime = -1f;

        private static readonly GUIContent restartGUI = new GUIContent("Restart Delay", "The delay before restarting the animation once done.");
        
        protected override void OnChange(object anim)
        {
            base.OnChange(anim);
            animator.ResetTime();
            animator.ShowAll(true);
            timeDone = -1f;
            startTime = Time.time;

            this.anim.context = null;
        }

        protected override void DrawPreviewBar()
        {
            base.DrawPreviewBar();

            var prev = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 80;
            EditorGUILayout.BeginHorizontal();
            restartDelay = EditorGUILayout.Slider(restartGUI, restartDelay, 0, 10, GUILayout.Width(EditorGUIUtility.currentViewWidth / 2f));
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = prev;

            if (timeDone != -1f)
            {
                if (Time.time - timeDone >= restartDelay / 2f)
                {
                    animator.ShowAll(true);
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
                animator.HideAll(true);
            }
        }
    }
}