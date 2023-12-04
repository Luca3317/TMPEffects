using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static AYellowpaper.SerializedCollections.Editor.SerializedDictionaryDrawer;

namespace AYellowpaper.SerializedCollections.Editor.States
{
    internal class DefaultListState : ListState
    {
        public override int ListSize => Drawer.ListProperty.minArraySize;

        public DefaultListState(SerializedDictionaryInstanceDrawer serializedDictionaryDrawer) : base(serializedDictionaryDrawer)
        {
        }

        public override void OnEnter()
        {
            Drawer.ReorderableList.draggable = true;
        }

        public override void OnExit()
        {
        }

        public override ListState OnUpdate()
        {
            if (Drawer.SearchText.Length > 0)
                return Drawer.SearchState;

            return this;
        }

        public override void DrawElement(Rect rect, SerializedProperty property, DisplayType displayType)
        {
            SerializedDictionaryInstanceDrawer.DrawElement(rect, property, displayType);
        }

        public override SerializedProperty GetPropertyAtIndex(int index)
        {
            return Drawer.ListProperty.GetArrayElementAtIndex(index);
        }

        public override void RemoveElementAt(int index)
        {
            Drawer.ListProperty.DeleteArrayElementAtIndex(index);
        }

        public override void InserElementAt(int index)
        {
            Drawer.ListProperty.InsertArrayElementAtIndex(index);
            Drawer.ListProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}