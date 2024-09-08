/*using UnityEditor;
using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute { }
// Put this script in the "Editor" folder
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyOverrides : PropertyDrawer
{

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}*/

