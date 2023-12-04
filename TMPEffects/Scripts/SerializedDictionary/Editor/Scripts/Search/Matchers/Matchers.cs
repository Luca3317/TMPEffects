using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.Editor.Search
{
    public static class Matchers
    {
        public static IEnumerable<Matcher> RegisteredMatchers => _registeredMatchers;

        private static List<Matcher> _registeredMatchers = new List<Matcher>();

        static Matchers()
        {
            _registeredMatchers.Add(new NumericMatcher());
            _registeredMatchers.Add(new StringMatcher());
            _registeredMatchers.Add(new EnumMatcher());
        }

        public static void AddMatcher(Matcher matcher)
        {
            _registeredMatchers.Add(matcher);
        }

        public static bool RemoveMatcher(Matcher matcher)
        {
            return _registeredMatchers.Remove(matcher);
        }
    }
}