using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using TMPEffects.TMPSceneAnimations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using AnimationUtility = TMPEffects.TMPAnimations.AnimationUtility;
using Object = UnityEngine.Object;

namespace TMPEffects.Editor.Timeline
{
    public class KeyFrameEditorWindow : EditorWindow
    {
        protected PreviewRenderUtility previewUtility;
        protected Camera previewCamera;
        protected GameObject targetObject;
        protected TMP_Text targetText;
        protected TMPAnimator animator;
        protected float lastUpdateTime = -1f;

        protected char previewChar = 'A';

        private TMPMeshModifiers modifier;

        private WrapAnimation anim;

        private SmthThatAppliesModifiers applier;

        private GameObject shadowObject;
        private TMP_Text shadowText;

        private Rect vertexWindowRect;
        private Rect characterWindowRect;

        private string vertexWindowTitle = "";
        private string characterWindowTitle = "Character";

        private bool visualsFoldout;

        private bool shadowFoldout;
        private bool showCharacterShadow;
        private bool showMeshShadow;

        private Color meshShadowColor;
        private Color characterShadowColor;

        private Color meshColor = Color.white;

        [Flags]
        private enum Selection
        {
            BL,
            TL,
            TR,
            BR,
            Character
        }

        private Selection selection;

        private int blID, tlID, trID, brID;

        #region Init

        private void OnEnable()
        {
            Debug.Log("KeyFrameEditorWindow OnEnable");
            previewUtility = new PreviewRenderUtility(true, true);
            vertexWindowRect = new Rect(position.x, position.y, 250, 350);
            characterWindowRect = new Rect(position.x, position.y, 250, 350);

            SetupPreviewScene();
            wantsMouseMove = true;
        }

        protected void SetupPreviewScene()
        {
            targetObject = SetupTargetObject();
            targetText = SetupTargetText(targetObject);
            animator = SetupAnimator(targetObject);
            anim = SetupAnimation(targetObject, animator);
            shadowObject = SetupShadow(targetObject);
            shadowText = shadowObject.GetComponent<TMP_Text>();

            previewUtility.AddSingleGO(targetObject);
            previewUtility.camera.transform.position = new Vector3(0f, 0f, -10f);
            previewUtility.camera.nearClipPlane = 0.1f;
            previewUtility.camera.farClipPlane = 1000f;
            previewCamera = previewUtility.camera;
        }

        private GameObject SetupTargetObject()
        {
            var targetObject = EditorUtility.CreateGameObjectWithHideFlags("Test " + UnityEngine.Random.Range(0, 100),
                HideFlags.HideAndDontSave);

            targetObject.transform.position = Vector3.zero;
            return targetObject;
        }

        private TMP_Text SetupTargetText(GameObject targetObject)
        {
            var targetText = targetObject.AddComponent<TextMeshPro>();
            targetText.gameObject.AddComponent<MeshCollider>();

            targetText.alignment = TextAlignmentOptions.Center;
            targetText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            targetText.text = "<animation>" + previewChar.ToString();
            targetText.fontSize = 15;
            targetText.overflowMode = TextOverflowModes.Overflow;
            targetText.alignment = TextAlignmentOptions.Center;
            targetText.verticalAlignment = VerticalAlignmentOptions.Middle;
            targetText.enableWordWrapping = false;
            targetText.raycastTarget = true;

            return targetText;
        }

        private TMPAnimator SetupAnimator(GameObject targetObject)
        {
            var animator = targetObject.AddComponent<TMPAnimator>();
            animator.enabled = true;
            animator.SetUpdateFrom(TMPEffects.Components.Animator.UpdateFrom.Script);
            return animator;
        }

        private WrapAnimation SetupAnimation(GameObject targetObject, TMPAnimator animator)
        {
            var sceneAnimationGO = EditorUtility.CreateGameObjectWithHideFlags("Anim", HideFlags.HideAndDontSave);
            var anim = sceneAnimationGO.AddComponent<WrapAnimation>();

            anim.Modifier = new TMPMeshModifiers();
            modifier = anim.Modifier;
            anim.Applier = new SmthThatAppliesModifiers();
            applier = anim.Applier;

            sceneAnimationGO.transform.SetParent(targetObject.transform);
            if (!animator.SceneAnimations.TryAdd("animation", anim))
            {
                throw new Exception("Failed to add scene animation");
            }

            return anim;
        }

