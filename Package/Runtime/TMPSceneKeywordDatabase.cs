using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPEffects.SerializedCollections;
using UnityEngine;

public class TMPSceneKeywordDatabase : MonoBehaviour, ITMPKeywordDatabase
{
    public ReadOnlyDictionary<string, float> FloatKeywords
    {
        get
        {
            if (floatKeywordsRo == null)
                floatKeywordsRo = new ReadOnlyDictionary<string, float>(floatKeywords);
            
            return floatKeywordsRo;
        }
    }
    public ReadOnlyDictionary<string, int> IntKeywords
    {
        get
        {
            if (intKeywordsRo == null)
                intKeywordsRo = new ReadOnlyDictionary<string, int>(intKeywords);
            
            return intKeywordsRo;
        }
    }
    public ReadOnlyDictionary<string, bool> BoolKeywords
    {
        get
        {
            if (boolKeywordsRo == null)
                boolKeywordsRo = new ReadOnlyDictionary<string, bool>(boolKeywords);
            
            return boolKeywordsRo;
        }
    }
    public ReadOnlyDictionary<string, Color> ColorKeywords
    {
        get
        {
            if (colorKeywordsRo == null)
                colorKeywordsRo = new ReadOnlyDictionary<string, Color>(colorKeywords);
            
            return colorKeywordsRo;
        }
    }
    public ReadOnlyDictionary<string, Vector3> Vector3Keywords     
    {
        get
        {
            if (vectorKeywordsRo == null)
                vectorKeywordsRo = new ReadOnlyDictionary<string, Vector3>(vector3Keywords);
            
            return vectorKeywordsRo;
        }
    }
    public ReadOnlyDictionary<string, Vector2> AnchorKeywords     
    {
        get
        {
            if (anchorKeywordsRo == null)
                anchorKeywordsRo = new ReadOnlyDictionary<string, Vector2>(anchorKeywords);
            
            return anchorKeywordsRo;
        }
    }
    public ReadOnlyDictionary<string, AnimationCurve> AnimationCurveKeywords     
    {
        get
        {
            if (animKeywordsRo == null)
                animKeywordsRo = new ReadOnlyDictionary<string, AnimationCurve>(animationCurveKeywords);
            
            return animKeywordsRo;
        }
    }
    public ReadOnlyDictionary<string, OffsetTypePowerEnum> OffsetTypeKeywords     
    {
        get
        {
            if (offsetTypeKeywordsRo == null)
                offsetTypeKeywordsRo = new ReadOnlyDictionary<string, OffsetTypePowerEnum>(offsetTypeKeywords);
            
            return offsetTypeKeywordsRo;
        }
    }

    [System.NonSerialized] private ReadOnlyDictionary<string, float> floatKeywordsRo = null;
    [System.NonSerialized] private ReadOnlyDictionary<string, int> intKeywordsRo = null;
    [System.NonSerialized] private ReadOnlyDictionary<string, bool> boolKeywordsRo = null;
    [System.NonSerialized] private ReadOnlyDictionary<string, Color> colorKeywordsRo = null;
    [System.NonSerialized] private ReadOnlyDictionary<string, Vector3> vectorKeywordsRo = null;
    [System.NonSerialized] private ReadOnlyDictionary<string, Vector2> anchorKeywordsRo = null;
    [System.NonSerialized] private ReadOnlyDictionary<string, AnimationCurve> animKeywordsRo = null;
    [System.NonSerialized] private ReadOnlyDictionary<string, OffsetTypePowerEnum> offsetTypeKeywordsRo = null;

    [SerializedDictionary(keyName: "Keyword", valueName: "Float")] [SerializeField]
    SerializedDictionary<string, float> floatKeywords;

    [SerializedDictionary(keyName: "Keyword", valueName: "Int")] [SerializeField]
    SerializedDictionary<string, int> intKeywords;

    // Really pointless but here for completions sake
    [SerializedDictionary(keyName: "Keyword", valueName: "Bool")] [SerializeField]
    SerializedDictionary<string, bool> boolKeywords;

    [SerializedDictionary(keyName: "Keyword", valueName: "Color")] [SerializeField]
    SerializedDictionary<string, Color> colorKeywords;
    
    [SerializedDictionary(keyName: "Keyword", valueName: "Vector3")] [SerializeField]
    SerializedDictionary<string, Vector3> vector3Keywords;
    
    [SerializedDictionary(keyName: "Keyword", valueName: "Anchor")] [SerializeField]
    SerializedDictionary<string, Vector2> anchorKeywords;
    
    [SerializedDictionary(keyName: "Keyword", valueName: "Curve")] [SerializeField]
    SerializedDictionary<string, AnimationCurve> animationCurveKeywords;

    [SerializedDictionary(keyName: "Keyword", valueName: "OffsetType")] [SerializeField]
    SerializedDictionary<string, OffsetTypePowerEnum> offsetTypeKeywords;
}
