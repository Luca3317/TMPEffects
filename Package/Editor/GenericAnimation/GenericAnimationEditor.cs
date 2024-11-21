using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using TMPEffects.CharacterData;
using TMPEffects.Editor;
using TMPEffects.Extensions;
using TMPEffects.Parameters;
using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using AnimationUtility = TMPEffects.TMPAnimations.AnimationUtility;

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

        _ = new ReorderableList(serializedObject,
            serializedObject.FindProperty("Tracks").FindPropertyRelative("Tracks"), true, false, true, true);
        UpdateLists();

        var trackProp = serializedObject.FindProperty("Tracks").FindPropertyRelative("Tracks");
        for (int i = 0; i < trackProp.arraySize; i++)
        {
            var clips = trackProp.GetArrayElementAtIndex(i).FindPropertyRelative("clips");
            QuickSort(clips, 0, clips.arraySize - 1, new SortClipComparer());
        }
    }

    protected override void OnDisable()
    {
        var trackProp = serializedObject.FindProperty("Tracks").FindPropertyRelative("Tracks");
        for (int i = 0; i < trackProp.arraySize; i++)
        {
            var clips = trackProp.GetArrayElementAtIndex(i).FindPropertyRelative("clips");
            QuickSort(clips, 0, clips.arraySize - 1, new SortClipComparer());
        }
    }

    // Update all other durations when one was changed in the inspector
    // ChangedIndex: the index that was changed, 
    // ignoreIndex: further indices that should remain unchanged
    private void OnChangedStartOrDuration(int listIndex, int changedIndex, params int[] ignoreIndex)
    {
        var clips = serializedObject.FindProperty("Tracks").FindPropertyRelative("Tracks")
            .GetArrayElementAtIndex(listIndex).FindPropertyRelative("clips");
        var changedProp = clips.GetArrayElementAtIndex(changedIndex);

        // TODO This clamps the entry and exit to not be longer than the duration
        // In AnimationStepDrawer there is a method that clamps entry and exit when changed
        // Might be a little too spaghetti to have both of those
        changedProp.FindPropertyRelative("entryDuration").floatValue = Mathf.Min(
            changedProp.FindPropertyRelative("entryDuration").floatValue,
            changedProp.FindPropertyRelative("duration").floatValue);
        changedProp.FindPropertyRelative("exitDuration").floatValue = Mathf.Min(
            changedProp.FindPropertyRelative("exitDuration").floatValue,
            changedProp.FindPropertyRelative("duration").floatValue -
            changedProp.FindPropertyRelative("entryDuration").floatValue);

        float changedStartTime = changedProp.FindPropertyRelative("startTime").floatValue;
        float changedDuration = changedProp.FindPropertyRelative("duration").floatValue;
        float changedEndTime = changedStartTime + changedDuration;

        // Update all start times and durations to accomodate for changed time
        for (int i = 0; i < clips.arraySize; i++)
        {
            if (i == changedIndex || ignoreIndex.Contains(i)) continue;

            var step = clips.GetArrayElementAtIndex(i);

            float startTime = step.FindPropertyRelative("startTime").floatValue;
            if (startTime > changedEndTime) continue; // If starts after changed ends, cont

            float duration = step.FindPropertyRelative("duration").floatValue;
            float endTime = startTime + duration;
            if (endTime < changedStartTime) continue; // If ends before changed starts, cont
            // else, effected

            // if starts while changed already running
            if (startTime >= changedStartTime)
            {
                // if ends after changed, clamp starttime
                if (endTime >= changedEndTime)
                {
                    step.FindPropertyRelative("startTime").floatValue = changedEndTime;
                    OnChangedStartOrDuration(listIndex, i, ignoreIndex.Concat(new int[] { changedIndex }).ToArray());
                }
                // else meaning fully within the changed step
                else
                {
                    step.FindPropertyRelative("startTime").floatValue = changedStartTime;
                    step.FindPropertyRelative("duration").floatValue = 0f;
                    OnChangedStartOrDuration(listIndex, i, ignoreIndex.Concat(new int[] { changedIndex }).ToArray());
                }
            }

            // if changed starts while this is already running, clamp the duration
            else if (changedStartTime >= startTime)
            {
                step.FindPropertyRelative("duration").floatValue = changedStartTime - startTime;
                OnChangedStartOrDuration(listIndex, i, ignoreIndex.Concat(new int[] { changedIndex }).ToArray());
            }
        }
    }

    private void DrawElementCallback(int listindex, Rect rect, int index, bool isactive, bool isfocused)
    {
        var itemProp = serializedObject.FindProperty("Tracks").FindPropertyRelative("Tracks")
            .GetArrayElementAtIndex(listindex).FindPropertyRelative("clips").GetArrayElementAtIndex(index);

        var nameProp = itemProp.FindPropertyRelative("name");
        var animateProp = itemProp.FindPropertyRelative("animate");

        var toggleRect = new Rect(rect.x + 20, rect.y, 15f, EditorGUIUtility.singleLineHeight);
        animateProp.boolValue = EditorGUI.ToggleLeft(toggleRect, GUIContent.none, animateProp.boolValue);
        toggleRect.x += 17.5f;
        toggleRect.width = rect.width - 17.5f;
        EditorGUI.LabelField(toggleRect,
            string.IsNullOrWhiteSpace(nameProp.stringValue) ? "Element " + index : nameProp.stringValue);

        float fromWidth = 75;
        float gap = 10;
        var fromRect = new Rect(rect.x + rect.width - fromWidth * 2 - gap * 1.5f, rect.y, fromWidth,
            EditorGUIUtility.singleLineHeight);
        var toRect = new Rect(rect.x + rect.width - fromWidth - gap, rect.y, fromWidth,
            EditorGUIUtility.singleLineHeight);

        var startTimeProp = itemProp.FindPropertyRelative("startTime");
        var durationProp = itemProp.FindPropertyRelative("duration");

        EditorGUIUtility.labelWidth = 30;
        float newTime = Mathf.Max(0, EditorGUI.FloatField(fromRect, new GUIContent("from:"), startTimeProp.floatValue));
        EditorGUIUtility.labelWidth = 15;
        float newEndTime = Mathf.Max(newTime, EditorGUI.FloatField(toRect, new GUIContent("to:"),
            startTimeProp.floatValue + durationProp.floatValue));

        if (newEndTime < newTime) newEndTime = newTime;

        if (newTime != startTimeProp.floatValue)
        {
            startTimeProp.floatValue = newTime;
            OnChangedStartOrDuration(listindex, index);
        }
        else if (newEndTime != startTimeProp.floatValue + durationProp.floatValue)
        {
            durationProp.floatValue = newEndTime - startTimeProp.floatValue;
            OnChangedStartOrDuration(listindex, index);
        }

        EditorGUIUtility.labelWidth = 0;
        EditorGUI.PropertyField(rect, itemProp, GUIContent.none);
    }

    private float MainElementHeightCallback(int index)
    {
        var tracksprop = serializedObject.FindProperty("Tracks").FindPropertyRelative("Tracks");
        var clips = tracksprop.GetArrayElementAtIndex(index).FindPropertyRelative("clips");
        return EditorGUI.GetPropertyHeight(clips);
    }

    private void MainAddCallback(ReorderableList list)
    {
        var trackprop = serializedObject.FindProperty("Tracks").FindPropertyRelative("Tracks");
        trackprop.arraySize++;
        trackprop.GetArrayElementAtIndex(trackprop.arraySize - 1).FindPropertyRelative("clips").ClearArray();
    }

    private void MainDrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
    {
        EditorGUI.indentLevel++;
        var tracksprop = serializedObject.FindProperty("Tracks").FindPropertyRelative("Tracks");
        var clips = tracksprop.GetArrayElementAtIndex(index).FindPropertyRelative("clips");
        EditorGUI.PropertyField(rect, clips);
        EditorGUI.indentLevel--;
    }

    private void AddCallback(ReorderableList list)
    {
        var animStepsProp = serializedObject.FindProperty("animationSteps");
        animStepsProp.arraySize++;
        var newElement = animStepsProp.GetArrayElementAtIndex(animStepsProp.arraySize - 1);
        newElement.managedReferenceValue = new AnimationStep();
    }

    private struct SortClipComparer : IComparer<SerializedProperty>
    {
        public int Compare(SerializedProperty x, SerializedProperty y)
        {
            float
                xStart = x.FindPropertyRelative("startTime").floatValue,
                yStart = y.FindPropertyRelative("startTime").floatValue;

            if (xStart < yStart) return -1;
            if (yStart < xStart) return 1;

            float
                xDuration = x.FindPropertyRelative("duration").floatValue,
                yDuration = y.FindPropertyRelative("duration").floatValue;

            if (xDuration < yDuration) return -1;
            if (yDuration < xDuration) return 1;
            return 0;
        }
    }

    public static void QuickSort(SerializedProperty prop, int left, int right, IComparer<SerializedProperty> comparer)
    {
        int i = left, j = right;
        var pivot = prop.GetArrayElementAtIndex(left);

        while (i <= j)
        {
            while (comparer.Compare(prop.GetArrayElementAtIndex(i), pivot) == -1)
                i++;

            while (comparer.Compare(prop.GetArrayElementAtIndex(j), pivot) == 1)
                j--;

            // while (array[i] < pivot)
            //     i++;
            // while (array[j] > pivot)
            //     j--;

            if (i <= j)
            {
                // Swap
                var tmp = prop.GetArrayElementAtIndex(i).managedReferenceValue;
                prop.GetArrayElementAtIndex(i).managedReferenceValue =
                    prop.GetArrayElementAtIndex(j).managedReferenceValue;
                prop.GetArrayElementAtIndex(j).managedReferenceValue = tmp;

                i++;
                j--;
            }
        }

        // Recursive calls
        if (left < j)
            QuickSort(prop, left, j, comparer);
        if (i < right)
            QuickSort(prop, i, right, comparer);
    }

    private ReorderableList trackList;

    private void UpdateLists()
    {
        try
        {
            var trackprop = serializedObject.FindProperty("Tracks").FindPropertyRelative("Tracks");
            // EditorGUILayout.PropertyField(trackprop);
            trackList = ReorderableList.GetReorderableListFromSerializedProperty(trackprop);
            trackList.onAddCallback = MainAddCallback;
            trackList.drawElementCallback = MainDrawElementCallback;
            trackList.elementHeightCallback = MainElementHeightCallback;
            for (int i = 0; i < trackprop.arraySize; i++)
            {
                var list =
                    ReorderableList.GetReorderableListFromSerializedProperty(trackprop.GetArrayElementAtIndex(i)
                        .FindPropertyRelative("clips"));

                if (list == null)
                {
                    continue;
                }


                int listindex = i;
                list.drawElementCallback = (a, b, c, d) => DrawElementCallback(listindex, a, b, c, d);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
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

        if (GUILayout.Button("Sort Steps"))
        {
            var trackProp = serializedObject.FindProperty("Tracks").FindPropertyRelative("Tracks");
            for (int i = 0; i < trackProp.arraySize; i++)
            {
                var clips = trackProp.GetArrayElementAtIndex(i).FindPropertyRelative("clips");
                QuickSort(clips, 0, clips.arraySize - 1, new SortClipComparer());
            }
        }

        UpdateLists();
        EditorGUILayout.LabelField("Animation Steps", EditorStyles.boldLabel);
        trackList.DoLayoutList();


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
        Debug.LogWarning("HI");
        var anim = serializedObject.targetObject as GenericAnimation;
        if (anim == null)
            throw new System.InvalidOperationException(
                "The passed in serializedObject's target object is not a GenericAnimation");

        var tracks = GetTracks(serializedObject.FindProperty("Tracks").FindPropertyRelative("Tracks"));
        var repeats = serializedObject.FindProperty("repeat").boolValue;
        var duration = serializedObject.FindProperty("duration").floatValue;
        GenerateScriptFromModifier(name, exportPath, repeats, duration, tracks);
    }

    public static void Export(GenericAnimation anim, string filePath)
    {
        var repeats = anim.Repeat;
        var duration = anim.Duration;
        var tracks = anim.Tracks.Tracks.Select(track => track.Clips).ToList();
        GenerateScriptFromModifier2(filePath, repeats, duration, tracks);
    }

    static List<List<AnimationStep>> GetTracks(SerializedProperty property)
    {
        List<List<AnimationStep>> tracks = new List<List<AnimationStep>>();
        for (int i = 0; i < property.arraySize; i++)
        {
            var trackProp = property.GetArrayElementAtIndex(i).FindPropertyRelative("clips");
            var clips = GetAnimationSteps(trackProp);
            tracks.Add(clips);
        }

        return tracks;
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

    static Dictionary<int, List<string>> GetAnimationStepNames(List<List<AnimationStep>> tracks)
    {
        Dictionary<int, List<string>> names = new Dictionary<int, List<string>>();

        int trackCounter = 0;
        int stepCounter = 0;
        foreach (var track in tracks)
        {
            List<string> currnames = new List<string>();
            names[trackCounter] = currnames;

            foreach (var step in track)
            {
                string nameToAdd = step.name;

                // replace invlid characters
                nameToAdd = Regex.Replace(nameToAdd, @"[^a-zA-Z0-9_]", "");

                if (string.IsNullOrWhiteSpace(nameToAdd))
                {
                    nameToAdd = "Track_" + trackCounter + "_" + stepCounter;
                }
                else nameToAdd = "Track_" + trackCounter + "_" + nameToAdd;

                nameToAdd = ReplaceWhitespaceWithUnderscore(nameToAdd);

                if (currnames.Contains(nameToAdd))
                {
                    nameToAdd += "_";

                    int counter = 0;
                    while (currnames.Contains(nameToAdd + counter.ToString()))
                    {
                        counter++;
                    }

                    nameToAdd += counter.ToString();
                }

                currnames.Add(nameToAdd);
                stepCounter++;
            }

            trackCounter++;
        }

        return names;
    }

    static string ReplaceWhitespaceWithUnderscore(string s)
    {
        return Regex.Replace(s, @"\s+", "_");
    }

    static bool GenerateScriptFromModifier2(string filePath, bool repeats, float duration,
        List<List<AnimationStep>> steps)
    {
        var name = Path.GetFileNameWithoutExtension(filePath);
        var path = Path.GetDirectoryName(filePath);
        return GenerateScriptFromModifier(name, path, repeats, duration, steps);
    }

    static bool GenerateScriptFromModifier(string className, string fileNamePath, bool repeats, float duration,
        List<List<AnimationStep>> steps)
    {
        var names = GetAnimationStepNames(steps);
        className = ReplaceWhitespaceWithUnderscore(className);

        string code = string.Format(@"using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
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

        // The Animate method automatically called
        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {{
            CreateStepsSorted(ref data.Steps);

            IAnimatorContext ac = context.AnimatorContext;
            var steps = data.Steps;
            
            // Create modifiers if needed
            data.modifiersStorage ??= new CharDataModifiers();
            data.modifiersStorage2 ??= new CharDataModifiers();
            data.accModifier ??= new CharDataModifiers();
            data.current ??= new CharDataModifiers();

            // Calculate timeValue
            float timeValue =
                data.repeat ? ac.PassedTime % data.duration : ac.PassedTime;

            // Reset accModifier
            data.accModifier.Reset();

            // For every track
            foreach (var track in steps)
            {{
                // Find the currently active clip (max 1)
                var active = FindCurrentlyActive(timeValue, track);
                if (active == -1) continue;
                
                // If that clip is disabled continue
                var step = track[active];
                if (!step.animate) continue;

                // Adjust the timeValue for extrapolation setting
                float t = AdjustTimeForExtrapolation(step, timeValue, cData, ac);
                t -= - step.startTime;
                
                // Calculate the weight of the current clip
                float weight = CalcWeight(step, t, step.duration, cData, ac);

                // Lerp the animation step using the weight
                LerpAnimationStepWeighted(step, weight, cData, ac,
                    data.modifiersStorage, data.modifiersStorage2, data.current);

                // Combine the lerped modifiers with the previously calculated ones
                data.accModifier.MeshModifiers.Combine(data.current.MeshModifiers);
                data.accModifier.CharacterModifiers.Combine(data.current.CharacterModifiers);
            }}

            // Combine the calculated modifiers into the CharData's one
            cData.MeshModifiers.Combine(data.accModifier.MeshModifiers);
            cData.CharacterModifiers.Combine(data.accModifier.CharacterModifiers);
        }}

        // Ensure steps are sorted
        private void CreateStepsSorted(ref List<List<AnimationStep>> steps)
        {{
            if (steps != null) return;

            steps = CreateStepList();
            
            for (int i = 0; i < steps.Count; i++)
            {{
                steps[i].Sort(new StepComparer());
            }}

            List<List<AnimationStep>> CreateStepList()
            {{
                {1}
            }}
        }}
        
        // Get the currently active step
        private int FindCurrentlyActive(float timeValue, List<AnimationStep> steps)
        {{
            if (steps == null || steps.Count == 0) return -1;

            // TODO This asssumes each track is sorted
            for (int i = 0; i < steps.Count; i++)
            {{
                var step = steps[i];

                if (step == null) continue;

                // If on step
                if (step.startTime < timeValue && step.EndTime > timeValue)
                    return i;

                // If before step
                if (step.startTime > timeValue)
                {{
                    if (i == 0)
                    {{
                        if (step.preExtrapolation != AnimationStep.ExtrapolationMode.None) return i;
                        return -1;
                    }}

                    if (steps[i - 1].postExtrapolation != AnimationStep.ExtrapolationMode.None)
                        return i - 1;

                    if (step.preExtrapolation != AnimationStep.ExtrapolationMode.None)
                        return i;

                    return -1;
                }}
            }}

            return steps.Count - 1;
        }}
        
        // Adjust the raw time value according to the extrapolation
        public static float AdjustTimeForExtrapolation(AnimationStep step, float timeValue, CharData cData,
            IAnimatorContext context)
        {{
            if (timeValue < step.startTime)
            {{
                switch (step.preExtrapolation)
                {{
                    case AnimationStep.ExtrapolationMode.Continue:
                        break; // Continue only different in how the value should be handled
                    case AnimationStep.ExtrapolationMode.Hold:
                        timeValue = step.startTime;
                        break;
                    case AnimationStep.ExtrapolationMode.Loop:
                        float diff = (step.startTime - timeValue) % step.duration;
                        timeValue = step.EndTime - diff;
                        break;
                    case AnimationStep.ExtrapolationMode.PingPong:
                        diff = (step.startTime - timeValue) % step.duration;
                        timeValue = step.startTime + diff;
                        break;
                }}
            }}

            if (timeValue > step.EndTime)
            {{
                switch (step.postExtrapolation)
                {{
                    case AnimationStep.ExtrapolationMode.Continue:
                        break; // Continue only different in how the value should be handled
                    case AnimationStep.ExtrapolationMode.Hold:
                        timeValue = step.EndTime;
                        break;
                    case AnimationStep.ExtrapolationMode.Loop:
                        float diff = (timeValue - step.EndTime) % step.duration;
                        timeValue = step.startTime + diff;
                        break;
                    case AnimationStep.ExtrapolationMode.PingPong:
                        diff = (timeValue - step.EndTime) % step.duration;
                        timeValue = step.EndTime - diff;
                        break;
                }}
            }}

            return timeValue;
        }}

        // Calculate the weight of the step using the time value
        public static float CalcWeight(AnimationStep step, float timeValue, float duration,
            CharData cData, IAnimatorContext context)
        {{
            float weight = 1;
            if (step.entryDuration > 0 /*&& currTime <= behaviour.Step.Step.entryDuration*/)
            {{
                weight = step.entryCurve.Evaluate(cData, context,
                    timeValue / step.entryDuration);
            }}

            if (step.exitDuration > 0 /*&& currTime >= duration - behaviour.Step.Step.exitDuration*/)
            {{
                float preTime = duration - step.exitDuration;
                var multiplier = (step.exitCurve.Evaluate(cData, context,
                    1f - (timeValue - preTime) /
                    step.exitDuration));

                weight *= multiplier;
            }}

            if (step.useWave)
            {{
                var offset = AnimationUtility.GetOffset(cData, context,
                    step.offsetType);
                weight *= step.wave.Evaluate(timeValue, offset).Value;
            }}

            return weight;
        }}

        // Lerp the step using the weight
        public static void LerpAnimationStepWeighted(AnimationStep step, float weight, CharData cData,
            IAnimatorContext context,
            CharDataModifiers storage, CharDataModifiers storage2, CharDataModifiers result)
        {{
            result.Reset();

            if (step.useInitialModifiers)
            {{
                // Reset storage
                storage.Reset();
                storage2.Reset();

                // Set modifiers
                step.initModifiers.ToCharDataModifiers(cData, context, storage);
                step.modifiers.ToCharDataModifiers(cData, context, storage2);

                // Lerp modifiers and store into current
                CharDataModifiers.LerpUnclamped(cData, context, storage, storage2, weight, result);
            }}
            else
            {{
                // Reset storage
                storage.Reset();

                // Set modifier
                step.modifiers.ToCharDataModifiers(cData, context, storage);

                // Lerp modifier and store into current
                CharDataModifiers.LerpUnclamped(cData, storage, weight, result);
            }}
        }}

        [AutoParametersStorage]
        private partial class AutoParametersData
        {{
            public List<List<AnimationStep>> Steps = null;

            public CharDataModifiers modifiersStorage;
            public CharDataModifiers modifiersStorage2;
            public CharDataModifiers accModifier;
            public CharDataModifiers current;
        }}

        private struct StepComparer : IComparer<AnimationStep>
        {{
            public int Compare(AnimationStep x, AnimationStep y)
            {{
                if (x == null || y == null) return 0;
                if (x.startTime < y.startTime) return -1;
                if (x.startTime > y.startTime) return 1;
                if (x.EndTime < y.EndTime) return -1;
                if (x.EndTime > y.EndTime) return 1;
                return 0;
            }}
        }}

    }}
}}", GenerateStepParameters(steps, names), GenerateCreateStepListCode(steps, names));
        return GenerateScriptFromContext(fileNamePath + "/" + className + ".cs", code);
    }

    private static string GenerateCreateStepListCode(List<List<AnimationStep>> steps,
        Dictionary<int, List<string>> names)
    {
        string code = @"
                return new List<List<AnimationStep>>
                {";

        List<List<string>> nameslist = names
            .OrderBy(pair => pair.Key)
            .Select(pair => pair.Value)
            .ToList();

        for (int i = 0; i < nameslist.Count; i++)
        {
            code += @"new List<AnimationStep>()
                    {";
            foreach (var name in nameslist[i])
            {
                code += @$"
                        Step_{name},";
            }

            code += @"
                    },";
        }

        code += @"
                };";

        return code;
    }

    private static string GenerateStepParameters(List<List<AnimationStep>> tracks,
        Dictionary<int, List<string>> names)
    {
        string code = "";
        int trackIndex = -1;
        foreach (var steps in tracks)
        {
            trackIndex++;

            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                var name = names[trackIndex][i];

                code +=
                    $@"
        [SerializeField] private AnimationStep Step_{name} = new AnimationStep()
        {{
            name = ""{step.name}"",
            entryDuration = {GetFloatString(step.entryDuration)},
            entryCurve = {GetFancyAnimCurveString(step.entryCurve)},
            exitDuration = {GetFloatString(step.exitDuration)},
            exitCurve = {GetFancyAnimCurveString(step.exitCurve)},
            loops = {step.loops.ToString().ToLower()},
            startTime = {GetFloatString(step.startTime)},
            duration = {GetFloatString(step.duration)},
            useWave = {step.useWave.ToString().ToLower()},
            wave = new AnimationUtility.Wave( 
                {GetAnimCurveString(step.wave.UpwardCurve)}, 
                {GetAnimCurveString(step.wave.DownwardCurve)}, 
                {GetFloatString(step.wave.UpPeriod)}, {GetFloatString(step.wave.DownPeriod)}, {GetFloatString(step.wave.Amplitude)},
                {GetFloatString(step.wave.CrestWait)}, {GetFloatString(step.wave.TroughWait)}),
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
                {(step.modifiers.TR_Color.Override != 0 ? $"TR_Color = {GetColorOverrideString(step.modifiers.TR_Color)}," : "")}
                {(step.modifiers.BR_Color.Override != 0 ? $"BR_Color = {GetColorOverrideString(step.modifiers.BR_Color)}," : "")}
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
                {(step.initModifiers.TR_Color.Override != 0 ? $"TR_Color = {GetColorOverrideString(step.initModifiers.TR_Color)}," : "")}
                {(step.initModifiers.BR_Color.Override != 0 ? $"BR_Color = {GetColorOverrideString(step.initModifiers.BR_Color)}," : "")}
            }}" : "")}
        }};";

                code = string.Join(Environment.NewLine, code
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                    .Where(line => !string.IsNullOrWhiteSpace(line)));

                code += "\n";
            }
        }

        return code;
    }

    private static string GetColorOverrideString(ColorOverride modifiersBLColor)
    {
        return
            $"new ColorOverride({GetColorString(modifiersBLColor.Color)}, {GetColorOverrideModeString(modifiersBLColor.Override)})";
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
            str +=
                $"\nnew EditorFriendlyRotation({GetVector3String(rot.eulerAngles)}, {GetTypedVector3String(rot.pivot)}),";
        }

        return str.Substring(0, str.Length - 1);
    }

    private static string GetFancyAnimCurveString(AnimationUtility.FancyAnimationCurve curve)
    {
        // TODO TMPWrapMode and WaveOffsetType (for that second one wait to see if i do poweroffsettype)
        var str = $@"new FancyAnimationCurve()
{{
    Curve = {GetAnimCurveString(curve.Curve)}
}}
";
        return str;
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

    static string GenerateAnimateCode(List<List<AnimationStep>> tracks, Dictionary<int, List<string>> names)
    {
        string code = @"            accumulated.Reset();
            float timeValue = data.repeat ? context.AnimatorContext.PassedTime % data.duration : context.AnimatorContext.PassedTime;
";

        int trackIndex = -1;
        foreach (var steps in tracks)
        {
            trackIndex++;

            for (int i = 0; i < steps.Count; i++)
            {
                var name = names[trackIndex][i];
                code += $@"
            if (ApplyAnimationStep(Step_{name}, timeValue, cData, context))
                accumulated.Combine(current);
";
            }
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