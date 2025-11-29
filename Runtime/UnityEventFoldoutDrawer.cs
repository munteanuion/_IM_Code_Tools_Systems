using UnityEngine;

#if UNITY_EDITOR

namespace Plugins
{
    using UnityEditor;

    [CustomPropertyDrawer(typeof(UnityEventFoldoutAttribute))]
    public class UnityEventFoldoutDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string key = "UnityEventFoldout_" + property.propertyPath + "_" + property.serializedObject.targetObject.GetInstanceID();

            bool foldout = EditorPrefs.HasKey(key) ? EditorPrefs.GetBool(key) : false;

            bool newFoldout = EditorGUI.Foldout(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                foldout, label, true);

            if (newFoldout != foldout)
            {
                EditorPrefs.SetBool(key, newFoldout);
            }

            if (newFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(
                    new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2,
                        position.width,
                        EditorGUI.GetPropertyHeight(property, true)),
                    property, GUIContent.none, true);
                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            string key = "UnityEventFoldout_" + property.propertyPath + "_" + property.serializedObject.targetObject.GetInstanceID();
            bool foldout = EditorPrefs.HasKey(key) ? EditorPrefs.GetBool(key) : false;

            if (foldout)
                return EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.singleLineHeight + 2;
            else
                return EditorGUIUtility.singleLineHeight;
        }
    }

}

#endif

public class UnityEventFoldoutAttribute : PropertyAttribute
{
    public UnityEventFoldoutAttribute()
    {
    }
}
