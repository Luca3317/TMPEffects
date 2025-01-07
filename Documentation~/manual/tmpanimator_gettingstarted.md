<link rel="stylesheet" type="text/css" href="../styles.css">

# Getting started with TMPAnimator

After adding TMPEffects to your project, add a TMPAnimator component to a GameObject with a TMP_Text component (either TextMeshPro - Text 
or TextMeshPro - Text (UI)).

## Applying your first animation
Write some placeholder text in the TextMeshPro's component textbox. Analogous to TextMeshPro's
built-in rich text tags (e.g. &lt;color&gt;, &lt;s&gt;), you can add animations to your text by simply adding animation tags.
Try adding <mark class="markstyle">&lt;wave&gt;</mark> before your placeholder text, and then hitting the <mark class="markstyle">Toggle Preview</mark> button in the TMPAnimator's inspector.
In the scene and game view, you should now see that your text is being animated. It should look something like this:

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/firstanimation.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

> [!WARNING]
> :warning: If the <mark class="markstyle">&lt;wave&gt;</mark> tag is still visible in the scene / game view, the tag is not being correctly processed. Make sure to use
the default database by toggling <mark class="markstyle">Use default database</mark> in the TMPAnimator inspector's <mark class="markstyle">Animations</mark> foldout.


You can close the animation using <mark class="markstyle">&lt;/wave&gt;</mark>. Only text between the opening and closing tag is animated.


## Modifying the animation
Optionally, you can pass various parameter to animation tags.
For example, the <mark class="markstyle">&lt;wave&gt;</mark> tag supports <mark class="markstyle">amplitude</mark> and <mark class="markstyle">uniformity</mark>, among others.
You could modify the tag like so: <mark class="markstyle">&lt;wave amplitude=10 uniformity=0.5&gt;</mark>, which should result in something like this:

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/firstanimation2.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

Try to play around with these two values to get something you like! Theres a lot more customization you can apply for almost all animation tags; a complete overview of all tags
and their respective parameters can be found in [Built-in animations](tmpanimator_builtinbasicanimations.md).


## Stacking animations
First, close the <mark class="markstyle">&lt;wave&gt;</mark> if you haven't already. After the closing tag, add another tag, <mark class="markstyle">&lt;palette&gt;</mark>, as well as some text after this tag that will be animated by it. Your text should look like this now: "&lt;wave&gt; \*Text\* &lt;/wave&gt; &lt;palette&gt; \*More text\* &lt;/palette&gt;", although you will notice the last closing tag is completely optional in this case.
The animated text should look like this:

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/firstanimation3.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

If you now remove the closing <mark class="markstyle">&lt;/wave&gt;</mark> tag, the second text will be animated by both tags (assuming the <mark class="markstyle">Animations override</mark> toggle in the <mark class="markstyle">Animator settings</mark> foldout is set to false, which it
will be by default). It should look something like this:

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/firstanimation4.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

In this manner, you can stack a (theoretically) limitless amount of animations. Of course, there's no guarantee all combinations will mesh well together :wink:

If you switch the <mark class="markstyle">Animations override</mark> toggle in the TMPAnimator's inspector to true, the second text will be animated as before.
This toggle only defines the default behavior of animation tags; you can decide whether a tag should override the previous tags individually by adding the <mark class="markstyle">override</mark> parameter to a tag, like so: <mark class="markstyle">&lt;palette override=true&gt;</mark>. All animation tags support this parameter.
