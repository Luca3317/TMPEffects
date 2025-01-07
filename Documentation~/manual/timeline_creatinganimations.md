<link rel="stylesheet" type="text/css" href="../styles.css">

# Creating animations using Timeline
With <mark class="markstyle">TMPMeshModifierClips</mark>, you can use [Unity's Timeline](https://docs.unity3d.com/Packages/com.unity.timeline@1.2/manual/index.html) as a visual animation creation tool.

## Setting up your timeline
First, you'll have to create your timeline asset, as you always have to do when using the timeline.
Next, add a <mark class="markstyle">TMPMeshModifierTrack</mark> in the timeline window. Assign a <mark class="markstyle">TMPAnimator</mark> from your scene to the track you just created.
As a final step before creating your animation, enable the preview on <mark class="markstyle">TMPAnimator</mark> component so you will get a live update of your animation in the scene / game window.

## Creating your animation
You can now begin creating your animation. You do so by adding <mark class="markstyle">TMPMeshModifierClips</mark> to your track, and defining the modifiers you want applied. Then drag the playhead of the timeline on said clip and you will see the effect in action.  
<br/>
In the example below, the position modifier is set to (10, 10, 0), and the color modifier for the bottom-left and bottom-right vertex is set to a reddish color:
<br/>
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/timeline/timeline_example_1.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

<br/>
You can blend each <mark class="markstyle">TMPMeshModifierClip</mark> in and / or out, where you can freely define the curve and duration of the blend:
<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/timeline/timeline_example_2.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>  

  
<br/>
<br/>
You can also set the offset for each blend:

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/timeline/timeline_example_3.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>  

  
<br/>
<br/>
By default, each character is blended from its default state to the modifiers you set. Alternatively, you can blend from a second set of initial modifiers to your other modifiers.  
Here, the initial modifiers were set to be rotated 45 degrees along the z-axis:

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/timeline/timeline_example_4.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>  

  
<br/>
<br/>
You can also pre- and / or post-extrapolate your <mark class="markstyle">TMPMeshModifierClips</mark>.  
Here, ping-pong is used for the pre-extrapolation and hold for the post-extrapolation:
<br/>

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/timeline/timeline_example_5.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>  


  
<br/>
<br/>
By combining multiple tracks and clips you can make some very complex animations this way!  
Ideally, if you plan on making your own animations, you should play around with this system a bit and make yourself familiar.

If you only want to use these animations using the timeline itself, there's nothing else you have to do.  
If you however want to use them like you would an "normal" animation (with tags etc.), you can export your animation.

## Exporting your animation: Asset
Right-click on one of the <mark class="markstyle">TMPMeshModifierTracks</mark> in your timeline. You will see the option "Export / Asset".
When you select it, a window will pop up allowing you to choose where to store the exported animation, and whether you want to export all present <mark class="markstyle">TMPMeshModifierTracks</mark> or only the ones you currently have selected.
Choose whichever option you want, and a <mark class="markstyle">GenericAnimation</mark> that is fully equivalent to your timeline animation will be created in the directory you specified.
If you want to, you can of course make further changes in the <mark class="markstyle">GenericAnimation's</mark> inspector, as you would with any other <mark class="markstyle">GenericAnimation</mark> (see [this section](genericanimations.html) if you haven't yet).
  
If you want to make further changes but prefer doing so using the timeline, you can simply drag the <mark class="markstyle">GenericAnimation</mark> asset into your timeline. 
This will create a <mark class="markstyle">TMPAnimationClip</mark> on which you can right-click and choose "Unpack generic animation". Tracks and clips will be created that are the same as your original, pre-export timeline.  
As a sidenote, this will also work with any <mark class="markstyle">GenericAnimation</mark> not created using the timeline.  

## Exporting your animation: Script
Alternatively to exporting your timeline clips to a <mark class="markstyle">GenericAnimation</mark>, you can choose the "Export / Script" option.  
This will export your timeline to a <mark class="markstyle">TMPAnimation</mark> script instead, allowing you to add completely custom code to the animation.
Be aware that, unlike when exporting to a <mark class="markstyle">GenericAnimation</mark>, you can't get your original timeline from the exported script. 
Therefore, when using this method, you should keep the timeline asset around so you won't have to recreate it if you ever want to change any exported parts of the animation.