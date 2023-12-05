using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new TMPShowAnimationDatabase", menuName = "TMPEffects/Database/Show Animation Database", order = 12)]
public class TMPShowAnimationDatabase : TMPAnimationDatabaseBase<TMPShowAnimation>
{
    [SerializedDictionary(keyName: "Tag Name", valueName: "Show Animation")]
    [SerializeField] SerializedDictionary<string, TMPShowAnimation> showAnimations;

    public override bool Contains(string name)
    {
        return showAnimations.ContainsKey(name);
    }

    public override TMPShowAnimation GetEffect(string name)
    {
        return showAnimations[name];
    }
}