        private GameObject SetupShadow(GameObject targetObject)
        {
            var shadow = SetupTargetObject();
            var shadowText = SetupTargetText(shadow);
            shadow.name = "Shadow";
            shadowText.text = "A"; // TODO ugly

            shadow.transform.SetParent(targetObject.transform, true);
            shadow.transform.position = Vector3.zero;
            shadowText.color = characterShadowColor;

            shadowText.GetComponent<Renderer>().sortingOrder = -1;

            return shadow;
        }

        private void OnDisable()
        {
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

            if (previewUtility != null) previewUtility.Cleanup();
        }

        #endregion

        private void HandleVisualSettings()
        {
            visualsFoldout = EditorGUILayout.Foldout(visualsFoldout, "Visual Settings");
            if (!visualsFoldout) return;

            EditorGUI.indentLevel++;
            HandleMesh();
            HandleShadow();
            HandleGrid();
            EditorGUI.indentLevel--;
        }

        private void HandleMesh()
        {
            EditorGUI.indentLevel++;
            meshColor = EditorGUILayout.ColorField(new GUIContent("Mesh Color"), meshColor, GUILayout.Width(240));
            EditorGUI.indentLevel--;
        }

        private void HandleShadow()
        {
            shadowFoldout = EditorGUILayout.Foldout(shadowFoldout, "Shadow");
            if (shadowFoldout)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal(GUILayout.Width(240));

                EditorGUI.BeginChangeCheck();
                showCharacterShadow =
                    EditorGUILayout.Toggle(new GUIContent("Show shadow"), showCharacterShadow, GUILayout.Width(135));
                if (EditorGUI.EndChangeCheck())
                {
                    shadowObject.SetActive(showCharacterShadow);
                    if (showCharacterShadow)
                    {
                        shadowText.color = characterShadowColor;
                        shadowText.ForceMeshUpdate(true, true);
                    }
                }

                EditorGUI.BeginChangeCheck();
                characterShadowColor = EditorGUILayout.ColorField(characterShadowColor, GUILayout.Width(100));
                if (EditorGUI.EndChangeCheck())
                {
                    shadowText.color = characterShadowColor;
                    shadowText.ForceMeshUpdate(true, true);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(GUILayout.Width(240));
                showMeshShadow =
                    EditorGUILayout.Toggle(new GUIContent("Show Mesh Shadow"), showMeshShadow, GUILayout.Width(135));
                meshShadowColor = EditorGUILayout.ColorField(meshShadowColor, GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }
        }

        private float gridSize = 0.25f;
        private bool enableGrid = false;
        private bool gridFoldout = false;

        private void HandleGrid()
        {
            gridFoldout = EditorGUILayout.Foldout(gridFoldout, new GUIContent("Grid"));
            if (gridFoldout)
            {
                EditorGUI.indentLevel++;
                enableGrid = EditorGUILayout.Toggle(new GUIContent("Enable Grid"), enableGrid, GUILayout.Width(100));

                EditorGUILayout.LabelField(new GUIContent("Grid Size"));
                gridSize = EditorGUILayout.Slider(GUIContent.none, gridSize, 0.01f, 1f,
                    GUILayout.Width(240));


                EditorGUI.indentLevel--;
            }
        }

        private void HandleWindows()
        {
            BeginWindows();
            characterWindowRect =
                GUI.Window(GUIUtility.GetControlID(FocusType.Passive), characterWindowRect, OnCharacterWindowGUI,
                    "Character");

            if (selectedID.HasValue)
            {
                vertexWindowRect =
                    GUI.Window(selectedID.Value, vertexWindowRect, OnVertexWindowGUI,
                        vertexWindowTitle);
            }

            EndWindows();
        }

        private int? selectedID;

        private void HandleSelection()
        {
            var hotControl = EditorGUIUtility.hotControl;

            if (hotControl == blID)
            {
                selectedID = blID;
                vertexWindowTitle = "Vertex BL";
            }
            else if (hotControl == tlID)
            {
                selectedID = tlID;
                vertexWindowTitle = "Vertex TL";
            }
            else if (hotControl == trID)
            {
                selectedID = trID;
                vertexWindowTitle = "Vertex TR";
            }
            else if (hotControl == brID)
            {
                selectedID = brID;
                vertexWindowTitle = "Vertex BR";
            }
        }

        private void OnGUI()
        {
            blID = GUIUtility.GetControlID(FocusType.Passive);
            tlID = GUIUtility.GetControlID(FocusType.Passive);
            trID = GUIUtility.GetControlID(FocusType.Passive);
            brID = GUIUtility.GetControlID(FocusType.Passive);

            HandleSelection();

            UpdateAnimation();
            
            DrawPreview(new Rect(0, 0, position.width, position.height));

            Handles.DrawAAPolyLine(2f, new Color[] { Color.red, Color.red },
                new Vector3[] { position.center,  (Vector3)position.center + new Vector3(0f, -position.height) });
            
            if (GUILayout.Button("Reset character"))
            {
                targetText.transform.position = Vector3.zero;
                targetText.transform.localScale = Vector3.one;
                targetText.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                modifier = new TMPMeshModifiers();
                anim.Modifier = modifier;
                applier =
                    new SmthThatAppliesModifiers(); // To make sure changes show instantly and not when moving again
                anim.Applier = applier;
                UpdateAnimation();
            }

            HandleVisualSettings();
            HandleWindows();
        }

        #region Hover Windows

        private void OnVertexWindowGUI(int id)
        {
            (var bl, var tl, var tr, var br) =
                applier.CalculateVertexPositions(
                    new CharData(0, targetText.textInfo.characterInfo[0], 0), modifier);

            if (id == blID)
            {
                modifier.BL_Delta += EditorGUILayout.Vector3Field("Position", bl) - bl;
                modifier.BL_Delta =
                    EditorGUILayout.Vector3Field("Offset from original position", modifier.BL_Delta);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Color", GUILayout.Width(EditorGUIUtility.labelWidth));
                if (EditorGUILayout.Toggle(modifier.BL_Color.HasValue))
                {
                    EditorGUILayout.EndHorizontal();
                    if (!modifier.BL_Color.HasValue)
                        modifier.BL_Color.Value = Color.white;

                    Color c = modifier.BL_Color.Value;

                    modifier.BL_Color.Value = EditorGUILayout.ColorField("Color", c);
                }
                else
                {
                    modifier.BL_Color = null;
                    EditorGUILayout.EndHorizontal();
                }
            }
            else if (id == tlID)
            {
                modifier.TL_Delta += EditorGUILayout.Vector3Field("Position", tl) - tl;
                modifier.TL_Delta =
                    EditorGUILayout.Vector3Field("Offset from original position", modifier.TL_Delta);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Color", GUILayout.Width(EditorGUIUtility.labelWidth));
                if (EditorGUILayout.Toggle(modifier.TL_Color.HasValue))
                {
                    EditorGUILayout.EndHorizontal();
                    if (!modifier.TL_Color.HasValue)
                        modifier.TL_Color.Value = Color.white;

                    Color c = modifier.TL_Color.Value;

                    modifier.TL_Color.Value = EditorGUILayout.ColorField("Color", c);
                }
                else
                {
                    modifier.TL_Color = null;
                    EditorGUILayout.EndHorizontal();
                }
            }
            else if (id == trID)
            {
                modifier.TR_Delta += EditorGUILayout.Vector3Field("Position", tr) - tr;
                modifier.TR_Delta =
                    EditorGUILayout.Vector3Field("Offset from original position", modifier.TR_Delta);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Color", GUILayout.Width(EditorGUIUtility.labelWidth));
                if (EditorGUILayout.Toggle(modifier.TR_Color.HasValue))
                {
                    EditorGUILayout.EndHorizontal();
                    if (!modifier.TR_Color.HasValue)
                        modifier.TR_Color.Value = Color.white;

                    Color c = modifier.TR_Color.Value;

                    modifier.TR_Color.Value = EditorGUILayout.ColorField("Color", c);
                }
                else
                {
                    modifier.TR_Color = null;
                    EditorGUILayout.EndHorizontal();
                }
            }
            else if (id == brID)
            {
                modifier.BR_Delta += EditorGUILayout.Vector3Field("Position", br) - br;
                modifier.BR_Delta =
                    EditorGUILayout.Vector3Field("Offset from original position", modifier.BR_Delta);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Color", GUILayout.Width(EditorGUIUtility.labelWidth));
                if (EditorGUILayout.Toggle(modifier.BR_Color.HasValue))
                {
                    EditorGUILayout.EndHorizontal();
                    if (!modifier.BR_Color.HasValue)
                        modifier.BR_Color.Value = Color.white;

                    Color c = modifier.BR_Color.Value;

                    modifier.BR_Color.Value = EditorGUILayout.ColorField("Color", c);
                }
                else
                {
                    modifier.BR_Color = null;
                    EditorGUILayout.EndHorizontal();
                }
            }
            else EditorGUILayout.LabelField("Aint shit here; BL is " + blID + " selected is " + selectedID);


            GUI.DragWindow();
        }

        private void OnCharacterWindowGUI(int id)
        {
            // modifier.PositionDelta += EditorGUILayout.Vector3Field("Position", modifier.PositionDelta) - modifier.PositionDelta;
            modifier.PositionDelta =
                EditorGUILayout.Vector3Field("Position", modifier.PositionDelta);

            // TODO inspector rotation broken
            Vector3 angles = modifier.RotationDelta.eulerAngles;
            angles =
                EditorGUILayout.Vector3Field("Rotation", angles);
            modifier.RotationDelta = Quaternion.Euler(angles);

            Vector3 scale = modifier.ScaleDelta.MultiplyPoint3x4(Vector3.one);
            scale =
                EditorGUILayout.Vector3Field("Scale", scale);
            modifier.ScaleDelta = Matrix4x4.Scale(scale);

            GUI.DragWindow();
        }

        #endregion

        private void UpdateAnimation()
        {
            animator.UpdateAnimations(lastUpdateTime == -1f ? 0f : Time.time - lastUpdateTime);
            lastUpdateTime = Time.time;
        }

        // todo provisory name
        private void PreviewHandles()
        {
            // Get vertex positions
            (var bl, var tl, var tr, var br) =
                applier.CalculateVertexPositions(
                    new CharData(0, targetText.textInfo.characterInfo[0], 0), modifier);

            Handles.zTest = CompareFunction.Less;
            
            // Draw shadow
            if (showMeshShadow)
            {
                CharData charData = new CharData(0, targetText.textInfo.characterInfo[0], 0);
                Handles.DrawAAPolyLine(
                    new Color[] { meshShadowColor, meshShadowColor, meshShadowColor, meshShadowColor, meshShadowColor },
                    new Vector3[]
                    {
                        charData.initialMesh.GetPosition(0), charData.initialMesh.GetPosition(1),
                        charData.initialMesh.GetPosition(2), charData.initialMesh.GetPosition(3),
                        charData.initialMesh.GetPosition(0),
                    }
                );

                Handles.color = meshShadowColor;
                Handles.SphereHandleCap(0, charData.initialMesh.GetPosition(0), Quaternion.identity, 0.04f,
                    EventType.Repaint);
                Handles.SphereHandleCap(0, charData.initialMesh.GetPosition(1), Quaternion.identity, 0.04f,
                    EventType.Repaint);
                Handles.SphereHandleCap(0, charData.initialMesh.GetPosition(2), Quaternion.identity, 0.04f,
                    EventType.Repaint);
                Handles.SphereHandleCap(0, charData.initialMesh.GetPosition(3), Quaternion.identity, 0.04f,
                    EventType.Repaint);
            }

            Handles.zTest = CompareFunction.Always;
            // Character tools handle
            switch (Tools.current)
            {
                case Tool.Move:
                    modifier.PositionDelta = Handles.PositionHandle(Handles.PositionHandleIds.@default,
                                                 targetText.transform.position + modifier.PositionDelta,
                                                 Quaternion.identity) -
                                             targetText.transform.position;
                    break;

                case Tool.Scale:
                    var vec = modifier.ScaleDelta.MultiplyPoint3x4(Vector3.one);
                    vec = Handles.ScaleHandle(vec, modifier.PositionDelta + targetText.transform.position,
                        Quaternion.identity);
                    modifier.ScaleDelta = Matrix4x4.Scale(vec);
                    break;

                case Tool.Rotate:
                    modifier.RotationDelta = Handles.RotationHandle(modifier.RotationDelta,
                        targetText.transform.position + modifier.PositionDelta);
                    break;
            }

            // Vertex handles
            Handles.color = meshColor;
            modifier.BL_Delta +=
                Handles.FreeMoveHandle(blID, bl, 0.04f, Vector3.zero,
                    Handles.SphereHandleCap) - bl;
            modifier.TL_Delta +=
                Handles.FreeMoveHandle(tlID, tl, 0.04f, Vector3.zero,
                    Handles.SphereHandleCap) - tl;
            modifier.TR_Delta +=
                Handles.FreeMoveHandle(trID, tr, 0.04f, Vector3.zero,
                    Handles.SphereHandleCap) - tr;
            modifier.BR_Delta +=
                Handles.FreeMoveHandle(brID, br, 0.04f, Vector3.zero,
                    Handles.SphereHandleCap) - br;

            // Background lines
            Handles.DrawAAPolyLine(
                new Color[] { meshColor, meshColor, meshColor, meshColor, meshColor },
                new Vector3[] { bl, tl, tr, br, bl }
            );
        }

        public void DrawPreview(Rect previewArea)
        {
            previewUtility.BeginPreview(previewArea, previewBackground: GUIStyle.none);
            previewUtility.camera.Render();

            using (new Handles.DrawingScope(Matrix4x4.identity))
            {
                Handles.SetCamera(previewUtility.camera);
                PreviewHandles();
            }
            
            previewUtility.EndAndDrawPreview(previewArea);
        }

        [MenuItem("TMPEffects/Editor/KeyFrameEditorWindow")]
        public static void ShowWindow()
        {
            KeyFrameEditorWindow window = GetWindow<KeyFrameEditorWindow>();
            window.titleContent = new GUIContent("TMPEffects Keyframe Editor");
        }

        private class WrapAnimation : TMPSceneAnimation
        {
            public TMPMeshModifiers Modifier;
            public SmthThatAppliesModifiers Applier;

            public override void Animate(CharData cData, IAnimationContext context)
            {
                (var bl, var tl, var tr, var br) =
                    Applier.CalculateVertexPositions(cData, Modifier);

                cData.mesh.SetPosition(0, AnimationUtility.GetRawVertex(0, bl, cData, context));
                cData.mesh.SetPosition(1, AnimationUtility.GetRawVertex(1, tl, cData, context));
                cData.mesh.SetPosition(2, AnimationUtility.GetRawVertex(2, tr, cData, context));
                cData.mesh.SetPosition(3, AnimationUtility.GetRawVertex(3, br, cData, context));

                (var blC, var tlC, var trC, var brC) =
                    Applier.CalculateVertexColors(cData, Modifier);

                cData.mesh.SetColor(0, blC);
                cData.mesh.SetColor(1, tlC);
                cData.mesh.SetColor(2, trC);
                cData.mesh.SetColor(3, brC);
            }

            public override bool ValidateParameters(IDictionary<string, string> parameters)
            {
                return true;
            }

            public override object GetNewCustomData()
            {
                return null;
            }

            public override void SetParameters(object customData, IDictionary<string, string> parameters)
            {
            }
        }
    }
}


/*
 *
 *using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using TMPEffects.Components.Animator;
using TMPEffects.TMPAnimations;
using TMPEffects.TMPSceneAnimations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using AnimationUtility = TMPEffects.TMPAnimations.AnimationUtility;
using Object = UnityEngine.Object;

namespace TMPEffects.Editor.Timeline
{
    public class KeyFrameEditorWindow : EditorWindow
    {
        protected PreviewRenderUtility previewUtility;
        protected Camera previewCamera;
        protected GameObject targetObject;
        protected TMP_Text targetText;
        protected TMPAnimator animator;
        protected float lastUpdateTime = -1f;

        protected char previewChar = 'A';

        private TMPMeshModifiers modifier;

        private WrapAnimation anim;

        private SmthThatAppliesModifiers applier;

        private GameObject shadowObject;
        private TMP_Text shadowText;

        private Rect vertexWindowRect;
        private Rect characterWindowRect;

        private bool shadowFoldout;
        private bool showCharacterShadow;
        private bool showMeshShadow;

        private Color meshShadowColor;
        private Color characterShadowColor;

        [Flags]
        private enum Selection
        {
            BL,
            TL,
            TR,
            BR,
            Character
        }

        private Selection selection;

        #region Init

        private void OnEnable()
        {
            Debug.Log("KeyFrameEditorWindow OnEnable");
            previewUtility = new PreviewRenderUtility();
            vertexWindowRect = new Rect(position.x, position.y, 250, 350);
            characterWindowRect = new Rect(position.x, position.y, 250, 350);

            SetupPreviewScene();
            wantsMouseMove = true;
        }

        protected void SetupPreviewScene()
        {
            targetObject = SetupTargetObject();
            targetText = SetupTargetText(targetObject);
            animator = SetupAnimator(targetObject);
            anim = SetupAnimation(targetObject, animator);
            shadowObject = SetupShadow(targetObject);
            shadowText = shadowObject.GetComponent<TMP_Text>();

            previewUtility.AddSingleGO(targetObject);
            previewUtility.camera.transform.position = new Vector3(0f, 0f, -10f);
            previewUtility.camera.nearClipPlane = 0.1f;
            previewUtility.camera.farClipPlane = 1000f;
            previewCamera = previewUtility.camera;
        }

        private GameObject SetupTargetObject()
        {
            var targetObject = EditorUtility.CreateGameObjectWithHideFlags("Test " + UnityEngine.Random.Range(0, 100),
                HideFlags.HideAndDontSave);

            targetObject.transform.position = Vector3.zero;
            return targetObject;
        }

        private TMP_Text SetupTargetText(GameObject targetObject)
        {
            var targetText = targetObject.AddComponent<TextMeshPro>();
            targetText.gameObject.AddComponent<MeshCollider>();

            targetText.alignment = TextAlignmentOptions.Center;
            targetText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            targetText.text = "<animation>" + previewChar.ToString();
            targetText.fontSize = 15;
            targetText.overflowMode = TextOverflowModes.Overflow;
            targetText.alignment = TextAlignmentOptions.Center;
            targetText.verticalAlignment = VerticalAlignmentOptions.Middle;
            targetText.enableWordWrapping = false;
            targetText.raycastTarget = true;

            return targetText;
        }

        private TMPAnimator SetupAnimator(GameObject targetObject)
        {
            var animator = targetObject.AddComponent<TMPAnimator>();
            animator.enabled = true;
            animator.SetUpdateFrom(TMPEffects.Components.Animator.UpdateFrom.Script);
            return animator;
        }

        private WrapAnimation SetupAnimation(GameObject targetObject, TMPAnimator animator)
        {
            var sceneAnimationGO = EditorUtility.CreateGameObjectWithHideFlags("Anim", HideFlags.HideAndDontSave);
            var anim = sceneAnimationGO.AddComponent<WrapAnimation>();

            anim.Modifier = new TMPMeshModifiers();
            modifier = anim.Modifier;
            anim.Applier = new SmthThatAppliesModifiers();
            applier = anim.Applier;

            sceneAnimationGO.transform.SetParent(targetObject.transform);
            if (!animator.SceneAnimations.TryAdd("animation", anim))
            {
                throw new Exception("Failed to add scene animation");
            }

            return anim;
        }

        private GameObject SetupShadow(GameObject targetObject)
        {
            var shadow = SetupTargetObject();
            var shadowText = SetupTargetText(shadow);
            shadow.name = "Shadow";
            shadowText.text = "A"; // TODO ugly

            shadow.transform.SetParent(targetObject.transform, true);
            shadow.transform.position = Vector3.zero;
            shadowText.color = characterShadowColor;

            shadowText.GetComponent<Renderer>().sortingOrder = -1;

            return shadow;
        }

        private void OnDisable()
        {
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

            if (previewUtility != null) previewUtility.Cleanup();
        }

        #endregion

        private void HandleShadow()
        {
            shadowFoldout = EditorGUILayout.Foldout(shadowFoldout, "Shadow");
            if (shadowFoldout)
            {
                GUILayout.BeginHorizontal(GUIContent.none, GUIStyle.none, GUILayout.Width(240));

                EditorGUI.BeginChangeCheck();
                showCharacterShadow =
                    GUILayout.Toggle(showCharacterShadow, new GUIContent("Show shadow"), GUILayout.Width(135));
                if (EditorGUI.EndChangeCheck())
                {
                    shadowObject.SetActive(showCharacterShadow);
                    if (showCharacterShadow)
                    {
                        shadowText.color = characterShadowColor;
                        shadowText.ForceMeshUpdate(true, true);
                    }
                }

                EditorGUI.BeginChangeCheck();
                characterShadowColor = EditorGUILayout.ColorField(characterShadowColor, GUILayout.Width(100));
                if (EditorGUI.EndChangeCheck())
                {
                    shadowText.color = characterShadowColor;
                    shadowText.ForceMeshUpdate(true, true);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(GUIContent.none, GUIStyle.none, GUILayout.Width(240));
                showMeshShadow =
                    GUILayout.Toggle(showMeshShadow, new GUIContent("Show Mesh Shadow"), GUILayout.Width(135));
                meshShadowColor = EditorGUILayout.ColorField(meshShadowColor, GUILayout.Width(100));
                GUILayout.EndHorizontal();
            }
        }

        private void HandleWindows()
        {
            BeginWindows();
            characterWindowRect =
                GUI.Window(GUIUtility.GetControlID(FocusType.Passive), characterWindowRect, OnCharacterWindowGUI,
                    "Character");
            vertexWindowRect =
                GUI.Window(GUIUtility.GetControlID(FocusType.Passive), vertexWindowRect, OnVertexWindowGUI, "MyWindow");
            EndWindows();
        }

        private void OnGUI()
        {
            Debug.Log("Targetobj pos: " + targetObject.transform.position + " : Shadowobj pos: " + shadowObject.transform.position);

            UpdateAnimation();
            DrawPreview(new Rect(0, 0, position.width, position.height));

            if (GUILayout.Button("Reset character"))
            {
                targetText.transform.position = Vector3.zero;
                targetText.transform.localScale = Vector3.one;
                targetText.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                modifier = new TMPMeshModifiers();
                anim.Modifier = modifier;
                applier =
                    new SmthThatAppliesModifiers(); // To make sure changes show instantly and not when moving again
                anim.Applier = applier;
                UpdateAnimation();
            }

            HandleShadow();
            HandleWindows();
        }

        #region Hover Windows

        private void OnVertexWindowGUI(int id)
        {
            (var bl, var tl, var tr, var br) =
                applier.CalculateVertexPositions(
                    new CharData(0, targetText.textInfo.characterInfo[0], 0), modifier);

            modifier.BL_Delta += EditorGUILayout.Vector3Field("Position", bl) - bl;
            modifier.BL_Delta =
                EditorGUILayout.Vector3Field("Offset from original position", modifier.BL_Delta);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Color", GUILayout.Width(EditorGUIUtility.labelWidth));
            if (EditorGUILayout.Toggle(modifier.BL_Color.HasValue))
            {
                EditorGUILayout.EndHorizontal();
                if (!modifier.BL_Color.HasValue)
                    modifier.BL_Color = Color.white;

                Color c = modifier.BL_Color.Value;

                modifier.BL_Color = EditorGUILayout.ColorField("Color", c);
            }
            else
            {
                modifier.BL_Color = null;
                EditorGUILayout.EndHorizontal();
            }

            GUI.DragWindow();
        }

        private void OnCharacterWindowGUI(int id)
        {
            // modifier.PositionDelta += EditorGUILayout.Vector3Field("Position", modifier.PositionDelta) - modifier.PositionDelta;
            modifier.PositionDelta =
                EditorGUILayout.Vector3Field("Position", modifier.PositionDelta);

            // TODO inspector rotation broken
            Vector3 angles = modifier.RotationDelta.eulerAngles;
            angles =
                EditorGUILayout.Vector3Field("Rotation", angles);
            modifier.RotationDelta = Quaternion.Euler(angles);

            Vector3 scale = modifier.ScaleDelta.MultiplyPoint3x4(Vector3.one);
            scale =
                EditorGUILayout.Vector3Field("Scale", scale);
            modifier.ScaleDelta = Matrix4x4.Scale(scale);

            GUI.DragWindow();
        }

        #endregion

        private void UpdateAnimation()
        {
            animator.UpdateAnimations(lastUpdateTime == -1f ? 0f : Time.time - lastUpdateTime);
            lastUpdateTime = Time.time;
        }

        // todo provisory name
        private void PreviewHandles()
        {
            // Get vertex positions
            (var bl, var tl, var tr, var br) =
                applier.CalculateVertexPositions(
                    new CharData(0, targetText.textInfo.characterInfo[0], 0), modifier);

            // Draw shadow
            if (showMeshShadow)
            {
                CharData charData = new CharData(0, targetText.textInfo.characterInfo[0], 0);
                Handles.DrawAAPolyLine(
                    new Color[] { meshShadowColor, meshShadowColor, meshShadowColor, meshShadowColor, meshShadowColor },
                    new Vector3[]
                    {
                        charData.initialMesh.GetPosition(0), charData.initialMesh.GetPosition(1),
                        charData.initialMesh.GetPosition(2), charData.initialMesh.GetPosition(3),
                        charData.initialMesh.GetPosition(0),
                    }
                );

                Handles.color = meshShadowColor;
                Handles.SphereHandleCap(0, charData.initialMesh.GetPosition(0), Quaternion.identity, 0.04f,
                    EventType.Repaint);
                Handles.SphereHandleCap(0, charData.initialMesh.GetPosition(1), Quaternion.identity, 0.04f,
                    EventType.Repaint);
                Handles.SphereHandleCap(0, charData.initialMesh.GetPosition(2), Quaternion.identity, 0.04f,
                    EventType.Repaint);
                Handles.SphereHandleCap(0, charData.initialMesh.GetPosition(3), Quaternion.identity, 0.04f,
                    EventType.Repaint);
            }

            // Character tools handle
            switch (Tools.current)
            {
                case Tool.Move:
                    modifier.PositionDelta = Handles.PositionHandle(Handles.PositionHandleIds.@default,
                                                 targetText.transform.position + modifier.PositionDelta,
                                                 Quaternion.identity) -
                                             targetText.transform.position;
                    break;

                case Tool.Scale:
                    var vec = modifier.ScaleDelta.MultiplyPoint3x4(Vector3.one);
                    vec = Handles.ScaleHandle(vec, modifier.PositionDelta + targetText.transform.position,
                        Quaternion.identity);
                    modifier.ScaleDelta = Matrix4x4.Scale(vec);
                    break;

                case Tool.Rotate:
                    modifier.RotationDelta = Handles.RotationHandle(modifier.RotationDelta,
                        targetText.transform.position + modifier.PositionDelta);
                    break;
            }

            int blID = GUIUtility.GetControlID(FocusType.Passive);
            int tlID = GUIUtility.GetControlID(FocusType.Passive);
            int trID = GUIUtility.GetControlID(FocusType.Passive);
            int brID = GUIUtility.GetControlID(FocusType.Passive);

            // Vertex handles
            modifier.BL_Delta +=
                Handles.FreeMoveHandle(blID, bl, 0.04f, Vector3.zero,
                    Handles.SphereHandleCap) - bl;
            modifier.TL_Delta +=
                Handles.FreeMoveHandle(tlID, tl, 0.04f, Vector3.zero,
                    Handles.SphereHandleCap) - tl;
            modifier.TR_Delta +=
                Handles.FreeMoveHandle(trID, tr, 0.04f, Vector3.zero,
                    Handles.SphereHandleCap) - tr;
            modifier.BR_Delta +=
                Handles.FreeMoveHandle(brID, br, 0.04f, Vector3.zero,
                    Handles.SphereHandleCap) - br;

            // Background lines
            Handles.DrawAAPolyLine(
                new Color[] { Color.white, Color.white, Color.white, Color.white, Color.white, },
                new Vector3[] { bl, tl, tr, br, bl }
            );

        }

        public void DrawPreview(Rect previewArea)
        {
            previewUtility.BeginPreview(previewArea, previewBackground: GUIStyle.none);
            previewUtility.camera.Render();

            using (new Handles.DrawingScope(Matrix4x4.identity))
            {
                Handles.SetCamera(previewUtility.camera);
                PreviewHandles();
            }

            previewUtility.EndAndDrawPreview(previewArea);
        }

        [MenuItem("TMPEffects/Editor/KeyFrameEditorWindow")]
        public static void ShowWindow()
        {
            KeyFrameEditorWindow window = GetWindow<KeyFrameEditorWindow>();
            window.titleContent = new GUIContent("TMPEffects Keyframe Editor");
        }

        private class WrapAnimation : TMPSceneAnimation
        {
            public TMPMeshModifiers Modifier;
            public SmthThatAppliesModifiers Applier;

            public override void Animate(CharData cData, IAnimationContext context)
            {
                (var bl, var tl, var tr, var br) =
                    Applier.CalculateVertexPositions(cData, Modifier);

                cData.mesh.SetPosition(0, AnimationUtility.GetRawVertex(0, bl, cData, context));
                cData.mesh.SetPosition(1, AnimationUtility.GetRawVertex(1, tl, cData, context));
                cData.mesh.SetPosition(2, AnimationUtility.GetRawVertex(2, tr, cData, context));
                cData.mesh.SetPosition(3, AnimationUtility.GetRawVertex(3, br, cData, context));

                (var blC, var tlC, var trC, var brC) =
                    Applier.CalculateVertexColors(cData, Modifier);

                cData.mesh.SetColor(0, blC);
                cData.mesh.SetColor(1, tlC);
                cData.mesh.SetColor(2, trC);
                cData.mesh.SetColor(3, brC);
            }

            public override bool ValidateParameters(IDictionary<string, string> parameters)
            {
                return true;
            }

            public override object GetNewCustomData()
            {
                return null;
            }

            public override void SetParameters(object customData, IDictionary<string, string> parameters)
            {
            }
        }
    }
}
 *
 *
 */