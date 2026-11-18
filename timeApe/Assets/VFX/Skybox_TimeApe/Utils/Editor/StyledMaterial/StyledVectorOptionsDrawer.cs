// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;
using Boxophobic.Utility;

namespace Boxophobic.StyledGUI
{
    public class StyledVectorOptionsDrawer : MaterialPropertyDrawer
    {
        public float show = 4;
        public float top = 0;
        public float down = 0;
        public string xOption, yOption, zOption, wOption = "";
        public string action = "";

        public StyledVectorOptionsDrawer(float show)
        {
            this.show = show;
        }

        public StyledVectorOptionsDrawer(float show, string xOption, string yOption, string zOption, string wOption, string action, float top, float down)
        {
            this.show = show;
            this.xOption = xOption;
            this.yOption = yOption;
            this.zOption = zOption;
            this.wOption = wOption;
            this.action = action;
            this.top = top;
            this.down = down;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            float y = position.y + top;
            float height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;

            Rect labelRect;
            Rect fieldRect;

            if (EditorGUIUtility.currentViewWidth > 330)
            {
                labelRect = new Rect(position.x, y, EditorGUIUtility.labelWidth - 1, height);
                fieldRect = new Rect(position.x + EditorGUIUtility.labelWidth + 2, y, position.width - EditorGUIUtility.labelWidth - 2, height);
            }
            else
            {
                labelRect = new Rect(position.x, y, position.width, height);
                fieldRect = new Rect(position.x, y + height, position.width, height);
            }

            Vector4 vector = Vector4.zero;

            if (show == 4)
            {
                var tooltip = BoxoUtils.FormatEnum(xOption + " " + yOption + " " + zOption + " " + wOption);
                GUIContent content = new GUIContent(label, tooltip);
                EditorGUI.LabelField(labelRect, content);

                vector = EditorGUI.Vector4Field(fieldRect, GUIContent.none, prop.vectorValue);
            }
            else if (show == 3)
            {
                var tooltip = BoxoUtils.FormatEnum(xOption + " " + yOption + " " + zOption);
                GUIContent content = new GUIContent(label, tooltip);
                EditorGUI.LabelField(labelRect, content);

                vector = EditorGUI.Vector3Field(fieldRect, GUIContent.none, prop.vectorValue);
            }
            else if (show == 2)
            {
                var tooltip = BoxoUtils.FormatEnum(xOption + " " + yOption);
                GUIContent content = new GUIContent(label, tooltip);
                EditorGUI.LabelField(labelRect, content);

                vector = EditorGUI.Vector2Field(fieldRect, GUIContent.none, prop.vectorValue);
            }

            if (action.Contains("ACTION_CLAMP_01"))
            {
                vector = new Vector4(Mathf.Clamp(vector.x, 0, 1), Mathf.Clamp(vector.y, 0, 1), Mathf.Clamp(vector.z, 0, 1), Mathf.Clamp(vector.w, 0, 1));
            }
            else if (action.Contains("ACTION_CLAMP_NEGATIVE_VALUES"))
            {
                vector = new Vector4(Mathf.Clamp(vector.x, 0, Mathf.Infinity), Mathf.Clamp(vector.y, 0, Mathf.Infinity), Mathf.Clamp(vector.z, 0, Mathf.Infinity), Mathf.Clamp(vector.w, 0, Mathf.Infinity));
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorGUI.showMixedValue = false;
                prop.vectorValue = vector;
            }

            if (EditorGUIUtility.currentViewWidth > 330)
            {
                y += height;
            }
            else
            {
                y += height * 2 + 2;
            }
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (EditorGUIUtility.currentViewWidth > 330)
            {
                return top + height + down;
            }

            return top + height * 2 + 2 + down;
        }
    }
}