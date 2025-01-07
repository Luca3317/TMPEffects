<link rel="stylesheet" type="text/css" href="../styles.css">

# AutoParameters
<mark class="markstyle">AutoParameters</mark> is a set of attributes that minimizes any parameter-related boilerplate for animations and commands by automatically handling anything related to parameters,
specifically implementing the <mark class="markstyle">ValidateParameters</mark>, <mark class="markstyle">SetParameters</mark> and <mark class="markstyle">GetNewCustomData</mark> methods for you.

In the vast majoritiy of cases, you should be using <mark class="markstyle">AutoParameters</mark>. Only when you need some special parameter handling will you be required to write custom code for it, and even then in many cases the [Hooks](#hooks) should suffice.

## Creating a class with AutoParameters
To create an animation or command using AutoParameters, create a class and make sure it derives from <mark class="markstyle">ITMPAnimation</mark> or <mark class="markstyle">ITMPCommand</mark>, or any of its sub types.
Your IDE should complain that the class is not partial and doesn't implement the <mark class="markstyle">Animate</mark> method (or <mark class="markstyle">ExecuteCommand</mark> method if youre making a command); 
go ahead and make it partial and implement the method. You can do this either manually or using the pre-defined code fixes. Once you did your class should look somewhat like this:

```csharp
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.TMPAnimations;

[AutoParameters]
public partial class YourClass : TMPAnimation
{
    // if youre making a command, this will of course say ExecuteCommand
    private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
    {
    }
}
```

## Adding valid parameters
To any field on your class that fulfills certain requirements, you can add the <mark class="markstyle">AutoParameter</mark> attribute.
This will automatically generate code in each of the parameter handling methods respectively, and allows you to use it in your animation / command code.
Each field decorated with this attribute must:
- Be serializable (public or decorated with [SerializeField](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/SerializeField.html)).
- Be a valid <mark class="markstyle">AutoParameters</mark> type. This means it must either be a built-in parameter type (see [Parameter Types](parametertypes.md)) or a type decorated with the <mark class="markstyle">[TMPParameterType](creatingparameters.md)</mark> attribute.

Let's add a <mark class="markstyle">float</mark> and a <mark class="markstyle">string</mark> parameter to our class:

```csharp
[AutoParameters]
public partial class SomeClass : TMPAnimation
{
    [AutoParameter(required: true, name: "myFloat", aliases: "myF", "mf"), SerializeField]
    private float myFloat = "12";

    [AutoParameter("myString", "myStr"), SerializeField]
    private string myString = "default string";

    private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
    {
    }
}
```

Thats it! You can now use them through the generated AutoParametersData passed in to your Animate/ExecuteCommand method.
```csharp
    private partial void Animate(CharData cData, AutoParametersData data, IAnimationContext context)
    {
        Debug.Log("myFloat: " + data.myFloat + "; myString: " + data.myString);
    }
```

Here's some example outputs for various tags (assuming we named our animation "myanim" in the database):
- <mark class="markstyle">&lt;myanim&gt;</mark>: "myFloat: 12; myString: default string"
- <mark class="markstyle">&lt;myanim myFloat=23 myStr=anotherstring&gt;</mark>: "myFloat: 23; myString: anotherstring"
- <mark class="markstyle">&lt;myanim mf=-54.34&gt;</mark>: "myFloat: -54.34; myString: default string"

## Adding valid parameters using bundles
Any class decorated with the <mark class="markstyle">TMPParameterBundle</mark> attribute is a parameter bundle.
These classes then, when used as fields and decorated with the <mark class="markstyle">AutoParametersBundle</mark> attribute, define multiple parameters at once.
For example, [Wave](tmpanimator_animationutility_wave.md) is a built-in parameter bundle. 
Using it like so:

```csharp
[AutoParameters]
public partial class SomeClass : TMPAnimation
{
    [AutoParameterBundle(prefix: ""), SerializeField]
    private Wave wave;

    [AutoParameterBundle("w2:"), SerializeField]
    private Wave wave;

    // ...
}
```

allows you to use any parameter defined by [Wave](tmpanimator_animationutility_wave.md) to modify the wave.

- <mark class="markstyle">&lt;myanim amp=10 upperiod=10&gt;</mark>: Sets the amplitude and up period of wave to ten.
- <mark class="markstyle">&lt;myanim amp=10 w2:upperiod=10&gt;</mark>: Sets the amplitude of wave and upperiod of wave2 to ten.
- <mark class="markstyle">&lt;myanim amp=10 w2:amp=10&gt;</mark>: Set the amplitude of both to ten.


## Hooks
You can hook into each of the generated methods to still allow you some custom parameter handling even when using <mark class="markstyle">AutoParameters</mark>.
Each of the hooks has its own code fix, so you don't need to memorize the hook method signatures. Just in case the code fixes don't work for you for some reason, here is them written out:

<details>
  <summary>Hooks</summary>
  
  <ul>
    <li>bool ValidateParameters_Hook(IDictionary<string, string> parameters, ITMPKeywordDatabase keywordDatabase)</li>
    <li>void SetParameters_Hook(object customData, IDictionary<string, string> parameters, ITMPKeywordDatabase keywordDatabase)</li>
    <li>void GetNewCustomData_Hook(object customData)</li>
  </ul>

</details>

## AutoParametersStorage
The <mark class="markstyle">AutoParametersStorage</mark> attribute marks a type as type used to store parsed tag parameters of the class.
It must, like the <mark class="markstyle">AutoParameter</mark> attribute, be contained in a class decorated with <mark class="markstyle">AutoParameters</mark>, and must also be partial.
For example:

```csharp
[AutoParameters]
public partial class SomeClass : TMPAnimation
{
    [AutoParametersStorage]
    private partial class MyStorage
    {
    }
}
```

If you do declare a type like so, <mark class="markstyle">AutoParameters</mark> will no longer generate the default storage type <mark class="markstyle">AutoParametersData</mark>.
You will therefore have to change your <mark class="markstyle">Animate / ExecuteCommand</mark> method like so:

```csharp
[AutoParameters]
public partial class SomeClass : TMPAnimation
{
    private partial void Animate(CharData cData, MyStorage data, IAnimationContext context)
    {
    }

    [AutoParametersStorage]
    private partial class MyStorage
    {
    }
}
```

The advantage of declaring your own <mark class="markstyle">AutoParametersStorage</mark> is being able to store non-parameter related per-animation data, such as a random number generator,
or anything else you need really.

```csharp
[AutoParameters]
public partial class SomeClass : TMPAnimation
{
    // ...
    
    [AutoParametersStorage]
    private partial class MyStorage
    {
        public System.Random rand = new System.Random();
        public Dictionary<CharData, int> someDict = new Dictionary<CharData, int>();
    }
}
```