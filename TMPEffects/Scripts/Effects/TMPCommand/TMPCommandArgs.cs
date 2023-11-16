using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMPCommandArgs
{
    public int index;
    public string name;
    public Dictionary<string, string> args;

    public TMPCommandArgs(int index, string name, Dictionary<string, string> args) 
    {
        this.index = index;
        this.name = name;
        this.args = args;
    }
}
