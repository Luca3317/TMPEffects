using System;
using System.Collections.Generic;

namespace TMPEffects.Tags
{
    public class TMPEventArgs : EventArgs
    {
        public string name;
        public int index;
        public Dictionary<string, string> parameters;

        public TMPEventArgs(int index, string name, Dictionary<string, string> parameters)
        {
            this.index = index;
            this.name = name;
            this.parameters = parameters;
        }
    }
}
