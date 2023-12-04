using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "new TMPEffectDatabase", menuName = "TMPEffects/Effect Database")]
public class TMPEffectsDatabase : ScriptableObject
{
    [SerializeField] TMPAnimation[] effects;

    Dictionary<int, TMPAnimation> effectDict;

    void CreateDict()
    {
        effectDict = new Dictionary<int, TMPAnimation>();
        foreach (var effect in effects)
        {
            if (effect == null)
            {
                continue;
            }

            TMPEffectAttribute att = effect.GetType().GetCustomAttribute<TMPEffectAttribute>();
            if (att == null)
            {
                continue;
            }

            effectDict.Add(att.Tag.GetHashCode(), effect);
        }
    }

    public bool Contains(string name)
    {
        if (effectDict == null) CreateDict();
        return effectDict.ContainsKey(name.GetHashCode());
    }

    public bool Contains(int nameHashCode)
    {
        if (effectDict == null) CreateDict();
        return effectDict.ContainsKey(nameHashCode);
    }

    public TMPAnimation GetEffect(string name)
    {
        if (effectDict == null) CreateDict();
        return effectDict[name.GetHashCode()];
    }

    public TMPAnimation GetEffect(int nameHashCode)
    {
        if (effectDict == null) CreateDict();
        return effectDict[nameHashCode];
    }

#if UNITY_EDITOR
    int prevCount = 0;
    private void OnValidate()
    {
        if (prevCount != effects.Length)
        {
            prevCount = effects.Length;
            CreateDict();
        }
    }
#endif
}
