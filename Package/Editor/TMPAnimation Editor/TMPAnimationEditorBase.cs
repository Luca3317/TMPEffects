using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPEffects.TMPAnimations;
using TMPro;
using TMPEffects.Components;
using TMPEffects.TMPSceneAnimations;
using TMPEffects.CharacterData;
using TMPEffects.Tags;
using System.Collections.ObjectModel;
using TMPEffects.Components.Animator;
using TMPEffects.Databases;
using TMPEffects.ObjectChanged;

namespace TMPEffects.Editor
{
    public class TMPAnimationEditorBase : UnityEditor.Editor
    {
        protected PreviewRenderUtility previewUtility;
        protected GameObject targetObject;
        protected TMP_Text targetText;
        protected TMPAnimator animator;
        protected WrapperAnimation anim;
        protected float lastUpdateTime = -1f;
        protected bool animate = true;

        protected virtual void OnEnable()
        {
            previewUtility = new PreviewRenderUtility();
            SetupPreviewScene();
            SetAnimation();

            (target as INotifyObjectChanged).ObjectChanged -= OnChange;
            (target as INotifyObjectChanged).ObjectChanged += OnChange;
        }

        protected virtual void OnDisable()
        {
            if (previewUtility != null) previewUtility.Cleanup();
            (target as INotifyObjectChanged).ObjectChanged -= OnChange;
            if (targetObject != null)
            {
                try
                {
                    Object.DestroyImmediate(targetObject);
                }
                catch
                {
                    Debug.LogError("Failed to dispose targetObject correctly");
                }
            }
        }

        public override GUIContent GetPreviewTitle()
        {
            return new GUIContent("TMPAnimation Preview");
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnPreviewSettings()
        {
            GUIStyle animationButtonStyle = new GUIStyle(GUI.skin.button);
            animationButtonStyle.richText = true;
            char animationC = animate ? '\u2713' : '\u2717';
            GUIContent animationButtonContent = new GUIContent("Toggle preview " + (animate ? "<color=#90ee90>" : "<color=#f1807e>") + animationC.ToString() + "</color>");

            if (GUILayout.Button(animationButtonContent, animationButtonStyle))
            {
                animate = !animate;
                if (!animate) animator.ResetAnimations();
            }

            if (GUILayout.Button("Restart"))
            {
                animator.ResetTime();
                OnChange(anim);
            }
        }

        public override void DrawPreview(Rect previewArea)
        {
            if (animator.Tags.Count == 0)
            {
                if (!animator.Tags.TryAdd(new TMPEffectTag("preview", TMPAnimator.ANIMATION_PREFIX, new Dictionary<string, string>()), new TMPEffectTagIndices(0, -1, 0)))
                {
                    Debug.LogError("Failed to add tag; SetAnimation not called correctly?");
                }
            }

            UpdateAnimation();

            previewUtility.BeginPreview(previewArea, previewBackground: GUIStyle.none);
            previewUtility.Render();
            var texture = previewUtility.EndPreview();
            GUI.DrawTexture(previewArea, texture);

            DrawPreviewBar();
        }

        protected virtual void UpdateAnimation()
        {
            if (!animate)
            {
                lastUpdateTime = Time.time;
                return;
            }
            animator.UpdateAnimations(lastUpdateTime == -1f ? 0f : Time.time - lastUpdateTime);
            lastUpdateTime = Time.time;
        }

        protected virtual void DrawPreviewBar()
        {
            var prev = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 30;

            EditorGUILayout.BeginHorizontal();

            targetText.fontSize = EditorGUILayout.Slider(new GUIContent("Size"), targetText.fontSize, 1, 50, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.35f));
            targetText.text = EditorGUILayout.TextField(targetText.text, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.65f));

            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = prev;
        }

        protected virtual void OnChange(object anim)
        {
            SetAnimation();
        }

        protected void SetupPreviewScene()
        {
            targetObject = EditorUtility.CreateGameObjectWithHideFlags("Test " + UnityEngine.Random.Range(0, 100), HideFlags.HideAndDontSave);
            targetText = targetObject.AddComponent<TextMeshPro>();
            targetObject.transform.position = Vector3.zero;

            targetText.alignment = TextAlignmentOptions.Center;
            targetText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            targetText.text = "TMPEffects";
            targetText.fontSize = 15;
            targetText.overflowMode = TextOverflowModes.Overflow;
            targetText.enableWordWrapping = false;

            animator = targetObject.AddComponent<TMPAnimator>();
            animator.enabled = true;
            animator.SetUpdateFrom(TMPEffects.Components.Animator.UpdateFrom.Script);

            previewUtility.AddSingleGO(targetObject);
            previewUtility.camera.transform.position = new Vector3(0f, 0f, -10f);
            previewUtility.camera.nearClipPlane = 5f;
            previewUtility.camera.farClipPlane = 20f;

            anim = targetObject.AddComponent<WrapperAnimation>();
        }

        protected virtual void SetAnimation()
        {
            anim.tmpanimation = target as ITMPAnimation;
            animator.SceneAnimations.Clear();
            animator.SceneAnimations.Add("preview", anim);
            targetText.ForceMeshUpdate(true, true);
        }

        public override bool HasPreviewGUI()
        {
            ITMPAnimation t = target as ITMPAnimation;
            return t != null && t.ValidateParameters(dummyDict, animator.KeywordDatabase);
        }

        protected class WrapperAnimation : TMPSceneAnimation
        {
            public ITMPAnimation tmpanimation;

            public IAnimationContext context;

            public override void Animate(CharData cData, IAnimationContext context)
            {
                this.context = context;
                tmpanimation.Animate(cData, context);
            }

            public override object GetNewCustomData() => tmpanimation.GetNewCustomData();

            public override void SetParameters(object customData, IDictionary<string, string> parameters,
                ITMPKeywordDatabase keywordDatabase)
            {
                tmpanimation.SetParameters(customData, parameters, keywordDatabase);
            }

            public override bool ValidateParameters(IDictionary<string, string> parameters, ITMPKeywordDatabase keywordDatabase)
                => tmpanimation.ValidateParameters(parameters, keywordDatabase);
        }

        protected ReadOnlyDictionary<string, string> dummyDict = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>() { { "", "" } });
    }
}

