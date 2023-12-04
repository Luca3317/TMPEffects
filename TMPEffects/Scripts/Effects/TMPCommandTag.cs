using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMPCommandTag : TMPEffectTag
{
    //public int index;
    //public int length;
    //public string name;
    //public Dictionary<string, string> parameters;

    //public TMPCommandArgs(int index, string name, Dictionary<string, string> parameters, int length = -1) 
    //{
    //    this.index = index;
    //    this.name = name;
    //    this.parameters = parameters;
    //    this.length = length;
    //}
    public TMPCommandTag(string name, int startIndex, Dictionary<string, string> parameters) : base(name, startIndex, parameters)
    { }
}
