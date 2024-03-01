using System.ComponentModel;

namespace TMPEffects.ObjectChanged
{
    internal interface INotifyObjectChanged
    {
        event ObjectChangedEventHandler ObjectChanged;
    }
}