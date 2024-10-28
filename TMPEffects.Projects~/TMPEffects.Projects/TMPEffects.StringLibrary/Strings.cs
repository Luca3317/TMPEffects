using System;
using System.Collections.Generic;
using System.Linq;

namespace TMPEffects.StringLibrary
{
    public static class Strings
    {
        public const string DefaultStorageName = "AutoParametersData";


        #region AutoParametersAttribute

        public const string AutoParametersAttributeName =
            "TMPEffects.AutoParameters.Attributes.AutoParametersAttribute";

        public const string AutoParametersStorageAttributeName =
            "TMPEffects.AutoParameters.Attributes.AutoParametersStorageAttribute";

        public const string AutoParameterAttributeName =
            "TMPEffects.AutoParameters.Attributes.AutoParameterAttribute";

        public const string AutoParameterBundleAttributeName =
            "TMPEffects.AutoParameters.Attributes.AutoParameterBundleAttribute";

        #endregion


        #region Various TMPEffects namespace paths and names

        public const string AnimationUtilityPath = "TMPEffects.TMPAnimations.AnimationUtility";
        public const string ParameterUtilityPath = "TMPEffects.Parameters.ParameterUtility";
        public const string ParameterTypesPath = "TMPEffects.Parameters.ParameterTypes";
        public const string ParameterParsingPath = "TMPEffects.Parameters.ParameterParsing";

        public const string CharDataName = "TMPEffects.CharacterData.CharData";
        public const string IAnimationContextName = "TMPEffects.TMPAnimations.IAnimationContext";

        #endregion

        #region Supported Types

        #region Unity

        public const string SerializeFieldAttributeName = "UnityEngine.SerializeField";
        public const string Vector4Name = "UnityEngine.Vector4";
        public const string Vector3Name = "UnityEngine.Vector3";
        public const string Vector2Name = "UnityEngine.Vector2";
        public const string AnimationCurveName = "UnityEngine.AnimationCurve";
        public const string ColorName = "UnityEngine.Color";

        #endregion

        #region Custom

        public const string WaveOffsetTypeName = ParameterTypesPath + ".WaveOffsetType";
        public const string WaveName = AnimationUtilityPath + ".Wave";
        public const string TypedVector3Name = ParameterTypesPath + ".TypedVector3";
        public const string TypedVector2Name = ParameterTypesPath + ".TypedVector2";

        #endregion

        #endregion

        #region Validate, Set and StringTo methods

        public const string ParameterDefinedName = ParameterUtilityPath + ".ParameterDefined";
        public const string TryGetDefinedParameter = ParameterUtilityPath + ".TryGetDefinedParameter";
        public const string GetDefinedParameter = ParameterUtilityPath + ".GetDefinedParameter";
        public const string HasNonTypeParameterName = ParameterUtilityPath + ".HasNon{0}Parameter";
        public const string HasTypeParameterName = ParameterUtilityPath + ".Has{0}Parameter";
        public const string TryGetTypeParameterName = ParameterUtilityPath + ".TryGet{0}Parameter";
        public const string StringToTypeName = ParameterParsingPath + ".StringTo{0}";
        public const string ValidateWaveParameters = ParameterUtilityPath + ".ValidateWaveParameters";

        #endregion

        public static Dictionary<string, string> SupportedTypes =>
            SupportedTypesList.GroupBy(t => t.Item1)
                .Select(t => t.First())
                .ToDictionary(t => t.Item1, t => t.Item2);

        public static readonly List<(string, string)> SupportedTypesList = new List<(string, string)>()
        {
            ("float", "Float"),
            ("int", "Int"),
            ("bool", "Bool"),

            (WaveOffsetTypeName, "WaveOffset"),
            (TypedVector3Name, "TypedVector3"),
            (TypedVector2Name, "TypedVector2"),
            (Vector2Name, "Vector2"),
            (Vector3Name, "Vector3"),
            (Vector2Name, "Vector2Offset"),
            (Vector3Name, "Vector3Offset"),
            (Vector2Name, "Anchor"),
            (ColorName, "Color"),
            (AnimationCurveName, "AnimCurve"),
        };

        public static readonly List<string> ValidAutoParameterTypes = new List<string>()
        {
            "float",
            "int",
            "bool",
            "string",
            WaveOffsetTypeName,
            TypedVector3Name,
            TypedVector2Name,
            Vector3Name,
            Vector2Name,
            ColorName,
            AnimationCurveName,
        };

        public static readonly List<string> ValidAutoParameterBundleTypes = new List<string>()
        {
            WaveName
        };
    }
}