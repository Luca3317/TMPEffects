using System.ComponentModel;

namespace TMPEffects.ObjectChanged
{
    public interface INotifyObjectChanged
    {
        event ObjectChangedEventHandler ObjectChanged;
    }
}