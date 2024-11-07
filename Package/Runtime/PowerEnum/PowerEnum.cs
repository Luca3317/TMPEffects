using System;
using TMPEffects.Parameters;
using UnityEngine;

/*
 * TODO
 * You should be able to assign MonoBehaviours and ScriptableObjects to PowerEnums in the inspector
 * (for MonoBehaviours, ScriptableObjects ofc cant take scene references)
 *
 * TODO
 * Doesnt necessarily have to work for PowerEnums in general, just OffsetType is fine
 */

[Serializable]
public abstract class PowerEnum<TEnum, TCustom> where TEnum : Enum where TCustom : UnityEngine.Object
{
    public TCustom Value => customValue;
    public TEnum EnumValue => enumValue;
    public bool UseCustom => useCustom;

    [SerializeField] private TEnum enumValue;
    [SerializeField] private TCustom customValue;
    [SerializeField] protected bool useCustom;

    public PowerEnum(TEnum enumValue, TCustom customValue, bool useCustom)
    {
        this.enumValue = enumValue;
        this.customValue = customValue;
        this.useCustom = useCustom;
    }

    public PowerEnum(TEnum enumValue, TCustom customValue)
    {
        this.enumValue = enumValue;
        this.customValue = customValue;
        useCustom = (this.customValue != null);
    }

    public PowerEnum(TEnum enumValue) : this()
    {
        this.enumValue = enumValue;
    }
    
    public PowerEnum()
    {
        customValue = null;
        useCustom = false;
    }
}