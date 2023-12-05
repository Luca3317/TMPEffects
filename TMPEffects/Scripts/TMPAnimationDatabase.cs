using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "new TMPAnimationDatabase", menuName = "TMPEffects/Database/Animation Database", order = 0)]
public class TMPAnimationDatabase : TMPEffectDatabase<TMPAnimation>
{
    public TMPBasicAnimationDatabase basicAnimationDatabase;
    public TMPShowAnimationDatabase showAnimationDatabase;
    public TMPHideAnimationDatabase hideAnimationDatabase;

    public override bool Contains(string name)
    {
        if (basicAnimationDatabase.Contains(name)) return true;
        if (showAnimationDatabase.Contains(name)) return true;
        if (hideAnimationDatabase.Contains(name)) return true;
        return false;
    }

    public override TMPAnimation GetEffect(string name)
    {
        if (basicAnimationDatabase.Contains(name)) return basicAnimationDatabase.GetEffect(name);
        if (showAnimationDatabase.Contains(name)) return showAnimationDatabase.GetEffect(name);
        if (hideAnimationDatabase.Contains(name)) return hideAnimationDatabase.GetEffect(name);
        throw new KeyNotFoundException();
    }
}
