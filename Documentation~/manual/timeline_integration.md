<link rel="stylesheet" type="text/css" href="../styles.css">

# Timeline integration
TMPEffects is integrated with [Unity's Timeline package](https://docs.unity3d.com/Packages/com.unity.timeline@1.2/manual/index.html), providing its own timeline clips and markers.

## Clips
At the moment, TMPEffects provides two different clip types.
### TMPAnimationClip
The <mark class="markstyle">TMPAnimationClip</mark> plays an animation (any asset implementing <mark class="markstyle">ITMPAnimation</mark> works) while it is active.  
It additionally allows you to blend the animation in and out, as well as pre/post extrapolating it. <p style="color:blue">TODO Check if this is actually true, and add gif showing how clip works</p>

### TMPMeshModifier clip
The <mark class="markstyle">TMPMeshModifierClip</mark> allows you to apply one-time mesh modifications that aren't necessarily suited to periodic animations.  
For example, at some specific point in your cutscene, you might want your text to change color or jump up once.  

Using an <mark class="markstyle">TMPMeshModifierClip</mark> you may modify the character's positions, rotation and scale, as well as position, color and alpha of the individual vertices.
Blending and extrapolation is supported.  
You may also blend in from a different set of modifiers, using the <mark class="markstyle">Initial Modifiers</mark> option in the clip inspector, or use the <mark class="markstyle">Wave</mark> option to continuously blend the modifiers 
(more about <mark class="markstyle">Waves</mark> [here](tmpanimator_animationutility_wave.md)).  
<mark class="markstyle">TMPMeshModifierClip</mark> also supports <p style="color:blue">TODO Offset chapter linked here</p>.  


By combining multiple (tracks of) <mark class="markstyle">TMPMeshModifierClips</mark>, you can create quite complex periodic animations as well, which you may then export to a standard <mark class="markstyle">TMPAnimation</mark> asset to use in your animation databases.  
This process is explained in more detail in the next section, [Creating animations using Timeline](timeline_creatinganimations.html).


## Markers
Custom markers allow you to control a <mark class="markstyle">TMPWriter</mark> and <mark class="markstyle">TMPAnimator</mark> by calling their most common / important methods.  
Be aware that you will need a <mark class="markstyle">TMPWriterMarkReceiver</mark> or <mark class="markstyle">TMPAnimatorMarkReceiver</mark> component respectively on the same GameObject in order for these markers to work.  
For <mark class="markstyle">TMPWriter</mark>, the following methods have corresponding timeline markers:

- StartWriter()
- StopWriter()
- ResetWriter(int)
- SkipWriter(bool)
- RestartWriter(int)
- Wait(float)
- SetSkippable(bool)
- ResetWait() 

For <mark class="markstyle">TMPAnimator</mark>, the following methods have corresponding timeline markers:

- StartAnimation()
- StopAnimation()
- UpdateAnimations(float)
- SetUpdateFrom(UpdateFrom)
- ResetAnimations()
- ResetAnimations(float)
