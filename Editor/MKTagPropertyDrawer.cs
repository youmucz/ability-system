using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Minikit.AbilitySystem.Internal;

namespace Minikit.AbilitySystem.Editor
{
    [CustomPropertyDrawer(typeof(MKTag))]
    public class MKTagPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text += " (Tag)";
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            List<string> nativeTags = MKTagReflector.GetNativelyDefinedTags();
            if (nativeTags.Count > 0)
            {
                nativeTags.Insert(0, "Invalid");
            }
            else
            {
                nativeTags.Insert(0, "Native Tags Not Found");
            }

            float padding = 5f;
            float clearButtonWidth = 50f;
            Rect clearRect = new Rect(position.x, position.y, clearButtonWidth, position.height);
            Rect keyFieldRect = new Rect(position.x + clearButtonWidth + padding, position.y, position.width - clearButtonWidth - padding, position.height);

            if (GUI.Button(clearRect, "Clear"))
            {
                property.FindPropertyRelative("key").stringValue = "";
            }
            else
            {
                SerializedProperty keyProperty = property.FindPropertyRelative("key");

                if (string.IsNullOrEmpty(keyProperty.stringValue))
                {
                    // TODO: `option` is offset by -1 for each tag property drawer being rendered before the Popup call

                    // Display a dropdown of natively defined tags on the next line
                    int option = EditorGUI.Popup(keyFieldRect, 0, nativeTags.ToArray());
                    if (option > nativeTags.Count)
                    {
                        option = nativeTags.Count;
                    }
                    else if (option < 0)
                    {
                        option = 0;
                    }

                    if (option != 0)
                    {
                        property.FindPropertyRelative("key").stringValue = nativeTags[option];
                    }
                }
                else
                {
                    EditorGUI.PropertyField(keyFieldRect, keyProperty, GUIContent.none);
                }
            }

            EditorGUI.EndProperty();
        }
    }
} // Minikit.AbilitySystem.Editor namespace
