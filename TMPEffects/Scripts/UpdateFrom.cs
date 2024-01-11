using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum UpdateFrom : int
{
    Update = 0,
    LateUpdate = 5,
    FixedUpdate = 10,
    Script = 15
}
