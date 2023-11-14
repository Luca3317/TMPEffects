using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEventArgs
{
    public string name;
    public string[] parameters;

    public TextEventArgs(string name, string[] parameters)
    {
        this.name = name;
        this.parameters = parameters;
    }
}
