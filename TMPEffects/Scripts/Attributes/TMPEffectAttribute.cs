using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TMPEffectAttribute : Attribute
{
    public string Tag => tag;
    private string tag;
    public TMPEffectAttribute(string tag)
    {
        this.tag = tag;
    }
}
