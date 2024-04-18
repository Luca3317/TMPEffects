# TMPWriter overview
This section gives an overview of the actual TMPWriter component, both for the inspector and scripting. The full API documentation can be found [here](../api/TMPEffects.Components.TMPWriter.yml).

## Preview
To preview the writer in editor mode, you can hit the play button in the <mark style="color: lightgray; background-color: #191a18">Writer preview</mark> section at the top of the TMPWriter inspector.
Right next to it, are the buttons for resetting, stopping, and skipping the writer respectively.
The progress bar lets you freely skip to any point of the writing process.

The two toggles above the player decide whether events and commands are executed in the editor preview.
There is a few things to consider with them:

- Event toggle
    - You will also have to set the actual events you want to raise to <mark style="color: lightgray; background-color: #191a18">Editor and Runtime</mark>.
    - :warning: Be careful about which events you allow to be raised in preview mode. Generally I'd recommend setting the event toggle to false completely.

- Command toggle
    - <mark style="color: lightgray; background-color: #191a18">SceneCommands</mark> are never raised in preview mode.
    - If you create any new commands, you can decide whether it should be raised in preview mode through its <mark style="color: lightgray; background-color: #191a18">ExecuteInPreview</mark> property.

## Controlling the writer
TMPWriter supplies multiple methods to control the writer.

- **StartWriter()**: Starts (or resumes) the writing process
- **StopWriter()**: Stops the writing process
- **ResetWriter()**: Stops the writing process and resets it
- **ResetWriter(int index)**: Stops the writing process and resets it to the given index (must be smaller than the current index of the writer)
- **SkipWriter(bool skipShowAnimations)**: Skips the current text until the next unskippable section, or until the end of the text. Does nothing if the current section is unskippable
- **RestartWriter(bool skipShowAnimations)**: Stops the writing process, resets it and then starts it again

There are also a few method that let you modify the writing process in a more subtle way:

- **Wait(float seconds)**: Wait for the given amount of time until showing the next character; behavior is equivalent to the <mark style="color: lightgray; background-color: #191a18">wait</mark> tag
- **SetDelay(float seconds)**: Sets the delay used after each character; behavior is equivalent to the <mark style="color: lightgray; background-color: #191a18">delay</mark> tag
- **SetSkippable(bool skippable)**: Sets whether the current text section is skippable; behavior is equivalent to the <mark style="color: lightgray; background-color: #191a18">skippable</mark> tag
- **WaitUntil(Func<bool> condition)**: Wait until the given condition evaluates to true; :warning: There is no built-in timeout. It is up to you to ensure the condition wont be false forever / for too long

Default values for the delay as well as the "skippability" of the text can be set in the TMPWriter inspector.


## Command databases
The TMPWriter inspector has a foldout labeled <mark style="color: lightgray; background-color: #191a18">Commands</mark>.
There, you may choose the command database that is used to process command tags from the TextMeshPro component's text.
If you toggle <mark style="color: lightgray; background-color: #191a18">Use default database</mark> to true, the default command database is automatically selected for you.
The default database is defined in the TMPEffects preferences file.
You can also set the database used by the TMPWriter component through script, using the <mark style="color: lightgray; background-color: #191a18">SetDatabase(TMPCommandDatabase db)</mark> method.

Below the database, there is another field, <mark style="color: lightgray; background-color: #191a18">SceneCommands</mark>, which is simply a dictionary that allows you to map tag names to SceneCommands. Tags defined here are also parsed by the TMPWriter.

For more about databases, see [Databases](databases.md). For more about SceneCommands, see [SceneCommands](tmpwriter_scenecommands.md).

## Writer events
Besides the OnTextEvent (see [Getting started](tmpwriter_gettingstarted.md)), there are the following events you may listen to:

- **OnCharacterShown(CharData cData)**: Raised whenever the writer shows a new character; passes the newly shown character
- **OnStartWriter()**: Raised whenever the writing process is started
- **OnStopWriter()**: Raised whenever the writing process is stopped
- **OnResetWriter(int index)**: Raised whenever the writing process is reset; passes the index that was reset to
- **OnResetWriter(int index)**: Raised whenever the writing process is skipped; passes the index that was skipped to
- **OnFinishWriter()**: Raised whenever the writing process is finished, and the whole text is shown


## Adding & removing tags through script
The TMPWriter class exposes three different <mark style="color: lightgray; background-color: #191a18">[TagCollections](tagcollections.md)</mark>: <mark style="color: lightgray; background-color: #191a18">CommandTags</mark>, which contains all parsed command tags and <mark style="color: lightgray; background-color: #191a18">EventTags</mark>, which contains all parsed event tags. Additionally, <mark style="color: lightgray; background-color: #191a18">Tags</mark> is the union of the other two collections.

For each of the <mark style="color: lightgray; background-color: #191a18">TagCollections</mark>, you may freely add and remove tags at any point.