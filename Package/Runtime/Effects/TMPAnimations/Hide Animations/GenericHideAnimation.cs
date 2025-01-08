    using System.Collections.Generic;
    using TMPEffects.AutoParameters.Attributes;
    using TMPEffects.CharacterData;
    using TMPEffects.Modifiers;
    using UnityEngine;

    namespace TMPEffects.TMPAnimations.HideAnimations
    {
        [AutoParameters]
        [CreateAssetMenu(fileName = "new GenericHideAnimation",
            menuName = "TMPEffects/Animations/Hide Animations/Generic Animation")]
        public sealed partial class GenericHideAnimation : TMPHideAnimation, IGenericAnimation
        {
            #region Editor stuff

            protected override void OnValidate()
            {
                base.OnValidate();
                GenericAnimationUtility.EnsureNonOverlappingTimings_Editor(Tracks);
            }

            #endregion

            #region Fields + Properties

            [field: SerializeField]
            public GenericAnimationUtility.TrackList Tracks { get; set; } = new GenericAnimationUtility.TrackList();

            public bool Repeat
            {
                get => repeat;
                set => repeat = value;
            }

            public float Duration
            {
                get => duration;
                set => duration = value;
            }

            [AutoParameter("repeat", "rp"), SerializeField]
            private bool repeat;

            [AutoParameter("duration", "dur"), SerializeField]
            private float duration;

            private CharDataModifiers modifiersStorage, modifiersStorage2;
            private CharDataModifiers accModifier, current;

            #endregion

            private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
            {
                float t = Mathf.Lerp(0, 1,
                    (context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData)) / data.duration);

                if (t >= 1)
                {
                    context.FinishAnimation(cData);
                    return;
                }

                GenericAnimationUtility.Animate(cData, Tracks, ref data.Steps, data.CachedOffsets, data.repeat, data.duration, 
                    context.AnimatorContext.PassedTime - context.AnimatorContext.StateTime(cData), context,
                    ref modifiersStorage, ref modifiersStorage2, ref accModifier, ref current);
            }

            [AutoParametersStorage]
            private partial class AutoParametersData
            {
                public List<List<AnimationStep>> Steps = null;
                public Dictionary<AnimationStep,
                    (GenericAnimationUtility.CachedOffset inOffset, GenericAnimationUtility.CachedOffset outOffset)> CachedOffsets =
                    new Dictionary<AnimationStep, (GenericAnimationUtility.CachedOffset inOffset, GenericAnimationUtility.CachedOffset outOffset
                        )>();
            }
        }
    }