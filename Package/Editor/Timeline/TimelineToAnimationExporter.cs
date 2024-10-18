using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using ScriptableWizard = UnityEditor.ScriptableWizard;

internal static class TimelineToAnimationExporter
{
    public static int ExportAsGenericDialog(ActionContext context)
    {
        GenericAnimation animation = ScriptableObject.CreateInstance<GenericAnimation>();

        int result = EditorUtility.DisplayDialogComplex("Export to Generic Animation",
            @"You are exporting to a generic animation asset.
Which clips do you want to use for the export?", "Selected Only", "Cancel", "All Clips");

        // ExportToGenericAnimationUtilityWindow.Init(Application.dataPath);

        return result;
    }

    private static GenericAnimation CreateGenericFromAllClips(ActionContext context)
    {
        var anim = ScriptableObject.CreateInstance<GenericAnimation>();
        anim.AnimationSteps.Clear();
        var clips = context.timeline.GetOutputTracks().OfType<TMPMeshModifierTrack>()
            .SelectMany(track => track.GetClips());
        
        foreach (var clip in clips)
        {
            TMPMeshModifierClip mClip = clip.asset as TMPMeshModifierClip;
            if (mClip == null) continue;

            var copy = UnityEngine.Object.Instantiate(mClip);
            copy.Step.Step.name = copy.name + "_" + copy.Step.Step.name;
            anim.AnimationSteps.Add(copy.Step.Step);

            float endTime = (float)clip.end;
            if (endTime > anim.Duration)
            {
                anim.Duration = endTime;
            }
        }

        anim.Repeat = true;

        return anim;
    }

    private static GenericAnimation CreateGenericAnimationFromSelectedClips(ActionContext context)
    {
        var anim = ScriptableObject.CreateInstance<GenericAnimation>();
        anim.AnimationSteps.Clear();
        var clips = context.tracks.OfType<TMPMeshModifierTrack>().SelectMany(track => track.GetClips());

        foreach (var clip in clips)
        {
            TMPMeshModifierClip mClip = clip.asset as TMPMeshModifierClip;
            if (mClip == null) continue;

            var copy = UnityEngine.Object.Instantiate(mClip);
            copy.name = mClip.name;
            copy.Step.Step.name = copy.name +  (string.IsNullOrWhiteSpace(copy.Step.Step.name) ? "" : "_" + copy.Step.Step.name);

            copy.Step.Step.duration = (float)clip.duration;
            copy.Step.Step.startTime = (float)clip.start;
            anim.AnimationSteps.Add(copy.Step.Step);
            
            
            
            float endTime = (float)clip.end;
            if (endTime > anim.Duration)
            {
                anim.Duration = endTime;
            }
        }

        anim.Repeat = true;
        
        return anim;
    }
    
    public static bool ExportAsGeneric(ActionContext context, int option)
    {
        if (option == 1) throw new SystemException();

        // Selected only
        if (option == 0)
        {
            var anim = CreateGenericAnimationFromSelectedClips(context);
            GenerateScriptableFromContext(Application.dataPath + "/GenericExpors.asset", anim);
            return true;
        }

        // All clips
        if (option == 2)
        {
            var anim = CreateGenericFromAllClips(context);
            GenerateScriptableFromContext(Application.dataPath + "/GenericExpors.asset", anim);
            return true;
        }

        return false;
    }

    public static string MakeRelative(string filePath, string referencePath)
    {
        var fileUri = new Uri(filePath);
        var referenceUri = new Uri(referencePath);
        return Uri.UnescapeDataString(referenceUri.MakeRelativeUri(fileUri).ToString())
            .Replace('/', Path.DirectorySeparatorChar);
    }

    static void EnsureDirectoryExists(string directoryPath)
    {
        var hierarchy = directoryPath.Split('/');
        string dirPath = hierarchy[0];

        // Ensure directory exists
        for (int i = 1; i < hierarchy.Length; i++)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            dirPath += "/" + hierarchy[i];
        }
    }

    static void GenerateScriptableFromContext(string filePath, GenericAnimation anim)
    {
        EnsureDirectoryExists(filePath);

        string relativePath = MakeRelative(filePath, Application.dataPath);
        Debug.Log(relativePath);
        AssetDatabase.CreateAsset(anim, relativePath);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = anim;
    }

    static bool GenerateScriptFromContext(string fileNamePath, string code)
    {
        var hierarchy = fileNamePath.Split('/');
        string filename = hierarchy[hierarchy.Length - 1];
        string dirPath = hierarchy[0];

        for (int i = 1; i < hierarchy.Length; i++)
        {
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

        File.WriteAllText(dirPath, code);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        return true;
    }

    public static int ExportAsScriptDialog(ActionContext context)
    {
        int result = EditorUtility.DisplayDialogComplex("Export to TMPAnimation Script",
            @"You are exporting to an animation script. 
Which clips do you want to use for the export?", "Selected Only", "Cancel", "All Clips");

        return result;
    }

    public static bool ExportAsScript(ActionContext context, int option)
    {       
        if (option == 1) throw new SystemException();

        // Selected only
        if (option == 0)
        {
            var anim = CreateGenericAnimationFromSelectedClips(context);
            GenericAnimationExporter.Export(anim, Application.dataPath + "/GenericExpors.cs");
            return true;
        }

        // All clips
        if (option == 2)
        {
            var anim = CreateGenericFromAllClips(context);
            GenericAnimationExporter.Export(anim, Application.dataPath + "/GenericExpors.cs");
            return true;
        }

        return false;
    }
}