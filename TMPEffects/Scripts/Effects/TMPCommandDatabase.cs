using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "new TMPCommandDatabase", menuName = "TMPEffects/Database/Command Database", order = 30)]
public class TMPCommandDatabase : TMPEffectDatabase<TMPCommand>
{
    [SerializedDictionary(keyName: "Tag Name", valueName: "Command")]
    [SerializeField] SerializedDictionary<string, TMPCommand> commandDict;

    public override bool Contains(string name)
    {
        return commandDict.ContainsKey(name);
    }

    public override TMPCommand GetEffect(string name)
    {
        return commandDict[name];
    }
}
