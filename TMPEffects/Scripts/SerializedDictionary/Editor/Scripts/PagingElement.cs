using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AYellowpaper.SerializedCollections.Editor
{
    public class PagingElement
    {
        public int Page
        {
            get => _page;
            set
            {
                _page = value;
                EnsureValidPageIndex();
            }
        }
        public int PageCount
        {
            get => _pageCount;
            set
            {
                Debug.Assert(value >= 1, $"{nameof(PageCount)} needs to be 1 or larger but is {value}.");
                _pageCount = value;
                EnsureValidPageIndex();
            }
        }

        private const int buttonWidth = 20;
        private const int inputWidth = 20;
        private const int labelWidth = 30;

        private int _page = 1;
        private int _pageCount = 1;

        public PagingElement(int pageCount = 1)
        {
            PageCount = pageCount;
        }

        public float GetDesiredWidth()
        {
            return buttonWidth * 2 + inputWidth + labelWidth;
        }

        public void OnGUI(Rect rect)
        {
            Rect leftButton = rect.WithXAndWidth(rect.x, buttonWidth);
            Rect inputRect = leftButton.AppendRight(inputWidth);
            Rect labelRect = inputRect.AppendRight(labelWidth);
            Rect rightButton = labelRect.AppendRight(buttonWidth);
            using (new GUIEnabledScope(Page != 1))
                if (GUI.Button(leftButton, "<"))
                    Page--;
            using (new GUIEnabledScope(Page != PageCount))
                if (GUI.Button(rightButton, ">"))
                    Page++;
            Page = EditorGUI.IntField(inputRect, Page);
            GUI.Label(labelRect, "/" + PageCount.ToString());
        }
        
        private void EnsureValidPageIndex()
        {
            _page = Mathf.Clamp(_page, 1, PageCount);
        }
    }
}