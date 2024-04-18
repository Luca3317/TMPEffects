# CharData
The <mark style="color: lightgray; background-color: #191a18">CharData</mark> class holds information about a character, which is primarily used by the TMPAnimator and its animations.  
In addition to holding a selection of data supplied by the respective [TMP_CharacterInfo](https://docs.unity3d.com/Packages/com.unity.textmeshpro@1.0/api/TMPro.TMP_CharacterInfo.html), accessible through the <mark style="color: lightgray; background-color: #191a18">info</mark> field, also holds TMPEffects-specific data and methods to manipulate said data.

## Modifiable properties
Each <mark style="color: lightgray; background-color: #191a18">CharData</mark> has a <mark style="color: lightgray; background-color: #191a18">position</mark>, <mark style="color: lightgray; background-color: #191a18">rotation</mark> and <mark style="color: lightgray; background-color: #191a18">scale</mark> property. You may modify all of those properties using the respective setter methods.  
<mark style="color: lightgray; background-color: #191a18">CharData</mark> also exposes the initial, readonly value of each of those properties.

Through the <mark style="color: lightgray; background-color: #191a18">mesh</mark> field, you can access the character's <mark style="color: lightgray; background-color: #191a18">VertexData</mark>.  
In TextMeshPro, each character consists of a rectangular mesh.  
<mark style="color: lightgray; background-color: #191a18">VertexData</mark> allows you to modify the properties of each of the four vertices of the character mesh.  
These properties are:

- Position
- Color
- UV0
- UV2

<mark style="color: lightgray; background-color: #191a18">VertexData</mark> also exposes a <mark style="color: lightgray; background-color: #191a18">ReadOnlyVertexData</mark> object through its <mark style="color: lightgray; background-color: #191a18">initial</mark> field.  
It contains the initial, readonly <mark style="color: lightgray; background-color: #191a18">VertexData</mark>.

## Animating CharData
For an explanation and examples as to how you can animate characters by modifying the mentioned properties, see [Animating a character](animatingacharacter.md).

