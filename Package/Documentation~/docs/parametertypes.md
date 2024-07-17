# Parameter types
TMPEffects supports a variety of different parameter types that come with built-in parsing utilities (see the [next section](parameterutility.md)).  

## Supported types
This is the full list of currently supported parameter types:

- float
- int
- bool
- Color
- Vector2
- Vector3
- [Vector2Offset]()
- [Vector3Offset]()
- [Anchor]()
- [TypedVector2]()
- [TypedVector3]()
- [AnimationCurve](https://docs.unity3d.com/ScriptReference/AnimationCurve.html)
- [WaveOffsetType](animationutility.md#wave-utility)
- Array

## Type formatting
This is an overview of how to correctly format the different parameter types in your tags, most with at least one example.

- float: Must use <mark style="color: lightgray; background-color: #191a18">.</mark> as decimal, not <mark style="color: lightgray; background-color: #191a18">,</mark>
    - <mark style="color: lightgray; background-color: #191a18">2.54</mark>, <mark style="color: lightgray; background-color: #191a18">3</mark>  
- int: Just a plain integer number
    - <mark style="color: lightgray; background-color: #191a18">12</mark>  
- bool: Either <mark style="color: lightgray; background-color: #191a18">true</mark> or <mark style="color: lightgray; background-color: #191a18">false</mark>  
- Color: Colors may be defined either in HEX format, HSV format or RGB(A) format. In addition to that, there are a bunch of supported keywords
    - <mark style="color: lightgray; background-color: #191a18">#DEADBEEF</mark>,<mark style="color: lightgray; background-color: #191a18">hsv(0.3,64,52)</mark>, <mark style="color: lightgray; background-color: #191a18">rgb(0,0.5,0.5)</mark>, <mark style="color: lightgray; background-color: #191a18">indigo</mark>  
- Vector2: Two bracketed floats, separated by comma
    - <mark style="color: lightgray; background-color: #191a18">(0.3, 22.4)</mark>  
- Vector3: Three bracketed floats, separated by comma (or two; third float automatically set to zero)
    - <mark style="color: lightgray; background-color: #191a18">(0.3, 22.4, 0)</mark> = <mark style="color: lightgray; background-color: #191a18">(0.3, 22.4)</mark>  
- Vector2Offset: Same as Vector2, but with leading <mark style="color: lightgray; background-color: #191a18">o:</mark>
    - <mark style="color: lightgray; background-color: #191a18">o:(0.3, 22.4)</mark>  
- Vector2Offset: Same as Vector3, but with leading <mark style="color: lightgray; background-color: #191a18">o:</mark>
    - <mark style="color: lightgray; background-color: #191a18">(0.3, 22.4, 0)</mark> = <mark style="color: lightgray; background-color: #191a18">o:(0.3, 22.4)</mark>  
- Anchor: Same as Vector2, but with leading <mark style="color: lightgray; background-color: #191a18">a:</mark>
    - <mark style="color: lightgray; background-color: #191a18">a:(0.3, 22.4)</mark>  
- TypedVector2: Format of either Vector2, Vector2Offset or Anchor
    - <mark style="color: lightgray; background-color: #191a18">(0.3, 22.4)</mark>, <mark style="color: lightgray; background-color: #191a18">o:(0.3, 22.4)</mark>, <mark style="color: lightgray; background-color: #191a18">a:(0.3, 22.4)</mark>  
- TypedVector3: Format of either Vector3, Vector3Offset or Anchor
    - <mark style="color: lightgray; background-color: #191a18">(0.3, 22.4, 0)</mark>, <mark style="color: lightgray; background-color: #191a18">o:(0.3, 22.4, 0)</mark>, <mark style="color: lightgray; background-color: #191a18">a:(0.3, 22.4)</mark>  
- AnimationCurve: Generally keywords; though you may also construct custom curves by specifying one of the predefined methods (<mark style="color: lightgray; background-color: #191a18">cubic</mark>, <mark style="color: lightgray; background-color: #191a18">quadratic</mark>, <mark style="color: lightgray; background-color: #191a18">linear</mark>) or just a raw vector sequence. See [AnimationCurveUtility](animationcurveutility.md) for more info.
    - <mark style="color: lightgray; background-color: #191a18">easinoutsine</mark>, <mark style="color: lightgray; background-color: #191a18">quadratic((0,0),(0.2,0.7),(1,1))</mark>, <mark style="color: lightgray; background-color: #191a18">(0,0),(0.2,0.7),(1,1)</mark>  
- WaveOffsetType: Keywords  
    - <mark style="color: lightgray; background-color: #191a18">index / idx</mark>, <mark style="color: lightgray; background-color: #191a18">segmentindex / sindex / sidx</mark>, <mark style="color: lightgray; background-color: #191a18">x / xpos</mark>, <mark style="color: lightgray; background-color: #191a18">y / ypos</mark>  
- Array: Multiple of the desired type, separated by a semicolon
    - <mark style="color: lightgray; background-color: #191a18">0.3;4.82;1</mark>, <mark style="color: lightgray; background-color: #191a18">red;green;blue</mark>, <mark style="color: lightgray; background-color: #191a18">true;false;false</mark>
