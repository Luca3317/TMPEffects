<link rel="stylesheet" type="text/css" href="../styles.css">

# Databases

In TMPEffects, the set of animations a <mark class="markstyle">TMPAnimator</mark> can use (or the set of commands a <mark class="markstyle">TMPWriter</mark> can use)
is defined by the database it uses.

Databases, like animations and commands, are [ScriptableObject](https://docs.unity3d.com/Manual/class-ScriptableObject.html) assets.  
You can create a new database by right-clicking in your project view, then Create -> TMPEffects -> Database, again, just like animations and commands.  
You can then add any animation / command to your database, and assign it to a TMPAnimator / TMPWriter component in the inspector.  
Of course, you can also modify the built-in default databases any way you want, or assign different databases to be used as default database in the TMPEffects settings
(in the top bar, Edit -> Preferences -> TMPEffects).

[SceneAnimations](tmpanimator_sceneanimations.md) and [SceneCommands](tmpwriter_scenecommands.md) are separate from databases and are instead added to a dictionary in the component's inspector; see the individual sections on them.  

In addition to commands and animations, there are also keyword databases that allow you to define your own keywords for the various [parameter types](parametertypes.md).