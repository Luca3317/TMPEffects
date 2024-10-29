using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components.Animator;
using UnityEditor;
using UnityEngine;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new HideAnimationStack", menuName = "TMPEffects/Animations/Hide Animations/AnimationStack", order = int.MinValue)]
    public class HideAnimationStackObject : TMPHideAnimation
    {
        [SerializeField] private HideAnimationStack stack;

        public override void Animate(CharData cData, IAnimationContext context) => stack.Animate(cData, context);
        public override object GetNewCustomData(IAnimationContext context) => stack.GetNewCustomData(context);
        public override void SetParameters(object customData, IDictionary<string, string> parameters,
            IAnimationContext context) => stack.SetParameters(customData, parameters, context);
        public override bool ValidateParameters(IDictionary<string, string> parameters, IAnimatorContext context) => stack.ValidateParameters(parameters, context);

#if UNITY_EDITOR
        new void OnValidate()
        {
            for (int i = 0; i < stack.Animations.Count; i++)
            {
                if (stack.Animations[i].animation == this)
                {
                    stack.Animations[i] = new AnimationStack<TMPHideAnimation>.AnimPrefixTuple(null, stack.Animations[i].prefix);
                }
            }

            EditorApplication.QueuePlayerLoopUpdate();
            base.OnValidate();
        }
#endif
    }
}