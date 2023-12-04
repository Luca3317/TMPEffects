using System;
using System.Collections;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.KeysGenerators
{
    [KeyListGenerator("Int Range", typeof(int))]
    public class IntRangeGenerator : KeyListGenerator
    {
        [SerializeField]
        private int _startValue = 1;
        [SerializeField]
        private int _endValue = 10;

        public override IEnumerable GetKeys(Type type)
        {
            int dir = Math.Sign(_endValue - _startValue);
            dir = dir == 0 ? 1 : dir;
            for (int i = _startValue; i != _endValue; i += dir)
                yield return i;
            yield return _endValue;
        }
    }
}