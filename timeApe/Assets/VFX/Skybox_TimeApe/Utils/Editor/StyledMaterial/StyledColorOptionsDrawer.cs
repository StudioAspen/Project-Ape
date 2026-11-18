// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;
using Boxophobic.Utility;
using Boxophobic.Constants;

namespace Boxophobic.StyledGUI
{
    public class StyledColorOptionsDrawer : MaterialPropertyDrawer
    {
        public string rOption = "";
        public string gOption = "";
        public string bOption = "";
        public string aOption = "";
        public float top = 0;
        public float down = 0;

        bool showAdvancedSettings = false;

        public StyledColorOptionsDrawer(string rOption, string gOption, string bOption, string aOption)
        {
            this.rOption = rOption;
            this.gOption = gOption;
            this.bOption = bOption;
            this.aOption = aOption;
        }

        public StyledColorOptionsDrawer(string rOption, string gOption, string bOption, string aOption, float top, float down)
        {
            this.rOption = rOption;
            this.gOption = gOption;
            this.bOption = bOption;
            this.aOption = aOption;

            this.top = top;
            this.down = down;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            float y = position.y + top;
            float height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;

            Rect buttonRect = new Rect(position.x - 14, y, EditorGUIUtility.labelWidth - 1, height);
            Rect labelRect = new Rect(position.x, y, EditorGUIUtility.labelWidth - 1, height);
            Rect fieldRect = new Rect(position.x + EditorGUIUtility.labelWidth + 2, y, position.width - EditorGUIUtility.labelWidth - 2, height);
            Rect arrowRect = new Rect(position.x - 14, position.y - 2, 18, 18);

            var tooltip = BoxoUtils.FormatEnum(rOption + " " + gOption + " " + bOption + " " + aOption);
            GUIContent content = new GUIContent(label, tooltip);
            //EditorGUI.LabelField(labelRect, label);

            if (GUI.Button(buttonRect, "", GUIStyle.none))
            {
                showAdvancedSettings = !showAdvancedSettings;
            }

            EditorGUI.LabelField(labelRect, content);
            Color color = EditorGUI.ColorField(fieldRect, GUIContent.none, prop.colorValue, false, true, false);

            y += height + EditorGUIUtility.standardVerticalSpacing;

            if (showAdvancedSettings)
            {
                GUI.color = new Color(1, 1, 1, 0.19f);
                GUI.Label(arrowRect, "<size=8>▼</size>", Constant.HeaderStyle);
                GUI.color = Color.white;

                Rect rRect = new Rect(position.x, y, position.width, height);
                color.r = EditorGUI.Slider(rRect, "      " + rOption, color.r, 0, 1);

                y += height + EditorGUIUtility.standardVerticalSpacing;

                Rect gRect = new Rect(position.x, y, position.width, height);
                color.g = EditorGUI.Slider(gRect, "      " + gOption, color.g, 0, 1);

                y += height + EditorGUIUtility.standardVerticalSpacing;

                Rect bRect = new Rect(position.x, y, position.width, height);
                color.b = EditorGUI.Slider(bRect, "      " + bOption, color.b, 0, 1);

                y += height + EditorGUIUtility.standardVerticalSpacing;

                Rect aRect = new Rect(position.x, y, position.width, height);
                color.a = EditorGUI.Slider(aRect, "      " + aOption, color.a, 0, 1);
            }
            else
            {
                GUI.color = new Color(1, 1, 1, 0.19f);
                GUI.Label(arrowRect, "<size=8>►</size>", Constant.HeaderStyle);
                GUI.color = Color.white;
            }

            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                prop.colorValue = color;
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            if (showAdvancedSettings)
            {
                return top + EditorGUIUtility.singleLineHeight * 5 + EditorGUIUtility.standardVerticalSpacing * 4 + down;
            }
            else
            {
                return top + EditorGUIUtility.singleLineHeight + down;
            }
        }
    }
}