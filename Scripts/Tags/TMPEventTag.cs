using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TMPEventTag
{
    public string name { get; private set; }
    public int startIndex { get; private set; }

    public Dictionary<string, string> parameters { get; private set; }

    public TMPEventTag(string name, int startIndex, Dictionary<string, string> parameters)
    {
        this.name = name;
        this.startIndex = startIndex;
        this.parameters = parameters;
    }
}
