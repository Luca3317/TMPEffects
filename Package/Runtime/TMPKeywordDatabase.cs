using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.Parameters;
using TMPEffects.SerializedCollections;
using TMPEffects.TMPAnimations;
using UnityEngine;

public interface ITMPKeywordDatabase
{
    public IReadOnlyDictionary<string, Color> ColorKeywords { get; }
    public IReadOnlyDictionary<string, Vector3> Vector3Keywords { get; }
    public IReadOnlyDictionary<string, Vector2> Vector2Keywords { get; }
    public IReadOnlyDictionary<string, bool> BoolKeywords { get; }
    public IReadOnlyDictionary<string, float> FloatKeywords { get; }
    public IReadOnlyDictionary<string, int> IntKeywords { get; }
}

[CreateAssetMenu(fileName = "new KeywordDatabase", menuName = "TMPEffects/Database/Keywords")]
public class TMPKeywordDatabase : ScriptableObject, ITMPKeywordDatabase
{
    public IReadOnlyDictionary<string, Color> ColorKeywords => colorKeywords;
    public IReadOnlyDictionary<string, Vector3> Vector3Keywords => vector3Keywords;
    public IReadOnlyDictionary<string, Vector2> Vector2Keywords => vector2Keywords;
    public IReadOnlyDictionary<string, bool> BoolKeywords => boolKeywords;
    public IReadOnlyDictionary<string, float> FloatKeywords => floatKeywords;
    public IReadOnlyDictionary<string, int> IntKeywords => intKeywords;

    [SerializedDictionary(keyName: "Keyword", valueName: "Color")] [SerializeField]
    SerializedDictionary<string, Color> colorKeywords;

    [SerializedDictionary(keyName: "Keyword", valueName: "Vector3")] [SerializeField]
    SerializedDictionary<string, Vector3> vector3Keywords;

    [SerializedDictionary(keyName: "Keyword", valueName: "Vector2")] [SerializeField]
    SerializedDictionary<string, Vector2> vector2Keywords;

    [SerializedDictionary(keyName: "Keyword", valueName: "Float")] [SerializeField]
    SerializedDictionary<string, float> floatKeywords;

    [SerializedDictionary(keyName: "Keyword", valueName: "Int")] [SerializeField]
    SerializedDictionary<string, int> intKeywords;

    // Really pointless but here for completions sake
    [SerializedDictionary(keyName: "Keyword", valueName: "Bool")] [SerializeField]
    SerializedDictionary<string, bool> boolKeywords;

    [SerializedDictionary(keyName: "Keyword", valueName: "Curve")] [SerializeField]
    SerializedDictionary<string, AnimationCurve> animationCurveKeywords;

    [SerializedDictionary(keyName: "Keyword", valueName: "OffsetType")] [SerializeField]
    SerializedDictionary<string, OffsetTypePowerEnum> offsetTypeKeywords;
}