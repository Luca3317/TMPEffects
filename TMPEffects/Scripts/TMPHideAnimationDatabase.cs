using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new TMPHideAnimationDatabase", menuName = "TMPEffects/Database/Hide Animation Database", order = 13)]
public class TMPHideAnimationDatabase : TMPAnimationDatabaseBase<TMPHideAnimation>
{
    [SerializedDictionary(keyName: "Tag Name", valueName: "Hide Animation")]
    [SerializeField] SerializedDictionary<string, TMPHideAnimation> hideAnimations;

    public override bool Contains(string name)
    {
        return hideAnimations.ContainsKey(name);
    }

    public override TMPHideAnimation GetEffect(string name)
    {
        return hideAnimations[name];
    }
}

