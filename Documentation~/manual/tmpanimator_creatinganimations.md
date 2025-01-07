<link rel="stylesheet" type="text/css" href="../styles.css">

# Creating animations
This section walks you through creating your own animations, specifically basic animations, or animations that derive from <mark class="markstyle">TMPAnimation</mark>.
For what small differences there are, see [Creating show / hide animation](#creating-show--hide-animations) and [Creating scene animations](#creating-scene-animations).

## Creating the class
First, create a new C# script in the Unity editor.
Add the following using statement at the top of your class: <mark class="markstyle">using TMPEffects.TMPAnimations;</mark>.  
Then, make the created class derive from <mark class="markstyle">TMPAnimation</mark>.
In order to be able to create the animation object in the Unity editor and add it to your database, make sure to decorate the class with the [CreateAssetMenu](https://docs.unity3d.com/ScriptReference/CreateAssetMenuAttribute.html) attribute.

## Methods
You will have errors due to <mark class="markstyle">TMPAnimation's</mark> abstract members not being implemented.
Auto-implement them using your IDE, or add them manually.
When you are done, your class should look something like this:

```csharp
using UnityEngine;
using TMPEffects.TMPAnimations;

[CreateAssetMenu(fileName="new YourFirstAnimation", menuName="Your/Path/YourFirstAnimation")]
public class YourFirstAnimation : TMPAnimation
{
    public override void Animate(CharData cData, IAnimationContext context)
    {
        throw new System.NotImplementedException();
    }

    public override bool ValidateParameters(IDictionary<string, string> parameters)
    {
        throw new System.NotImplementedException();
    }

    public override object GetNewCustomData()
    {
        throw new System.NotImplementedException();
    }

    public override void SetParameters(object customData, IDictionary<string, string> parameters)
    {
        throw new System.NotImplementedException();
    }
}
```

Let's go over each method individually.

**<mark class="markstyle">Animate(CharData cData, IAnimationContext context)</mark>**: The primary method of your animation. 
This method will be called each animation update, once for each animated character. 
We'll go into more detail about this method in [the next section](animatingacharacter.md).

**<mark class="markstyle">ValidateParameters(IDictionary&lt;string, string&gt;)</mark>**: This method is called once during tag processing. 
It allows you to specify whether a given tag for this animation has valid parameters. [ParameterUtility](parameterutility.md) will come in handy here.
Return true if the parameters are valid, return false if not. If false, the tag will not be processed.

**<mark class="markstyle">GetNewCustomData()</mark>**: Allows you to create a piece of custom data that will be passed into <mark class="markstyle">Animate</mark> as
part of the <mark class="markstyle">IAnimationContext</mark>. Used for storing parameters, keeping other consistent values (for example, create an RNG once and store it here instead of creating it every
<mark class="markstyle">Animate</mark> call), and anything else you need.
In here, you should also set the default values for the parameters defined in the inspector.

**<mark class="markstyle">SetParameters(object customData, IDictionary&lt;string, string&gt;)</mark>**: This method is called once, right after tag processing is done.
The passed in <mark class="markstyle">customData</mark> object is the object you created and returned in <mark class="markstyle">GetNewCustomData</mark>.
It allows you to store the tag parameters in your object and access them in <mark class="markstyle">Animate</mark>.

### Full example
The code below is the full implementation of the built-in <mark class="markstyle">wave</mark> animation.  
If the code seems somewhat daunting don't worry; you will have to have looked at [AnimationUtility](animationutility.md), [ParameterUtility](parameterutility.md), and [Animating a character](animatingacharacter.md)
to fully get what's going on here :slightly_smiling_face:

```csharp
using System.Collections.Generic;
using UnityEngine;
using TMPEffects.CharacterData;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.Animations
{
    [CreateAssetMenu(fileName = "new WaveAnimation", menuName = "TMPEffects/Animations/Wave")]
    public class WaveAnimation : TMPAnimation
    {
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\nFor more information about Wave, see the section on it in the documentation.")]
        [SerializeField] Wave wave = new Wave(AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 0.5f, 0.5f, 1f, 1f, 0.2f);
        [Tooltip("The way the offset for the wave is calculated.\nFor more information about Wave, see the section on it in the documentation.\nAliases: waveoffset, woffset, waveoff, woff")]
        [SerializeField] OffsetType waveOffsetType = OffsetType.XPos;

        // Animate the character
        public override void Animate(CharData cData, IAnimationContext context)
        {
            // Cast your custom data object to the type
            Data data = (Data)context.customData;

            // Evaluate the wave data structure at the current time, with the characters offset (see AnimationUtility section for info on this)
            float eval = data.wave.Evaluate(context.animatorContext.PassedTime, GetWaveOffset(cData, context, data.waveOffsetType)).Value;

            // Set the new position of the character
            cData.SetPosition(cData.info.initialPosition + Vector3.up * eval);
        }

        // Validate the tag's parameters
        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            // If there is no parameters, return true (wave does not have any required parameters)
            if (parameters == null) return true;

            // If there is a parameter "waveoffset" (or one of its aliases)
            // but it has the wrong type, return false 
            if (HasNonWaveOffsetParameter(parameters, "waveoffset")) return false;

            // If the wave parameters could not be validated, return false
            // Note: "WaveParameters" does not refer to anything specific to "WaveAnimation" here.
            // WaveParameters is a predefined parameter bundle in ParameterUtility. See the section on it for more info.
            if (!ValidateWaveParameters(parameters)) return false;

            // else return true
            return true;
        }

        // Create the custom data object, set the default values for the parameters, and return it
        public override object GetNewCustomData()
        {
            return new Data() { wave = this.wave, waveOffsetType = this.waveOffsetType };
        }

        // Set the parameters defined in the tag
        public override void SetParameters(object customData, IDictionary<string, string> parameters)
        {
            // If there is no parameters, return early
            if (parameters == null) return;

            // Cast your custom data object to the type
            Data data = (Data)customData;

            // If has the waveoffset parameter set it in your custom data object
            if (TryGetWaveOffsetParameter(out var wot, parameters, "waveoffset")) data.waveOffsetType = wot;

            // Set the wave in your custom data object
            // As with ValidateWaveParameters, "Wave" refers to the parameter bundle
            // defined in ParameterUtility, not "WaveAnimation".
            data.wave = CreateWave(this.wave, GetWaveParameters(parameters));
        }

        // The class used to store the parameter values
        private class Data
        {
            public Wave wave;
            public OffsetType waveOffsetType;
        }
    }
}
```

### Full example using AutoParameters
Generally, you will want to use [AutoParameters](autoparameters.md) when creating your animation which will automatically take care of anything related to parameters, significantly streamlining the process of creating animations.
The below is completely equivalent to the previous example (take notice that the signature of the Animate method changed):

```csharp
using System.Collections.Generic;
using UnityEngine;
using TMPEffects.CharacterData;
using static TMPEffects.ParameterUtility;
using static TMPEffects.TMPAnimations.AnimationUtility;
using TMPEffects.Extensions;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new WaveAnimation", menuName = "TMPEffects/Animations/Wave")]
    public partial class WaveAnimation : TMPAnimation
    {
        [Tooltip("The wave that defines the behavior of this animation. No prefix.\nFor more information about Wave, see the section on it in the documentation.")]
        [SerializeField, AutoParameterBundle("")] Wave wave = new Wave(AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 0.5f, 0.5f, 1f, 1f, 0.2f);
        [Tooltip("The way the offset for the wave is calculated.\nFor more information about Wave, see the section on it in the documentation.\nAliases: waveoffset, woffset, waveoff, woff")]
        [SerializeField, AutoParameter("waveoffset")] OffsetType waveOffsetType = OffsetType.XPos;

        // Animate the character
        private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
        {
            // Evaluate the wave data structure at the current time, with the characters offset (see AnimationUtility section for info on this)
            float eval = data.wave.Evaluate(context.animatorContext.PassedTime, GetWaveOffset(cData, context, data.waveOffsetType)).Value;

            // Set the new position of the character
            cData.SetPosition(cData.info.initialPosition + Vector3.up * eval);
        }
    }
}
```

See the documentation on [AutoParameters](autoparameters.md) to get more info on how to use it.

## Adding the animation to a database
To actually use the animation in your text, you will have to follow these steps:

1. Create an animation object: Right click in your project view and create it (it will be in the path you specified in the [CreateAssetMenu](https://docs.unity3d.com/ScriptReference/CreateAssetMenuAttribute.html) attribute).
2. Add that object to the database you want to use and give it a name
3. Use that database in the TMPAnimator component

Done! You can now use your custom animation like any of the built-in ones.

## Creating show / hide animations
Creating show and hide animations works 99% the same as creating basic animations.
The only differences are:

- Instead of deriving from <mark class="markstyle">TMPAnimation</mark>, you must derive from <mark class="markstyle">TMPShowAnimation</mark> or <mark class="markstyle">TMPHideAnimation</mark> respectively.
> [!WARNING] 
> You HAVE to call <mark class="markstyle">context.FinishAnimation(cData)</mark> at some point in the animation; This will notify the animator that this show animation is finished, and the character
> may transition from the <mark class="markstyle">Showing</mark> state to the <mark class="markstyle">Shown</mark> state.
> Because of this, all built-in show and hide animations have a <mark class="markstyle">duration</mark> parameter, and <mark class="markstyle">context.FinishAnimation(cData);</mark>
> is called when that duration is exceeded. See the example below for a simple way to do that.

```csharp
public void Animate(CharData cData, IAnimationContext context)
{
    ReadOnlyAnimatorContext ac = context.animatorContext;
    Data d = context.customData as Data;

    // Check if the difference between the time that passed since the animator started
    // playing and the time the character entered the SHOWING state exceeds the duration
    if (ac.PassedTime - ac.StateTime(cData) >= d.duration) 
    {
        context.FinishAnimation(cData);
        return;
    }

    // Actual animation logic here...
}
```

## Creating scene animations
Creating a scene animation, scene show animation or scene hide animation is almost the exact same as creating a basic animation, show animation or hide animation; the only difference is that
you will have to derive from <mark class="markstyle">TMPSceneAnimation</mark>, <mark class="markstyle">TMPSceneShowAnimation</mark> or <mark class="markstyle">TMPSceneHideAnimation</mark> respectively, and that you don't add the [CreateAssetMenu](https://docs.unity3d.com/ScriptReference/CreateAssetMenuAttribute.html) attribute.
Since it is not a ScriptableObject, you of course don't add it to a database either; instead, you add it as a component to a GameObject in your scene, and add that GameObject to your TMPAnimator as described here: [Adding scene animation](tmpanimator_sceneanimations.md#adding-scene-animations).

> [!WARNING]
> The <mark class="markstyle">context.FinishAnimation(cData)</mark> call is required for <mark class="markstyle">TMPSceneShowAnimation</mark> and <mark class="markstyle">TMPSceneHideAnimation</mark> as well.

