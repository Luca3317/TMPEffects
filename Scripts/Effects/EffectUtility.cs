using System.Collections;
using System.Collections.Generic;
using TMPEffects.TextProcessing;
using UnityEngine;

namespace TMPEffects
{
    public static class EffectUtility
    {
        public static bool HasFloatParameter(string name, IDictionary<string, string> parameters)
            => TryGetFloatParameter(name, parameters, out _);

        public static bool TryGetFloatParameter(string name, IDictionary<string, string> parameters, out float parameterValue)
        {
            parameterValue = default;
            if (parameters == null) return false;
            if (!parameters.ContainsKey(name)) return false;
            return ParsingUtility.StringToFloat(name, out parameterValue);
        }

        public static bool HasIntegerParameter(string name, IDictionary<string, string> parameters)
            => TryGetIntegerParameter(name, parameters, out _);

        public static bool TryGetIntegerParameter(string name, IDictionary<string, string> parameters, out int parameterValue)
        {
            parameterValue = default;
            if (parameters == null) return false;
            if (!parameters.ContainsKey(name)) return false;
            return ParsingUtility.StringToInt(name, out parameterValue);
        }

        public static bool HasBooleanParameter(string name, IDictionary<string, string> parameters)
            => TryGetBooleanParameter(name, parameters, out _);

        public static bool TryGetBooleanParameter(string name, IDictionary<string, string> parameters, out bool parameterValue)
        {
            parameterValue = default;
            if (parameters == null) return false;
            if (!parameters.ContainsKey(name)) return false;
            return ParsingUtility.StringToBool(name, out parameterValue);
        }

        public static bool HasVector2Parameter(string name, IDictionary<string, string> parameters)
            => TryGetVector2Parameter(name, parameters, out _);

        public static bool TryGetVector2Parameter(string name, IDictionary<string, string> parameters, out Vector2 parameterValue)
        {
            parameterValue = default;
            if (parameters == null) return false;
            if (!parameters.ContainsKey(name)) return false;
            return ParsingUtility.StringToVector2(name, out parameterValue);
        }

        public static bool HasVector3Parameter(string name, IDictionary<string, string> parameters)
            => TryGetVector3Parameter(name, parameters, out _);

        public static bool TryGetVector3Parameter(string name, IDictionary<string, string> parameters, out Vector3 parameterValue)
        {
            parameterValue = default;
            if (parameters == null) return false;
            if (!parameters.ContainsKey(name)) return false;
            return ParsingUtility.StringToVector3(name, out parameterValue);
        }
    }
}