<link rel="stylesheet" type="text/css" href="../styles.css">

# TMPAnimator API
This section gives an overview of <mark class="markstyle">TMPAnimator's</mark> most important API.  
The full API documentation can be found [here](../api/TMPEffects.Components.TMPAnimator.yml).

## Settings
Everything in the [previous section](tmpanimator_inspectoroverview.md) can be set through script as well, with the exception of preview related things.

## Controlling the animator
The animator can be updated automatically (by setting [UpdateFrom](../api/TMPEffects.Components.TMPAnimator.yml#TMPEffects_Components_TMPAnimator_UpdateFrom) to <mark class="markstyle">Update</mark>, <mark class="markstyle">LateUpdate</mark> or <mark class="markstyle">FixedUpdate</mark>).  
In this case, you can control the animator using the methods <mark class="markstyle">StartAnimating</mark> and <mark class="markstyle">StopAnimating</mark>.  
Otherwise, if <mark class="markstyle">UpdateFrom</mark> is set to <mark class="markstyle">Script</mark>, you have to update the animator manually from one of your scripts, using <mark class="markstyle">UpdateAnimations(float deltaTime)</mark>.

## Adding & removing tags through script
The <mark class="markstyle">TMPAnimator</mark> class exposes four different <mark class="markstyle">[TagCollections](tagcollections.md)</mark>: <mark class="markstyle">BasicTags</mark>, which contains all parsed basic animation tags, <mark class="markstyle">ShowTags</mark>, which contains all parsed show animation tags and <mark class="markstyle">HideTags</mark>, which contains all parsed hide animation tags. Additionally, <mark class="markstyle">Tags</mark> is the union of the other three collections.

For each of the <mark class="markstyle">TagCollections</mark>, you may freely add and remove tags at any point.

## Post-animation event
You can register methods as a post-animation delegate using <mark class="markstyle">RegisterPostAnimationHook</mark>. If you do so, your delegate will be called everytime a character's animations has been updated, allowing you to make further changes to the [CharData](chardata.md).
This is primarily useful when you want to make one-off changes, where a periodic animation doesn't really "fit".

## Built-in tag parameters
Any tag parsed by the <mark class="markstyle">TMPAnimator</mark> automatically supports the <mark class="markstyle">override</mark> (shorthand: <mark class="markstyle">or</mark>) parameter, allowing you to set for each tag whether it should override previous tags.
If you don't set this, it will override based on whether or not the <mark class="markstyle">Override animations</mark> toggle is set on the <mark class="markstyle">TMPAnimator</mark>.

Another parameter supported by all animation tags is <mark class="markstyle">late</mark>. 
If set, the animation will be applied in a second pass within the TMPAnimator, meaning it will be applied after all animations that do not have this parameter. 

You will not need this parameter in the large majority of cases. It is useful primarily for when you need the mesh data of the character to consider the changes made by the other animations.
For example, the flashlight effect shown below needs the <mark class="markstyle">late</mark> parameter to work correctly, as it operates on the vertex positions
of the characters. If it was applied before the wave animation, the flashlight would use the incorrect, initial vertex positions.

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/flashlightanimation.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>