using UnityEditor;
using UnityEngine;
using Boxophobic.Constants;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledRemap))]
    public class StyledRemapAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            StyledRemap a = (StyledRemap)attribute;

            if (!string.IsNullOrEmpty(a.label))
            {
                label.text = a.label;
            }

            var popupStyle = new GUIStyle(EditorStyles.popup)
            {
                fontSize = 9
            };

            EditorGUI.BeginChangeCheck();

            Vector4 value;

            if (a.supportInvert)
            {
                value = property.vector4Value;
            }
            else
            {
                value = property.vector2Value;
            }

            float minValue;
            float maxValue;

            if (value.w == 0)
            {
                minValue = value.x;
                maxValue = value.y;
            }
            else
            {
                minValue = value.y;
                maxValue = value.x;
            }

            float height = EditorGUIUtility.singleLineHeight;

            Rect line = position;
            line.height = height;

            Rect labelRect = line;
            labelRect.width = EditorGUIUtility.labelWidth;

            Rect buttonRect = new Rect(position.x + 2, position.y, EditorGUIUtility.labelWidth - 1, height);
            Rect arrowRect = new Rect(position.x + 2, position.y - 2, 18, 18);

            if (a.supportInvert)
            {
                if (GUI.Button(buttonRect, "", GUIStyle.none))
                {
                    a.showAdvancedSettings = !a.showAdvancedSettings;
                }

                Rect sliderRect = line;
                sliderRect.width -= EditorGUIUtility.labelWidth + 54;

                EditorGUI.MinMaxSlider(sliderRect, label, ref minValue, ref maxValue, a.min, a.max);

                Rect popupRect = line;
                popupRect.x = position.xMax - 50;
                popupRect.width = 50;

                value.w = EditorGUI.Popup(popupRect, (int)value.w, new[] { "Remap", "Invert" });
            }
            else
            {

                if (GUI.Button(buttonRect, "", GUIStyle.none))
                {
                    a.showAdvancedSettings = !a.showAdvancedSettings;
                }

                EditorGUI.MinMaxSlider(line, label, ref minValue, ref maxValue, a.min, a.max);
            }

            if (a.showAdvancedSettings)
            {
                GUI.color = new Color(1, 1, 1, 0.19f);
                GUI.Label(arrowRect, "<size=8>▼</size>", Constant.HeaderStyle);
                GUI.color = Color.white;

                line.y += EditorGUIUtility.singleLineHeight + 2;

                minValue = Mathf.Clamp(EditorGUI.Slider(line, "      Remap Min", minValue, a.min, a.max), a.min, maxValue);

                line.y += EditorGUIUtility.singleLineHeight + 2;

                maxValue = Mathf.Clamp(EditorGUI.Slider(line, "      Remap Max", maxValue, a.min, a.max), minValue, a.max);
            }
            else
            {
                GUI.color = new Color(1, 1, 1, 0.19f);
                GUI.Label(arrowRect, "<size=8>►</size>", Constant.HeaderStyle);
                GUI.color = Color.white;
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (value.w == 0)
                {
                    value.x = minValue;
                    value.y = maxValue;
                }
                else
                {
                    value.x = maxValue;
                    value.y = minValue;
                }

                value.z = Mathf.Abs(value.y - value.x) > 0.0001f ? 1.0f / (value.y - value.x) : 0.0f;

                if (a.supportInvert)
                {
                    property.vector4Value = value;
                }
                else
                {
                    property.vector2Value = new Vector2(value.x, value.y);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            StyledRemap a = (StyledRemap)attribute;

            if (a.showAdvancedSettings)
            {
                return EditorGUIUtility.singleLineHeight * 3 + 4;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }
    }
}