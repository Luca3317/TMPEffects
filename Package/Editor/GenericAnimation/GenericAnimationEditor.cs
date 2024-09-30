using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPEffects.CharacterData;
using TMPEffects.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(GenericAnimation))]
public class GenericAnimationEditor : TMPAnimationEditorBase
{
    private static string exportPath = "Assets/Exported TMPEffects Animations";
    private static string exportName = "";

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Animation settings", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("repeat"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("duration"));
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("animationSteps"));

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Export", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        exportPath = EditorGUILayout.TextField("Export path", exportPath);
        exportName = EditorGUILayout.TextField("Export name", exportName);
        EditorGUI.indentLevel--;

        if (GUILayout.Button("Export"))
        {
            bool okay = EditorUtility.DisplayDialog("Exporting Generic Animation",
                "This will export this GenericAnimation animation as a .cs file, allowing you to make further edits.\nThe file will be saved as: " +
                exportPath + "/" +
                (string.IsNullOrWhiteSpace(exportName)
                    ? "GenericAnimation" + DateTime.Now.ToString("yyyyMMddHHmmss")
                    : exportName), "Okay", "Cancel");

            if (okay) GenericAnimationExporter.Export(serializedObject, exportPath , exportName);
        }

        if (serializedObject.hasModifiedProperties)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    public override void OnPreviewSettings()
    {
        base.OnPreviewSettings();
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


        List<GenericAnimation.AnimationStep> steps = new List<GenericAnimation.AnimationStep>();
        for (int i = 0; i < serializedObject.FindProperty("animationSteps").arraySize; i++)
        {
            var stepProp = serializedObject.FindProperty("animationSteps").GetArrayElementAtIndex(i);
            steps.Add(GetAnimationStep(stepProp));
        }

        GenerateScriptFromModifier(name, exportPath, steps);
    }

    static GenericAnimation.AnimationStep GetAnimationStep(SerializedProperty prop)
    {
        var animationStep = prop.managedReferenceValue as GenericAnimation.AnimationStep;
        if (animationStep == null) throw new System.InvalidOperationException("Couldnt cast animationsteps");
        return animationStep;
    }

    static bool GenerateScriptFromModifier(string className, string fileNamePath,
        List<GenericAnimation.AnimationStep> steps)
    {
        string code = string.Format(@"using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.TMPAnimations;
using static TMPEffects.Parameters.ParameterUtility;
using static TMPEffects.Parameters.ParameterParsing;
using static TMPEffects.Parameters.ParameterTypes;
using static TMPEffects.TMPAnimations.AnimationUtility;
using UnityEngine;

[AutoParameters]
[CreateAssetMenu(fileName=""new " + className + @""", menuName=""TMPEffects/Animations/Exported/" + className + @""")]
public partial class " + className + @" : TMPAnimation
{{
    [AutoParameter(""repeat"", ""rp""), SerializeField]
    private bool repeat;

    [AutoParameter(""duration"", ""dur""), SerializeField]
    private float duration;

    {0}

    private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
    {{
        {1}
    }}

    {2}
}}", GenerateStepParameters(steps), GenerateAnimateCode(steps), GenerateStepMethods(steps));
        return GenerateScriptFromContext(fileNamePath + "/" + className + ".cs", code);
    }

    private static string GenerateStepParameters(List<GenericAnimation.AnimationStep> steps)
    {
        string code = "";

        foreach (var step in steps)
        {
            code += $@"
    [SerializeField] private GenericAnimation.AnimationStep Step_{step.name};";
        }

        return code;
    }

    static string GenerateAnimateCode(List<GenericAnimation.AnimationStep> steps)
    {
        string code = @"
            TMPMeshModifiers result = new TMPMeshModifiers();
            TMPMeshModifiers tmp;
            float timeValue = data.repeat ? context.AnimatorContext.PassedTime % data.duration : context.AnimatorContext.PassedTime;";

        foreach (var step in steps)
        {
            code += $@"
            tmp = Animate_{step.name}(timeValue, Step_{step.name}, cData, data, context);
            if (tmp != null) result += tmp;
";
        }

        code += @"
            SmthThatAppliesModifiers applier = new SmthThatAppliesModifiers();
            applier.ApplyToCharData(cData, result);";

        return code;
    }

    static string GenerateStepMethods(List<GenericAnimation.AnimationStep> steps)
    {
        string code = "";

        foreach (var step in steps)
        {
            code += @$"
        private TMPMeshModifiers Animate_{step.name}(float timeValue, GenericAnimation.AnimationStep step, CharData cData, AutoParametersData data, IAnimationContext context)
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
        var folderPath = "Assets/Testing"; //context.overrideFolderPath ?? UnityCodeGenUtility.defaultFolderPath;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var hierarchy = fileNamePath.Split('/');
        var fileName = hierarchy[hierarchy.Length - 1];
        var path = folderPath;
        for (int i = 0; i < hierarchy.Length; i++)
        {
            path += "/" + hierarchy[i];
            if (i == hierarchy.Length - 1) break;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        if (File.Exists(path))
        {
            var text = File.ReadAllText(path);
            if (text == code)
            {
                // fileNames.Add(code.fileName);
                return false;
            }
        }

        File.WriteAllText(path, code);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        return true;
    }
}