using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPEffects.CharacterData;
using TMPEffects.Editor;
using TMPEffects.Extensions;
using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(GenericAnimation))]
public class GenericAnimationEditor : TMPAnimationEditorBase
{
    private const string ExportPathKey = "TMPEffects.EditorPrefKeys.GenericAnimationExportPath";
    private static string exportPath = "Assets/Exported TMPEffects Animations";
    private static string exportName = "";

    private GUIStyle fileBrowserButtonStyle;


    protected override void OnEnable()
    {
        base.OnEnable();
        exportPath = EditorPrefs.GetString(ExportPathKey, exportPath);
    }

    private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
    {
        var itemProp = serializedObject.FindProperty("animationSteps").GetArrayElementAtIndex(index);
        var nameProp = itemProp.FindPropertyRelative("name");
        var animateProp = itemProp.FindPropertyRelative("animate");

        var toggleRect = new Rect(rect.x + 10, rect.y, 15f, EditorGUIUtility.singleLineHeight);
        animateProp.boolValue = EditorGUI.ToggleLeft(toggleRect, GUIContent.none, animateProp.boolValue);
        toggleRect.x += 17.5f;
        toggleRect.width = rect.width - 17.5f;
        EditorGUI.LabelField(toggleRect,
            string.IsNullOrWhiteSpace(nameProp.stringValue) ? "Element " + index : nameProp.stringValue);
        EditorGUI.PropertyField(rect, itemProp, GUIContent.none);
    }

    private void AddCallback(ReorderableList list)
    {
        var animStepsProp = serializedObject.FindProperty("animationSteps");
        animStepsProp.arraySize++;
        var newElement = animStepsProp.GetArrayElementAtIndex(animStepsProp.arraySize - 1);
        newElement.managedReferenceValue = new GenericAnimation.AnimationStep();
    }

    // TODO Calling this all the properties of the arrayElement property are null? When adding a new element to list
    private void UpdateAnimationDuration()
    {
        var durationProp = serializedObject.FindProperty("duration");
        var animStepsProp = serializedObject.FindProperty("animationSteps");

        float reqLength = 0f;
        Debug.Log("animstepsporp array size " + animStepsProp.arraySize);
        for (int i = 0; i < animStepsProp.arraySize; i++)
        {
            var arrayElement = animStepsProp.GetArrayElementAtIndex(i);
            if (arrayElement == null) continue;
            if (arrayElement.FindPropertyRelative("duration") == null) Debug.Log("NO DURATION=!");
            if (arrayElement.FindPropertyRelative("startTime") == null) Debug.Log("NO startTime=!");
            reqLength += Mathf.Max(0f,
                arrayElement.FindPropertyRelative("duration").floatValue +
                arrayElement.FindPropertyRelative("startTime").floatValue);
        }

        durationProp.floatValue = Mathf.Max(durationProp.floatValue, reqLength);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Animation settings", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("repeat"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("duration"));
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        var animStepsProp = serializedObject.FindProperty("animationSteps");
        EditorGUILayout.PropertyField(animStepsProp);
        var list = ReorderableList.GetReorderableListFromSerializedProperty(animStepsProp);
        list.drawElementCallback = DrawElementCallback;
        list.onAddCallback = AddCallback;

        EditorGUILayout.Space();


        fileBrowserButtonStyle = new GUIStyle(GUI.skin.button);
        fileBrowserButtonStyle.normal.background = EditorGUIUtility.IconContent("d_Folder Icon").image as Texture2D;
        fileBrowserButtonStyle.fixedHeight = 30;
        fileBrowserButtonStyle.fixedWidth = 35;

        EditorGUILayout.LabelField("Export", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        var textrect = GUILayoutUtility.GetLastRect();
        textrect.y += textrect.height;
        textrect.height = EditorGUIUtility.singleLineHeight;
        GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth,
            EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, fileBrowserButtonStyle);


        var newExportPath = EditorGUI.TextField(textrect, "Export path", exportPath);
        if (newExportPath != exportPath)
        {
            Debug.Log("Setting!");
            EditorPrefs.SetString(ExportPathKey, newExportPath);
            exportPath = newExportPath;
        }

        textrect.x += EditorGUIUtility.labelWidth - 35;
        textrect.width = EditorGUIUtility.singleLineHeight * 1.5f;
        textrect.height = EditorGUIUtility.singleLineHeight * 1.5f;
        textrect.y -= 10;
        if (GUI.Button(textrect, "", fileBrowserButtonStyle))
        {
            string path = EditorUtility.OpenFolderPanel("Select folder to export to", "Assets", "");
            if (!string.IsNullOrWhiteSpace(path))
            {
                EditorPrefs.SetString(ExportPathKey, path);
                exportPath = path;
            }
        }

        exportName = EditorGUILayout.TextField("Export name", exportName);
        EditorGUI.indentLevel--;

        if (GUILayout.Button("Export"))
        {
            if (string.IsNullOrWhiteSpace(exportName))
            {
                EditorUtility.DisplayDialog("Empty export name", "You must specify a name for the exported file",
                    "Okay");
            }
            else
            {
                string exportNameUnderscored = Regex.Replace(exportName, @"\s+", "_");
                bool okay = EditorUtility.DisplayDialog("Exporting Generic Animation",
                    "This will export this GenericAnimation animation as a .cs file, allowing you to make further edits.\nThe file will be saved as: \n" +
                    exportPath + "/" + exportNameUnderscored + ".cs", "Okay", "Cancel");

                if (okay) GenericAnimationExporter.Export(serializedObject, exportPath, exportNameUnderscored);
            }
        }


        serializedObject.ApplyModifiedProperties();
    }


