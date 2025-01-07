<link rel="stylesheet" type="text/css" href="../styles.css">

# Scene animations
In addition to <mark class="markstyle">TMPAnimation</mark>, <mark class="markstyle">TMPShowAnimation</mark> and <mark class="markstyle">TMPHideAnimation</mark>, which the animations we've seen so far derive from and which are stored on disk, there is also a <mark class="markstyle">SceneAnimation</mark> equivalent for each.
These are alternative versions of the base classes for the respective category which derive from Unity's MonoBehaviour.
The primary purpose of them is to allow you to easily reference Scene objects.

## Adding scene animations
When you have a GameObject with a <mark class="markstyle">SceneAnimation</mark> component on it, you can simply drag it into the corresponding dictionary in TMPAnimator's <mark class="markstyle">Animations</mark> foldout,
the same way you would assign any scene reference. Once you did that, enter a fitting name for the animation in the field next to where you dragged the <mark class="markstyle">SceneAnimation</mark>. That's it! You can now use the animation in your
text through a tag like any of the built-in animations.

## Applying scene animations
Scene animations are applied in the exact same way as their respective counterpart: Scene animation tags can be directly inserted into the text, where scene show animations are prefixed with a '+', scene hide animations with a '-', and basic scene animation tags are not prefixed.

## Creating scene animations
For information about how to create scene animations, see [Creating animations](tmpanimator_creatinganimations.md).