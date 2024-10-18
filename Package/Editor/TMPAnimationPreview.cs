//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using TMPEffects.CharacterData;
//using TMPEffects.Components;
//using TMPEffects.Tags;
//using TMPEffects.TMPAnimations;
//using TMPEffects.TMPSceneAnimations;
//using TMPro;
//using UnityEditor;
//using UnityEngine;

//[CustomPreview(typeof(ITMPAnimation))]
//public class TMPAnimationPreview : ObjectPreview
//{

//    PreviewRenderUtility previewUtility;
//    private GameObject targetObject;
//    private TMP_Text targetText;
//    private TMPAnimator animator;

//    private void Call()
//    {
//        Debug.Log("CALL");
//    }

//    public override bool HasPreviewGUI()
//    {
//        return target != null && (target as ITMPAnimation).ValidateParameters(new Dictionary<string, string>());
//    }

//    public override void Initialize(UnityEngine.Object[] targets)
//    {
//        base.Initialize(targets);

//        previewUtility = new PreviewRenderUtility();
//        SetupPreviewScene();
//    }

//    private class WrapperAnimation : TMPSceneAnimation
//    {
//        public ITMPAnimation tmpanimation;

//        public override void Animate(CharData cData, IAnimationContext context)
//        {
//            tmpanimation.Animate(cData, context);
//        }

//        public override object GetNewCustomData() => tmpanimation.GetNewCustomData();

//        public override void SetParameters(object customData, IDictionary<string, string> parameters)
//             => tmpanimation.SetParameters(customData, parameters);

//        public override bool ValidateParameters(IDictionary<string, string> parameters)
//            => tmpanimation.ValidateParameters(parameters);
//    }

//    private void SetupPreviewScene()
//    {
//        EditorApplication.update += Updatess;

//        //Debug.Log("Called with null " + (m_Targets == null));
//        //Debug.Log("Called with " + m_Targets.Length + " And " + m_Targets.Where(x => x != null).Count());

//        //Debug.Log("Hello");
//        targetObject = new GameObject("Test");
//        targetText = targetObject.AddComponent<TextMeshPro>();
//        targetObject.transform.position = Vector3.zero;

//        targetText.alignment = TextAlignmentOptions.Center;
//        targetText.horizontalAlignment = HorizontalAlignmentOptions.Center;
//        targetText.text = "TMPEffects";
//        targetText.fontSize = 15;


//        //Debug.Log("Adding");
//        animator = targetObject.AddComponent<TMPAnimator>();
//        GameObject go = new GameObject();
//        go.transform.parent = targetObject.transform;
//        WrapperAnimation anim = go.AddComponent<WrapperAnimation>();

//        //if (go == null) Debug.LogWarning("GO NULL");
//        //else if (anim == null) Debug.LogWarning("ANIM NULL");
//        //else if (target == null) Debug.LogWarning("target NULL");
//        //else if (target as ITMPAnimation == null) Debug.LogWarning("target cast NULL");
//        //else Debug.Log("all fucking good");

//        anim.tmpanimation = target as ITMPAnimation;
//        animator.SceneAnimations.Add("preview", anim);
//        animator.enabled = true;

//        animator.SetUpdateFrom(TMPEffects.Components.Animator.UpdateFrom.Script);

//        Debug.Log("Animator " + animator.isActiveAndEnabled + " " + animator.enabled + " " + animator.gameObject.activeInHierarchy);

//        // Since we want to manage this instance ourselves, hide it
//        // from the current active scene, but remember to also destroy it.
//        //targetObject.hideFlags = HideFlags.HideAndDontSave;
//        previewUtility.AddSingleGO(targetObject);

//        // Camera is spawned at origin, so position is in front of the cube.
//        previewUtility.camera.transform.position = new Vector3(0f, 0f, -10f);

//        // This is usually set very small for good performance, but
//        // we need to shift the range to something our cube can fit between.
//        previewUtility.camera.nearClipPlane = 5f;
//        previewUtility.camera.farClipPlane = 20f;

