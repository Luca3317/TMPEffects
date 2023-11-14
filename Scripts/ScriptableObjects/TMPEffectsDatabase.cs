using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "new TMPEffectDatabase", menuName = "TMPEffects/Effect Database")]
public class TMPEffectsDatabase : ScriptableObject
{
    [SerializeField] TMPEffect[] effects;

    Dictionary<int, TMPEffect> effectDict;

    private void OnEnable()
    {
        Debug.Log("Called awake and will now populate effectDict");
        effectDict = new Dictionary<int, TMPEffect>();
        foreach (var effect in effects)
        {
            if (effect == null)
            {
                Debug.Log("Empty effect in " + name);
                continue;
            }
            
            TMPEffectAttribute att = effect.GetType().GetCustomAttribute<TMPEffectAttribute>();
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

    public TMPEffect GetEffect(string name)
    {
        return effectDict[name.GetHashCode()];
    }

    public TMPEffect GetEffect(int nameHashCode)
    {
        return effectDict[nameHashCode];
    }
}
