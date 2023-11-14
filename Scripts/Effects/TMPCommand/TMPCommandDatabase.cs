using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "new TMPCommandDatabase", menuName = "TMPEffects/Command Database")]
public class TMPCommandDatabase : ScriptableObject
{
    [SerializeField] TMPCommand[] commands;

    Dictionary<int, TMPCommand> commandDict;

    private void OnEnable()
    {
        Debug.Log("Called awake and will now populate effectDict");
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
        return commandDict.ContainsKey(name.GetHashCode());
    }

    public bool Contains(int nameHashCode)
    {
        return commandDict.ContainsKey(nameHashCode);
    }

    public TMPCommand GetCommand(string name)
    {
        return commandDict[name.GetHashCode()];
    }

    public TMPCommand GetCommand(int nameHashCode)
    {
        return commandDict[nameHashCode];
    }
}
