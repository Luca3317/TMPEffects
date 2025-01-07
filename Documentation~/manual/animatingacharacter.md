<link rel="stylesheet" type="text/css" href="../styles.css">

# Animating a character 
This section guides you through how you animate characters in TMPEffects.

## Applying transformations to a character
#### Moving the character
To move the character, simply use the <mark class="markstyle">SetPosition(Vector3 position)</mark> or  <mark class="markstyle">AddPositionDelta(Vector3 delta)</mark>
method on <mark class="markstyle">CharData</mark>.  
Most of the time, you will want to do this using the original position of the character and an offset.

```csharp
public void Animate(CharData cData, IAnimationContext context)
{
    // Move the character up 125 units over time, then down again; indefinitely
    float val = Mathf.PingPong(context.animatorContext.PassedTime * 50, 125);
    cData.SetPosition(cData.InitialPosition + Vector3.up * val);
}
```

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/animatingacharacter/moving.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

#### Rotating the character
To rotate the character, use <mark class="markstyle">CharData's</mark> <mark class="markstyle">SetRotation(Quaternion rotation)</mark> method.
If you want to rotate around a specific pivot, you may set it using either the <mark class="markstyle">SetPivot(Vector3 pivot)</mark> method or the <mark class="markstyle">AddPivotDelta(Vector3 delta)</mark> method.  
If you don't set a pivot for the rotation, it will rotate around the center of the character.

```csharp
public void Animate(CharData cData, IAnimationContext context)
{
    // Rotate the character indefinitely in the z axis over time
    float angle = context.animatorContext.PassedTime * 50 % 360;
    cData.SetRotation(Quaternion.Euler(0, 0, angle));

    // And by adding this line, it will use the pivot you set;
    // in this case the character will rotate around the point 150 units from its
    // center on the x axis
    cData.AddPivotDelta(Vector3.right * 150);
}
```

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/animatingacharacter/rotating.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

#### Scaling the character
To scale the character, use the <mark class="markstyle">SetScale(Vector3 scale)</mark> method.

```csharp
public void Animate(CharData cData, IAnimationContext context)
{
    // Ping-pong the scale between (0, 0, 0) and (1, 1, 1) over time
    float val = Mathf.PingPong(context.animatorContext.PassedTime, 1);
    Vector3 scale = Vector3.one * val;
    cData.SetScale(scale);
}
```

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/animatingacharacter/scaling.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

## Modifying a character's vertices
#### Setting the vertices' positions
You can set the positions of the character's vertices using the <mark class="markstyle">SetPosition(int i, Vector3 value)</mark> method on the <mark class="markstyle">VertexData</mark> type. The integer specifies the vertex to modify (again, see [CharData](chardata.md)) while the vector specifies the new position.

```csharp
public void Animate(CharData cData, IAnimationContext context)
{
    // Pingpong the magnitude of the offset between 0 and 125 over time,
    // then add that offset to the two top vertices 
    float val = Mathf.PingPong(context.animatorContext.PassedTime * 50, 125);
    for (int i = 1; i < 3; i++)
    {
        cData.mesh.SetPosition(i, cData.initialMesh.GetPosition(i) + Vector3.up * val);
    }
}
```

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/animatingacharacter/positions.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

#### Setting the vertices' colors
Using the <mark class="markstyle">SetColor(int i, Color32 value, bool ignoreAlpha)</mark> method, you can set the color value of each vertex.   
If you pass <mark class="markstyle">true</mark> for <mark class="markstyle">ignoreAlpha</mark>, only the RGB color channels are overwritten;
the alpha channel will remain unchanged.  
If you want to do the opposite of this and only set the alpha channel, then you can use the <mark class="markstyle">SetAlpha(int i, float alpha)</mark> method.
```csharp
public void Animate(CharData cData, IAnimationContext context)
{
    // Set each vertex color to red and set the alpha dependent on passed time.
    Color32 color = Color.red;
    float alpha = Mathf.PingPong(context.animatorContext.PassedTime * 125, 255);
    for (int i = 0; i < 4; i++)
    {
        cData.mesh.SetColor(i, color, true);
        cData.mesh.SetAlpha(i, alpha);
    }
}
```

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/animatingacharacter/colors.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

#### Setting the vertices' UVs
Using the <mark class="markstyle">SetUV0(int i, Vector2 uv)</mark> and <mark class="markstyle">SetUV2(int i, Vector2 uv)</mark> methods, you can set the <mark class="markstyle">UV0</mark> and <mark class="markstyle">UV2</mark> values of each vertex respectively.

These properties are more niche compared to the other ones, and you will likely use them much less; <mark class="markstyle">char</mark> is the only built-in animation to utilize this property.
```csharp
public void Animate(CharData cData, IAnimationContext context)
{
    // Pan the UV0 of the character over time
    Vector2 delta = Vector2.right * context.animatorContext.PassedTime;
    for (int i = 0; i &lt; 4; i++)
    {
        cData.mesh.SetUV0(i, cData.initialMesh.GetUV0(i) + delta);
    }
}
```

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/animatingacharacter/uvs.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

## Making your animation fancier
The above character transformations and vertex modifiers are all you really need to animate your character!  
Combined with easing functions (see: [AnimationCurveUtility](animationcurveutility.md), [Waves](tmpanimator_animationutility_wave.md)), even these really simple animations above can look quite nice already.

For example, the animation code for the built-in <mark class="markstyle">[jump](tmpanimator_builtinbasicanimations.md)</mark> animation is hardly more complicated than that
for the [first animation on this page](animatingacharacter.md#moving-the-character) (at least once you've looked at [Waves](tmpanimator_animationutility_wave.md) :wink:).

```csharp
public override void Animate(CharData cData, IAnimationContext context)
{
    Data data = (Data)context.customData;

    float eval = data.wave.Evaluate(context.animatorContext.PassedTime, GetWaveOffset(cData, context, data.waveOffsetType)).Value;
    cData.SetPosition(cData.InitialPosition + Vector3.up * eval);
}
```

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/animatingacharacter/jumping.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>