    // public override void OnInspectorGUI()
    // {
    //     EditorGUILayout.LabelField("Animation settings", EditorStyles.boldLabel);
    //     EditorGUI.indentLevel++;
    //     EditorGUILayout.PropertyField(serializedObject.FindProperty("repeat"));
    //     EditorGUILayout.PropertyField(serializedObject.FindProperty("duration"));
    //     EditorGUI.indentLevel--;
    //
    //     EditorGUILayout.Space();
    //
    //     var animStepsProp = serializedObject.FindProperty("animationSteps");
    //     EditorGUILayout.PropertyField(animStepsProp);
    //     var list = ReorderableList.GetReorderableListFromSerializedProperty(animStepsProp);
    //     list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
    //     {
    //         var itemProp = animStepsProp.GetArrayElementAtIndex(index);
    //
    //         var toggleRect = new Rect(rect.x, rect.y, 15f, EditorGUIUtility.singleLineHeight);
    //         EditorGUI.ToggleLeft(toggleRect, itemProp.FindPropertyRelative("name").stringValue, true);
    //         // if (itemProp.isExpanded)
    //         // {
    //         // }
    //         // else
    //         // {
    //         //     itemProp.isExpanded =
    //         //         EditorGUI.Foldout(rect, itemProp.isExpanded, itemProp.FindPropertyRelative("name").stringValue);
    //         // }
    //         EditorGUI.PropertyField(rect, itemProp, GUIContent.none);
    //     };
    //     // list.drawHeaderCallback = (Rect rect) =>
    //     // {
    //     //     animStepsProp.isExpanded = EditorGUI.Foldout(rect, animStepsProp.isExpanded, "Element hey");
    //     // };
    //
    //     EditorGUILayout.Space();
    //
    //     EditorGUILayout.LabelField("Export", EditorStyles.boldLabel);
    //     EditorGUI.indentLevel++;
    //     exportPath = EditorGUILayout.TextField("Export path", exportPath);
    //     exportName = EditorGUILayout.TextField("Export name", exportName);
    //     EditorGUI.indentLevel--;
    //
    //     if (GUILayout.Button("Export"))
    //     {
    //         bool okay = EditorUtility.DisplayDialog("Exporting Generic Animation",
    //             "This will export this GenericAnimation animation as a .cs file, allowing you to make further edits.\nThe file will be saved as: " +
    //             exportPath + "/" +
    //             (string.IsNullOrWhiteSpace(exportName)
    //                 ? "GenericAnimation" + DateTime.Now.ToString("yyyyMMddHHmmss")
    //                 : exportName), "Okay", "Cancel");
    //
    //         if (okay) GenericAnimationExporter.Export(serializedObject, exportPath, exportName);
    //     }
    //
    //     if (serializedObject.hasModifiedProperties)
    //     {
    //         serializedObject.ApplyModifiedProperties();
    //     }
    // }

    private int sliderControlID;
    private bool? wasPlaying = null;

