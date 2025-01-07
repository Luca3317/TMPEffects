<link rel="stylesheet" type="text/css" href="../styles.css">

# CharData
The <mark class="markstyle">CharData</mark> class holds information about a character, which is primarily used by the TMPAnimator and its animations.  
In addition to holding a selection of data supplied by the respective [TMP_CharacterInfo](https://docs.unity3d.com/Packages/com.unity.textmeshpro@1.0/api/TMPro.TMP_CharacterInfo.html), accessible through the <mark class="markstyle">info</mark> field, also holds TMPEffects-specific data and methods to manipulate said data.

## Modifiable properties
Each <mark class="markstyle">CharData</mark> has a <mark class="markstyle">position</mark>, <mark class="markstyle">rotation</mark> and <mark class="markstyle">scale</mark> property. You may modify all of those properties using the respective setter methods.  
<mark class="markstyle">CharData</mark> also exposes the initial, readonly value of each of those properties.

Through the <mark class="markstyle">mesh</mark> field, you can access the character's <mark class="markstyle">VertexData</mark>.  
In TextMeshPro, each character consists of a rectangular mesh.  
<mark class="markstyle">VertexData</mark> allows you to modify the properties of each of the four vertices of the character mesh.  
These properties are:

- Position
- Color
- UV0
- UV2

<mark class="markstyle">VertexData</mark> also exposes a <mark class="markstyle">ReadOnlyVertexData</mark> object through its <mark class="markstyle">initial</mark> field.  
It contains the initial, readonly <mark class="markstyle">VertexData</mark>.

## Animating CharData
For an explanation and examples as to how you can animate characters by modifying the mentioned properties, see [Animating a character](animatingacharacter.md).

