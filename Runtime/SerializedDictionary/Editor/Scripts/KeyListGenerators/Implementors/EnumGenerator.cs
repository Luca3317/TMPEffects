using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPEffects.SerializedCollections.KeysGenerators
{
    [KeyListGenerator("Populate Enum", typeof(System.Enum), false)]
    internal class EnumGenerator : KeyListGenerator
    {
        public override IEnumerable GetKeys(System.Type type)
        {
            return System.Enum.GetValues(type);
        }
    }
}