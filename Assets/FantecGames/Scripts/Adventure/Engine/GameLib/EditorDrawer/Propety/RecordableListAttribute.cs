using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
#endif

namespace fantec
{


    [System.Serializable]
    public class RecorderableList<T>
    {
        public List<T> List { get { return list; } }
        [SerializeField]
        List<T> list = new List<T>();

#if UNITY_EDITOR
        public class RecorderableListDrawer : PropertyDrawerEx
        {
            SerializedProperty listProperty;
            ReorderableList recorderableList;

            public override void OnGUI(Rect rect, SerializedProperty serializedProperty, GUIContent label)
            {
                if (TryInitIfMissing(serializedProperty, label))
                {
                    float height = 0f;
                    for (var i = 0; i < listProperty.arraySize; i++)
                    {
                        height = Mathf.Max(height, EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
                    }
                    recorderableList.elementHeight = height;
                    recorderableList.DoList(rect);
                }
                else
                {
                    base.OnGUI(rect, serializedProperty, label);
                }
            }

            public override float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent label)
            {
                if (TryInitIfMissing(serializedProperty, label))
                {
                    return recorderableList.GetHeight();
                }
                else
                {
                    return base.GetPropertyHeight(serializedProperty, label);
                }
            }

            bool TryInitIfMissing(SerializedProperty serializedProperty, GUIContent label)
            {
                if (recorderableList == null)
                {
                    listProperty = serializedProperty.FindPropertyRelative("list");
                    if (listProperty == null)
                    {
                        Debug.LogError("list is not found");
                    }
                    else
                    {
                        string headerText = label.text;
                        recorderableList = new ReorderableList(listProperty.serializedObject, listProperty);
                        recorderableList.drawHeaderCallback = (rect) => {
                            EditorGUI.LabelField(rect, headerText);
                        };

                        recorderableList.drawElementCallback = (rect, index, isActive, isFocused) => {
                            var property = listProperty.GetArrayElementAtIndex(index);
                            EditorGUI.indentLevel++;
                            EditorGUI.PropertyField(rect, property, true);
                            EditorGUI.indentLevel--;
                        };
                    }
                }
                return recorderableList != null;
            }

        }
#endif
    }
}
