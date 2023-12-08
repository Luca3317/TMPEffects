using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Context class for TMPAnimations.
 * 
 * TODO Context variables
 *  animation scaling - rn, animations will look different for different text sizes; scaleAnimations = true => scale the animation to look uniform
 *  passed time - time since animator started animating (Maybe: scaled vs unscaled time)
 *  
 *  (Maybe) info about the text; charactercount linecount pagecount etc
 *  (Maybe) delta time - time since last animation update
 */

[System.Serializable]
public class AnimationContext
{
    // Animation scaling
    public bool scaleAnimations = true;
    public bool useScaledTime = true;
    [System.NonSerialized, HideInInspector] public float deltaTime;
    [System.NonSerialized, HideInInspector] public float passedTime;


    // TextData holding a lot of TMP_Text.TextInfo information

}
