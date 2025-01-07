<link rel="stylesheet" type="text/css" href="../styles.css">

# AnimationUtility
<mark class="markstyle">AnimationUtility</mark> is a static utility class to be used with all animation types.
The full API docs can be found [here](../api/TMPEffects.TMPAnimations.AnimationUtility.yml).

## Raw Positions & Deltas
One of TMPAnimator's settings is a toggle that decides whether animations should be scaled or not (see [TMPAnimator Settings](tmpanimator_componentoverview.md#animator-settings)).  
In some cases, you will want to ignore this scaling in your animation though. For example, the built-in <mark class="markstyle">spread</mark>
animation sets the individual vertices of the character to the center point of the character, to make it invisible at first and then over time spread out from the center point
to the original vertex positions. If this was scaled, then the vertices would in many cases move either not enough, or too far, to make the character invisible.  
These methods allow you to get and set positions and deltas that ignore the scaling of the animator:

#### Getters
**<mark class="markstyle">Vector3 GetRawVertex(int index, Vector3 position, CharData cData, ref IAnimationContext ctx)</mark>** - 
Calculate the raw version of the passed in vertex position, i.e. the one that will ignore the animator's scaling.

**<mark class="markstyle">Vector3 GetRawPosition(Vector3 position, CharData cData, ref IAnimationContext ctx)</mark>** - 
Calculate the raw version of the passed in character position, i.e. the one that will ignore the animator's scaling.

**<mark class="markstyle">Vector3 GetRawDelta(Vector3 delta, CharData cData, ref IAnimationContext ctx)</mark>** - 
Calculate the raw version of the passed in delta, i.e. the one that will ignore the animator's scaling.


#### Setters
**<mark class="markstyle">void SetVertexRaw(int index, Vector3 position, CharData cData, IAnimationContext ctx)</mark>** - 
Set the raw position of the vertex at the given index. This position will ignore the animator's scaling.

**<mark class="markstyle">void SetPositionRaw(Vector3 position, CharData cData, IAnimationContext ctx)</mark>** - 
Set the raw position of the character. This position will ignore the animator's scaling.

**<mark class="markstyle">void AddVertexDeltaRaw(int index, Vector3 delta, CharData cData, IAnimationContext ctx)</mark>** - 
Add a raw delta to the vertex at the given index. This delta will ignore the animator's scaling.

**<mark class="markstyle">void AddPositionDeltaRaw(Vector3 delta, CharData cData, IAnimationContext ctx)</mark>** - 
Add a raw delta to the position of the character. This delta will ignore the animator's scaling.


#### AnchorToPosition
Given a Vector2 that represents an anchor (see [ParameterTypes](parametertypes.md)),
you can calculate the actual position on the character using the <mark class="markstyle">Vector2 AnchorToPosition(Vector2 anchor, CharData cData)</mark> method.

## GetValue
A simple wrapper method that allows you to evaluate an [AnimationCurve](https://docs.unity3d.com/ScriptReference/AnimationCurve.html) in any [WrapMode](https://docs.unity3d.com/ScriptReference/WrapMode.html).