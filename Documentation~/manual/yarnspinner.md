# Yarn Spinner 

[YarnSpinner](https://yarnspinner.dev) is compatible with TMPEffects animations out of the box -- simply add the TMPAnimator component to the relevant GameObjects as you would normally.  

In order to use TMPWriter with YarnSpinner, you will want to disable both the "Typewriter Effect" and "Fade Effect" on your DialogueView component (which, if you use the standard objects imported when you install YarnSpinner, is the LineView component on the Dialogue System prefab).
Then add the TMPWriter component on the GameObject that holds the text, as you did with the TMPAnimator.

### Variable storage as keywords
You can use variables defined in YarnSpinner as keywords for your tag parameters. For example, if in your YarnSpinner script you define a variable like so:

```markdown
&lt;&lt;declare $someValue=12&gt;&gt;
```

You can then use that value as tag parameter like so:

```markdown
This is a cool line in my &lt;wave amplitude=$someVal&gt;YarnSpinner&lt;/&gt; script!
```

To achieve this, you'll have to:
- Create a class that derives from [TMPSceneKeywordDatabaseBase](../api/TMPEffects.Databases.TMPSceneKeywordDatabaseBase.yml)
- Give that class a reference to the relevant VariableStorage (in the standard Unity YarnSpinner setup this is a component on DialogueSystem)
- Implement the methods for the types you want to use (example below shows how to do this for float variables)
- Put your database on a GameObject, and assign that GameObject in the TMPAnimator/TMPWriter inspector

```csharp
public class MyDB : TMPSceneKeywordDatabaseBase
{
    [SerializeField] private InMemoryVariableStorage storage;

    public override bool TryGetFloat(string str, out float result)
    {
        return storage.TryGetValue(str, out result);
    }

    // We dont want to support AnimationCurve keywords defined in YarnSpinner, so just return false
    public override bool TryGetAnimCurve(string str, out AnimationCurve result)
    {
        result = default;
        return false;
    }

    // ...
}
```