//        targetObject.hideFlags = HideFlags.HideAndDontSave;
//        go.hideFlags = HideFlags.HideAndDontSave;
//    }

//    public override GUIContent GetPreviewTitle()
//    {
//        return new GUIContent("TMPEffects Animation");
//    }

//    public override string GetInfoString()
//    {
//        return "This is the info";
//    }

//    public override void Cleanup()
//    {
//        base.Cleanup();
//        if (targetObject != null) UnityEngine.Object.DestroyImmediate(targetObject);
//        if (previewUtility != null) previewUtility.Cleanup();
//    }

//    void Update()
//    {
//        Debug.Log("Update queueed");
//    }

//    void Updatess()
//    {
//        if (animator != null)
//            animator.UpdateAnimations(Time.deltaTime + 0.1f);
//        EditorApplication.QueuePlayerLoopUpdate();
//    }

//    public override void OnPreviewGUI(Rect r, GUIStyle background)
//    {
//        if (animator.Tags.Count == 0)
//        {
//            if (!animator.Tags.TryAdd(new TMPEffectTag("preview", TMPAnimator.ANIMATION_PREFIX, new Dictionary<string, string>()), new TMPEffectTagIndices(0, -1, 0)))
//                Debug.LogError("COULDNT ADD TAG??");
//            else Debug.LogWarning("ADDED!!!");
//        }

//        animator.UpdateAnimations(Time.deltaTime);


//        Rect rect = r;
//        previewUtility.BeginPreview(rect, previewBackground: GUIStyle.none);
//        previewUtility.Render();
//        var texture = previewUtility.EndPreview();
//        GUI.DrawTexture(rect, texture);

//        //Debug.Log(Environment.StackTrace);
//    }
//}


////using UnityEngine;
////using UnityEditor;
////using TMPEffects.TMPAnimations;
////using TMPro;
////using TMPEffects.Components;
////using TMPEffects.Databases.AnimationDatabase;
////using TMPEffects.TMPSceneAnimations;
////using TMPEffects.CharacterData;
////using System.Collections.Generic;
////using TMPEffects.Tags;
////using System.Linq;

////[CustomPreview(typeof(TMPAnimation))]
////public class TMPAnimationPreview : ObjectPreview
////{
////    private PreviewRenderUtility previewUtility;
////    private GameObject targetObject;
////    private TMP_Text targetText;
////    private TMPAnimator animator;

////    private void OnEnable()
////    {

////    }

////    public override GUIContent GetPreviewTitle()
////    {
////        return new GUIContent("Animation preview");
////    }

////    public override void Initialize(Object[] targets)
////    {
////        base.Initialize(targets);

////        Debug.LogWarning("CALLED");
////        previewUtility = new PreviewRenderUtility();
////        SetupPreviewScene();
////    }

////    private class WrapperAnimation : TMPSceneAnimation
////    {
////        public ITMPAnimation tmpanimation;

////        public override void Animate(CharData cData, IAnimationContext context)
////        {
////            tmpanimation.Animate(cData, context);
////        }

////        public override object GetNewCustomData() => tmpanimation.GetNewCustomData();

////        public override void SetParameters(object customData, IDictionary<string, string> parameters)
////             => tmpanimation.SetParameters(customData, parameters);

////        public override bool ValidateParameters(IDictionary<string, string> parameters)
////            => tmpanimation.ValidateParameters(parameters);
////    }

////    public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
////    {
////        Debug.Log("Called as well");
////        base.OnInteractivePreviewGUI(r, background);
////    }

////    public override void OnPreviewSettings()
////    {
////        base.OnPreviewSettings();
////        EditorGUILayout.LabelField(new GUIContent("AYOOOOOOOO"));
////    }

////    private void SetupPreviewScene()
////    {
////        Debug.Log("Called with null " + (m_Targets == null));
////        Debug.Log("Called with " + m_Targets.Length + " And " + m_Targets.Where(x => x != null).Count());

