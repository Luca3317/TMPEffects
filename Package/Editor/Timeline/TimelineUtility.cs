using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Timeline;
using ScriptableWizard = UnityEditor.ScriptableWizard;

internal static class TimelineUtility
{
    #region ExportAsGeneric
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
        anim.Tracks.Tracks.Clear();

        var tracks = context.timeline.GetOutputTracks().OfType<TMPMeshModifierTrack>().ToList();

        foreach (var track in tracks)
        {
            var clips = track.GetClips();

            var animTrack = new GenericAnimation.Track();
            anim.Tracks.Tracks.Add(animTrack);
            foreach (var clip in clips)
            {
                TMPMeshModifierClip mClip = clip.asset as TMPMeshModifierClip;
                if (mClip == null) continue;
            
                var copy = UnityEngine.Object.Instantiate(mClip);
                var step = copy.Step.Step;

                // Copy over clip properties
                step.name = clip.displayName;
                step.duration = (float)clip.duration;
                step.startTime = (float)clip.start;
                step.preExtrapolation = clip.preExtrapolationMode.ConvertExtrapolation();
                step.postExtrapolation = clip.postExtrapolationMode.ConvertExtrapolation();
                
                animTrack.Clips.Add(step);
            
                // TODO What was this about again?
                float endTime = (float)clip.end;
                if (endTime > anim.Duration)
                {
                    anim.Duration = endTime;
                }
            }
        }

        anim.Repeat = true;

        return anim;
    }

    private static GenericAnimation CreateGenericAnimationFromSelectedClips(ActionContext context)
    {
        var anim = ScriptableObject.CreateInstance<GenericAnimation>();
        anim.Tracks.Tracks.Clear();

        var tracks = context.tracks.OfType<TMPMeshModifierTrack>().ToList();

        foreach (var track in tracks)
        {
            var clips = track.GetClips();

            var animTrack = new GenericAnimation.Track();
            anim.Tracks.Tracks.Add(animTrack);
            foreach (var clip in clips)
            {
                TMPMeshModifierClip mClip = clip.asset as TMPMeshModifierClip;
                if (mClip == null) continue;
            
                var copy = UnityEngine.Object.Instantiate(mClip);
                var step = copy.Step.Step;

                // Copy over clip properties
                step.name = clip.displayName;
                step.duration = (float)clip.duration;
                step.startTime = (float)clip.start;
                step.preExtrapolation = clip.preExtrapolationMode.ConvertExtrapolation();
                step.postExtrapolation = clip.postExtrapolationMode.ConvertExtrapolation();
                
                animTrack.Clips.Add(step);
            
                // TODO What was this about again?
                float endTime = (float)clip.end;
                if (endTime > anim.Duration)
                {
                    anim.Duration = endTime;
                }
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
    #endregion

    #region Directory stuff
    static string MakeRelative(string filePath, string referencePath)
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
    #endregion

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

    #region ExportAsScript
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
    #endregion
    
    #region UnpackGeneric
    public static bool UnpackGenericDialog(ActionContext context)
    {
        bool result = EditorUtility.DisplayDialog("Unpack generic animation",
            @"You are unpacking a GenericAnimation, which will convert the GenericAnimation clip into equivalent, editable TMPMeshModifier clips.
The GenericAnimation asset itself will remain unchanged.
Proceed?", "Ok", "Cancel");

        return result;
    }

    public static bool UnpackGeneric(ActionContext context)
    {
        var clips = context.clips.Where(clip => clip.asset is TMPAnimationClip);
        HashSet<TrackAsset> tracks = new HashSet<TrackAsset>();

        foreach (var clip in clips)
        {
            var track = clip.GetParentTrack();
            tracks.Add(track);

            var tmpAnimationClip = clip.asset as TMPAnimationClip;
            var genericAnim = tmpAnimationClip.animation;
            var mainParentTrack = context.timeline.CreateTrack<GroupTrack>(genericAnim.name);

            int trackCounter = 0;
            foreach (var animTrack in genericAnim.Tracks.Tracks) 
            {
                var parentTrack = context.timeline.CreateTrack<TMPMeshModifierTrack>(mainParentTrack, genericAnim.name + "_Track" + trackCounter++);

                foreach (var animClip in animTrack.Clips)
                {
                    var timelineClip = parentTrack.CreateClip<TMPMeshModifierClip>();
                    var modClip = timelineClip.asset as TMPMeshModifierClip;
                    modClip.Step.Step = new AnimationStep(animClip); 
                    
                    timelineClip.displayName = animClip.name;
                    
                    timelineClip.start = animClip.startTime;
                    timelineClip.duration = animClip.duration;
                    
                    // TODO Is there really no way to set the mode on creation...
                    var reflected = typeof(TimelineClip).GetField("m_PostExtrapolationMode", BindingFlags.NonPublic | BindingFlags.Instance);
                    reflected.SetValue(timelineClip, animClip.postExtrapolation.ConvertExtrapolation());
                    
                    reflected = typeof(TimelineClip).GetField("m_PreExtrapolationMode", BindingFlags.NonPublic | BindingFlags.Instance);
                    reflected.SetValue(timelineClip, animClip.preExtrapolation.ConvertExtrapolation());
                }
            }
            
            EditorUtility.SetDirty(context.timeline);
            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
        }

        // foreach (var track in tracks)
        // {
        //     context.timeline.DeleteTrack(track);
        // }

        return true;
    }
    #endregion
}