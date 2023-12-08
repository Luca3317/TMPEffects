using System.Collections.Generic;

namespace TMPEffects.Tags
{
    public abstract class TMPEffectTag
    {
        public string name { get; private set; }
        public int startIndex { get; private set; }

        public int length { get; private set; }
        public Dictionary<string, string> parameters { get; private set; }

        private int nameHashCode;

        public TMPEffectTag(string name, int startIndex, Dictionary<string, string> parameters)
        {
            this.name = name;
            this.startIndex = startIndex;
            this.parameters = parameters;
            length = -1;
            nameHashCode = name.GetHashCode();
        }

        public void Close(int endIndex)
        {
            //if (!IsOpen) throw new System.InvalidOperationException();
            length = endIndex - startIndex + 1;
        }

        public bool IsOpen => length == -1;
        public bool IsEqual(string name) => nameHashCode == name.GetHashCode();
    }
}
