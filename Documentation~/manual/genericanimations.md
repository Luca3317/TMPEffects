<link rel="stylesheet" type="text/css" href="../styles.css">

<style>
    .my-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    grid-column-gap:3em;
    }
</style>

# Generic Animation
<mark class="markstyle">GenericAnimations</mark> allow you to create animations in the inspector.  

<div class="my-grid">
<div>
The way they work is analogous to Unity's timeline; you can define multiple lists, where each list is equivalent to a timeline track,
and populate each list with animation steps, that are equivalent to a timeline clip (in this case, specifically the [TMPMeshModifierClip](timeline_integration.md) provided by the timeline integration of TMPEffects).
Each animation step allows you to define a duration during which the step is active (again, analogous to timeline clips), mesh modifiers to be applied, blend curves and extrapolation, and some other smaller features.  
<p style="color:blue">TODO Add image of inspector</p>
</div>
<div>
<img src="../images/placeholder.png" alt="GenericAnimation example">
</div>
</div>

Due to their logic being so similiar to timeline, <mark class="markstyle">GenericAnimations</mark> are compatible with it; you can export animations you created on the timeline to a <mark class="markstyle">GenericAnimation</mark>, 
and you can import a <mark class="markstyle">GenericAnimation</mark> into the timeline, unpacking it to equivalent tracks and clips.

There are separate types of <mark class="markstyle">GenericAnimation</mark> for show and hide animations (<mark class="markstyle">GenericShowAnimation</mark> and <mark class="markstyle">GenericHideAnimation</mark> respectively), which otherwise function the exact same way. 