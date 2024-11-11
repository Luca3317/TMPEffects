﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace TMPEffects.StringLibrary
{
    public static class Strings
    {
        public const string DefaultStorageName = "AutoParametersData";

        public const string IAnimatorContextKeywordDatabasePath = ".KeywordDatabase";
        public const string IAnimationContextKeywordDatabasePath = ".AnimatorContext.KeywordDatabase";

        public const string IWriterContexKeywordDatabasePath = ".KeywordDatabase";
        public const string ICommandContexKeywordDatabasePath = ".WriterContext.KeywordDatabase";

        public const string ITMPAnimationName = "TMPEffects.TMPAnimations.ITMPAnimation";
        public const string ITMPCommandName = "TMPEffects.TMPCommands.ITMPCommand";

        public const string IDictionaryName = "System.Collections.Generic.IDictionary<string, string>";

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
        public const string IAnimatorContextName = "TMPEffects.Components.Animator.IAnimatorContext";

        public const string ICommandContextName = "TMPEffects.TMPCommands.ICommandContext";
        public const string IWriterContextName = "TMPEffects.Components.Writer.IWriterContext";

        #endregion

        #region Supported Types

        #region Unity

        public const string SerializeFieldAttributeName = "UnityEngine.SerializeField";
        public const string Vector4Name = "UnityEngine.Vector4";
        public const string Vector3Name = "UnityEngine.Vector3";
        public const string Vector2Name = "UnityEngine.Vector2";
        public const string AnimationCurveName = "UnityEngine.AnimationCurve";
        public const string ColorName = "UnityEngine.Color";
        public const string UnityObjectName = "UnityEngine.Object";

        #endregion

        #region Custom

        public const string ITMPOffsetProviderName = ParameterTypesPath + ".ITMPOffsetProvider";

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

        public const string GenerateParametersAttributeName =
            "TMPEffects.ParameterUtilityGenerator.Attributes.GenerateParameterTypeAttribute";       
        public const string TMPParameterTypeName =
            "TMPEffects.ParameterUtilityGenerator.Attributes.TMPParameterTypeAttribute";

        public static Dictionary<string, string> TypeToDisplayString =>
            SupportedTypesList.GroupBy(t => t.Item1)
                .Select(t => t.First())
                .ToDictionary(t => t.Item1, t => t.Item2);

        public static string GetClosestTypeFit(ITypeSymbol type)
        {
            if (type.SpecialType == SpecialType.System_String)
            {
                return "string";
            }

            if (type.SpecialType == SpecialType.System_Array)
            {
                return type.ToDisplayString();
            }

            var dict = TypeToDisplayString;
            if (dict.TryGetValue(type.ToDisplayString(), out var typeName))
            {
                return type.ToDisplayString();
            }

            if (ValidAutoParameterBundleTypes.Contains(type.ToDisplayString()))
            {
                return type.ToDisplayString();
            }

            foreach (var interf in type.AllInterfaces)
            {
                if (ValidAutoParameterBaseTypes.Contains(interf.ToDisplayString()))
                {
                    return interf.ToDisplayString();
                }
            }

            var curr = type.BaseType;
            while (curr != null)
            {
                if (ValidAutoParameterBaseTypes.Contains(curr.ToDisplayString()))
                {
                    return curr.ToDisplayString();
                }

                curr = curr.BaseType;
            }

            return null;
        }

        public static bool TypeToStringForStorage(ITypeSymbol type, out string displayString)
        {
            if (type.SpecialType == SpecialType.System_String)
            {
                displayString = "string";
                return true;
            }

            if (type.TypeKind == TypeKind.Array)
            {
                displayString = (type as IArrayTypeSymbol).ElementType.ToDisplayString() + "[]";
                return true;
            }

            var dict = TypeToDisplayString;
            if (dict.TryGetValue(type.ToDisplayString(), out displayString))
            {
                displayString = type.ToDisplayString();
                return true;
            }

            if (ValidAutoParameterBundleTypes.Contains(type.ToDisplayString()))
            {
                displayString = type.ToDisplayString();
                return true;
            }

            foreach (var interf in type.AllInterfaces)
            {
                if (ValidAutoParameterBaseTypes.Contains(interf.ToDisplayString()))
                {
                    displayString = interf.ToDisplayString();
                    return true;
                }
            }

            var curr = type.BaseType;
            while (curr != null)
            {
                if (ValidAutoParameterBaseTypes.Contains(curr.ToDisplayString()))
                {
                    displayString = curr.ToDisplayString();
                    return true;
                }

                curr = curr.BaseType;
            }

            return false;
        }

        public static bool TypeStringToDisplayString(ITypeSymbol type, out string displayString)
        {
            var dict = TypeToDisplayString;
            if (dict.TryGetValue(type.ToDisplayString(), out displayString))
                return true;

            foreach (var interf in type.AllInterfaces)
            {
                if (dict.TryGetValue(interf.ToDisplayString(), out displayString))
                    return true;
            }

            var curr = type.BaseType;
            while (curr != null)
            {
                if (dict.TryGetValue(curr.ToDisplayString(), out displayString))
                    return true;
                curr = curr.BaseType;
            }

            return false;
        }


        public static Dictionary<string, string> DisplayStringToType =>
            SupportedTypesList.GroupBy(t => t.Item2)
                .Select(t => t.First())
                .ToDictionary(t => t.Item2, t => t.Item1);

        
        
        
        public static bool TryGetClosestFitAutoParameterType(IFieldSymbol field, out ITypeSymbol closestType)
        {
            var tmp = field.Type;
            if (field.Type is IArrayTypeSymbol array)
                tmp = array.ElementType;
            
            if (ValidAutoParameterBundleTypes.Contains(tmp.ToDisplayString()))
            {
                closestType = tmp;
                return true;
            }

            if (ValidAutoParameterTypes.Contains(tmp.ToDisplayString()))
            {
                closestType = tmp;
                return true;
            }
            
            var curr = tmp.BaseType;
            while (curr != null)
            {
                if (ValidAutoParameterBundleTypes.Contains(curr.ToDisplayString()))
                {
                    closestType = curr;
                    return true;
                }
                
                if (ValidAutoParameterTypes.Contains(curr.ToDisplayString()))
                {
                    closestType = curr;
                    return true;
                }
                
                curr = curr.BaseType;
            }

            foreach (var interf in tmp.AllInterfaces)
            {
                if (ValidAutoParameterBundleTypes.Contains(tmp.ToDisplayString()))
                {
                    closestType = tmp;
                    return true;
                }
                
                if (ValidAutoParameterTypes.Contains(interf.ToDisplayString()))
                {
                    closestType = interf;
                    return true;
                }
            }

            closestType = null;
            return false;
        }
        
        
        public static bool IsValidAutoParameterType(ITypeSymbol type)
        {
            if (type is IArrayTypeSymbol arr)
            {
                type = arr.ElementType;
            }
            
            if (ValidAutoParameterTypes.Contains(type.ToDisplayString()))
                return true;

            foreach (var interf in type.AllInterfaces)
            {
                if (ValidAutoParameterBaseTypes.Contains(interf.ToDisplayString()))
                    return true;
            }

            var curr = type;
            while (curr != null)
            {
                if (ValidAutoParameterBaseTypes.Contains(curr.ToDisplayString()))
                    return true;
                curr = curr.BaseType;
            }

            return false;
        }
        
        public static AttributeData AutoParameterDecorations(ISymbol symbol)
        {
            AutoParametersDecoration decorations = 0;
            var attributes = symbol.GetAttributes();
            foreach (var attribute in attributes)
            {
                var attr = attribute.AttributeClass;
                if (attr == null) continue;
                var attrString = attr.ToDisplayString();

                switch (attrString)
                {
                    case AutoParameterAttributeName:
                        decorations |= AutoParametersDecoration.AutoParameter;
                        break;
                    case AutoParameterBundleAttributeName:
                        decorations |= AutoParametersDecoration.AutoParameterBundle;
                        break;
                }
            }

            return null;
        }
        
        public struct AutoParameterInfo
        {
            public ITypeSymbol TypeSymbol;
            
            public string TypeString;
            public string NameString;

            public bool specifiesRequirement;
            public bool required;

            public string[] Aliases;
        }

        public struct AutoParameterBundleInfo
        {
            public ITypeSymbol TypeSymbol;
            
            public string TypeString;
            public string NameString;

            public string prefix;
        }
        
        [Flags]
        public enum AutoParametersDecoration : byte
        {
            None = 0,
            AutoParameter = 1,
            AutoParameterBundle = 1 << 1,
            All = byte.MaxValue
            
            // TODO Put other stuff in here to do to be more universally reusable?
            // AutoParameterStorage = 1 << 2,
            // AutoParameters = 1 << 3,
        }

        public static bool IsValidAutoParameterBundleType(ITypeSymbol type)
        {
            return ValidAutoParameterBundleTypes.Contains(type.ToDisplayString());
        }


        // TypedVector3/2 should use Vector3/2 keywords, this is a quick workaround for
        // how ParameterUtilityGenerator decided keywordtype
        // Changed tvectors to be handled in special case now; still keeping this for vec2
        public static string ReturnTypeToKeywordType(string type)
        {
            if (type == "Vector2Offset") return Vector3Name;
            if (type == "Vector2") return Vector3Name;

            return DisplayStringToType[type];
        }

        public static readonly List<(string, string)> SupportedTypesList = new List<(string, string)>()
        {
            ("float", "Float"),
            ("int", "Int"),
            ("bool", "Bool"),

            (ITMPOffsetProviderName, "OffsetProvider"),
            (UnityObjectName, "UnityObject"),

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

        // Used to check whether a field is a valid auto parameter (and nothing more)
        private static readonly List<string> ValidAutoParameterTypes = new List<string>()
        {
            "float",
            "int",
            "bool",
            "string",
            TypedVector3Name,
            TypedVector2Name,
            Vector3Name,
            Vector2Name,
            ColorName,
            AnimationCurveName,
        };

        private static readonly List<string> ValidAutoParameterBaseTypes = new List<string>()
        {
            ITMPOffsetProviderName,
            UnityObjectName
        };

        private static readonly List<string> ValidAutoParameterBundleTypes = new List<string>()
        {
            WaveName
        };
    }
}