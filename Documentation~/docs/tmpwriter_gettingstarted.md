# Getting started with TMPWriter
After adding TMPEffects to your project, add a TMPWriter component to a GameObject with a TMP_Text component (either TextMeshPro - Text or TextMeshPro - Text (UI)).

## Writing your first text
Write some placeholder text in the TextMeshPro's component textbox. Hit the play button in the preview section of the TMPWriter inspector, or start playing the scene.
Your text should be being written, and it should look something like this, with each character appearing one after the other, with a short delay in between:

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/writetextnoanim.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

:warning: If your text is instantly showing all at once, or is writing too slowly, ensure the <mark style="color: lightgray; background-color: #191a18">Delay</mark> field in the inspector, which defines the delay after showing a character in seconds, is set to a sensible value. 
The example above uses a delay of 0.075 seconds.

## Adding command tags
TMPWriter allows you to easily execute commands when a specific index is reached. 
You may add them using command tags, prefixed by a '!'. For example, <mark style="color: lightgray; background-color: #191a18">&lt;!wait=1.5&gt;</mark> will pause the writer for 1.5 seconds before continuing.

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/writetextnoanim2.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

A full overview of all built-in command tags is given in the next section.

## Adding event tags
In addition to command tags, TMPWriter also supports event tags.
TMPWriter exposes multiple UnityEvents, to which you may subscribe in the inspector or through code.
One of these events is <mark style="color: lightgray; background-color: #191a18">TextEvent</mark>.
Whenever the writer reaches the index of an event tag, <mark style="color: lightgray; background-color: #191a18">TextEvent</mark> is raised with the parsed tag as parameter.
Unlike command tags or animation tags, you may use any name for event tags, as well as any parameters.
Typically, you would use the tag name in the event callbacks to check whether to process the event / tag.  
Here are a few example tags: <mark style="color: lightgray; background-color: #191a18">&lt;?myevent&gt;</mark>, <mark style="color: lightgray; background-color: #191a18">&lt;?characterspeaking="Faust"&gt;</mark>, <mark style="color: lightgray; background-color: #191a18">&lt;?alert message="\*Your message\*" priority="warning"&gt;</mark>

## Animating text appearances
The examples above look pretty boring; you can change the way the text is shown, as well as hidden, by using a TMPAnimator component along with TMPWriter.
The example below shows you a few variations.

<video style="min-width: 300px; max-width: 2000px; width:75%; height:auto;" src="../videos/writetextanimated.mp4" width="320" height="240" autoplay loop muted>
  Your browser does not support the video tag.
</video>

For info about how to set up the TMPAnimator, see the sections on [TMPAnimator](tmpanimator.md).