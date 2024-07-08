using System.ComponentModel;

namespace TMPEffects.ObjectChanged
{
    /// <summary>
    /// Notigy clients when the object changed.
    /// </summary>
    public interface INotifyObjectChanged
    {
        event ObjectChangedEventHandler ObjectChanged;
    }
}