    public override void OnPreviewSettings()
    {
        // Draw the slider / playbar
        // TODO its finicky; jumps around if your mouse isnt almost perfectly on it
        var duration = serializedObject.FindProperty("duration");
        float useValue = animator.AnimatorContext.PassedTime %
                         duration.floatValue;
        // EditorGUILayout.LabelField("" + animator.AnimatorContext.PassedTime.ToString("F2"), GUILayout.Width(40));

        EditorGUILayout.LabelField(animator.AnimatorContext.PassedTime.ToString("F2"), GUILayout.MinWidth(0f),
            GUILayout.MaxWidth(40));
        sliderControlID = GUIUtility.GetControlID(FocusType.Passive);
        useValue = EditorGUILayout.Slider(useValue, 0f, duration.floatValue, GUILayout.MinWidth(200));


        if (GUIUtility.hotControl == sliderControlID + 2)
        {
            if (!wasPlaying.HasValue) wasPlaying = animate;
            animate = false;
            float fullPlays = (int)(animator.AnimatorContext.PassedTime / duration.floatValue);
            animator.ResetTime(fullPlays + useValue);
            animator.UpdateAnimations(0f);
        }
        else if (wasPlaying.HasValue)
        {
            animate = wasPlaying.Value;
            wasPlaying = null;
        }

        // Copied from TMPAnimationEditorBase w minor changes
        GUIStyle animationButtonStyle = new GUIStyle(GUI.skin.button);
        animationButtonStyle.richText = true;
        char animationC = animate ? '\u2713' : '\u2717';
        GUIContent animationButtonContent = new GUIContent("Play " + (animate ? "<color=#90ee90>" : "<color=#f1807e>") +
                                                           animationC.ToString() + "</color>");

        if (GUILayout.Button(animationButtonContent, animationButtonStyle))
        {
            animate = !animate;
            // if (!animate) animator.ResetAnimations();
        }

        if (GUILayout.Button("Restart"))
        {
            animator.ResetTime();
            OnChange(anim);
        }
    }
}

public static class GenericAnimationExporter
{
    public static void Export(SerializedObject serializedObject, string exportPath, string name)
    {
        var anim = serializedObject.targetObject as GenericAnimation;
        if (anim == null)
            throw new System.InvalidOperationException(
                "The passed in serializedObject's target object is not a GenericAnimation");

        var steps = GetAnimationSteps(serializedObject.FindProperty("animationSteps"));
        var repeats = serializedObject.FindProperty("repeat").boolValue;
        var duration = serializedObject.FindProperty("duration").floatValue;
        GenerateScriptFromModifier(name, exportPath, repeats, duration, steps);
    }

    static List<GenericAnimation.AnimationStep> GetAnimationSteps(SerializedProperty animationStepsProp)
    {
        List<GenericAnimation.AnimationStep> steps = new List<GenericAnimation.AnimationStep>();
        for (int i = 0; i < animationStepsProp.arraySize; i++)
        {
            var stepProp = animationStepsProp.GetArrayElementAtIndex(i);
            steps.Add(GetAnimationStep(stepProp));
        }

        return steps;
    }

    static GenericAnimation.AnimationStep GetAnimationStep(SerializedProperty prop)
    {
        var animationStep = prop.managedReferenceValue as GenericAnimation.AnimationStep;
        if (animationStep == null) throw new System.InvalidOperationException("Couldnt cast animationsteps");
        return animationStep;
    }

    private class OrderedHashSet<T> : KeyedCollection<T, T>
    {
        protected override T GetKeyForItem(T item)
        {
            return item;
        }
    }

    static OrderedHashSet<string> GetAnimationStepNames(List<GenericAnimation.AnimationStep> steps)
    {
        OrderedHashSet<string> names = new OrderedHashSet<string>();

        foreach (var step in steps)
        {
            string nameToAdd;
            if (string.IsNullOrWhiteSpace(step.name))
            {
                nameToAdd = "Element_" + names.Count;
            }
            else nameToAdd = step.name;

            if (names.Contains(nameToAdd))
            {
                nameToAdd += "_";

                int counter = 0;
                while (names.Contains(nameToAdd + counter.ToString()))
                {
                    counter++;
                }

                nameToAdd = nameToAdd + counter.ToString();
            }

            nameToAdd = ReplaceWhitespaceWithUnderscore(nameToAdd);
            names.Add(nameToAdd);
        }

        return names;
    }

    static string ReplaceWhitespaceWithUnderscore(string s)
    {
        return Regex.Replace(s, @"\s+", "_");
    }

