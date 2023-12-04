using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "new TMPAnimationDatabase", menuName = "TMPEffects/Animation Database")]
public class TMPAnimationDatabase : ScriptableObject
{
    [SerializedDictionary(keyName: "Tag Name", valueName: "Animation")]
    [SerializeField] SerializedDictionary<string, TMPAnimation> effectDict;

    public bool Contains(string name)
    {
        return effectDict.ContainsKey(name);
    }

    public TMPAnimation GetAnimation(string name)
    {
        return effectDict[name];
    }
}
