using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components.CharacterData;
using UnityEngine;
using static TMPEffects.TMPAnimations.AnimationUtility;

namespace TMPEffects.TMPAnimations.Animations
{
    public class SwingAnimation : TMPAnimation
    {
        [Tooltip("How fast the characters rotate, in rotations per second")]
        [SerializeField] private float speed = 1f;
        [Tooltip("Whether to consider the pivot vector as an offset relative to the characters position, or a raw position.")]
        [SerializeField] private bool offsetDelta = true;
        [Tooltip("The pivot position. Depending on the value of offsetDelta, this is either an offset relative to the characters position, or a raw position.")]
        [SerializeField] private Vector3 pivot = Vector2.zero;
        [Tooltip("The axis of rotation.")]
        [SerializeField] private Vector3 rotationAxis = Vector3.right;

        [Header("Limited rotations")]
        [Tooltip("Whether to limit the rotation to the given angles.")]
        [SerializeField] private bool limitRotation = false;
        [Tooltip("The maximum angle of the rotation. Ignored if limitRotation is false.")]
        [SerializeField] private float maxAngleLimit = 180;
        [Tooltip("The minimum angle of the rotation. Ignored if limitRotation is false.")]
        [SerializeField] private float minAngleLimit = -180f;
        [Tooltip("The wave to use for the rotation. Ignored if limitRotation is false.")]
        [SerializeField] Wave wave;
        [Tooltip("The offset to use for the wave. Ignored if limitRotation is false.")]
        [SerializeField] WaveOffsetType waveOffsetType;

        public override void Animate(CharData cData, IAnimationContext context)
        {



        }

        public override object GetNewCustomData()
        {
            return null;
        }

        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            return true;
        }
    }
}

