using System.ComponentModel;

namespace TMPEffects.ObjectChanged
{
    /// <summary>
    /// Notify clients when the object changed.
    /// </summary>
    public interface INotifyObjectChanged
    {
        /// <summary>
        /// Raised when the object is changed.
        /// </summary>
        event ObjectChangedEventHandler ObjectChanged;
    }
}