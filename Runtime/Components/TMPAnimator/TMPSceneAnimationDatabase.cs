
using TMPEffects.Components;
using TMPEffects.Databases;
using TMPEffects.ObjectChanged;
using TMPEffects.SerializedCollections;
using TMPEffects.TMPAnimations;
using UnityEngine;


//[System.Serializable]
//public class TMPSceneAnimationDatabase<T> : ITMPEffectDatabase<T>, INotifyObjectChanged, ISerializationCallbackReceiver where T : ITMPAnimation, INotifyObjectChanged
//{
//    [SerializedDictionary]
//    [SerializeField] public SerializedDictionary<string, T> dictionary;

//    public event ObjectChangedEventHandler ObjectChanged;

//    public void AddAnimation(string name, T animation)
//    {
//        dictionary[name] = animation;
//        animation.ObjectChanged += RaiseChanged;
//    }

//    public bool RemoveAnimation(string name)
//    {
//        if (!dictionary.ContainsKey(name)) { return false; }
//        dictionary[name].ObjectChanged -= RaiseChanged;
//        dictionary.Remove(name);
//        return true;
//    }

//    public bool ContainsEffect(string name)
//    {
//        return dictionary.ContainsKey(name);
//    }

//    private void RaiseChanged(object sender)
//    {
//        ObjectChanged?.Invoke(sender);
//    }

//    public T GetEffect(string name)
//    {
//        return dictionary[name];
//    }

//    public void OnBeforeSerialize()
//    { }

//    public void OnAfterDeserialize()
//    {
//        foreach (var kvp in dictionary)
//        {
//            kvp.Value.ObjectChanged -= RaiseChanged;
//            kvp.Value.ObjectChanged += RaiseChanged;
//        }
//    }
//}