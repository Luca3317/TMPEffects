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
using TMPEffects.Parameters;
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

        var toggleRect = new Rect(rect.x + 20, rect.y, 15f, EditorGUIUtility.singleLineHeight);
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
        newElement.managedReferenceValue = new AnimationStep();
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

    public static void Export(GenericAnimation anim, string filePath)
    {
        var steps = anim.AnimationSteps;
        var repeats = anim.Repeat;
        var duration = anim.Duration;
        GenerateScriptFromModifier(filePath, repeats, duration, steps);
    }

    static List<AnimationStep> GetAnimationSteps(SerializedProperty animationStepsProp)
    {
        List<AnimationStep> steps = new List<AnimationStep>();
        for (int i = 0; i < animationStepsProp.arraySize; i++)
        {
            var stepProp = animationStepsProp.GetArrayElementAtIndex(i);
            steps.Add(GetAnimationStep(stepProp));
        }

        return steps;
    }

    static AnimationStep GetAnimationStep(SerializedProperty prop)
    {
        var animationStep = prop.managedReferenceValue as AnimationStep;
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

    static OrderedHashSet<string> GetAnimationStepNames(List<AnimationStep> steps)
    {
        OrderedHashSet<string> names = new OrderedHashSet<string>();

        foreach (var step in steps)
        {
            string nameToAdd = step.name;

            // replace invlid characters
            Debug.Log("Turned " + nameToAdd + " into " + Regex.Replace(nameToAdd, @"[^a-zA-Z0-9_]", "") );
            nameToAdd = Regex.Replace(nameToAdd, @"[^a-zA-Z0-9_]", "");
            
            if (string.IsNullOrWhiteSpace(nameToAdd))
            {
                nameToAdd = "Element_" + names.Count;
            }

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

    static bool GenerateScriptFromModifier(string filePath, bool repeats, float duration,
        List<AnimationStep> steps)
    {
        var name = Path.GetFileNameWithoutExtension(filePath);
        var path = Path.GetDirectoryName(filePath);
        return GenerateScriptFromModifier(name, path, repeats, duration, steps);
    }
    
    static bool GenerateScriptFromModifier(string className, string fileNamePath, bool repeats, float duration,
        List<AnimationStep> steps)
    {
        OrderedHashSet<string> names = GetAnimationStepNames(steps);
        className = ReplaceWhitespaceWithUnderscore(className);

        string code = string.Format(@"using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.TMPAnimations;
using static TMPEffects.Parameters.ParameterUtility;
using static TMPEffects.Parameters.ParameterParsing;
using static TMPEffects.Parameters.ParameterTypes;
using static TMPEffects.TMPAnimations.AnimationUtility;
using UnityEngine;

namespace TMPEffects.TMPAnimations.GenericExports
{{
    // This class was generated off of a <see cref=""TMPEffects.TMPAnimations.GenericAnimation""/>.
    [AutoParameters]
    [CreateAssetMenu(fileName=""new " + className + @""", menuName=""TMPEffects/Animations/Exported/" + className +
                                    @""")]
    public partial class " + className + @" : TMPAnimation
    {{
        [AutoParameter(""repeat"", ""rp""), SerializeField]
        private bool repeat = " + repeats.ToString().ToLower() + @";

        [AutoParameter(""duration"", ""dur""), SerializeField]
        private float duration = " + GetFloatString(duration) + @";
        
        #region Generated Animation Step Fields
{0}
        #endregion
        
        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {{
{1}
        }}

        private CharDataModifiers storage, storage2;
        private CharDataModifiers current, accumulated;

        // Apply an AnimationStep, storing into the ""current"" CharDataModifier.
        private bool ApplyAnimationStep(AnimationStep step, float timeValue, CharData cData, IAnimationContext context)
        {{
            // Check if should apply
            if (step == null) return false;
            if (!step.animate) return false;
            if (step.startTime > timeValue) return false;
            if (step.EndTime < timeValue) return false;

            // Calculate weight, based on the currently relevant blend curve
            float weight = 1;
            float entry = timeValue - step.startTime;
            if (entry <= step.entryDuration)
            {{
                weight = step.entryCurve.Evaluate(entry / step.entryDuration);
            }}

            float exit = step.EndTime - timeValue;
            if (exit <= step.exitDuration)
            {{
                weight *= step.exitCurve.Evaluate(exit / step.exitDuration);
            }}

            // Apply the wave to the weight
            if (step.useWave)
            {{
                var offset = AnimationUtility.GetWaveOffset(cData, context, step.waveOffsetType);
                weight *= step.wave.Evaluate(timeValue, offset).Value;
            }}
            
            // Reset the current, since it'll be used to store the result
            current.Reset();
            
            // If you should use initial modifiers
            if (step.useInitialModifiers)
            {{
                // Reset storage
                storage.Reset();
                storage2.Reset();

                // Set modifiers
                step.initModifiers.ToCharDataModifiers(cData, context, storage);
                step.modifiers.ToCharDataModifiers(cData, context, storage2);

                // Lerp modifiers and store into current
                CharDataModifiers.LerpUnclamped(cData, storage, storage2, weight, current);
            }}
            // If you should lerp from the CharData itself
            else
            {{
                // Reset storage
                storage.Reset();

                // Set modifier
                step.modifiers.ToCharDataModifiers(cData, context, storage);

                // Lerp modifier and store into current
                CharDataModifiers.LerpUnclamped(cData, storage, weight, current);
            }}

            // Return true to indicate that ""current"" should be applied
            return true;
        }}
    }}
}}", GenerateStepParameters(steps, names), GenerateAnimateCode(steps, names)/*, GenerateStepMethods(steps, names)*/);
        return GenerateScriptFromContext(fileNamePath + "/" + className + ".cs", code);
    }

    private static string GenerateStepParameters(List<AnimationStep> steps,
        OrderedHashSet<string> names)
    {
        string code = "";

        for (int i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            var name = names[i];
            
            code +=
                $@"
        [SerializeField] private AnimationStep Step_{name} = new AnimationStep()
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
            modifiers = new EditorFriendlyCharDataModifiers()
            {{
                {(!step.modifiers.Position.Equals(new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero)) ? $"Position = {GetTypedVector3String(step.modifiers.Position)}," : "")}
                {(!step.modifiers.Scale.Equals(Vector3.one) ? $"Scale = {GetVector3String(step.modifiers.Scale)}," : "")}
                {(step.modifiers.Rotations.Count != 0 ? @$"Rotations = new List<EditorFriendlyRotation>()
                {{{GetRotationsString(step.modifiers.Rotations)}
                }}," : "")}
                {(!step.modifiers.BL_Position.Equals(new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero)) ? $"BL_Position = {GetTypedVector3String(step.modifiers.BL_Position)}," : "")}
                {(!step.modifiers.TL_Position.Equals(new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero)) ? $"TL_Position = {GetTypedVector3String(step.modifiers.TL_Position)}," : "")}
                {(!step.modifiers.TR_Position.Equals(new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero)) ? $"TR_Position = {GetTypedVector3String(step.modifiers.TR_Position)}," : "")}
                {(!step.modifiers.BR_Position.Equals(new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero)) ? $"BR_Position = {GetTypedVector3String(step.modifiers.BR_Position)}," : "")}
                {(step.modifiers.BL_Color.Override != 0 ? $"BL_Color = {GetColorOverrideString(step.modifiers.BL_Color)}," : "")}
                {(step.modifiers.TL_Color.Override != 0 ? $"TL_Color = {GetColorOverrideString(step.modifiers.TL_Color)}," : "")}
                {(step.modifiers.TR_Color.Override != 0 ? $"BL_Color = {GetColorOverrideString(step.modifiers.TR_Color)}," : "")}
                {(step.modifiers.BR_Color.Override != 0 ? $"BL_Color = {GetColorOverrideString(step.modifiers.BR_Color)}," : "")}
            }},
            {(step.useInitialModifiers ? @$"initModifiers = new EditorFriendlyCharDataModifiers()
            {{
                {(!step.initModifiers.Position.Equals(new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero)) ? $"Position = {GetTypedVector3String(step.initModifiers.Position)}," : "")}
                {(!step.initModifiers.Scale.Equals(Vector3.one) ? $"Scale = {GetVector3String(step.initModifiers.Scale)}," : "")}
                {(step.initModifiers.Rotations.Count != 0 ? @$"Rotations = new List<EditorFriendlyRotation>()
                {{{GetRotationsString(step.initModifiers.Rotations)}
                }}," : "")}
                {(!step.initModifiers.BL_Position.Equals(new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero)) ? $"BL_Position = {GetTypedVector3String(step.initModifiers.BL_Position)}," : "")}
                {(!step.initModifiers.TL_Position.Equals(new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero)) ? $"TL_Position = {GetTypedVector3String(step.initModifiers.TL_Position)}," : "")}
                {(!step.initModifiers.TR_Position.Equals(new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero)) ? $"TR_Position = {GetTypedVector3String(step.initModifiers.TR_Position)}," : "")}
                {(!step.initModifiers.BR_Position.Equals(new ParameterTypes.TypedVector3(ParameterTypes.VectorType.Offset, Vector3.zero)) ? $"BR_Position = {GetTypedVector3String(step.initModifiers.BR_Position)}," : "")}
                {(step.initModifiers.BL_Color.Override != 0 ? $"BL_Color = {GetColorOverrideString(step.initModifiers.BL_Color)}," : "")}
                {(step.initModifiers.TL_Color.Override != 0 ? $"TL_Color = {GetColorOverrideString(step.initModifiers.TL_Color)}," : "")}
                {(step.initModifiers.TR_Color.Override != 0 ? $"BL_Color = {GetColorOverrideString(step.initModifiers.TR_Color)}," : "")}
                {(step.initModifiers.BR_Color.Override != 0 ? $"BL_Color = {GetColorOverrideString(step.initModifiers.BR_Color)}," : "")}
            }}" : "" )}
        }};";
            
            code = string.Join(Environment.NewLine, code
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .Where(line => !string.IsNullOrWhiteSpace(line)));

            code += "\n";
        }

        return code;
    }

    private static string GetColorOverrideString(ColorOverride modifiersBLColor)
    {
        return $"new ColorOverride({GetColorString(modifiersBLColor.Color)}, {GetColorOverrideModeString(modifiersBLColor.Override)})";
    }

    private static string GetColorOverrideModeString(ColorOverride.OverrideMode overr)
    {
        if (overr.HasFlag(ColorOverride.OverrideMode.Color) && overr.HasFlag(ColorOverride.OverrideMode.Alpha))
        {
            return "ColorOverride.OverrideMode.Alpha | ColorOverride.OverrideMode.Color";
        }

        if (overr == 0) return "0";
        
        return "ColorOverride.OverrideMode." + overr;
    }

    private static string GetRotationsString(List<EditorFriendlyRotation> modifiersRotations)
    {
        string str = " ";
        foreach (var rot in modifiersRotations)
        {
            str += $"\nnew EditorFriendlyRotation({GetVector3String(rot.eulerAngles)}, {GetTypedVector3String(rot.pivot)}),";
        }

        return str.Substring(0, str.Length - 1);
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

    private static string GetTypedVector3String(ParameterTypes.TypedVector3 vector)
    {
        return $"new TypedVector3({GetVectorTypeString(vector.type)}, {GetVector3String(vector.vector)})";
    }

    private static string GetVectorTypeString(ParameterTypes.VectorType type)
    {
        return "VectorType." + type;
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

    static string GenerateAnimateCode(List<AnimationStep> steps, OrderedHashSet<string> names)
    {
        string code = @"            accumulated.Reset();
            float timeValue = data.repeat ? context.AnimatorContext.PassedTime % data.duration : context.AnimatorContext.PassedTime;
";

        for (int i = 0; i < steps.Count; i++)
        {
            var name = names[i];
            code += $@"
            if (ApplyAnimationStep(Step_{name}, timeValue, cData, context))
                accumulated.Combine(current);
";
        }

        code += $@"
            cData.CharacterModifierss.Combine(accumulated.CharacterModifiers);
            cData.MeshModifiers.Combine(accumulated.MeshModifiers);";

        return code;
    }

    static string GenerateStepMethods(List<AnimationStep> steps, OrderedHashSet<string> names)
    {
        string code = "";

        for (int i = 0; i < steps.Count; i++)
        {
            var name = names[i];
            code += @$"
        private CharDataModifiers Animate_{name}(float timeValue, AnimationStep step, CharData cData, AutoParametersData data, IAnimationContext context)
        {{
            if (step.startTime > timeValue) return null;
            if (step.EndTime < timeValue) return null;

            float weight = 1;
            float entry = timeValue - step.startTime;
            if (entry <= step.entryDuration)
            {{
                weight = step.entryCurve.Evaluate(entry / step.entryDuration);
            }}

            float exit = step.EndTime - timeValue;
            if (exit <= step.exitDuration)
            {{
                weight *= step.exitCurve.Evaluate(exit / step.exitDuration);
            }}

            CharDataModifiers result;
            CharDataModifiers modifiers = step.modifiers.ToCharDataModifiers(cData, context);
            if (step.useWave)
            {{
                var offset = GetWaveOffset(cData, context, step.waveOffsetType);
                result =
                    CharDataModifiers.LerpUnclamped(cData, modifiers,
                        step.wave.Evaluate(timeValue, offset).Value * weight);
            }}
            else
            {{
                 result = CharDataModifiers.LerpUnclamped(cData, modifiers, weight);
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