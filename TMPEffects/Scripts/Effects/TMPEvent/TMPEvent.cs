using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TMPEvent : UnityEvent<TMPEventArgs>
{
    public TMPEvent() : base() { }
}