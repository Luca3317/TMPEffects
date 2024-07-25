using System.Collections.Generic;
using TMPEffects.CharacterData;
using UnityEditor;
using UnityEngine;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new ShowAnimationStack", menuName = "TMPEffects/Animations/Show Animations/AnimationStack", order = int.MinValue)]
    public class ShowAnimationStackObject : TMPShowAnimation
    {
        [SerializeField] private ShowAnimationStack stack;

        public override void Animate(CharData cData, IAnimationContext context) => stack.Animate(cData, context);
        public override object GetNewCustomData() => stack.GetNewCustomData();
        public override void SetParameters(object customData, IDictionary<string, string> parameters) => stack.SetParameters(customData, parameters);
        public override bool ValidateParameters(IDictionary<string, string> parameters) => stack.ValidateParameters(parameters);

#if UNITY_EDITOR
        new void OnValidate()
        {
            for (int i = 0; i < stack.Animations.Count; i++)
            {
                if (stack.Animations[i].animation == this)
                {
                    stack.Animations[i] = new AnimationStack<TMPShowAnimation>.AnimPrefixTuple(null, stack.Animations[i].prefix);
                }
            }

            EditorApplication.QueuePlayerLoopUpdate();
            base.OnValidate();
        }
#endif
    }
}
