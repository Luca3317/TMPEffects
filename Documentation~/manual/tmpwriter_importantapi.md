<link rel="stylesheet" type="text/css" href="../styles.css">

# TMPWriter API
This section gives an overview of <mark class="markstyle">TMPWriter's</mark> most important API.  
The full API documentation can be found [here](../api/TMPEffects.Components.TMPWriter.yml).

## Settings & Events
Everything in the [previous section](tmpwriter_inspectoroverview.md) can be set through script as well, with the exception of preview related things.

## Controlling the writer
<mark class="markstyle">TMPWriter</mark> supplies multiple methods to control the writer.

- **StartWriter()**: Starts (or resumes) the writing process
- **StopWriter()**: Stops the writing process
- **ResetWriter()**: Stops the writing process and resets it
- **ResetWriter(int index)**: Stops the writing process and resets it to the given index (must be smaller than the current index of the writer)
- **SkipWriter(bool skipShowAnimations)**: Skips the current text until the next unskippable section, or until the end of the text. Does nothing if the current section is unskippable
- **RestartWriter(bool skipShowAnimations)**: Stops the writing process, resets it and then starts it again

There are also a few method that let you modify the writing process itself:

- **Wait(float seconds)**: Wait for the given amount of time until showing the next character; behavior is equivalent to the <mark class="markstyle">wait</mark> tag
- **SetDelay(float seconds)**: Sets the delay used after each character; behavior is equivalent to the <mark class="markstyle">delay</mark> tag
- **SetSkippable(bool skippable)**: Sets whether the current text section is skippable; behavior is equivalent to the <mark class="markstyle">skippable</mark> tag
- **WaitUntil(Func<bool> condition)**: Wait until the given condition evaluates to true;

> [!WARNING] 
> :warning: WaitUntil has no built-in timeout. It is up to you to ensure the condition wont be false forever / for too long

Default values for the delay as well as the "skippability" of the text can be set in the TMPWriter inspector.

## Adding & removing tags through script
The <mark class="markstyle">TMPWriter</mark> class exposes three different <mark class="markstyle">[TagCollections](tagcollections.md)</mark>: <mark class="markstyle">CommandTags</mark>, which contains all parsed command tags and <mark class="markstyle">EventTags</mark>, which contains all parsed event tags. Additionally, <mark class="markstyle">Tags</mark> is the union of the other two collections.

For each of the <mark class="markstyle">TagCollections</mark>, you may freely add and remove tags at any point.