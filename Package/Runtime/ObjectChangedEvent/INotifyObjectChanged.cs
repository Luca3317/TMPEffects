using System.ComponentModel;

namespace TMPEffects.ObjectChanged
{
    /// <summary>
    /// Notify clients when the object changed.
    /// </summary>
    public interface INotifyObjectChanged
    {
        event ObjectChangedEventHandler ObjectChanged;
    }
}