using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPEffects.CharacterData;
using TMPEffects.TMPAnimations;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="new AnimationStack", menuName ="TMPEffects/Animations/AnimationStack", order =int.MinValue)]
public class AnimationStackObject : TMPAnimation
{
    [SerializeField] private BasicAnimationStack stack;

    public override void Animate(CharData cData, IAnimationContext context) => stack.Animate(cData, context);
    public override object GetNewCustomData() => stack.GetNewCustomData();
    public override void SetParameters(object customData, IDictionary<string, string> parameters) => stack.SetParameters(customData, parameters);
    public override bool ValidateParameters(IDictionary<string, string> parameters) => stack.ValidateParameters(parameters);

    #region Editor
    new void OnValidate()
    {
        for (int i = 0; i < stack.Animations.Count; i++)
        {
            if (stack.Animations[i].animation == this)
            {
                stack.Animations[i] = new AnimationStack<TMPAnimation>.AnimPrefixTuple(null, stack.Animations[i].prefix);
            }
        }

        EditorApplication.QueuePlayerLoopUpdate();
        base.OnValidate();
    }
    #endregion
}
