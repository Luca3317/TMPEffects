using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITagWrapper
{
    public EffectTag Tag { get; }
    public EffectTagIndices Indices { get; }
}
