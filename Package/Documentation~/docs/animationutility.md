# AnimationUtility
<mark style="color: lightgray; background-color: #191a18">AnimationUtility</mark> is a static utility class to be used with all animation types.
The full API docs can be found [here](../api/TMPEffects.TMPAnimations.AnimationUtility.yml).

## Raw Positions & Deltas
One of TMPAnimator's settings is a toggle that decides whether animations should be scaled or not (see [TMPAnimator Settings](tmpanimator_componentoverview.md#animator-settings)).  
In some cases, you will want to ignore this scaling in your animation though. For example, the built-in <mark style="color: lightgray; background-color: #191a18">spread</mark>
animation sets the individual vertices of the character to the center point of the character, to make it invisible at first and then over time spread out from the center point
to the original vertex positions. If this was scaled, then the vertices would in many cases move either not enough, or too far, to make the character invisible.  
These methods allow you to get and set positions and deltas that ignore the scaling of the animator:

#### Getters
**<mark style="color: lightgray; background-color: #191a18">Vector3 GetRawVertex(int index, Vector3 position, CharData cData, ref IAnimationContext ctx)</mark>** - 
Calculate the raw version of the passed in vertex position, i.e. the one that will ignore the animator's scaling.

**<mark style="color: lightgray; background-color: #191a18">Vector3 GetRawPosition(Vector3 position, CharData cData, ref IAnimationContext ctx)</mark>** - 
Calculate the raw version of the passed in character position, i.e. the one that will ignore the animator's scaling.

**<mark style="color: lightgray; background-color: #191a18">Vector3 GetRawDelta(Vector3 delta, CharData cData, ref IAnimationContext ctx)</mark>** - 
Calculate the raw version of the passed in delta, i.e. the one that will ignore the animator's scaling.


#### Setters
**<mark style="color: lightgray; background-color: #191a18">void SetVertexRaw(int index, Vector3 position, CharData cData, IAnimationContext ctx)</mark>** - 
Set the raw position of the vertex at the given index. This position will ignore the animator's scaling.

**<mark style="color: lightgray; background-color: #191a18">void SetPositionRaw(Vector3 position, CharData cData, IAnimationContext ctx)</mark>** - 
Set the raw position of the character. This position will ignore the animator's scaling.

**<mark style="color: lightgray; background-color: #191a18">void AddVertexDeltaRaw(int index, Vector3 delta, CharData cData, IAnimationContext ctx)</mark>** - 
Add a raw delta to the vertex at the given index. This delta will ignore the animator's scaling.

**<mark style="color: lightgray; background-color: #191a18">void AddPositionDeltaRaw(Vector3 delta, CharData cData, IAnimationContext ctx)</mark>** - 
Add a raw delta to the position of the character. This delta will ignore the animator's scaling.


#### AnchorToPosition
Given a Vector2 that represents an anchor (see [ParameterTypes](parametertypes.md)),
you can calculate the actual position on the character using the <mark style="color: lightgray; background-color: #191a18">Vector2 AnchorToPosition(Vector2 anchor, CharData cData)</mark> method.

## GetValue
A simple wrapper method that allows you to evaluate an [AnimationCurve](https://docs.unity3d.com/ScriptReference/AnimationCurve.html) in any [WrapMode](https://docs.unity3d.com/ScriptReference/WrapMode.html).

## Wave Utility
The <mark style="color: lightgray; background-color: #191a18">AnimationUtility</mark> class contains a <mark style="color: lightgray; background-color: #191a18">Wave</mark> type; 
for more information about it as well as the <mark style="color: lightgray; background-color: #191a18">WaveOffsetType</mark> enum, see the [next section](tmpanimator_animationutility_wave.md).

These are the utility methods for the <mark style="color: lightgray; background-color: #191a18">Wave</mark> type:

#### Converting functions
There are a few simple, general converting functions (that are not specific to the <mark style="color: lightgray; background-color: #191a18">Wave</mark> type, but to waves in general):

**<mark style="color: lightgray; background-color: #191a18">float FrequencyToPeriod(float frequency)</mark>** - Get the period of a wave from its frequency  
**<mark style="color: lightgray; background-color: #191a18">float PeriodToFrequency(float period)</mark>** - Get the frequency of a wave from its period  
**<mark style="color: lightgray; background-color: #191a18">float WaveLengthVelocityToFrequency(float wavelength, float wavevelocity)</mark>** - Get the frequency of a wave from its wavelength and velocity  
**<mark style="color: lightgray; background-color: #191a18">float WaveLengthFrequencyToVelocity(float wavelength, float frequency)</mark>** - Get the velocity of a wave from its wavelength and frequency  
**<mark style="color: lightgray; background-color: #191a18">float WaveVelocityFrequencyToLength(float wavevelocity, float frequency)</mark>** - Get the wavelength of a wave from its velocity and frequency

#### GetWaveOffset
When evaluating a <mark style="color: lightgray; background-color: #191a18">Wave</mark>, you have to pass in an offset, which is dependent on the current character you are animating as well as
the <mark style="color: lightgray; background-color: #191a18">WaveOffsetType</mark> you are using; the <mark style="color: lightgray; background-color: #191a18">float GetWaveOffset(CharData cData, IAnimationContext ctx, WaveOffsetType type)</mark>
calculates the correct offset for you.