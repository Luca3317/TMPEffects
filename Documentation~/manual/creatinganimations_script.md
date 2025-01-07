<link rel="stylesheet" type="text/css" href="../styles.css">

# Creating animations via script
This section walks you through creating your own animations, specifically basic animations, or animations that derive from <mark class="markstyle">TMPAnimation</mark>.
For what small differences there are, see [Creating show / hide animation](#creating-show--hide-animations) and [Creating scene animations](#creating-scene-animations).

Various utility systems will be used in these examples, most importantly [AutoParameters](autoparameters.md).
Using the <mark class="markstyle">AutoParameters</mark> will automatically take care of anything related to your animation's parameters, including parsing, validating and setting them.

If you want to get the "nitty-gritty" version of writing your own animations, see [Creating animations - InDepth](creatinganimations_scriptindepth.md).

## Creating the class
First, create a new C# script in the Unity editor.
Remove all the default Unity stuff (inheriting from MonoBehaviour, Start and Update method).
Then, make the created class derive from <mark class="markstyle">TMPAnimation</mark>, and decorate it with the [AutoParameters](autoparameters.md) attribute.
In order to be able to create the animation object in the Unity editor and add it to your database, make sure to also decorate the class with the [CreateAssetMenu](https://docs.unity3d.com/ScriptReference/CreateAssetMenuAttribute.html) attribute.

Your IDE should complain that the class is not partial and doesn't implement the animate method; go ahead and make it partial and implement the method. You can do this either manually or using the pre-defined code fixes. Once you did your class should look somewhat like this:

```csharp
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.TMPAnimations;

[AutoParameters]
public partial class YourAnimation : TMPAnimation
{
    // Your animation logic goes here
    private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
    {
    }
}
```

## Beginning to animate
For now your animation doesn't do much.
Let's plug in this piece of code, which simply scale each character to double its original size.

```csharp
    private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
    {
        cData.SetScale(Vector3.one * 2);
    }
```

We'll keep the animation itself very simple in this section, the next section [Animating a character](animatingacharacter.md) will go into more detail.

## Parametrizing
Let's say you want this to be a general text scale animation, and you want to be able to set the size through a parameter in your tags.
Simply add a field to your class, and decorate it with the <mark class="markstyle">AutoParameterAttribute</mark>. You can define the valid names of the parameter in your tags

```csharp
    [AutoParameter("scalar", "sclr", "s")] [SerializeField]
    private float scalar;

    private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
    {
        cData.SetScale(Vector3.one * data.scalar);
    }
```

## Adding parameters


