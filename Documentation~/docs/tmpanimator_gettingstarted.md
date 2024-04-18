# Getting started with TMPAnimator

After adding TMPEffects to your project, add a TMPAnimator component to a GameObject with a TMP_Text component (either TextMeshPro - Text 
or TextMeshPro - Text (UI)).

## Applying your first animation
Write some placeholder text in the TextMeshPro's component textbox. Analogous to TextMeshPro's
built-in rich text tags (e.g. &lt;color&gt;, &lt;s&gt;), you can add animations to your text by simply adding animation tags.
Try adding <mark style="color: lightgray; background-color: #191a18">&lt;wave&gt;</mark> before your placeholder text, and then hitting the <mark style="color: lightgray; background-color: #191a18">Toggle Preview</mark> button in the TMPAnimator's inspector.
In the scene and game view, you should now see that your text is being animated. It should look something like this:

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/firstanimation.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

:warning: If the <mark style="color: lightgray; background-color: #191a18">&lt;wave&gt;</mark> tag is still visible in the scene / game view, the tag is not being correctly processed. Make sure to use
the default database by toggling <mark style="color: lightgray; background-color: #191a18">Use default database</mark> in the TMPAnimator inspector's <mark style="color: lightgray; background-color: #191a18">Animations</mark> foldout.

You can close the animation using <mark style="color: lightgray; background-color: #191a18">&lt;/wave&gt;</mark>. Only text between the opening and closing tag is animated.


## Modifying the animation
Optionally, you can pass various parameter to animation tags.
For example, the <mark style="color: lightgray; background-color: #191a18">&lt;wave&gt;</mark> tag supports <mark style="color: lightgray; background-color: #191a18">amplitude</mark> and <mark style="color: lightgray; background-color: #191a18">uniformity</mark>, among others.
You could modify the tag like so: <mark style="color: lightgray; background-color: #191a18">&lt;wave amplitude=10 uniformity=0.5&gt;</mark>, which should result in something like this:

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/firstanimation2.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

Try to play around with these two values to get something you like! Theres a lot more customization you can apply for almost all animation tags; a complete overview of all tags
and their respective parameters can be found in [Built-in animations](tmpanimator_builtinanimations.md).


## Stacking animations
First, close the <mark style="color: lightgray; background-color: #191a18">&lt;wave&gt;</mark> if you haven't already. After the closing tag, add another tag, <mark style="color: lightgray; background-color: #191a18">&lt;palette&gt;</mark>, as well as some text after this tag that will be animated by it. Your text should look like this now: "&lt;wave&gt; \*Text\* &lt;/wave&gt; &lt;palette&gt; \*More text\* &lt;/palette&gt;", although you will notice the last closing tag is completely optional in this case.
The animated text should look like this:

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/firstanimation3.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

If you now remove the closing <mark style="color: lightgray; background-color: #191a18">&lt;/wave&gt;</mark> tag, the second text will be animated by both tags (assuming the <mark style="color: lightgray; background-color: #191a18">Animations override</mark> toggle in the <mark style="color: lightgray; background-color: #191a18">Animator settings</mark> foldout is set to false, which it
will be by default). It should look something like this:

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/firstanimation4.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

In this manner, you can stack a (theoretically) limitless amount of animations. Of course, there's no guarantee all combinations will mesh well together :wink:

If you switch the <mark style="color: lightgray; background-color: #191a18">Animations override</mark> toggle in the TMPAnimator's inspector to true, the second text will be animated as before.
This toggle only defines the default behavior of animation tags; you can decide whether a tag should override the previous tags individually by adding the <mark style="color: lightgray; background-color: #191a18">override</mark> parameter to a tag, like so: <mark style="color: lightgray; background-color: #191a18">&lt;palette override=true&gt;</mark>. All animation tags support this parameter.

## Late animations / second pass
Another parameter supported by all animation tags is <mark style="color: lightgray; background-color: #191a18">late</mark>, and is used like so: <mark style="color: lightgray; background-color: #191a18">&lt;wave late&gt;</mark>. If set, the animation will be applied in a second pass within the TMPAnimator, meaning it will be applied after all animations that do not have
this parameter. 

You will not need this parameter in the large majority of cases. It is useful primarily for when you need the mesh data of the character to consider the changes made by the other animations.
For example, the flashlight effect shown below needs the <mark style="color: lightgray; background-color: #191a18">late</mark> parameter to work correctly, as it operates on the vertex positions
of the characters. If it was applied before the wave animation, the flashlight would use the incorrect, initial vertex positions.

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/flashlightanimation.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

More information about how animations work (and how to create your own) can be found in [Creating Animations](tmpanimator_creatinganimations.md).
