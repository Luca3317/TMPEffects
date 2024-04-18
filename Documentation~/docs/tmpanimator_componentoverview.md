# TMPAnimator overview
This section gives an overview of the actual TMPAnimator component, both for the inspector and scripting. The full API documentation can be found [here](../api/TMPEffects.Components.TMPAnimator.yml).

## Preview
To toggle the editor preview of animations, press the <mark style="color: lightgray; background-color: #191a18">TogglePreview</mark> at the top of the TMPAnimator inspector.
Next to it, the button labeled <mark style="color: lightgray; background-color: #191a18">Reset time</mark> resets the time tracked by the TMPAnimator component, and therefore all animations.

## Updating the animations
In the inspector or through the <mark style="color: lightgray; background-color: #191a18">SetUpdateFrom</mark> method, you can set how the animations are updated.

If <mark style="color: lightgray; background-color: #191a18">UpdateFrom</mark> is set to either <mark style="color: lightgray; background-color: #191a18">Update</mark>, <mark style="color: lightgray; background-color: #191a18">LateUpdate</mark> or <mark style="color: lightgray; background-color: #191a18">FixedUpdate</mark>, the animations are automatically updated in the respective Unity callback. In order to play animations in play mode, you will have to call <mark style="color: lightgray; background-color: #191a18">StartAnimating</mark> or set the <mark style="color: lightgray; background-color: #191a18">Play On Start</mark> to true in either the inspector or some other script's <mark style="color: lightgray; background-color: #191a18">Awake</mark> function.
You can then stop animating again by simply calling <mark style="color: lightgray; background-color: #191a18">StopAnimating</mark>.

Alternatively, if you want more fine-tuned control over when and how often animations are updated, for example if you want to limit the updates per second to at most 300, you can set the TMPAnimator's <mark style="color: lightgray; background-color: #191a18">UpdateFrom</mark> to
<mark style="color: lightgray; background-color: #191a18">Script</mark>. This causes the animations to no longer be updated automatically; instead you may call <mark style="color: lightgray; background-color: #191a18">UpdateAnimations(float deltaTime)</mark> manually whenever you like.

Note that if <mark style="color: lightgray; background-color: #191a18">UpdateFrom</mark> is set to <mark style="color: lightgray; background-color: #191a18">Script</mark>, you should not call <mark style="color: lightgray; background-color: #191a18">StartAnimating</mark> or <mark style="color: lightgray; background-color: #191a18">StopAnimating</mark>, since this will have no effect besides logging a warning to Unity's console. Vice versa, if <mark style="color: lightgray; background-color: #191a18">UpdateFrom</mark> is set to be automatically updated, you should not call <mark style="color: lightgray; background-color: #191a18">UpdateAnimations(float deltaTime)</mark>; it again does nothing but log a warning.

The state of <mark style="color: lightgray; background-color: #191a18">UpdateFrom</mark> has no effect on the editor preview.

## Animation databases
The TMPAnimator inspector has a foldout labeled <mark style="color: lightgray; background-color: #191a18">Animations</mark>.
There, you may choose the animation database that is used to process animation tags from the TextMeshPro component's text.
If you toggle <mark style="color: lightgray; background-color: #191a18">Use default database</mark> to true, the default animation database is automatically selected for you.
The default database is defined in the TMPEffects preferences file.
You can also set the database used by the TMPAnimator component through script, using the <mark style="color: lightgray; background-color: #191a18">SetDatabase(TMPAnimationDatabase db)</mark> method.

Below the database, there are three other fields: <mark style="color: lightgray; background-color: #191a18">SceneAnimations</mark>, <mark style="color: lightgray; background-color: #191a18">SceneShowAnimations</mark> and <mark style="color: lightgray; background-color: #191a18">SceneHideAnimations</mark>. These are simply dictionaries that allow you to map tag names to SceneAnimations. Tags defined here are also parsed by the TMPAnimator.

For more about databases, see <mark style="color: red; background-color: #191a18">[TODO](s)</mark>. For more about SceneAnimations, see [SceneAnimations](tmpanimator_sceneanimations.md).

## Animator settings
TMPAnimator has various settings that modify the way it animates its text. Each of these is settable through both the inspector and through script.
- Animations override:<br>
The default override behavior for all animation tags. If true, each tag overrides any of its category (basic / show / hide) that came before it, and only that one is applied. Otherwise, animations are stacked by default. Each tag can manually
define its override behavior by using the <mark style="color: lightgray; background-color: #191a18">override</mark> (shorthand: <mark style="color: lightgray; background-color: #191a18">or</mark>) parameter.
<br>

- Default show / hide string:<br>
Allows you to define a default show / hide animation that is used for the entirety of the text, if no other show / hide animation tag effects it. Set this like you would add any tag to your text, e.g. <mark style="color: lightgray; background-color: #191a18">&lt;+fade dur=0.65 anc=a:bottom>&gt;</mark>, <mark style="color: lightgray; background-color: #191a18">&lt;-spread crv=easeinoutsine>&gt;</mark>.
<br>

- Exclusions:<br>
For each of the animationg categories (basic / show / hide), you can define a set of characters that is excluded from all animations. For example, if you don't want numbers to be animated, you could set <mark style="color: lightgray; background-color: #191a18">Excluded Characters</mark> to "1234567890". In addition to this, there is an <mark style="color: lightgray; background-color: #191a18">Exclude Punctuation</mark> toggle for each of the categories.
<br>

- Scale animations:<br>
Defines whether animations should be scaled to the font size property of the TMP_Text component. If true, animations will look identical regardless of font size.
<br>

- Use scaled time:<br>
Defines whether animations should use [scaled time](https://docs.unity3d.com/ScriptReference/Time-timeScale.html) or not.
<br>

## Adding & removing tags through script
The TMPAnimator class exposes four different <mark style="color: lightgray; background-color: #191a18">[TagCollections](tagcollections.md)</mark>: <mark style="color: lightgray; background-color: #191a18">BasicTags</mark>, which contains all parsed basic animation tags, <mark style="color: lightgray; background-color: #191a18">ShowTags</mark>, which contains all parsed show animation tags and <mark style="color: lightgray; background-color: #191a18">HideTags</mark>, which contains all parsed hide animation tags. Additionally, <mark style="color: lightgray; background-color: #191a18">Tags</mark> is the union of the other three collections.

For each of the <mark style="color: lightgray; background-color: #191a18">TagCollections</mark>, you may freely add and remove tags at any point.