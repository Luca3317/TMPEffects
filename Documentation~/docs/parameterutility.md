# ParameterUtility
<mark style="color: lightgray; background-color: #191a18">ParameterUtility</mark> is a static utility class for parameter validation and parsing, to be used with all types of animations and commands.  
The full API docs can be found [here](../api/TMPEffects.ParameterUtility.yml).

For each of the supported parameter types listed in the previous section, there is a <mark style="color: lightgray; background-color: #191a18">HasXYZParameter</mark> method, a <mark style="color: lightgray; background-color: #191a18">HasNonXYZParameter</mark> method, a
<mark style="color: lightgray; background-color: #191a18">GetXYZParameter</mark> and a <mark style="color: lightgray; background-color: #191a18">TryGetXZYParameter</mark> method.
Each of these methods are explained individually below.

### HasParameter
<mark style="color: lightgray; background-color: #191a18">bool HasXYZParameter(IDictionary&lt;string, string&gt;, string name, params string[] aliases)</mark> 
checks whether the given set of parameters contains a parameter of the given name, or any of the aliases, that is of type <mark style="color: lightgray; background-color: #191a18">XYZ</mark>.  

Example: <mark style="color: lightgray; background-color: #191a18">HasFloatParameter(parameters, "duration", "dur", "d")</mark> returns true if <mark style="color: lightgray; background-color: #191a18">parameters</mark>
contains a parameter named either "duration", "dur" or "d", and the value could be converted to type float. Otherwise, it returns false.

### HasNonParameter
<mark style="color: lightgray; background-color: #191a18">bool HasNonXYZParameter(IDictionary&lt;string, string&gt;, string name, params string[] aliases)</mark> 
checks whether the given set of parameters contains a parameter of the given name, or any of the aliases, that is NOT of type <mark style="color: lightgray; background-color: #191a18">XYZ</mark>.  

Example: <mark style="color: lightgray; background-color: #191a18">HasNonFloatParameter(parameters, "duration", "dur", "d")</mark> returns true exactly when <mark style="color: lightgray; background-color: #191a18">parameters</mark>
contains a parameter named either "duration", "dur" or "d", but the value could NOT be converted to type float. Otherwise, it returns false.

### GetParameter
<mark style="color: lightgray; background-color: #191a18">XYZ GetXYZParameter(IDictionary&lt;string, string&gt;, string name, params string[] aliases)</mark> 
returns the parameter defined by the given name, or any of the aliases, converted to type <mark style="color: lightgray; background-color: #191a18">XYZ</mark>. Otherwise, it will throw an exception.  

Example: <mark style="color: lightgray; background-color: #191a18">GetFloatParameter(parameters, "duration", "dur", "d")</mark> throws an exception if <mark style="color: lightgray; background-color: #191a18">parameters</mark>
does not contain a parameter named either "duration", "dur" or "d", or, if the parameter is defined, it could not be converted to type float. Otherwise, it will return the parameter converted to type float.

### TryGetParameter
<mark style="color: lightgray; background-color: #191a18">bool TryGetXYZParameter(out XYZ value, IDictionary&lt;string, string&gt;, string name, params string[] aliases)</mark>
wraps <mark style="color: lightgray; background-color: #191a18">GetParameter</mark> in a try-catch statement, returning true if it was successful, otherwise it returns false.
If successful, you can get the value of the converted parameter from the <mark style="color: lightgray; background-color: #191a18">out XYZ value</mark> parameter.  

Example: <mark style="color: lightgray; background-color: #191a18">TryGetFloatParameter(out float value, parameters, "duration", "dur", "d")</mark> returns false if <mark style="color: lightgray; background-color: #191a18">parameters</mark>
does not contain a parameter named either "duration", "dur" or "d", or, if the parameter is defined, it could not be converted to type float. Otherwise, it will return true and <mark style="color: lightgray; background-color: #191a18">value</mark>
will be set to the parameter converted to type float.


### Array
For array parameters, each of these four methods have an additional required parameter: <mark style="color: lightgray; background-color: #191a18">ParseDelegate&lt;string, T, IDictionary&lt;string, T&gt;</mark>,  
where <mark style="color: lightgray; background-color: #191a18">ParseDelegate</mark> is defined as: <mark style="color: lightgray; background-color: #191a18">public delegate W ParseDelegate<T, U, V, W>(T input, out U output, V keywords)</mark>.

Essentially, this delegate is used to parse the individual elements of the array.  
You can use the <mark style="color: lightgray; background-color: #191a18">ParsingUtility.StringToXYZ</mark> methods for this (they are not further explained here, but you can look at the [API docs for them](../api/TMPEffects.TextProcessing.ParsingUtility.yml)).  

Example: <mark style="color: lightgray; background-color: #191a18">TryGetArrayParameter&lt;float&gt;(out float[] value, parameters, ParsingUtility.StringToFloat, "numbers", "nums")</mark>

### ParameterDefined
In addition to these type-specific methods, there are also generic methods for checking whether a parameter is defined, without performing any type checks.  
These are:  
<mark style="color: lightgray; background-color: #191a18">bool ParameterDefined(IDictionary&lt;string, string&gt;, string name, params string[] aliases)</mark>:
Checks whether a parameter of the given name or any of its aliases is present in the dictionary. EXACTLY one must be defined for this to return true;
if for example two aliases are present it is considered not defined.

<mark style="color: lightgray; background-color: #191a18">string GetDefinedParameter(IDictionary&lt;string, string&gt;, string name, params string[] aliases)</mark>:
If the parameter is defined according to <mark style="color: lightgray; background-color: #191a18">ParameterDefined</mark>, this will return the value of that parameter.
Otherwise, it will throw an exception.

<mark style="color: lightgray; background-color: #191a18">bool TryGetDefinedParameter(out string value, IDictionary&lt;string, string&gt;, string name, params string[] aliases)</mark>:
Wraps <mark style="color: lightgray; background-color: #191a18">GetDefinedParameter</mark> in a try-catch statement.
If successful, the parameter value will be stored in the <mark style="color: lightgray; background-color: #191a18">out string value</mark> parameter.

### Waves
If your animation uses [Waves](tmpanimator_animationutility_wave.md), you can use the pre-defined wave parameters set by using <mark style="color: lightgray; background-color: #191a18">ValidateWaveParameters(IDictionary&lt;string, string&gt; parameters, string prefix = "")</mark>
and <mark style="color: lightgray; background-color: #191a18">GetWaveParameters(IDictionary&lt;string, string&gt; parameters, string prefix = "")</mark> in <mark style="color: lightgray; background-color: #191a18">ValidateParameters</mark> and <mark style="color: lightgray; background-color: #191a18">SetParameters</mark> respectively. The passed in <mark style="color: lightgray; background-color: #191a18">prefix</mark> lets you use multiple waves with differently prefixed parameter names.

If you have a default wave, you can combine it with the set parameters like this:
```
[SerializeField] Wave wave;

public void SetParameters(object customData, IDictionary&lt;string, string&gt; parameters)
{
    Data d = customData as Data; // Cast custom data to whatever type it is
    d.Wave = CreateWave(wave, GetWaveParameters(parameters));
}
```
This will create a new wave from the set parameters and the values defined in the default wave as fallback values for the non-set parameters.

For the full list of parameters that are part of the wave parameters set, see the [API documentation](../api/TMPEffects.ParameterUtility.yml).