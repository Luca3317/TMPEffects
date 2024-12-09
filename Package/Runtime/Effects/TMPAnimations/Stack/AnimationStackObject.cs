using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using TMPEffects.Databases;
using UnityEditor;
using UnityEngine;

namespace TMPEffects.TMPAnimations.Animations
{
    /// <summary>
    /// A stack of <see cref="TMPAnimation"/>, allowing you to easily combine animations in one tag.
    /// </summary>
    [CreateAssetMenu(fileName = "new AnimationStack", menuName = "TMPEffects/Animations/Basic Animations/AnimationStack", order = int.MinValue)]
    public class AnimationStackObject : TMPAnimation
    {
        [SerializeField] private BasicAnimationStack stack;

        public override void Animate(CharData cData, IAnimationContext context) => stack.Animate(cData, context);
        public override object GetNewCustomData() => stack.GetNewCustomData();
        public override void SetParameters(object customData, IDictionary<string, string> parameters,
            ITMPKeywordDatabase keywordDatabase) => stack.SetParameters(customData, parameters, keywordDatabase);
        public override bool ValidateParameters(IDictionary<string, string> parameters, ITMPKeywordDatabase keywordDatabase) => stack.ValidateParameters(parameters, keywordDatabase);

#if UNITY_EDITOR
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
#endif
    }
}