    static bool GenerateScriptFromModifier(string className, string fileNamePath, bool repeats, float duration,
        List<GenericAnimation.AnimationStep> steps)
    {
        OrderedHashSet<string> names = GetAnimationStepNames(steps);
        className = ReplaceWhitespaceWithUnderscore(className);

        string code = string.Format(@"using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.TMPAnimations;
using static TMPEffects.Parameters.ParameterUtility;
using static TMPEffects.Parameters.ParameterParsing;
using static TMPEffects.Parameters.ParameterTypes;
using static TMPEffects.TMPAnimations.AnimationUtility;
using UnityEngine;

namespace TMPEffects.TMPAnimations.GenericExports
{{
    // This class was generated off of a <see cref=""TMPEffects.TMPAnimations.GenericAnimation""/>
    [AutoParameters]
    [CreateAssetMenu(fileName=""new " + className + @""", menuName=""TMPEffects/Animations/Exported/" + className +
                                    @""")]
    public partial class " + className + @" : TMPAnimation
    {{
        [AutoParameter(""repeat"", ""rp""), SerializeField]
        private bool repeat = " + repeats.ToString().ToLower() + @";

        [AutoParameter(""duration"", ""dur""), SerializeField]
        private float duration = " + GetFloatString(duration) + @";

{0}
        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {{
{1}
        }}

{2}
    }}
}}", GenerateStepParameters(steps, names), GenerateAnimateCode(steps, names), GenerateStepMethods(steps, names));
        return GenerateScriptFromContext(fileNamePath + "/" + className + ".cs", code);
    }

    private static string GenerateStepParameters(List<GenericAnimation.AnimationStep> steps,
        OrderedHashSet<string> names)
    {
        string code = "";

        for (int i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            var name = names[i];

            code +=
                $@"
        [SerializeField] private GenericAnimation.AnimationStep Step_{name} = new GenericAnimation.AnimationStep()
        {{
            name = ""{step.name}"",
            entryDuration = {GetFloatString(step.entryDuration)},
            entryCurve = {GetAnimCurveString(step.entryCurve)},
            exitDuration = {GetFloatString(step.exitDuration)},
            exitCurve = {GetAnimCurveString(step.exitCurve)},
            loops = {step.loops.ToString().ToLower()},
            startTime = {GetFloatString(step.startTime)},
            duration = {GetFloatString(step.duration)},
            useWave = {step.useWave.ToString().ToLower()},
            wave = new AnimationUtility.Wave( 
                {GetAnimCurveString(step.wave.UpwardCurve)}, 
                {GetAnimCurveString(step.wave.DownwardCurve)}, 
                {GetFloatString(step.wave.UpPeriod)}, {GetFloatString(step.wave.DownPeriod)}, {GetFloatString(step.wave.Amplitude)},
                {GetFloatString(step.wave.CrestWait)}, {GetFloatString(step.wave.TroughWait)}, {GetFloatString(step.wave.Uniformity)}),
            modifiers = new TMPMeshModifiers()
            {{


            }}
        }};";

            // TODO Redo with chardatamodifier
            // PositionDelta = {GetVector3String(step.modifiers.PositionDelta)},
            // RotationDelta = {GetQuaternionString(step.modifiers.RotationDelta)}, 
            // BL_Delta = {GetVector3String(step.modifiers.BL_Delta)},
            // TL_Delta = {GetVector3String(step.modifiers.TL_Delta)},
            // TR_Delta = {GetVector3String(step.modifiers.TR_Delta)},
            // BR_Delta = {GetVector3String(step.modifiers.BR_Delta)},
            // {(step.modifiers.BL_Color.HasValue ? "BL_Color = new UnityNullable<Color32>(" + GetColorString(step.modifiers.BL_Color.Value) + ")," : "")}
            // {(step.modifiers.TL_Color.HasValue ? "TL_Color = new UnityNullable<Color32>(" + GetColorString(step.modifiers.TL_Color.Value) + ")," : "")}
            // {(step.modifiers.TR_Color.HasValue ? "TR_Color = new UnityNullable<Color32>(" + GetColorString(step.modifiers.TR_Color.Value) + ")," : "")}
            // {(step.modifiers.BR_Color.HasValue ? "BR_Color = new UnityNullable<Color32>(" + GetColorString(step.modifiers.BR_Color.Value) + ")," : "")}
            code = string.Join(Environment.NewLine, code
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .Where(line => !string.IsNullOrWhiteSpace(line)));

            code += "\n";
        }

        return code;
    }

    private static string GetAnimCurveString(AnimationCurve curve)
    {
        var str = "new AnimationCurve(";
        for (int i = 0; i < curve.keys.Length; i++)
        {
            var keyframe = curve.keys[i];
            str +=
                $" new Keyframe({GetFloatString(keyframe.time)}, {GetFloatString(keyframe.value)}, {GetFloatString(keyframe.inTangent)}, {GetFloatString(keyframe.outTangent)})";
            if (i == curve.keys.Length - 1) break;
            str += ",";
        }

        str += ")";
        return str;
    }

    private static string GetFloatString(float vcalue)
    {
        return vcalue.ToString("0.######", CultureInfo.InvariantCulture) + "f";
    }

    private static string GetVector3String(Vector3 vector)
    {
        return
            $"new Vector3({GetFloatString(vector.x)}, {GetFloatString(vector.y)}, {GetFloatString(vector.z)})";
    }

    private static string GetQuaternionString(Quaternion quat)
    {
        return
            $"new Quaternion({GetFloatString(quat.x)}, {GetFloatString(quat.y)}, {GetFloatString(quat.z)}, {GetFloatString(quat.w)})";
    }

    private static string GetColorString(Color32 color)
    {
        return
            $"new Color32({color.r}, {color.g}, {color.b}, {color.a})";
    }

    static string GenerateAnimateCode(List<GenericAnimation.AnimationStep> steps, OrderedHashSet<string> names)
    {
        string code = @"            TMPMeshModifiers result = new TMPMeshModifiers();
            TMPMeshModifiers tmp;
            float timeValue = data.repeat ? context.AnimatorContext.PassedTime % data.duration : context.AnimatorContext.PassedTime;";

        for (int i = 0; i < steps.Count; i++)
        {
            var name = names[i];
            code += $@"
            tmp = Animate_{name}(timeValue, Step_{name}, cData, data, context);
            if (tmp != null) result += tmp;
";
        }

        code += @"
            SmthThatAppliesModifiers applier = new SmthThatAppliesModifiers();
            applier.ApplyToCharData(cData, result);";

        return code;
    }

    static string GenerateStepMethods(List<GenericAnimation.AnimationStep> steps, OrderedHashSet<string> names)
    {
        string code = "";

        for (int i = 0; i < steps.Count; i++)
        {
            var name = names[i];
            code += @$"
        private TMPMeshModifiers Animate_{name}(float timeValue, GenericAnimation.AnimationStep step, CharData cData, AutoParametersData data, IAnimationContext context)
        {{
            if (step.startTime > timeValue) return null;
            if (step.EndTime < timeValue) return null;

            TMPMeshModifiers result;
            if (step.useWave)
            {{
                result =
                    TMPMeshModifiers.LerpUnclamped(cData, step.modifiers,
                        step.wave.Evaluate(timeValue,
                            AnimationUtility.GetWaveOffset(cData, context, step.waveOffsetType)).Value);
            }}
            else
            {{
                result = step.modifiers;
            }}
         
            float entry = timeValue - step.startTime;
            if (entry <= step.entryDuration)
            {{
                result = TMPMeshModifiers.LerpUnclamped(cData, result, step.entryCurve.Evaluate(entry / step.entryDuration));
            }}

            float exit = step.EndTime - timeValue;
            if (exit <= step.exitDuration)
            {{
                result = TMPMeshModifiers.LerpUnclamped(cData, result, step.exitCurve.Evaluate(exit / step.exitDuration));
            }}

            return result;
        }}
";
        }

        return code;
    }

    static bool GenerateScriptFromContext(string fileNamePath, string code)
    {
        var hierarchy = fileNamePath.Split('/');
        string filename = hierarchy[hierarchy.Length - 1];
        string dirPath = hierarchy[0];

        Debug.Log("Creating w " + fileNamePath);
        for (int i = 1; i < hierarchy.Length; i++)
        {
            Debug.Log("dirpatrh now " + dirPath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            dirPath += "/" + hierarchy[i];
        }

        if (File.Exists(dirPath))
        {
            var text = File.ReadAllText(dirPath);
            if (text == code)
            {
                // fileNames.Add(code.fileName);
                return false;
            }
        }

        Debug.Log("writing to " + dirPath);
        File.WriteAllText(dirPath, code);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        return true;
    }
}