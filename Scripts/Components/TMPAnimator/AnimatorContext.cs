using UnityEngine;

namespace TMPEffects.Components.Animator
{
    // TODO: Have to make it so that fields are only settable by TMPAnimator
    // Simple solution; make this struct (presuming it will stay this tiny) 

    // TODO: As of right now neither scaleAnimations nor useScaledTime is all that relevant to
    // animations anymore => scaling applied in animator itself and usescaledtime contained in
    // passed / deltatime.

    /// <summary>
    /// To be used with <see cref="TMPAnimator"/>.<br/>
    /// Contains context data of the respective <see cref="TMPAnimator"/>.
    /// </summary>
    [System.Serializable]
    public class AnimatorContext
    {
        /// <summary>
        /// Whether to scale the animations.
        /// </summary>
        public bool scaleAnimations = true;
        
        /// <summary>
        /// Whether to use scaled time.
        /// </summary>
        public bool useScaledTime = true;

        /// <summary>
        /// The deltaTime since the last animation update.
        /// </summary>
        [System.NonSerialized, HideInInspector] public float deltaTime;

        /// <summary>
        /// How much time has passed since the animator started playing its animations.
        /// </summary>
        [HideInInspector] public float passedTime { get => passed; set { passed = value; } }
        private float passed;
    }
}

