# TMPEffects [![view - Documentation](https://img.shields.io/badge/view-Documentation-blue?style=for-the-badge)](https://tmpeffects.luca3317.dev/manual/introduction.html)
Feel free to open issues for any questions you have that are not answered by the docs!  
Also, please share any cool stuff you made with TMPEffects -- I want to get a little example section going ☺️   
If you want to, please support me / TMPEffects on [Ko-fi](https://ko-fi.com/lweist3317)!

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![openupm](https://img.shields.io/npm/v/com.luca3317.tmpeffects?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.luca3317.tmpeffects/)
[![issues - TMPEffects](https://img.shields.io/github/issues/Luca3317/TMPEffects)](https://github.com/Luca3317/TMPEffects/issues)
![test-status](https://github.com/Luca3317/TMPEffects/actions/workflows/main.yml/badge.svg?branch=main)
[![CodeFactor](https://www.codefactor.io/repository/github/luca3317/tmpeffects/badge)](https://www.codefactor.io/repository/github/luca3317/tmpeffects)
***
TMPEffects allows you to easily apply all kinds of effects to your TextMeshPro texts using (custom) tags


<table style="border-collapse: collapse; border: none;">
  <tr>
    <td valign="middle" style="border: none;" align="center"><b>&bull; Animate text blocks continuously</b><br><br>
    <img src="/gifs/tmpeffects-git.gif" width=100% height=auto />
    </td>
    <td valign="middle" style="border: none;" align="center"><b>&bull; Show and hide text over time</b><br><br>
      <img src="/gifs/tmpeffectwrite-git.gif" width=100% height=auto />
    </td>
  </tr>
  <tr>
    <td valign="middle" style="border: none;" align="center"><b>&bull; Animate the show / hide sequence</b><br><br>
    <img src="/gifs/tmpeffectsshowhide-git.gif" width=100% height=auto />
    </td>
    <td valign="middle" style="border: none;" align="center"><b>&bull; Raise commands at any given index</b><br><br>
      <img src="/gifs/tmpeffectscommands-git.gif" width=100% height=auto />
    </td>
  </tr>
</table>

- **Raise events at any given index**

- **API to create your own animations / commands / tags**

- **If you don't like to code: create animations right in the inspector and / or in Unity's Timeline**

- **Many other features (timeline integration, keyword databases, ...)**


<br><br/>
The rest of this README gives just a quick overview of TMPEffects, you definitely should refer to [the manual](https://tmpeffects.luca3317.dev/manual/introduction.html) when questions come up!

## 🌟 Built-in animations (+ commands)
TMPEffects comes with a lot of built-in animations (most of which are really versatile when using tag parameters):
<div style="display:flex;justify-content:center;align-items:center;">
  <img src="/gifs/basic-overview.gif" width="80%"/>
</div>

There are also various built-in show/hide animations (which are just as versatile):
<div style="display:flex;justify-content:center;align-items:center;">
  <img src="/gifs/showhide-overview.gif" width="80%"/>
</div>


For a full preview of built-in [animations](https://tmpeffects.luca3317.dev/manual/tmpanimator_builtinbasicanimations.html), [show / hide animations](https://tmpeffects.luca3317.dev/manual/tmpanimator_builtinshowhideanimations.html), 
and [commands](https://tmpeffects.luca3317.dev/manual/tmpwriter_builtincommands.html), as well as each of their parameters, see the respective documentation.

## 🛠️ Creating your own effects
You can easily create your own animations and commands through custom scripts ([animations](https://tmpeffects.luca3317.dev/manual/animatingacharacter.html), [commands](https://tmpeffects.luca3317.dev/manual/tmpwriter_creatingcommands.html)).

Additionally, if you prefer not to code, you can create animations in the inspector using [GenericAnimations](https://tmpeffects.luca3317.dev/manual/genericanimations.html) or through Unity's timeline window, if you have it installed:
<div style="display:flex;justify-content:center;align-items:center;">
  <img src="/gifs/timeline.gif" width="80%"/>
</div>

## 🏷️ Parameters
TMPEffects comes with very strong support for tag parameters; when creating an animation or command from script adding a parameter to your animation is as easy as decorating it with the [AutoParameter] attribute:

```csharp
[AutoParameter("ampltiude", "amp"), SerializeField]
private float amplitude;
```

You can now set amplitude from the tag (e.g. &lt;wave amp=12&gt;) and use the value in your animation / command logic!

On top of that, TMPEffects has many types that are supported out of the box (float, Vector3, AnimationCurve...) as well as giving you the ability to easily create your own (which are also compatible with the AutoParameter attribute).
You can also define custom keywords using Keyword databases.

## 🤝 Integrations
These are the external packages TMPEffects is integrated and confirmed to work with.  
If there is some other package/tool you'd like to see TMPEffects integrated with, feel free to open an issue (or, even better, a pull request!)

### Timeline
TMPEffects is fully integrated with [Unity's Timeline package](https://docs.unity3d.com/Packages/com.unity.timeline@1.2/manual/index.html), providing custom tracks, clips and markers.

### YarnSpinner
TMPEffects works out of the box with [YarnSpinner](https://www.yarnspinner.dev/), you only have to disable YarnSpinner's built-in typewriter effects (see [the docs](https://tmpeffects.luca3317.dev/manual/yarnspinner.html) on that).  
You can even use variables defined in YarnSpinner scripts as tag parameters!


## 🧩 Dependencies and compatibility
- TMPEffects is compatible with Unity 2021.3 and up
    - Tested in 2021.3, 2022.3 and 2023.2; if you have compatibility issues in another version (>= 2021.3), please open an issue for it!
- Only dependency: Unity's TextMeshPro package (automatically included in Unity 2018.3 and up)

## 🚀 Quickstart
### Installation
TMPEffects is available on the [OpenUPM registry](https://openupm.com/packages/com.luca3317.tmpeffects/).  
Alternatively, you can install TMPEffects through the Unity Package Manager, using the git url:
```console
https://github.com/Luca3317/TMPEffects.git?path=/Package
```

If you don't know how to install packages using git urls, see the [docs](https://tmpeffects.luca3317.dev/manual/installation.html).

If you instead want to simply clone the repository,  
you will have to manually import the required resources located under Assets > TMPEffects > Resources.

### Set up
Add the TMPAnimator and/or TMPWriter component to a GameObject with a TextMeshPro(UI) component, and select "use default database" in both their inspectors.  
You will get a prompt to import the required resources. Hit the button and you're done setting up TMPEffects!

## 📚 Documentation
The full documentation can be found [here](https://tmpeffects.luca3317.dev/manual/introduction.html).

