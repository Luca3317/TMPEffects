using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * TODO parameter dictionary vs params string[]?
 */

public class TMPEventArgs : EventArgs
{
    public string name;
    public int index;
    public Dictionary<string, string> parameters;

    public TMPEventArgs(int index, string name, Dictionary<string, string> parameters)
    {
        this.index = index;
        this.name = name;
        this.parameters = parameters;
    }
}
