using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using UnityEngine;
using UnityEngine.Playables;

public class WriteClip : TMPWriterClip
{
    public List<TMPWriterClipData> methods = new List<TMPWriterClipData>();

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<WriterBehaviour>.Create(graph);
        
        WriterBehaviour wb = playable.GetBehaviour();

        
        return playable;
    }
}

[System.Serializable]
public struct TMPWriterClipData
{
    public TMPWriterClipMethods method;

    public bool boolValue;
    public float floatValue;
    public int intValue;
}

public struct TMPWriterMethod_Start
{
    public bool skipShowAnims;

    public void Invoke(TMPWriter writer)
    {
        writer.SkipWriter(skipShowAnims);
    }
}

public enum TMPWriterClipMethods : int
{
    Start,
    Stop,
    Reset,
    Skip,
    Restart,
    
    Wait,
    SetDelay,
    SetSkippable,
    // WaitUntil
}
