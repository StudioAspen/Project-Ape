// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic.StyledGUI
{
    public class StyledVector2Drawer : MaterialPropertyDrawer
    {
        public float top = 0;
        public float down = 0;

        public StyledVector2Drawer()
        {

        }

        public StyledVector2Drawer(float top, float down)
        {
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
                labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth - 1, position.height);
                fieldRect = new Rect(position.x + EditorGUIUtility.labelWidth + 2, position.y, position.width - EditorGUIUtility.labelWidth - 2, position.height);
            }
            else
            {
                labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                fieldRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            }

            EditorGUI.LabelField(labelRect, label);

            Vector4 vector = EditorGUI.Vector2Field(fieldRect, GUIContent.none, prop.vectorValue);

            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
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