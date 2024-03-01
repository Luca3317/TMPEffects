using System.Collections;
using System.Collections.Generic;
using TMPEffects.TextProcessing;
using UnityEngine;

namespace TMPEffects
{
    public static class EffectUtility
    {
        public static bool ParameterDefined(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                GetDefinedParameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GetDefinedParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string ret = null;
            if (parameters.ContainsKey(name)) ret = name;
            if (aliases == null)
            {
                if (ret != null) return ret;
                throw new System.Exception("Parameter " + name + " not defined");
            }

            for (int i = 0; i < aliases.Length; i++)
            {
                if (parameters.ContainsKey(aliases[i]))
                {
                    if (ret != null) throw new System.Exception("Parameter " + name + " or its aliases defined multiple times");
                    else ret = aliases[i];
                }
            }

            if (ret == null) throw new System.Exception("Parameter " + name + " not defined, nor any of its aliases");
            return ret;
        }

        public static bool HasFloatParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetFloatParameter(out float _, parameters, name, aliases);
        }

        public static bool TryGetFloatParameter(out float value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetFloatParameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static float GetFloatParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string defined = GetDefinedParameter(parameters, name, aliases);

            if (ParsingUtility.StringToFloat(parameters[defined], out float value))
            {
                return value;
            }

            throw new System.Exception("Parameter " + defined + " is not a valid float");
        }


        public static bool HasIntParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetIntParameter(out int _, parameters, name, aliases);
        }

        public static bool TryGetIntParameter(out int value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetIntParameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static int GetIntParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string defined = GetDefinedParameter(parameters, name, aliases);

            if (ParsingUtility.StringToInt(parameters[defined], out int value))
            {
                return value;
            }

            throw new System.Exception("Parameter " + defined + " is not a valid int");
        }


        public static bool HasBoolParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetBoolParameter(out bool _, parameters, name, aliases);
        }

        public static bool TryGetBoolParameter(out bool value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetBoolParameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static bool GetBoolParameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string defined = GetDefinedParameter(parameters, name, aliases);

            if (ParsingUtility.StringToBool(parameters[defined], out bool value))
            {
                return value;
            }

            throw new System.Exception("Parameter " + defined + " is not a valid bool");
        }


        public static bool HasVector2Parameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetVector2Parameter(out Vector2 _, parameters, name, aliases);
        }

        public static bool TryGetVector2Parameter(out Vector2 value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetVector2Parameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static Vector2 GetVector2Parameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string defined = GetDefinedParameter(parameters, name, aliases);

            if (ParsingUtility.StringToVector2(parameters[defined], out Vector2 value))
            {
                return value;
            }

            throw new System.Exception("Parameter " + defined + " is not a valid Vector2");
        }


        public static bool HasVector3Parameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            return TryGetVector3Parameter(out Vector3 _, parameters, name, aliases);
        }

        public static bool TryGetVector3Parameter(out Vector3 value, IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            try
            {
                value = GetVector3Parameter(parameters, name, aliases);
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public static Vector3 GetVector3Parameter(IDictionary<string, string> parameters, string name, params string[] aliases)
        {
            string defined = GetDefinedParameter(parameters, name, aliases);

            if (ParsingUtility.StringToVector3(parameters[defined], out Vector3 value))
            {
                return value;
            }

            throw new System.Exception("Parameter " + defined + " is not a valid Vector3");
        }
    }
}