using System;
using UnityEngine;

[Serializable]
public abstract class PowerEnum<TEnum, TCustom> where TEnum : Enum
{
    public TCustom Value => customValue;
    public TEnum EnumValue => enumValue;
    
    [SerializeField] private TEnum enumValue;
    [SerializeField] private TCustom customValue;
    [SerializeField] protected bool useCustom;
}