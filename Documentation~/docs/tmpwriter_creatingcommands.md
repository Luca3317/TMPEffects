# Creating commands
This section walks you through creating your own commands.
First, create a new C# script in the Unity editor.

### Creating the class
Add the following using statement at the top of your class: <mark style="color: lightgray; background-color: #191a18">using TMPEffects.TMPCommands;</mark>.  
Then, make the created class derive from <mark style="color: lightgray; background-color: #191a18">TMPCommand</mark>.
In order to be able to create the command object in the Unity editor and add it to your database, make sure to decorate the class with the [CreateAssetMenu](https://docs.unity3d.com/ScriptReference/CreateAssetMenuAttribute.html) attribute.

### Members
You will have errors due to <mark style="color: lightgray; background-color: #191a18">TMPCommand's</mark> abstract members not being implemented.
Auto-implement them using your IDE, or add them manually.
When you are done, your class should look something like this:

```csharp
using UnityEngine;
using TMPEffects.TMPCommands;

[CreateAssetMenu(fileName="new YourFirstCommand", menuName="Your/Path/YourFirstCommand")]
public class YourFirstCommand : TMPCommand
{
    public override TagType TagType => throw new System.NotImplementedException();

    public override bool ExecuteInstantly => throw new System.NotImplementedException();

    public override bool ExecuteOnSkip => throw new System.NotImplementedException();

    public override void ExecuteCommand(TMPCommandArgs args)
    {
        throw new System.NotImplementedException();
    }

    public override bool ValidateParameters(IDictionary<string, string> parameters)
    {
        throw new System.NotImplementedException();
    }
}
```

Let's go over each member individually.

### Properties
**<mark style="color: lightgray; background-color: #191a18">TagType</mark>**: Defines whether the tags for this command should operate on an index, a text block, or either option. For example, the built-in <mark style="color: lightgray; background-color: #191a18">wait</mark>
command operates on an index, and the built-in command <mark style="color: lightgray; background-color: #191a18">show</mark> operates on a text block (see [Built-in commands](tmpwriter_builtincommands.md)).

**<mark style="color: lightgray; background-color: #191a18">ExecuteInstantly</mark>**: Commands where this property is true are executed the moment the TMPWriter begins the writing process, instead of when their opening tag index is reached. From the built-in tags, only <mark style="color: lightgray; background-color: #191a18">show</mark> is executed instantly.

**<mark style="color: lightgray; background-color: #191a18">ExecuteOnSkip</mark>**: Commands where this property is true are executed even when their index is skipped over by the writer (i.e., when TMPWriter.SkipWriter() is called). This should be true for commands
that need to ensure they are being called even if skipped over, for example a command that starts a quest or adds an item to the player's inventory.

### Optional properties
There are a few optional properties. If you don't override them, they are set to false by default. In both cases, this is to protect you from yourself :wink:  
Only set these to true if you are sure it is safe for your case!

**<mark style="color: lightgray; background-color: #191a18">ExecuteRepeatable</mark>**: Commands where this property is true may be executed multiple times, specifically if the writer is reset / restarted at any point (i.e., when TMPWriter.ResetWriter() is called).
This should be false for commands that need to ensure they are only ever raised once, for example a command that starts a quest or adds an item to the player's inventory.

**<mark style="color: lightgray; background-color: #191a18">ExecuteInPreview</mark>**: Commands where this property is true are executed in the editor preview. :warning: Note that you must wrap this property in a <mark style="color: lightgray; background-color: #191a18">#if UNITY_EDITOR</mark>
preprocessor directive if you want to override it; otherwise your builds will fail.

### Methods
**<mark style="color: lightgray; background-color: #191a18">ValidateParameters(IDictionary&lt;string, string&gt; parameters)</mark>**: This method is called during tag processing. It allows you to specify whether a given tag for this command has valid parameters. [ParameterUtility](parameterutility.md) will come in handy here.
Return true if the parameters are valid, return false if not. If false, the tag will not be processed.

**<mark style="color: lightgray; background-color: #191a18">ExecuteCommand(TMPCommandArgs args)</mark>**: The meat of your command. This executes the actual command you are implementing.

### TMPCommandArgs
The sole argument for the <mark style="color: lightgray; background-color: #191a18">ExecuteCommand</mark> method. It's kept relatively simple:
it provides access to the actual <mark style="color: lightgray; background-color: #191a18">EffectTag</mark>, through which you may get the tag's parameters, the <mark style="color: lightgray; background-color: #191a18">EffectTagIndices</mark>, and the executing TMPWriter.


### Full example
As complete example, the class below is the implementation of the built-in <mark style="color: lightgray; background-color: #191a18">delay</mark> command.
```csharp
using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.TMPCommands.Commands
{
    [CreateAssetMenu(fileName = "new DelayCommand", menuName = "TMPEffects/Commands/Delay")]
    public class DelayCommand : TMPCommand
    {
        public override TagType TagType => TagType.Index;
        public override bool ExecuteInstantly => false;
        public override bool ExecuteOnSkip => true;
        public override bool ExecuteRepeatable => true;

#if UNITY_EDITOR
        public override bool ExecuteInPreview => true;
#endif

        public override void ExecuteCommand(TMPCommandArgs args)
        {
            if (ParameterUtility.TryGetFloatParameter(out float delay, args.tag.Parameters, ""))
            {
                args.writer.SetDelay(delay);
                return;
            }

            // Since validate parameters ensures the parameter is present and float,
            // this state should be impossible to reach
            throw new System.InvalidOperationException();
        }

        public override bool ValidateParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null) return false;
            if (!parameters.ContainsKey(""))
                return false;

            return ParameterUtility.HasFloatParameter(parameters, "");
        }
    }
}
```

### Adding the command to a database
To actually use the command in your text, you will have to follow these steps:

1. Create a command object: Right click in your project view and create it (it will be in the path you specified in the [CreateAssetMenu](https://docs.unity3d.com/ScriptReference/CreateAssetMenuAttribute.html) attribute).
2. Add that object to the database you want to use and give it a name
3. Use that database in the TMPWriter component

Done! You can now use your custom command like any of the built-in ones.

### Creating scene commands
See [Scene commands](tmpwriter_scenecommands.md) on how to add scene commands.