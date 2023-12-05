using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "new TMPBasicAnimationDatabase", menuName = "TMPEffects/Database/Basic Animation Database", order = 11)]
public class TMPBasicAnimationDatabase : TMPAnimationDatabaseBase<TMPAnimation>
{
    [SerializedDictionary(keyName: "Tag Name", valueName: "Animation")]
    [SerializeField] SerializedDictionary<string, TMPAnimation> animations;

    public override bool Contains(string name)
    {
        return animations.ContainsKey(name);
    }

    public override TMPAnimation GetEffect(string name)
    {
        return animations[name];
    }
}

