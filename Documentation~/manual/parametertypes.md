<link rel="stylesheet" type="text/css" href="../styles.css">

# Parameter types
TMPEffects supports a variety of different parameter types that come with built-in parsing utilities (see the [next section](parameterutility.md)).  

## Supported types
This is the full list of currently supported parameter types:

- float
- int
- bool
- Color
- Vector2/3
- [Anchor]()
- [TypedVector2/3]()
- [AnimationCurve](https://docs.unity3d.com/ScriptReference/AnimationCurve.html)
- [OffsetProvider TODO OffsetProvider link]()
- Array

## Type formatting
This is an overview of how to correctly format the different parameter types by example.  

> [!TIP]
> For any of these types, you can also define valid keywords using a <mark class="markstyle">TMPKeywordDatabase</mark>.  
> Most of these types have pre-defined keywords set in the global <mark class="markstyle">TMPKeywordDatabase</mark> as well; especially for AnimationCurves and OffsetProviders you will want to almost exclusively use keywords.


- float: <mark class="markstyle">2.54</mark>, <mark class="markstyle">3</mark>  
- int: <mark class="markstyle">12</mark>  
- bool: Either <mark class="markstyle">true</mark> or <mark class="markstyle">false</mark>  
- Color: Either in HEX format <mark class="markstyle">#DEADBEEF</mark>, HSV format <mark class="markstyle">hsv(0.3,64,52)</mark> or RGB(A) format <mark class="markstyle">rgb(0,0.5,0.5)</mark>
- Vector2/3: <mark class="markstyle">(0.3, 22.4)</mark>, <mark class="markstyle">(0.3, 22.4, 0)</mark>
- Anchor: Same as Vector2, but with leading "a:"; <mark class="markstyle">a:(0.3, 22.4)</mark>  
- TypedVector2/3: Same as Vector2/3, with either leading "a:", leading "o:", or no prefix; depending on the prefix, will be interpreted as Anchor, Offset or Position; <mark class="markstyle">(0.3, 22.4)</mark>, <mark class="markstyle">o:(0.3, 22.4)</mark>, <mark class="markstyle">a:(0.3, 22.4)</mark>  
- AnimationCurve: You may construct custom curves by specifying one of the predefined methods (<mark class="markstyle">cubic</mark>, <mark class="markstyle">quadratic</mark>, <mark class="markstyle">linear</mark>) or just a raw vector sequence. See [AnimationCurveUtility](animationcurveutility.md) for more info. <mark class="markstyle">quadratic((0,0),(0.2,0.7),(1,1))</mark>, <mark class="markstyle">(0,0),(0.2,0.7),(1,1)</mark>  
- OffsetProvider: This exclusively uses keywords
- Array: Multiple of the desired type, separated by a semicolon; <mark class="markstyle">0.3;4.82;1</mark>, <mark class="markstyle">red;green;blue</mark>, <mark class="markstyle">true;false;false</mark>
