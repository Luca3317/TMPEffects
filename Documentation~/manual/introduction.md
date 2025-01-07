<link rel="stylesheet" type="text/css" href="../styles.css">
<style>
    .center-vid {
        display: flex;
        justify-content: center;
        align-content: center;
    }
</style>

# Introduction

TMPEffects is a tool for Unity that allows you to easily apply many different kinds of effects to your text.
It consists of two main components:

- [TMPAnimator](tmpanimator.html) allows you to animate text over time
- [TMPWriter](tmpwriter.html) allows you to show and hide text over time, as well as execute commands or raise events at any given index

Using both components in conjunction also allows you to apply special animations to text that is in the process of being shown or hidden.  

TMPEffects has various built-in animations ([basic](tmpanimator_builtinbasicanimations.md), [show/hide](tmpanimator_builtinshowhideanimations.md)) and [commands](tmpwriter_builtincommands.md), but you can also create your own [animations](tmpanimator_creatinganimations.md) and [commands](tmpwriter_creatingcommands.md).

<br/>
<div class="center-vid">
<video class="center-vid" style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/tmpeffects-intro.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>  
</div>