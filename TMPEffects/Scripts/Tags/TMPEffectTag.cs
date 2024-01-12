using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.Tags
{
    public abstract class TMPEffectTag
    {
        public string name { get; private set; }
        public int startIndex { get; private set; }
        public int endIndex { get; private set; }

        public int length => endIndex == -1 ? -1 : (endIndex - startIndex) + 1;
        public Dictionary<string, string> parameters { get; private set; }

        private int nameHashCode;

        public TMPEffectTag(string name, int startIndex, Dictionary<string, string> parameters)
        {
            this.name = name;
            this.startIndex = startIndex;
            this.parameters = parameters;
            endIndex = -1;
            nameHashCode = name.GetHashCode();
        }

        public void Close(int endIndex)
        {
            //if (!IsOpen) throw new System.InvalidOperationException();
            this.endIndex = endIndex;
        }

        public void SetStartIndex(int startIndex)
        {
            this.startIndex = startIndex;
        }
        public void SetEndIndex(int endIndex) 
        {
            this.endIndex = endIndex; 
        }

        public bool IsOpen => length == -1;
        public bool IsEqual(string name) => nameHashCode == name.GetHashCode();
    }
}
