using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TMPEffects.SerializedCollections
{
    internal class DictionaryLookupTable<TKey, TValue> : IKeyable
    {
        private ISerializedDictionary<TKey, TValue> _dictionary;
        private Dictionary<TKey, List<int>> _occurences = new Dictionary<TKey, List<int>>();

        private static readonly List<int> EmptyList = new List<int>();

        public IEnumerable Keys => _dictionary.Keys;

        public DictionaryLookupTable(ISerializedDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public IReadOnlyList<int> GetOccurences(object key)
        {
            if (key is TKey castKey && _occurences.TryGetValue(castKey, out var list))
                return list;

            return EmptyList;
        }

        public void RecalculateOccurences()
        {
            _occurences.Clear();

            int count = _dictionary.SerializedList.Count;
            for (int i = 0; i < count; i++)
            {
                var kvp = _dictionary.SerializedList[i];
                if (!SerializedCollectionsUtility.IsValidKey(kvp.Key))
                    continue;

                if (!_occurences.ContainsKey(kvp.Key))
                    _occurences.Add(kvp.Key, new List<int>() { i });
                else
                    _occurences[kvp.Key].Add(i);
            }
        }

        public void RemoveKey(object key)
        {
            for (int i = _dictionary.SerializedList.Count - 1; i >= 0; i--)
            {
                var dictKey = _dictionary.SerializedList[i].Key;
                if ((object)dictKey == key || dictKey.Equals(key))
                    _dictionary.SerializedList.RemoveAt(i);
            }
        }

        public void RemoveAt(int index)
        {
            _dictionary.SerializedList.RemoveAt(index);
        }

        public object GetKeyAt(int index)
        {
            return _dictionary.SerializedList[index];
        }

        public void RemoveDuplicates()
        {
            _dictionary.SerializedList = _dictionary.SerializedList
                .GroupBy(x => x.Key)
                .Where(x => SerializedCollectionsUtility.IsValidKey(x.Key))
                .Select(x => x.First()).ToList();
        }

        public void AddKey(object key)
        {
            var entry = new SerializedKeyValuePair<TKey, TValue>();
            entry.Key = (TKey) key;
            _dictionary.SerializedList.Add(entry);
        }
    }
}