using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct SceneCommand
{
    public CommandType CommandType;
    public bool executeInstantly;
    public UnityEvent<Dictionary<string, string>> command;
}
