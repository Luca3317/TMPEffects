using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "new TMPEffectDatabase", menuName = "TMPEffects/Effect Database")]
public class TMPEffectDatabase : ScriptableObject
{
    [SerializeField] TMProEffect[] effects;

    Dictionary<int, TMProEffect> effectDict;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        Debug.Log("Called awake and will now populate effectDict");
        effectDict = new Dictionary<int, TMProEffect>();
        foreach (var effect in effects)
        {
            TMProEffectAttribute att = effect.GetType().GetCustomAttribute<TMProEffectAttribute>();
            if (att == null)
            {
                Debug.LogError("Could not get attribute");
                continue;
            }

            Debug.Log("added: " + att.Tag);
            effectDict.Add(att.Tag.GetHashCode(), effect);
        }
    }

    public bool Contains(string name)
    {
        return effectDict.ContainsKey(name.GetHashCode());
    }

    public bool Contains(int nameHashCode)
    {
        return effectDict.ContainsKey(nameHashCode);
    }

    public TMProEffect GetEffect(string name)
    {
        return effectDict[name.GetHashCode()];
    }

    public TMProEffect GetEffect(int nameHashCode)
    {
        return effectDict[nameHashCode];
    }
}
