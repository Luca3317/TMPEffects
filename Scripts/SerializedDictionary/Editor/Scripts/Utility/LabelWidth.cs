using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TMPEffects.SerializedCollections.Editor
{
    internal struct LabelWidth : IDisposable
    {
        public float PreviousWidth { get; }

        public LabelWidth(float width)
        {
            PreviousWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = width;
        }

        public void Dispose()
        {
            EditorGUIUtility.labelWidth = PreviousWidth;
        }
    }
}