////        //Debug.Log("Hello");
////        targetObject = new GameObject("Test");
////        targetText = targetObject.AddComponent<TextMeshPro>();
////        targetObject.transform.position = Vector3.zero;

////        targetText.alignment = TextAlignmentOptions.Center;
////        targetText.horizontalAlignment = HorizontalAlignmentOptions.Center;
////        targetText.text = "TMPEffects";
////        targetText.fontSize = 15;


////        //Debug.Log("Adding");
////        animator = targetObject.AddComponent<TMPAnimator>();
////        GameObject go = new GameObject();
////        go.transform.parent = targetObject.transform;
////        WrapperAnimation anim = go.AddComponent<WrapperAnimation>();

////        if (go == null) Debug.LogWarning("GO NULL");
////        else if (anim == null) Debug.LogWarning("ANIM NULL");
////        else if (target == null) Debug.LogWarning("target NULL");
////        else if (target as ITMPAnimation == null) Debug.LogWarning("target cast NULL");
////        else Debug.Log("all fucking good");

////        anim.tmpanimation = target as ITMPAnimation;
////        animator.AddSceneAnimation("preview", anim);
////        //animator.SetUpdateFrom(TMPEffects.Components.Animator.UpdateFrom.Script);

////        // Since we want to manage this instance ourselves, hide it
////        // from the current active scene, but remember to also destroy it.
////        //targetObject.hideFlags = HideFlags.HideAndDontSave;
////        previewUtility.AddSingleGO(targetObject);

////        // Camera is spawned at origin, so position is in front of the cube.
////        previewUtility.camera.transform.position = new Vector3(0f, 0f, -10f);

////        // This is usually set very small for good performance, but
////        // we need to shift the range to something our cube can fit between.
////        previewUtility.camera.nearClipPlane = 5f;
////        previewUtility.camera.farClipPlane = 20f;

////        targetObject.hideFlags = HideFlags.HideAndDontSave;
////        go.hideFlags = HideFlags.HideAndDontSave;
////    }

////    public override bool HasPreviewGUI()
////    {
////        return true;
////    }

////    //private void Update()
////    //{
////    //    Debug.Log("update");
////    //    // Just do some random modifications here.
////    //    float time = (float)EditorApplication.timeSinceStartup * 15;
////    //    //targetObject.transform.rotation = Quaternion.Euler(time * 2f, time * 4f, time * 3f);

////    //    if (!animator.Preqweqw)
////    //    {
////    //        animator.StartPreviewPublic();
////    //    }

////    //    // Since this is the most important window in the editor, let's use our
////    //    // resources to make this nice and smooth, even when running in the background.
////    //    Repaint();

////    //    //animator.UpdateAnimations(Time.deltaTime);
////    //}

////    public override void Cleanup()
////    {
////        base.Cleanup();
////        if (targetObject != null) Object.DestroyImmediate(targetObject);
////        if (previewUtility != null) previewUtility.Cleanup();
////    }

////    public override void OnPreviewGUI(Rect r, GUIStyle background)
////    {
////        Debug.Log("CALLELELE");
////        if (!animator.Preqweqw)
////        {
////            if (animator.Tags.Count == 0)
////            {
////                if (!animator.Tags.TryAdd(new TMPEffectTag("preview", TMPAnimator.ANIMATION_PREFIX, new Dictionary<string, string>()), new TMPEffectTagIndices(0, -1, 0)))
////                    Debug.LogError("COULDNT ADD TAG??");
////                else Debug.LogWarning("ADDED!!!");
////            }

////            animator.StartPreviewPublic();
////        }
////        ////Rect rect = new Rect(0, 0, base.position.width, base.position.height);
////        Rect rect = r;
////        previewUtility.BeginPreview(rect, previewBackground: GUIStyle.none);
////        previewUtility.Render();
////        var texture = previewUtility.EndPreview();
////        GUI.DrawTexture(rect, texture);

////        EditorApplication.QueuePlayerLoopUpdate();
////    }
////}