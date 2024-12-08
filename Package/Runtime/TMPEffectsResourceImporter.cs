using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR

namespace TMPEffects.Editor
{
    internal class TMPEffectsImporterWindow : EditorWindow
    {
        public static void ShowImporterWindow()
        {
            var window = GetWindow<TMPEffectsImporterWindow>(false, "TMPEffects Importer", true);

            Vector2 windowSize = new Vector2(650, 200);
            window.minSize = windowSize;
            window.maxSize = windowSize;
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            VisualElement box = new VisualElement();
            box.style.marginBottom = 2;
            box.style.marginRight = 2;
            box.style.marginTop = 2;
            box.style.marginLeft = 2;
            box.style.backgroundColor = new Color(64f / 255f, 64f / 255f, 64f / 255f, 255f / 255f);
            box.style.paddingTop = 4;
            box.style.paddingBottom = 4;
            box.style.paddingLeft = 4;
            box.style.paddingRight = 4;
            box.style.borderBottomLeftRadius = 2.5f;
            box.style.borderBottomRightRadius = 2.5f;
            box.style.borderTopLeftRadius = 2.5f;
            box.style.borderTopRightRadius = 2.5f;
            box.style.borderRightColor = Color.black;
            box.style.borderLeftColor = Color.black;
            box.style.borderBottomColor = Color.black;
            box.style.borderTopColor = Color.black;
            box.style.borderRightWidth = 0.25f;
            box.style.borderLeftWidth = 0.25f;
            box.style.borderBottomWidth = 0.25f;
            box.style.borderTopWidth = 0.25f;
            root.Add(box);

            // VisualElements objects can contain other VisualElement following a tree hierarchy
            Label header = new Label("Required resources");
            //header.style.fontSize = 14;
            //header.style.color = Color.white;
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.whiteSpace = WhiteSpace.Normal;
            //header.style.marginBottom = 2;
            //header.style.marginTop = 2;
            box.Add(header);

            Label text = new Label(
                "This appears to be the first time you are accessing TMPEffects; " +
                "you will need to import the required resources in order to be able to use the built-in animations, commands etc. " +
                "These new resources will be placed at the root of your project in the \"TMPEffects\" folder. " +
                "Reload the current scene afterwards to ensure all asset references are correctly loaded.");
            //text.style.fontSize = 12;
            //text.style.color = Color.white;
            text.style.whiteSpace = WhiteSpace.Normal;
            text.style.marginTop = 4;
            text.style.marginBottom = 4;
            box.Add(text);

            Button button = new Button(() =>
            {
                Import();
                Close();
            });
            button.text = "Import required resources";
            box.Add(button);
        }

        private void Import()
        {
            AssetDatabase.ImportPackage(
                "Packages/com.luca3317.tmpeffects/Resources/TMPEffects Required Resources.unitypackage", false);
        }
    }
}

#endif