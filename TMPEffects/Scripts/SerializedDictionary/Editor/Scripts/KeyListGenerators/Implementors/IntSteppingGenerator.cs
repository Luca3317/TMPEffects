using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.KeysGenerators
{
    [KeyListGenerator("Int Stepping", typeof(int))]
    public class IntSteppingGenerator : KeyListGenerator
    {
        [SerializeField]
        private int _startIndex = 0;
        [SerializeField]
        private int _stepDistance = 10;
        [SerializeField, Min(0)]
        private int _stepCount = 1;

        public override IEnumerable GetKeys(Type type)
        {
            for (int i = 0; i <= _stepCount; i++)
            {
                yield return _startIndex + i * _stepDistance;
            }
        }
    }
}