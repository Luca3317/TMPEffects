using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPEffects.Timeline;
using TMPEffects.TMPAnimations;
using TMPEffects.TMPAnimations.HideAnimations;
using TMPEffects.TMPAnimations.ShowAnimations;
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

        return result;
    }

    private static GenericAnimation CreateGenericAnimation(ActionContext context, List<TMPMeshModifierTrack> tracks)
    {
        var anim = ScriptableObject.CreateInstance<GenericAnimation>();
        anim.Tracks.Tracks.Clear();

        float duration = anim.Duration;
        anim.Tracks = CreateTrackList(tracks, ref duration);
        anim.Repeat = true;
        anim.Duration = duration;

        return anim;
    }

    private static GenericAnimationUtility.TrackList CreateTrackList(List<TMPMeshModifierTrack> tracks, ref float duration)
    {
        GenericAnimationUtility.TrackList result = new GenericAnimationUtility.TrackList();
        
        foreach (var track in tracks)
        {
            var clips = track.GetClips();

            var animTrack = new GenericAnimationUtility.Track();
            // anim.Tracks.Tracks.Add(animTrack);
            result.Tracks.Add(animTrack);
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
            
                float endTime = (float)clip.end;
                if (endTime > duration)
                {
                    duration = endTime;
                }
            }
        }
        
        return result;
    }
    
    private static GenericShowAnimation CreateGenericShowAnimation(ActionContext context,
        List<TMPMeshModifierTrack> tracks)
    {
        var anim = ScriptableObject.CreateInstance<GenericShowAnimation>();
        anim.Tracks.Tracks.Clear();
        
        float duration = anim.Duration;
        anim.Tracks = CreateTrackList(tracks, ref duration);
        anim.Repeat = true;
        anim.Duration = duration;
        return anim;
    }
    
    private static GenericHideAnimation CreateGenericHideAnimation(ActionContext context,
        List<TMPMeshModifierTrack> tracks)
    {
        var anim = ScriptableObject.CreateInstance<GenericHideAnimation>();
        anim.Tracks.Tracks.Clear();
        
        float duration = anim.Duration;
        anim.Tracks = CreateTrackList(tracks, ref duration);
        anim.Repeat = true;
        anim.Duration = duration;

        return anim;
    }
    

    public static bool ExportAsGeneric(ActionContext context, string directoryPath, string assetName, int option)
    {
        if (option == 1) throw new SystemException();

        List<TMPMeshModifierTrack> tracks;
        // Selected only
        if (option == 0)
        {
            tracks = context.tracks.OfType<TMPMeshModifierTrack>().ToList();
            var anim = CreateGenericAnimation(context, tracks);
            GenerateScriptableFromContext(Path.Combine(Path.GetFullPath(directoryPath), assetName), anim);
            return true;
        }

        // All clips
        if (option == 2)
        {
            tracks = context.timeline.GetOutputTracks().OfType<TMPMeshModifierTrack>().ToList();
            var anim = CreateGenericAnimation(context, tracks);
            GenerateScriptableFromContext(Path.Combine(Path.GetFullPath(directoryPath), assetName), anim);
            return true;
        }

        return false;
    }
    
    public static bool ExportAsGenericShow(ActionContext context, string directoryPath, string assetName, int option)
    {
        if (option == 1) throw new SystemException();

        List<TMPMeshModifierTrack> tracks;
        // Selected only
        if (option == 0)
        {
            tracks = context.tracks.OfType<TMPMeshModifierTrack>().ToList();
            var anim = CreateGenericShowAnimation(context, tracks);
            GenerateScriptableFromContext(Path.Combine(Path.GetFullPath(directoryPath), assetName), anim);
            return true;
        }

        // All clips
        if (option == 2)
        {
            tracks = context.timeline.GetOutputTracks().OfType<TMPMeshModifierTrack>().ToList();
            var anim = CreateGenericShowAnimation(context, tracks);
            GenerateScriptableFromContext(Path.Combine(Path.GetFullPath(directoryPath), assetName), anim);
            return true;
        }

        return false;
    }
    
    public static bool ExportAsGenericHide(ActionContext context, string directoryPath, string assetName, int option)
    {
        if (option == 1) throw new SystemException();

        List<TMPMeshModifierTrack> tracks;
        // Selected only
        if (option == 0)
        {
            tracks = context.tracks.OfType<TMPMeshModifierTrack>().ToList();
            var anim = CreateGenericHideAnimation(context, tracks);
            GenerateScriptableFromContext(Path.Combine(Path.GetFullPath(directoryPath), assetName), anim);
            return true;
        }

        // All clips
        if (option == 2)
        {
            tracks = context.timeline.GetOutputTracks().OfType<TMPMeshModifierTrack>().ToList();
            var anim = CreateGenericHideAnimation(context, tracks);
            GenerateScriptableFromContext(Path.Combine(Path.GetFullPath(directoryPath), assetName), anim);
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

    static void GenerateScriptableFromContext(string filePath, UnityEngine.Object anim)
    {
        EnsureDirectoryExists(filePath);

        string relativePath = MakeRelative(filePath, Application.dataPath);
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

    public static bool ExportAsScript(ActionContext context, string directoryPath, string scriptName, int option)
    {
        if (option == 1) throw new SystemException();

        // Selected only
        if (option == 0)
        {
            var anim = CreateGenericAnimation(context, context.tracks.OfType<TMPMeshModifierTrack>().ToList());
            GenericAnimationExporter.ExportGenericAnimation(anim, Path.Combine(Path.GetFullPath(directoryPath), scriptName));
            return true;
        }

        // All clips
        if (option == 2)
        {
            var anim = CreateGenericAnimation(context, context.timeline.GetOutputTracks().OfType<TMPMeshModifierTrack>().ToList());
            GenericAnimationExporter.ExportGenericAnimation(anim, Path.Combine(Path.GetFullPath(directoryPath), scriptName));
            return true;
        }

        return false;
    }
    
    public static bool ExportAsShowScript(ActionContext context, string directoryPath, string scriptName, int option)
    {
        if (option == 1) throw new SystemException();

        // Selected only
        if (option == 0)
        {
            var anim = CreateGenericAnimation(context, context.tracks.OfType<TMPMeshModifierTrack>().ToList());
            GenericAnimationExporter.ExportGenericShowAnimation(anim, Path.Combine(Path.GetFullPath(directoryPath), scriptName));
            return true;
        }

        // All clips
        if (option == 2)
        {
            var anim = CreateGenericAnimation(context, context.timeline.GetOutputTracks().OfType<TMPMeshModifierTrack>().ToList());
            GenericAnimationExporter.ExportGenericShowAnimation(anim, Path.Combine(Path.GetFullPath(directoryPath), scriptName));
            return true;
        }

        return false;
    }
    
    public static bool ExportAsHideScript(ActionContext context, string directoryPath, string scriptName, int option)
    {
        if (option == 1) throw new SystemException();

        // Selected only
        if (option == 0)
        {
            var anim = CreateGenericAnimation(context, context.tracks.OfType<TMPMeshModifierTrack>().ToList());
            GenericAnimationExporter.ExportGenericHideAnimation(anim, Path.Combine(Path.GetFullPath(directoryPath), scriptName));
            return true;
        }

        // All clips
        if (option == 2)
        {
            var anim = CreateGenericAnimation(context, context.timeline.GetOutputTracks().OfType<TMPMeshModifierTrack>().ToList());
            GenericAnimationExporter.ExportGenericHideAnimation(anim, Path.Combine(Path.GetFullPath(directoryPath), scriptName));
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
            var genericAnim = tmpAnimationClip.Animation as IGenericAnimation;
            var name = (tmpAnimationClip.Animation as UnityEngine.Object).name;
            var mainParentTrack = context.timeline.CreateTrack<GroupTrack>(name);

            int trackCounter = 0;
            foreach (var animTrack in genericAnim.Tracks.Tracks) 
            {
                var parentTrack = context.timeline.CreateTrack<TMPMeshModifierTrack>(mainParentTrack, name + "_Track" + trackCounter++);

                foreach (var animClip in animTrack.Clips)
                {
                    var timelineClip = parentTrack.CreateClip<TMPMeshModifierClip>();
                    var modClip = timelineClip.asset as TMPMeshModifierClip;
                    modClip.Step.Step = new AnimationStep(animClip); 
                    
                    timelineClip.displayName = animClip.name;
                    
                    timelineClip.start = animClip.startTime;
                    timelineClip.duration = animClip.duration;
                    
                    // There is no way to set the mode on creation; workaround
                    var reflected = typeof(TimelineClip).GetField("m_PostExtrapolationMode", BindingFlags.NonPublic | BindingFlags.Instance);
                    reflected.SetValue(timelineClip, animClip.postExtrapolation.ConvertExtrapolation());
                    
                    reflected = typeof(TimelineClip).GetField("m_PreExtrapolationMode", BindingFlags.NonPublic | BindingFlags.Instance);
                    reflected.SetValue(timelineClip, animClip.preExtrapolation.ConvertExtrapolation());
                }
            }
            
            EditorUtility.SetDirty(context.timeline);
            TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
        }
        return true;
    }
    #endregion
}