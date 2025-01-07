<link rel="stylesheet" type="text/css" href="../styles.css">

# ParameterUtility
<mark class="markstyle">ParameterUtility</mark> is a static utility class for parameter validation and parsing, to be used with all types of animations and commands.  
The full API docs can be found [here](../api/TMPEffects.ParameterUtility.yml).

For each of the supported parameter types listed in the previous section, there is a <mark class="markstyle">HasXYZParameter</mark> method, a <mark class="markstyle">HasNonXYZParameter</mark> method, a
<mark class="markstyle">GetXYZParameter</mark> and a <mark class="markstyle">TryGetXZYParameter</mark> method.
Each of these methods are explained individually below.

### HasParameter
<mark class="markstyle">bool HasXYZParameter(IDictionary&lt;string, string&gt;, string name, params string[] aliases)</mark> 
checks whether the given set of parameters contains a parameter of the given name, or any of the aliases, that is of type <mark class="markstyle">XYZ</mark>.  

Example: <mark class="markstyle">HasFloatParameter(parameters, "duration", "dur", "d")</mark> returns true if <mark class="markstyle">parameters</mark>
contains a parameter named either "duration", "dur" or "d", and the value could be converted to type float. Otherwise, it returns false.

### HasNonParameter
<mark class="markstyle">bool HasNonXYZParameter(IDictionary&lt;string, string&gt;, string name, params string[] aliases)</mark> 
checks whether the given set of parameters contains a parameter of the given name, or any of the aliases, that is NOT of type <mark class="markstyle">XYZ</mark>.  

Example: <mark class="markstyle">HasNonFloatParameter(parameters, "duration", "dur", "d")</mark> returns true exactly when <mark class="markstyle">parameters</mark>
contains a parameter named either "duration", "dur" or "d", but the value could NOT be converted to type float. Otherwise, it returns false.

### GetParameter
<mark class="markstyle">XYZ GetXYZParameter(IDictionary&lt;string, string&gt;, string name, params string[] aliases)</mark> 
returns the parameter defined by the given name, or any of the aliases, converted to type <mark class="markstyle">XYZ</mark>. Otherwise, it will throw an exception.  

Example: <mark class="markstyle">GetFloatParameter(parameters, "duration", "dur", "d")</mark> throws an exception if <mark class="markstyle">parameters</mark>
does not contain a parameter named either "duration", "dur" or "d", or, if the parameter is defined, it could not be converted to type float. Otherwise, it will return the parameter converted to type float.

### TryGetParameter
<mark class="markstyle">bool TryGetXYZParameter(out XYZ value, IDictionary&lt;string, string&gt;, string name, params string[] aliases)</mark>
wraps <mark class="markstyle">GetParameter</mark> in a try-catch statement, returning true if it was successful, otherwise it returns false.
If successful, you can get the value of the converted parameter from the <mark class="markstyle">out XYZ value</mark> parameter.  

Example: <mark class="markstyle">TryGetFloatParameter(out float value, parameters, "duration", "dur", "d")</mark> returns false if <mark class="markstyle">parameters</mark>
does not contain a parameter named either "duration", "dur" or "d", or, if the parameter is defined, it could not be converted to type float. Otherwise, it will return true and <mark class="markstyle">value</mark>
will be set to the parameter converted to type float.


### Array
For array parameters, each of these four methods have an additional required parameter: <mark class="markstyle">ParseDelegate&lt;string, T, IDictionary&lt;string, T&gt;</mark>,  
where <mark class="markstyle">ParseDelegate</mark> is defined as: <mark class="markstyle">public delegate W ParseDelegate<T, U, V, W>(T input, out U output, V keywords)</mark>.

Essentially, this delegate is used to parse the individual elements of the array.  
You can use the <mark class="markstyle">ParsingUtility.StringToXYZ</mark> methods for this (they are not further explained here, but you can look at the [API docs for them](../api/TMPEffects.TextProcessing.ParsingUtility.yml)).  

Example: <mark class="markstyle">TryGetArrayParameter&lt;float&gt;(out float[] value, parameters, ParsingUtility.StringToFloat, "numbers", "nums")</mark>

### ParameterDefined
In addition to these type-specific methods, there are also generic methods for checking whether a parameter is defined, without performing any type checks.  
These are:  
<mark class="markstyle">bool ParameterDefined(IDictionary&lt;string, string&gt;, string name, params string[] aliases)</mark>:
Checks whether a parameter of the given name or any of its aliases is present in the dictionary. EXACTLY one must be defined for this to return true;
if for example two aliases are present it is considered not defined.

<mark class="markstyle">string GetDefinedParameter(IDictionary&lt;string, string&gt;, string name, params string[] aliases)</mark>:
If the parameter is defined according to <mark class="markstyle">ParameterDefined</mark>, this will return the value of that parameter.
Otherwise, it will throw an exception.

<mark class="markstyle">bool TryGetDefinedParameter(out string value, IDictionary&lt;string, string&gt;, string name, params string[] aliases)</mark>:
Wraps <mark class="markstyle">GetDefinedParameter</mark> in a try-catch statement.
If successful, the parameter value will be stored in the <mark class="markstyle">out string value</mark> parameter.