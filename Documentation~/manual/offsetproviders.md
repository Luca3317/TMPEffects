<link rel="stylesheet" type="text/css" href="../styles.css">

# Offset Providers
Offset providers provide offsets (duh) for a given characters (specifically, for a [CharData](chardata.md)).
These supplied offsets are primarily used to make animation appear non-uniform for each character.  
OffsetProviders implement the <mark class="markstyle">ITMPOffsetProvider</mark> interface.

If it were to not use any offfsets, the e.g. <mark class="markstyle">&lt;wave&gt;</mark> animation would move all characters at the same time.  
<p style="color:blue">TODO Add gif of that</p>

This is the same animation using an offset based on the index of the character.
<p style="color:blue">TODO Add gif of that</p>


## OffsetBundle
A parameter bundle compatible with [AutoParameters](autoparameters.md).
Provides some other common modifiers to be applied to the offset (see its [scripting api](../api/TMPEffects.Parameters.OffsetBundle.yml) for more info).  

If you use this in one of your animations, its inspector will allow you to pick from a dropdown of default offsets (such as index, x-position, wordIndex etc.),
or supply a custom <mark class="markstyle">TMPOffsetProvider</mark> asset.
That way you can create your own offset providers to be used in any animation (including built-in ones, as they all use <mark class="markstyle">OffsetBundle</mark>, if they use any offset at all).

There is an equivalent <mark class="markstyle">SceneOffsetBundle</mark> to be used in [SceneAnimations](tmpanimator_sceneanimations.md) that allow you to provide a 
<mark class="markstyle">TMPSceneOffsetProvider</mark> (which is a component instead of an asset).