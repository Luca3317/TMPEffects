using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class TMPCommand : ScriptableObject
{
    // Position of command tag can either:
    //      1. define when command is executed              eg: This will just <!wait=5> wait for 5 seconds
    //      2. define what text the command operates on     eg: This will instantly <!show>show the enclosed</show> text
    //      
    // In the second case, the tag could either be executed when the opening tag is processed in the writer or instantly.
    // Also defined over some toggle (which is hidden in first case)
    public abstract CommandType CommandType { get; }
    public abstract bool ExecuteInstantly { get; }

    // TODO ExecuteCommand needs a context variable;
    // Can pass in both writer and animator, but would prefer a single dedicated context object later
    public abstract void ExecuteCommand(TMPCommandTag tag, TMPWriter writer/*, TMPAnimator animator*/);
    public abstract bool ValidateParameters(Dictionary<string, string> parameters);

#if UNITY_EDITOR
    public virtual void SceneGUI(Vector3 position, TMPCommandTag tag) { }
#endif
}

public enum CommandType
{
    Index = 0,
    Range = 1,
    Both = 2
}