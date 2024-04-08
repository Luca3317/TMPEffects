using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TMPEffects.Tags
{    
    /// <summary>
    /// A <see cref="TMPEffects"/> tag.<br/>
    /// Contains any data "inherent" to the given tag.
    /// </summary>
    public sealed class TMPEffectTag : IEquatable<TMPEffectTag>
    {
        /// <summary>
        /// The name of the tag.
        /// </summary>
        public string Name => name;
        /// <summary>
        /// The prefix of the tag.
        /// </summary>
        public char Prefix => prefix;
        /// <summary>
        /// The parameters of the tag.
        /// </summary>
        public ReadOnlyDictionary<string, string> Parameters => parameters;

        private readonly string name;
        private readonly char prefix;
        private readonly ReadOnlyDictionary<string, string> parameters;

        public TMPEffectTag(string name, char prefix, IDictionary<string, string> parameters)
        {
            this.name = name;
            this.prefix = prefix;
            if (parameters == null)
                this.parameters = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
            else
                this.parameters = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(parameters));
        }

        public bool Equals(TMPEffectTag other)
        {
            return name == other.name && prefix == other.prefix && parameters.SequenceEqual(other.parameters); 
        }
    }
}