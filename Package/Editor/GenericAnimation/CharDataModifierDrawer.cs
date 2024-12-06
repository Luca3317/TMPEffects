// TODO Can this be delted? unused?

// using UnityEditor;
// using UnityEngine;
// using Matrix4x4 = UnityEngine.Matrix4x4;
// using Vector3 = UnityEngine.Vector3;
// using Vector4 = UnityEngine.Vector4;
//
// [CustomPropertyDrawer(typeof(CharDataModifiers))]
// public class CharDataModifierDrawer : PropertyDrawer
// {
//     private Color32 backgroundColor;
//
//     private SerializedProperty positionDeltaProp;
//     private SerializedProperty scaleDeltaProp;
//     private SerializedProperty rotationsProp;
//     private SerializedProperty meshModifierProp;
//     private SerializedProperty characterModifierProp;
//     private SerializedProperty blDeltaProp, tlDeltaProp, trDeltaProp, brDeltaProp;
//     private SerializedProperty blColorProp, tlColorProp, trColorProp, brColorProp;
//     private SerializedProperty blUV0Prop, tlUV0Prop, trUV0Prop, brUV0Prop;
//     private SerializedProperty meshModifierDirtyProp;
//     private SerializedProperty characterMeshModifierDirtyProp;
//     
//     private const float TypedVectorFoldoutWidth = 20f;
//
//     private void Init(SerializedProperty property)
//     {
//         characterModifierProp = property.FindPropertyRelative("characterModifiers");
//
//         positionDeltaProp = characterModifierProp.FindPropertyRelative("positionDelta");
//         scaleDeltaProp = characterModifierProp.FindPropertyRelative("scaleDelta");
//         rotationsProp = characterModifierProp.FindPropertyRelative("rotations");
//         characterMeshModifierDirtyProp = characterModifierProp.FindPropertyRelative("modifier");
//
//         meshModifierProp = property.FindPropertyRelative("meshModifiers");
//
//         blDeltaProp = meshModifierProp.FindPropertyRelative("bl_Delta");
//         tlDeltaProp = meshModifierProp.FindPropertyRelative("tl_Delta");
//         trDeltaProp = meshModifierProp.FindPropertyRelative("tr_Delta");
//         brDeltaProp = meshModifierProp.FindPropertyRelative("br_Delta");
//
//         blColorProp = meshModifierProp.FindPropertyRelative("bl_Color");
//         tlColorProp = meshModifierProp.FindPropertyRelative("tl_Color");
//         trColorProp = meshModifierProp.FindPropertyRelative("tr_Color");
//         brColorProp = meshModifierProp.FindPropertyRelative("br_Color");
//
//         blUV0Prop = meshModifierProp.FindPropertyRelative("bl_UV0");
//         tlUV0Prop = meshModifierProp.FindPropertyRelative("tl_UV0");
//         trUV0Prop = meshModifierProp.FindPropertyRelative("tr_UV0");
//         brUV0Prop = meshModifierProp.FindPropertyRelative("br_UV0");
//
//         meshModifierDirtyProp = meshModifierProp.FindPropertyRelative("modifier");
//
//         backgroundColor = EditorGUIUtility.isProSkin
//             ? new Color32(56, 56, 56, 255)
//             : new Color32(194, 194, 194, 255);
//     }
//
//     private void DrawRawToggle(float y, SerializedProperty rawProp, Rect toggleRect, Rect labelRect)
//     {
//         toggleRect.y = y;
//         labelRect.y = y;
//         EditorGUI.LabelField(labelRect, "Scaled");
//         rawProp.boolValue =
//             EditorGUI.Toggle(toggleRect, rawProp.boolValue);
//     }
//
//     private Matrix4x4 GetMatrix()
//     {
//         Matrix4x4 matrix = new Matrix4x4();
//         matrix.m00 = scaleDeltaProp.FindPropertyRelative("e00").floatValue;
//         matrix.m01 = scaleDeltaProp.FindPropertyRelative("e01").floatValue;
//         matrix.m02 = scaleDeltaProp.FindPropertyRelative("e02").floatValue;
//         matrix.m03 = scaleDeltaProp.FindPropertyRelative("e03").floatValue;
//         matrix.m10 = scaleDeltaProp.FindPropertyRelative("e10").floatValue;
//         matrix.m11 = scaleDeltaProp.FindPropertyRelative("e11").floatValue;
//         matrix.m12 = scaleDeltaProp.FindPropertyRelative("e12").floatValue;
//         matrix.m13 = scaleDeltaProp.FindPropertyRelative("e13").floatValue;
//         matrix.m20 = scaleDeltaProp.FindPropertyRelative("e20").floatValue;
//         matrix.m21 = scaleDeltaProp.FindPropertyRelative("e21").floatValue;
//         matrix.m22 = scaleDeltaProp.FindPropertyRelative("e22").floatValue;
//         matrix.m23 = scaleDeltaProp.FindPropertyRelative("e23").floatValue;
//         matrix.m30 = scaleDeltaProp.FindPropertyRelative("e30").floatValue;
//         matrix.m31 = scaleDeltaProp.FindPropertyRelative("e31").floatValue;
//         matrix.m32 = scaleDeltaProp.FindPropertyRelative("e32").floatValue;
//         matrix.m33 = scaleDeltaProp.FindPropertyRelative("e33").floatValue;
//         return matrix;
//     }
//
//     public static Vector3 ExtractScale(Matrix4x4 matrix)
//     {
//         Vector3 scale;
//         scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
//         scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
//         scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
//         return scale;
//     }
//
//     private void ApplyBackToMatrix(Vector3 scale)
//     {
//         Debug.Log("Scale " + scale);
//         // matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
//         var matrix = Matrix4x4.Scale(scale);
//         Debug.Log("mat scale " + matrix.lossyScale);
//         scaleDeltaProp.FindPropertyRelative("e00").floatValue = matrix.m00;
//         scaleDeltaProp.FindPropertyRelative("e11").floatValue = matrix.m11;
//         scaleDeltaProp.FindPropertyRelative("e22").floatValue = matrix.m22;
//
//         for (int i = 0; i < 16; i++)
//             if (matrix[i] < 0)
//                 Debug.LogWarning(i + " negative!! " + matrix[i]);
//
//         var newmat = GetMatrix();
//     }
//
//     private Rect DrawCharacterModifier(Rect rect)
//     {
//         var bgRect = new Rect(rect.x, rect.y, rect.width,
//             EditorGUIUtility.singleLineHeight * 2 + EditorGUI.GetPropertyHeight(rotationsProp, true));
//         EditorGUI.DrawRect(bgRect, backgroundColor);
//
//         EditorGUI.BeginChangeCheck();
//         EditorGUI.PropertyField(rect, positionDeltaProp);
//         rect.y += EditorGUIUtility.singleLineHeight;
//         if (EditorGUI.EndChangeCheck())
//         {
//             // If is 0 (= default), but bit is set as dirty, fix
//             if (positionDeltaProp.vector3Value == Vector3.zero)
//             {
//                 if ((characterMeshModifierDirtyProp.enumValueFlag &
//                      (int)TMPCharacterModifiers.ModifierFlags.PositionDelta) != 0)
//                 {
//                     characterMeshModifierDirtyProp.enumValueFlag &=
//                         ~(int)TMPCharacterModifiers.ModifierFlags.PositionDelta;
//                 }
//             }
//             // If is not 0 (!= default), but bit is not set as dirty, fix
//             else
//             {
//                 if ((characterMeshModifierDirtyProp.enumValueFlag &
//                      (int)TMPCharacterModifiers.ModifierFlags.PositionDelta) == 0)
//                 {
//                     characterMeshModifierDirtyProp.enumValueFlag |=
//                         (int)TMPCharacterModifiers.ModifierFlags.PositionDelta;
//                 }
//             }
//         }
//
//         EditorGUI.BeginChangeCheck();
//         rect.height = EditorGUI.GetPropertyHeight(rotationsProp, true);
//         EditorGUI.PropertyField(rect, rotationsProp, true);
//         rect.y += rect.height;
//         rect.height = EditorGUIUtility.singleLineHeight;
//         if (EditorGUI.EndChangeCheck())
//         {
//             // If is 0 (= default), but bit is set as dirty, fix
//             if (rotationsProp.arraySize == 0)
//             {
//                 if ((characterMeshModifierDirtyProp.enumValueFlag &
//                      (int)TMPCharacterModifiers.ModifierFlags.Rotations) != 0)
//                 {
//                     characterMeshModifierDirtyProp.enumValueFlag &=
//                         ~(int)TMPCharacterModifiers.ModifierFlags.Rotations;
//                 }
//             }
//             // If is not 0 (!= default), but bit is not set as dirty, fix
//             else
//             {
//                 if ((characterMeshModifierDirtyProp.enumValueFlag &
//                      (int)TMPCharacterModifiers.ModifierFlags.Rotations) == 0)
//                 {
//                     characterMeshModifierDirtyProp.enumValueFlag |=
//                         (int)TMPCharacterModifiers.ModifierFlags.Rotations;
//                 }
//             }
//         }
//
//         EditorGUI.BeginChangeCheck();
//         Matrix4x4 mat = GetMatrix();
//         for (int i = 0; i < 16; i++)
//             if (mat[i] < 0)
//                 Debug.LogWarning(i + " MATT!!! negative!! " + mat[i]);
//         var lossyScale = ExtractScale(mat);
//
//         var newScale = EditorGUI.Vector3Field(rect, "Scale", lossyScale);
//         if (newScale != lossyScale)
//         {
//             ApplyBackToMatrix(newScale);
//         }
//
//         if (EditorGUI.EndChangeCheck())
//         {
//             // If is V3.one (= default), but bit is set as dirty, fix
//             if (newScale == Vector3.one)
//             {
//                 if ((characterMeshModifierDirtyProp.enumValueFlag &
//                      (int)TMPCharacterModifiers.ModifierFlags.Scale) != 0)
//                 {
//                     characterMeshModifierDirtyProp.enumValueFlag &=
//                         ~(int)TMPCharacterModifiers.ModifierFlags.Scale;
//                 }
//             }
//             // If is not V3.one (!= default), but bit is not set as dirty, fix
//             else
//             {
//                 if ((characterMeshModifierDirtyProp.enumValueFlag &
//                      (int)TMPCharacterModifiers.ModifierFlags.Scale) == 0)
//                 {
//                     characterMeshModifierDirtyProp.enumValueFlag |=
//                         (int)TMPCharacterModifiers.ModifierFlags.Scale;
//                 }
//             }
//         }
//
//         rect.y += EditorGUIUtility.singleLineHeight;
//         return rect;
//     }
//
//     private Rect DrawMeshDeltaModifier(Rect rect)
//     {
//         var bgRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 4);
//         EditorGUI.DrawRect(bgRect, backgroundColor);
//
//         EditorGUI.BeginChangeCheck();
//         EditorGUI.PropertyField(rect, blDeltaProp);
//         rect.y += EditorGUIUtility.singleLineHeight;
//         EditorGUI.PropertyField(rect, tlDeltaProp);
//         rect.y += EditorGUIUtility.singleLineHeight;
//         EditorGUI.PropertyField(rect, trDeltaProp);
//         rect.y += EditorGUIUtility.singleLineHeight;
//         EditorGUI.PropertyField(rect, brDeltaProp);
//         rect.y += EditorGUIUtility.singleLineHeight;
//         if (EditorGUI.EndChangeCheck())
//         {
//             // If is not 0 (!= default), but bit is not set as dirty, fix
//             if (blDeltaProp.vector3Value != Vector3.zero || tlDeltaProp.vector3Value != Vector3.zero ||
//                 trDeltaProp.vector3Value != Vector3.zero || brDeltaProp.vector3Value != Vector3.zero)
//             {
//                 if ((meshModifierDirtyProp.enumValueFlag &
//                      (int)TMPMeshModifiers.ModifierFlags.Deltas) == 0)
//                 {
//                     meshModifierDirtyProp.enumValueFlag |=
//                         (int)TMPMeshModifiers.ModifierFlags.Deltas;
//                 }
//             }
//             // If is 0 (= default), but bit is set as dirty, fix
//             else
//             {
//                 if ((meshModifierDirtyProp.enumValueFlag &
//                      (int)TMPMeshModifiers.ModifierFlags.Deltas) != 0)
//                 {
//                     meshModifierDirtyProp.enumValueFlag &=
//                         ~(int)TMPMeshModifiers.ModifierFlags.Deltas;
//                 }
//             }
//         }
//
//         return rect;
//     }
//
//     private Rect DrawMeshColorModifier(Rect rect)
//     {
//         var bgRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 4);
//         EditorGUI.DrawRect(bgRect, backgroundColor);
//
//         EditorGUI.BeginChangeCheck();
//         EditorGUI.PropertyField(rect, blColorProp);
//         rect.y += EditorGUIUtility.singleLineHeight;
//         EditorGUI.PropertyField(rect, tlColorProp);
//         rect.y += EditorGUIUtility.singleLineHeight;
//         EditorGUI.PropertyField(rect, trColorProp);
//         rect.y += EditorGUIUtility.singleLineHeight;
//         EditorGUI.PropertyField(rect, brColorProp);
//         rect.y += EditorGUIUtility.singleLineHeight;
//
//         if (EditorGUI.EndChangeCheck())
//         {
//             // If is not 0 (!= default), but bit is not set as dirty, fix
//             if (blColorProp.FindPropertyRelative("Override").enumValueFlag != 0 ||
//                 tlColorProp.FindPropertyRelative("Override").enumValueFlag != 0 ||
//                 trColorProp.FindPropertyRelative("Override").enumValueFlag != 0 ||
//                 brColorProp.FindPropertyRelative("Override").enumValueFlag != 0)
//             {
//                 if ((meshModifierDirtyProp.enumValueFlag &
//                      (int)TMPMeshModifiers.ModifierFlags.Colors) == 0)
//                 {
//                     meshModifierDirtyProp.enumValueFlag |=
//                         (int)TMPMeshModifiers.ModifierFlags.Colors;
//                 }
//             }
//             // If is 0 (= default), but bit is set as dirty, fix
//             else
//             {
//                 if ((meshModifierDirtyProp.enumValueFlag &
//                      (int)TMPMeshModifiers.ModifierFlags.Colors) != 0)
//                 {
//                     meshModifierDirtyProp.enumValueFlag &=
//                         ~(int)TMPMeshModifiers.ModifierFlags.Colors;
//                 }
//             }
//         }
//
//         return rect;
//     }
//
//     private Rect DrawMeshUVModifier(Rect rect)
//     {
//         var bgRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 4);
//         EditorGUI.DrawRect(bgRect, backgroundColor);
//
//         EditorGUI.PropertyField(rect, blUV0Prop);
//         rect.y += EditorGUIUtility.singleLineHeight;
//         EditorGUI.PropertyField(rect, tlUV0Prop);
//         rect.y += EditorGUIUtility.singleLineHeight;
//         EditorGUI.PropertyField(rect, trUV0Prop);
//         rect.y += EditorGUIUtility.singleLineHeight;
//         EditorGUI.PropertyField(rect, brUV0Prop);
//         rect.y += EditorGUIUtility.singleLineHeight;
//
//         if (EditorGUI.EndChangeCheck())
//         {
//             // If is not 0 (!= default), but bit is not set as dirty, fix
//             if (blUV0Prop.FindPropertyRelative("_override").enumValueFlag != 0 ||
//                 tlUV0Prop.FindPropertyRelative("_override").enumValueFlag != 0 ||
//                 trUV0Prop.FindPropertyRelative("_override").enumValueFlag != 0 ||
//                 brUV0Prop.FindPropertyRelative("_override").enumValueFlag != 0)
//             {
//                 if ((meshModifierDirtyProp.enumValueFlag &
//                      (int)TMPMeshModifiers.ModifierFlags.UVs) == 0)
//                 {
//                     meshModifierDirtyProp.enumValueFlag |=
//                         (int)TMPMeshModifiers.ModifierFlags.UVs;
//                 }
//             }
//             // If is 0 (= default), but bit is set as dirty, fix
//             else
//             {
//                 if ((meshModifierDirtyProp.enumValueFlag &
//                      (int)TMPMeshModifiers.ModifierFlags.UVs) != 0)
//                 {
//                     meshModifierDirtyProp.enumValueFlag &=
//                         ~(int)TMPMeshModifiers.ModifierFlags.UVs;
//                 }
//             }
//         }
//
//         return rect;
//     }
//
//
//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     {
//         Init(property);
//
//         var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
//
//         property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, property.displayName);
//         EditorGUI.indentLevel++;
//
//         if (!property.isExpanded) return;
//
//         rect.y += EditorGUIUtility.singleLineHeight;
//         positionDeltaProp.isExpanded =
//             EditorGUI.Foldout(rect, positionDeltaProp.isExpanded, "Character Deltas");
//         rect.y += EditorGUIUtility.singleLineHeight;
//
//         var ctrlRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
//         var togglerect = new Rect(ctrlRect.x + EditorGUIUtility.labelWidth - 20, rect.y, ctrlRect.width,
//             EditorGUIUtility.singleLineHeight);
//         var labelRect = new Rect(togglerect.x - 45, togglerect.y, togglerect.width, EditorGUIUtility.singleLineHeight);
//
//         if (positionDeltaProp.isExpanded)
//             rect = DrawCharacterModifier(rect);
//
//         blDeltaProp.isExpanded =
//             EditorGUI.Foldout(rect, blDeltaProp.isExpanded, "Vertex Deltas");
//         rect.y += EditorGUIUtility.singleLineHeight;
//
//         if (blDeltaProp.isExpanded)
//             rect = DrawMeshDeltaModifier(rect);
//
//         blColorProp.isExpanded =
//             EditorGUI.Foldout(rect, blColorProp.isExpanded, "Vertex Colors");
//         rect.y += EditorGUIUtility.singleLineHeight;
//
//         if (blColorProp.isExpanded)
//             rect = DrawMeshColorModifier(rect);
//
//         blUV0Prop.isExpanded =
//             EditorGUI.Foldout(rect, blUV0Prop.isExpanded, "Vertex UVs");
//         rect.y += EditorGUIUtility.singleLineHeight;
//
//         if (blUV0Prop.isExpanded)
//             rect = DrawMeshUVModifier(rect);
//
//         EditorGUI.indentLevel--;
//     }
//
//     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//     {
//         Init(property);
//         float height = EditorGUIUtility.singleLineHeight;
//
//         if (!property.isExpanded) return height;
//
//         height += EditorGUIUtility.singleLineHeight;
//
//         if (positionDeltaProp.isExpanded)
//             height += EditorGUIUtility.singleLineHeight * 2 + EditorGUI.GetPropertyHeight(rotationsProp, true);
//
//         height += EditorGUIUtility.singleLineHeight;
//
//         if (blDeltaProp.isExpanded)
//             height += EditorGUIUtility.singleLineHeight * 4;
//
//         height += EditorGUIUtility.singleLineHeight;
//
//         if (blColorProp.isExpanded)
//             height += EditorGUIUtility.singleLineHeight * 4;
//
//         height += EditorGUIUtility.singleLineHeight;
//
//         if (blUV0Prop.isExpanded)
//             height += EditorGUIUtility.singleLineHeight * 4;
//
//         return height;
//     }
// }