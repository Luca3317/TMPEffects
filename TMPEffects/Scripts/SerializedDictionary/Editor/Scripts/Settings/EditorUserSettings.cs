using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace AYellowpaper.SerializedCollections.Editor
{
    public sealed class EditorUserSettings : ScriptableObject
    {
        [SerializeField]
        private bool _alwaysShowSearch = false;
        [SerializeField, Range(1, 10)]
        private int _pageCountForSearch = 1;
        [SerializeField, Min(1)]
        private int _elementsPerPage = 10;

        public bool AlwaysShowSearch => _alwaysShowSearch;
        public int PageCountForSearch => _pageCountForSearch;
        public int ElementsPerPage => _elementsPerPage;

        private static EditorUserSettings _instance;

        private const string _filePath = "UserSettings/SerializedCollectionsEditorSettings.asset";

        public static EditorUserSettings Get()
        {
            if (_instance == null)
            {
                _instance = CreateInstance<EditorUserSettings>();
                LoadInto(_instance);
            }
            return _instance;
        }

        private static void LoadInto(EditorUserSettings settings)
        {
            if (!File.Exists(_filePath)) return;

            try
            {
                string json = File.ReadAllText(_filePath);
                EditorJsonUtility.FromJsonOverwrite(json, settings);
                return;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }
        }

        internal static void Save()
        {
            string contents = EditorJsonUtility.ToJson(Get());
            File.WriteAllText(_filePath, contents);
        }
    }
}