using System.ComponentModel;

namespace TMPEffects.ObjectChanged
{
    public interface INotifyObjectChanged
    {
#if UNITY_EDITOR
        event ObjectChangedEventHandler ObjectChanged;
#endif
    }
}