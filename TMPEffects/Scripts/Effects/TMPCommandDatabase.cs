using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "new TMPCommandDatabase", menuName = "TMPEffects/Command Database")]
public class TMPCommandDatabase : ScriptableObject
{
    [SerializedDictionary(keyName: "Tag Name", valueName: "Command")]
    [SerializeField] SerializedDictionary<string, TMPCommand> commandDict;

    public bool Contains(string name)
    {
        return commandDict.ContainsKey(name);
    }

    public TMPCommand GetCommand(string name)
    {
        return commandDict[name];
    }
}
