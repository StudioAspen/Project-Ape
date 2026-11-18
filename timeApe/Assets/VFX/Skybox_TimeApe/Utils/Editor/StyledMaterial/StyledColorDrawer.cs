// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic.StyledGUI
{
    public class StyledColorDrawer : MaterialPropertyDrawer
    {
        public string showEyedropperStr = "false";
        public string showAlphaStr = "false";
        public string showHDRStr = "false";
        public float top = 0;
        public float down = 0;

        public StyledColorDrawer(string showEyedropperStr, string showAlphaStr, string showHDRStr)
        {
            this.showEyedropperStr = showEyedropperStr;
            this.showAlphaStr = showAlphaStr;
            this.showHDRStr = showHDRStr;
        }

        public StyledColorDrawer(string showEyedropperStr, string showAlphaStr, string showHDRStr, float top, float down)
        {
            this.showEyedropperStr = showEyedropperStr;
            this.showAlphaStr = showAlphaStr;
            this.showHDRStr = showHDRStr;

            this.top = top;
            this.down = down;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            float y = position.y + top;
            float height = EditorGUIUtility.singleLineHeight;

            bool showEyedropper = false;
            bool showAlpha = false;
            bool showHDR = false;

            if (showEyedropperStr == "true")
            {
                showEyedropper = true;
            }

            if (showAlphaStr == "true")
            {
                showAlpha = true;
            }

            if (showHDRStr == "true")
            {
                showHDR = true;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;

            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth - 1, position.height);
            Rect fieldRect = new Rect(position.x + EditorGUIUtility.labelWidth + 2, position.y, position.width - EditorGUIUtility.labelWidth - 2, position.height);

            EditorGUI.LabelField(labelRect, label);

            Color color = EditorGUI.ColorField(fieldRect, GUIContent.none, prop.colorValue, showEyedropper, showAlpha, showHDR);

            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                prop.colorValue = color;
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            float height = EditorGUIUtility.singleLineHeight;

            return top + height + down;
        }
    }
}