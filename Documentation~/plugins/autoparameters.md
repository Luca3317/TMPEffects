# AutoParameters
AutoParameters greatly streamlines the handling of parameters of animations and minimizes boilerplate -- you no longer will have to implement <mark style="color: lightgray; background-color: #191a18">ValidateParameters</mark>,
<mark style="color: lightgray; background-color: #191a18">SetParameters</mark> and <mark style="color: lightgray; background-color: #191a18">GetNewCustomData</mark>.  
If you haven't yet, look at [Creating Animations](../docs/tmpanimator_creatinganimations.md) before this section.

#### Installation
Assuming you [installed TMPEffects](../docs/installation.md), you add AutoParameters to your project by simply downloading the two .dll files and their .meta files from the
[TMPEffects.AutoParameters](https://github.com/Luca3317/TMPEffects.AutoParameters/releases) releases and putting them anywhere within your project's asset folder.

#### Using AutoParameters
Once you installed AutoParameters, you can decorate any partial animation class (i.e. one that implements <mark style="color: lightgray; background-color: #191a18">ITMPAnimation</mark>)
with the <mark style="color: lightgray; background-color: #191a18">[AutoParameters]</mark> attribute. This allows you to use the other AutoParameters attributes.

- <mark style="color: lightgray; background-color: #191a18">[AutoParameter]</mark>
You can decorate any field of your class with this attribute, assuming it is of a [supported type](../docs/parametertypes.md).   
You can define whether the parameter is required (<mark style="color: lightgray; background-color: #191a18">false</mark> by default), its name as well as any desired amount of aliases.  
For every field decorated with this attribute, a field of the same name will be created in the [custom data object](../docs/tmpanimator_creatinganimations.md).

```csharp
[AutoParameters]
public partial class MyAnimation : TMPAnimation
{
    [AutoParameter("amplitude", "amp"), SerializeField]
    float amp;

    [AutoParameter("color", "colour", "col"), SerializeField]
    Color color;

    [AutoParameter(true, "someOtherValue"), SerializeField]
    int val;
}
```

- <mark style="color: lightgray; background-color: #191a18">[AutoParameterBundle]</mark>
Alternative to <mark style="color: lightgray; background-color: #191a18">[AutoParameter]</mark>, to be used with predefined parameter sets.  
This is at the moment used only for [Waves](../docs/tmpanimator_animationutility_wave.md).
You may define the prefix used for the wave.

```csharp
[AutoParameters]
public partial class MyAnimation : TMPAnimation
{
    [AutoParameterBundle(""), SerializeField]
    Wave wave;

    [AutoParameterBundle("w2:"), SerializeField]
    Wave wave2;
}
```

- <mark style="color: lightgray; background-color: #191a18">[AutoParametersStorage]</mark>
You can decorate exactly one nested, partial type with this attribute. This type will then be used as the [custom data object](../docs/tmpanimator_creatinganimations.md) for the animation.
You can add any other fields in here that don't have anything to do with parameters (for example, a RNG). 
Any initialization unrelated to parameters can be done in the type's default constructor (one without arguments), or in the <mark style="color: lightgray; background-color: #191a18">GetCustomData_Hook</mark> (see below).  
:warning: If you define a constructor for this type with arguments, you will also have to define an empty constructor.

If you don't decorate any nested type with this attribute, a type called <mark style="color: lightgray; background-color: #191a18">AutoParameterStorage_Generated</mark> will be automatically generated and
used as data object.

```csharp
[AutoParameters]
public partial class MyAnimation : TMPAnimation
{
    [AutoParametersStorage]
    private partial class Data
    {
        public System.Random rng;
        public Dictionary<int, float> someMapping;

        public Data()
        {
            rng = new System.Random();
            someMapping = new Dictionary<int, float>();
        }
    }
}
```

#### Hooks
You can hook into each of the generated methods.

- <mark style="color: lightgray; background-color: #191a18">bool ValidateParameters_Hook(IDictionary<string, string> parameters)</mark>  
Called at the very beginning of <mark style="color: lightgray; background-color: #191a18">ValidateParameters</mark>.  
Rest of validation code is only executed if your method returned <mark style="color: lightgray; background-color: #191a18">true</mark>.

- <mark style="color: lightgray; background-color: #191a18">void SetParameters_Hook(object customData, IDictionary<string, string> parameters)</mark>  
Called at the very end of <mark style="color: lightgray; background-color: #191a18">SetParameters</mark>.  
Is NOT called if passed in <mark style="color: lightgray; background-color: #191a18">parameters</mark> dictionary is null.

- <mark style="color: lightgray; background-color: #191a18">void GetNewCustomData_Hook(object customData)</mark>  
Called at the very end of <mark style="color: lightgray; background-color: #191a18">GetNewCustomData</mark>.  
Receives the custom data object fully populated with all <mark style="color: lightgray; background-color: #191a18">[AutoParameter]</mark>.

#### Full example
The above is all you need to know to use AutoParameters!  
This plugin removes all parameter-related boilerplate from your animations and allows you to focus on writing the actual animation logic.  
For example, below is the built-in wave animation, if it was written with AutoParameters:
```csharp
[AutoParameters]
public partial class MyAnimation : TMPAnimation
{
    [SerializeField, AutoParameterBundle("")] Wave wave = new Wave(AnimationCurveUtility.EaseInOutSine(), AnimationCurveUtility.EaseInOutSine(), 0.5f, 0.5f, 1f, 1f, 0.2f);
    [SerializeField, AutoParameter("waveoffset", "waveoff", "woff")] WaveOffsetType waveOffsetType = WaveOffsetType.XPos;

    public override void Animate(CharData cData, IAnimationContext context)
    {
        Data data = (Data)context.CustomData;

        // Evaluate the wave based on time and offset
        float eval = data.wave.Evaluate(context.AnimatorContext.PassedTime, GetWaveOffset(cData, context, data.waveOffsetType)).Item1;

        // Move the character up based on the wave evaluation
        cData.SetPosition(cData.InitialPosition + Vector3.up * eval);
    }

    [AutoParametersStorage]
    private partial class Data
    {    }
}
```