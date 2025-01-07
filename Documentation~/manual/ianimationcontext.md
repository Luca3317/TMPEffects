<link rel="stylesheet" type="text/css" href="../styles.css">

# IAnimationContext
An instance of the <mark class="markstyle">IAnimationContext</mark> type serves as the context for all animations.  

## Properties
<mark class="markstyle">IAnimationContext</mark> exposes various properties that will be useful for your animations:

- **CustomData**: The custom data object created in <mark class="markstyle">GetNewCustomData</mark> (see [Creating Animations](tmpanimator_creatinganimations.md)).

- **SegmentData**: Contains information about the animation segment the current character belongs to.
    - **StartIndex**: The first index of the segment within the containing text
    - **Length**: The lenght of the segment
    - **FirstVisibleIndex**: The index of the first visible character (i.e. non-whitespace character)
    - **LastVisibleIndex**: The index of the last visible character (i.e. non-whitespace character)
    - **FirstAnimationIndex**: The index of the first character will actually be animated (i.e. not whitespace, not excluded by TMPAnimator)
    - **LastAnimationIndex**: The index of the last character will actually be animated (i.e. not whitespace, not excluded by TMPAnimator)
    - **Max**: The maximum vertex positions of text in this segment
    - **Min**: The minimum vertex positions of text in this segment
    - **SegmentIndexOf(CharData)**: Get the index within this segment for the passed in <mark class="markstyle">CharData</mark>
<br>
<br>

- **State**: Exposes multiple readonly properties about the current state of the <mark class="markstyle">CharData</mark> (with previous animations already applied).
            Generally, to be used with the <mark class="markstyle">late</mark> tag parameters (see [Getting started with TMPAnimator](tmpanimator_gettingstarted.md)).
    - **CalculateVertexPositions()**: Calculate the current vertex positions. Results can be read from <mark class="markstyle">BL_Result</mark>, <mark class="markstyle">TL_Result</mark>, <mark class="markstyle">TR_Result</mark>, <mark class="markstyle">BR_Result</mark>.
<br>
<br>

- **Finished(int), Finished(CharData)**: Check whether the given <mark class="markstyle">CharData</mark> is done animating. To be used with show and hide animations (see [Creating show / hide animations](tmpanimator_creatinganimations.md#creating-show--hide-animations)).

- **FinishAnimation(CharData)**: Finish the animation for the given <mark class="markstyle">CharData</mark>. To be used with show and hide animations (see [Creating show / hide animations](tmpanimator_creatinganimations.md#creating-show--hide-animations)).

- **AnimatorContext**: The <mark class="markstyle">IAnimatorContext</mark> exposes properties about the animator state.
    - **PassedTime**: The time that has passed since the animator began animating. Generally speaking, you should use this as the time value for basic animations
    - **DeltaTime**: The current delta time used by the animator to update the animations.
    - **UseScaledTime**: Whether the animator uses scaled time (=> whether <mark class="markstyle">PassedTime</mark> is scaled)
    - **ScaleAnimations**: Whether the animator scales the animation. Used by various [AnimationUtility methods](animationutility.md#raw-positions--deltas).
    - **StateTime(CharData)**: How long the given <mark class="markstyle">CharData</mark> has been in its current visibility state for. Generally speaking, you should use this as the time value for show and hide animations
    - **VisibleTime(CharData)**: How long the given <mark class="markstyle">CharData</mark> has been visible for. Can use this alternatively to <mark class="markstyle">PassedTime</mark> for basic animations