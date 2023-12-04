using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "new TMPCommandDatabase", menuName = "TMPEffects/Command Database")]
public class TMPCommandDatabase : ScriptableObject
{
    [SerializeField] TMPCommand[] commands;

    Dictionary<int, TMPCommand> commandDict;

    void CreateDict()
    {
        commandDict = new Dictionary<int, TMPCommand>();
        foreach (var command in commands)
        {
            if (command == null)
            {
                Debug.Log("Empty command in " + name);
                continue;
            }

            TMPEffectAttribute att = command.GetType().GetCustomAttribute<TMPEffectAttribute>();
            if (att == null)
            {
                Debug.LogError("Could not get attribute");
                continue;
            }

            Debug.Log("added: " + att.Tag);
            commandDict.Add(att.Tag.GetHashCode(), command);
        }
    }

    public bool Contains(string name)
    {
        if (commandDict == null) CreateDict();
        return commandDict.ContainsKey(name.GetHashCode());
    }

    public bool Contains(int nameHashCode)
    {
        if (commandDict == null) CreateDict();
        return commandDict.ContainsKey(nameHashCode);
    }

    public TMPCommand GetCommand(string name)
    {
        if (commandDict == null) CreateDict();
        return commandDict[name.GetHashCode()];
    }

    public TMPCommand GetCommand(int nameHashCode)
    {
        if (commandDict == null) CreateDict();
        return commandDict[nameHashCode];
    }

#if UNITY_EDITOR
    int prevCount = 0;
    private void OnValidate()
    {
        if (prevCount != commands.Length)
        {
            prevCount = commands.Length;
            CreateDict();
        }
    }
#endif
}
