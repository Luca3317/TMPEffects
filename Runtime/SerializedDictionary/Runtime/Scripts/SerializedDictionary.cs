using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace TMPEffects.SerializedCollections
{
    [System.Serializable]
    public partial class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializedDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        public List<SerializedKeyValuePair<TKey, TValue>> SerializedList
        {
            get => _serializedList;
            set => _serializedList = value;
        }

        [SerializeField]
        internal List<SerializedKeyValuePair<TKey, TValue>> _serializedList = new List<SerializedKeyValuePair<TKey, TValue>>();

#if UNITY_EDITOR
        internal IKeyable LookupTable
        {
            get
            {
                if (_lookupTable == null)
                    _lookupTable = new DictionaryLookupTable<TKey, TValue>(this);
                return _lookupTable;
            }
        }

        private DictionaryLookupTable<TKey, TValue> _lookupTable;
#endif

        public void OnAfterDeserialize()
        {
            Clear();

            foreach (var kvp in _serializedList)
            {
#if UNITY_EDITOR
                if (!ContainsKey(kvp.Key))
                    Add(kvp.Key, kvp.Value);
#else
                    Add(kvp.Key, kvp.Value);
#endif
            }

#if UNITY_EDITOR
            LookupTable.RecalculateOccurences();
#else
            _serializedList.Clear();
#endif
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer)
                LookupTable.RemoveDuplicates();
#endif
        }
    }

    internal interface ISerializedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public List<SerializedKeyValuePair<TKey, TValue>> SerializedList { get; set; }
    }

    [System.Serializable]
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyPropertyChanged, IDisposable where TValue : INotifyPropertyChanged
    {
        protected bool mayRaise = true;
        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                _dictionary[key] = value;
                RaisePropertyChanged();
            }
        }

        public ICollection<TKey> Keys => _dictionary.Keys;

        public ICollection<TValue> Values => _dictionary.Values;

        public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            RaisePropertyChanged();
            if (value != null)
            {
                value.PropertyChanged += RaisePropertyChanged;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(item);
            RaisePropertyChanged();
            if (item.Value != null)
                item.Value.PropertyChanged += RaisePropertyChanged;
        }

        public void Clear()
        {
            foreach (var kvp in _dictionary)
            {
                if (kvp.Value != null)
                    kvp.Value.PropertyChanged -= RaisePropertyChanged;
            }

            _dictionary.Clear();
            RaisePropertyChanged();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key].PropertyChanged -= RaisePropertyChanged;
                if (!_dictionary.Remove(key))
                {
                    throw new System.InvalidOperationException("Failed to remove key despite it being present?");
                }
                RaisePropertyChanged();
                return true;
            }

            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (((IDictionary<TKey, TValue>)_dictionary).Contains(item))
            {
                _dictionary[item.Key].PropertyChanged -= RaisePropertyChanged;
                if (!_dictionary.Remove(item.Key))
                {
                    throw new System.InvalidOperationException("Failed to remove key despite it being present?");
                }
                RaisePropertyChanged();
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dictionary).GetEnumerator();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        protected void RaisePropertyChanged()
        {
            if (mayRaise) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("dictionary"));
        }

        protected void RaisePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (mayRaise) PropertyChanged?.Invoke(this, args);
        }
    }

    [System.Serializable]
    public partial class SerializedObservableDictionary<TKey, TValue> : ObservableDictionary<TKey, TValue>, ISerializedDictionary<TKey, TValue>, ISerializationCallbackReceiver where TValue : INotifyPropertyChanged
    {
        public List<SerializedKeyValuePair<TKey, TValue>> SerializedList
        {
            get => _serializedList;
            set => _serializedList = value;
        }

        [SerializeField]
        internal List<SerializedKeyValuePair<TKey, TValue>> _serializedList = new List<SerializedKeyValuePair<TKey, TValue>>();

#if UNITY_EDITOR
        internal IKeyable LookupTable
        {
            get
            {
                if (_lookupTable == null)
                    _lookupTable = new DictionaryLookupTable<TKey, TValue>(this);
                return _lookupTable;
            }
        }

        private DictionaryLookupTable<TKey, TValue> _lookupTable;
#endif

        public void OnAfterDeserialize()
        {
            mayRaise = false;
            Clear();

            foreach (var kvp in _serializedList)
            {
#if UNITY_EDITOR
                if (!ContainsKey(kvp.Key))
                    Add(kvp.Key, kvp.Value);
#else
                    Add(kvp.Key, kvp.Value); 
#endif
            }

#if UNITY_EDITOR
            LookupTable.RecalculateOccurences();
#else
            _serializedList.Clear();
#endif

            mayRaise = true;
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer)
                LookupTable.RemoveDuplicates();
#endif
        }
    }
}


