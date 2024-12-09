using System;
using System.Collections.Generic;
using TMPEffects.Databases;

namespace TMPEffects.Parameters
{
    /// <summary>
    /// Utility class for easy parameter handling.
    /// </summary>
    [GenerateParameterUtility]
    public static partial class TMPParameterUtility
    {
        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases.<br />
        /// A parameter is well-defined if there is exactly one of the given aliases (including the name) present in the parameters.
        /// </summary>
        /// <param name="value">Set to the name of the defined parameter if successful.</param>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if the parameter is well-defined, false otherwise.</returns>
        public static bool TryGetDefinedParameter(out string value, IDictionary<string, string> parameters, string name,
            params string[] aliases)
        {
            value = null;
            if (parameters.ContainsKey(name)) value = name;
            if (aliases == null)
            {
                return value != null;
            }

            for (int i = 0; i < aliases.Length; i++)
            {
                if (parameters.ContainsKey(aliases[i]))
                {
                    if (value != null) return false;
                    else value = aliases[i];
                }
            }

            return value != null;
        }

        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases.<br />
        /// A parameter is well-defined if there is exactly one of the given aliases (including the name) present in the parameters.
        /// </summary>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if the parameter is well-defined, false otherwise.</returns>
        public static bool ParameterDefined(IDictionary<string, string> parameters, string name,
            params string[] aliases)
        {
            return TryGetDefinedParameter(out _, parameters, name, aliases);
        }


        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is not of type Array&lt;T&gt; (=> can not be converted to Array&lt;T&gt;).
        /// </summary>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is not of type Array&lt;T&gt;, false otherwise.</returns>
        public static bool HasNonArrayParameter(IDictionary<string, string> parameters, string name,
            params string[] aliases)
        {
            if (!ParameterDefined(parameters, name, aliases)) return false;
            return !TryGetArrayParameter(out string[] _, parameters, name, aliases);
        }

        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is of type Array&lt;T&gt; (=> can no be converted to Array&lt;T&gt;).
        /// </summary>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is of type Array&lt;T&gt;, false otherwise.</returns>
        public static bool HasArrayParameter(IDictionary<string, string> parameters, string name,
            params string[] aliases)
        {
            return TryGetArrayParameter(out string[] _, parameters, name, aliases);
        }

        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is of type Array&lt;T&gt; (=> can be converted to Array&lt;T&gt;).<br />
        /// </summary>
        /// <param name="value">Set to the value of the parameter if successful.</param>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is of type Array&lt;T&gt;, false otherwise.</returns>
        public static bool TryGetArrayParameter(out string[] value, IDictionary<string, string> parameters, string name,
            params string[] aliases)
        {
            return TryGetArrayParameter(out value, parameters, stringParseDelegate, null, name, aliases);
        }

        private static ParseDelegate<string, string, ITMPKeywordDatabase, bool> stringParseDelegate =
            (string a, out string b, ITMPKeywordDatabase keywordDatabase) =>
            {
                b = a;
                return true;
            };

        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is not of type Array&lt;T&gt; (=> can not be converted to Array&lt;T&gt;).
        /// </summary>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is not of type Array&lt;T&gt;, false otherwise.</returns>
        public static bool HasNonArrayParameter<T>(IDictionary<string, string> parameters,
            ParseDelegate<string, T, ITMPKeywordDatabase, bool> func, string name, params string[] aliases)
        {
            return HasNonArrayParameter(parameters, func, null, name, aliases);
        }

        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is not of type Array&lt;T&gt; (=> can not be converted to Array&lt;T&gt;).
        /// </summary>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="func">The delegate used to parse the array items.</param>
        /// <param name="keywords">The keyword database to use.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is not of type Array&lt;T&gt;, false otherwise.</returns>
        public static bool HasNonArrayParameter<T>(IDictionary<string, string> parameters,
            ParseDelegate<string, T, ITMPKeywordDatabase, bool> func, ITMPKeywordDatabase keywords, string name,
            params string[] aliases)
        {
            if (!ParameterDefined(parameters, name, aliases)) return false;
            return !TryGetArrayParameter(out T[] _, parameters, func, keywords, name, aliases);
        }

        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is of type Array&lt;T&gt; (=> can no be converted to Array&lt;T&gt;).
        /// </summary>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="func">The delegate used to parse the array items.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is of type Array&lt;T&gt;, false otherwise.</returns>
        public static bool HasArrayParameter<T>(IDictionary<string, string> parameters,
            ParseDelegate<string, T, ITMPKeywordDatabase, bool> func, string name, params string[] aliases)
        {
            return HasArrayParameter(parameters, func, null, name, aliases);
        }

        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is of type Array&lt;T&gt; (=> can no be converted to Array&lt;T&gt;).
        /// </summary>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="func">The delegate used to parse the array items.</param>
        /// <param name="keywords">The keyword database to use.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is of type Array&lt;T&gt;, false otherwise.</returns>
        public static bool HasArrayParameter<T>(IDictionary<string, string> parameters,
            ParseDelegate<string, T, ITMPKeywordDatabase, bool> func, ITMPKeywordDatabase keywords, string name,
            params string[] aliases)
        {
            return TryGetArrayParameter(out T[] _, parameters, func, name, aliases);
        }

        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is of type Array&lt;T&gt; (=> can be converted to Array&lt;T&gt;).<br />
        /// </summary>
        /// <param name="value">Set to the value of the parameter if successful.</param>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="func">The delegate used to parse the array items.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is of type Array&lt;T&gt;, false otherwise.</returns>
        public static bool TryGetArrayParameter<T>(out T[] value, IDictionary<string, string> parameters,
            ParseDelegate<string, T, ITMPKeywordDatabase, bool> func, string name, params string[] aliases)
        {
            return TryGetArrayParameter(out value, parameters, func, null, name, aliases);
        }

        /// <summary>
        /// Check if there is a well-defined parameter of the given name or aliases that is of type Array&lt;T&gt; (=> can be converted to Array&lt;T&gt;).<br />
        /// </summary>
        /// <param name="value">Set to the value of the parameter if successful.</param>
        /// <param name="parameters">The parameters to check.</param>
        /// <param name="func">The delegate used to parse the array items.</param>
        /// <param name="keywords">The keyword database to use.</param>
        /// <param name="name">The name to check.</param>
        /// <param name="aliases">The aliases (alternative names) to check.</param>
        /// <returns>true if there is a well-defined parameter that is of type Array&lt;T&gt;, false otherwise.</returns>
        public static bool TryGetArrayParameter<T>(out T[] value, IDictionary<string, string> parameters,
            ParseDelegate<string, T, ITMPKeywordDatabase, bool> func, ITMPKeywordDatabase keywords, string name,
            params string[] aliases)
        {
            value = null;
            if (!TryGetDefinedParameter(out string parameterName, parameters, name, aliases)) return false;

            string[] contents = parameters[parameterName].Split(";");

            List<T> result = new List<T>();

            for (int i = 0; i < contents.Length; i++)
            {
                string contentValue = contents[i];

                if (!func(contentValue, out T t, keywords))
                {
                    return false;
                }

                result.Add(t);
            }

            value = result.ToArray();
            return true;
        }

        /// <summary>
        /// Delegate used in <see cref="TMPParameterUtility.HasArrayParameter"/> and related methods to parse array items.<br/>
        /// Generally, you can use the methods defined in <see cref="ParameterParsing"/> for this.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="W"></typeparam>
        public delegate W ParseDelegate<T, U, V, W>(T input, out U output, V keywords);
    }

    internal class GenerateParameterUtilityAttribute : Attribute
    {
    }
}