# Show / hide animations
In addition to basic animations, which are applied continuously, there are also show animations and hide animations, which will be applied only when the
effected text is in the process of being shown / hidden.

## Applying show / hide animations
Generally speaking, both show and hide animations require you to add a TMPWriter component to the same GameObject as your TMPAnimator.  
(You could also write a custom script to show and hide the text in the manner required for show / hide animations to take effect, using TMPAnimator's or TMPWriter's <mark style="color: lightgray; background-color: #191a18">Show/Hide</mark> methods.)

Show and hide animations are applied in much the same way as basic animations are; in your TMP_Text component, simply add the supported show / hide animation tags like you would regular TextMeshPro tags.
Show animation tags are prefixed with a '+', for example <mark style="color: lightgray; background-color: #191a18">&lt;+fade&gt;</mark>. Hide animation tags are prefixed with a '-', for example <mark style="color: lightgray; background-color: #191a18">&lt;-move&gt;</mark>.
For both, the corresponding closing tag must also include the prefix, after the slash: <mark style="color: lightgray; background-color: #191a18">&lt;/+fade&gt;</mark>, <mark style="color: lightgray; background-color: #191a18">&lt;/-move&gt;</mark>.

So, for example, the string "&lt;+fade&gt;&lt;-move&gt;My placeholder text" would animate the text like this:

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/showhide1.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

If you want to try out recreating this animation, you will have to add a TMPWriter component your TMPAnimator GameObject, and set it up like described in [TMPWriter](tmpwriter.md).

Setting parameters and stacking show / hide animations works completely analogous to basic animations.