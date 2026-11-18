

using System.IO;
using System.Linq;
using System.Text;
using Unity.Scripting.LifecycleManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace Boxophobic.Utility
{
    [NoAutoStaticsCleanup]
    public static class BoxoUtils
    {
        public static string GetProjectPipeline()
        {
            string pipeline = "Standard";

            if (GraphicsSettings.defaultRenderPipeline != null)
            {
                if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("Universal"))
                {
                    pipeline = "Universal";
                }

                if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("HD"))
                {
                    pipeline = "High Definition";
                }
            }

            if (QualitySettings.renderPipeline != null)
            {
                if (QualitySettings.renderPipeline.GetType().ToString().Contains("Universal"))
                {
                    pipeline = "Universal";
                }

                if (QualitySettings.renderPipeline.GetType().ToString().Contains("HD"))
                {
                    pipeline = "High Definition";
                }
            }

            return pipeline;
        }

        // Material Utils
        public static void SetMaterialBool(Material material, string valueProp, string internalProp)
        {
            if (material.HasProperty(valueProp))
            {
                var value = material.GetFloat(valueProp);

                if (value > 0)
                {
                    material.SetInt(internalProp, 1);
                }
                else
                {
                    material.SetInt(internalProp, 0);
                }
            }
        }

        public static void SetMaterialInt(Material material, string valueProp, string internalProp)
        {
            if (material.HasProperty(valueProp))
            {
                material.SetInt(internalProp, material.GetInt(valueProp));
            }
        }

        public static void SetMaterialFloat(Material material, string valueProp, string internalProp)
        {
            if (material.HasProperty(valueProp))
            {
                material.SetFloat(internalProp, material.GetFloat(valueProp));
            }
        }

        public static void SetMaterialVector(Material material, string valueProp, string internalProp)
        {
            if (material.HasProperty(valueProp))
            {
                material.SetVector(internalProp, material.GetVector(valueProp));
            }
        }

        public static void SetMaterialTexture(Material material, string valueProp, string internalProp)
        {
            if (material.HasProperty(valueProp))
            {
                material.SetTexture(internalProp, material.GetTexture(valueProp));
            }
        }

        public static void SetMaterialCoords(Material material, string modeProp, string valueProp, string internalProp)
        {
            if (material.HasProperty(modeProp) && material.HasProperty(valueProp))
            {
                var mode = material.GetInt(modeProp);
                var value = material.GetVector(valueProp);

                if (mode == 0)
                {
                    material.SetVector(internalProp, value);
                }
                else if (mode == 1)
                {
                    material.SetVector(internalProp, new Vector4(1 / value.x, 1 / value.y, value.z, value.w));
                }
            }
        }

        public static void SetMaterialBounds(Material material, string modeProp, string valueProp, string internalProp)
        {
            var offset = 0.0f;

            if (material.HasProperty(modeProp) && material.HasProperty(valueProp))
            {
                var mode = material.GetInt(modeProp);

                if (mode == 1)
                {
                    offset = 0.5f;
                }

                var value = material.GetVector(valueProp);
                var scale = new Vector2(1 / value.z, 1 / value.w);
                var pos = new Vector2(value.x * scale.x - offset, value.y * scale.y - offset) * -1;

                material.SetVector(internalProp, new Vector4(scale.x, scale.y, pos.x, pos.y));
            }
        }

        public static void SetMaterialOptions(Material material, string modeProp, string valueProp)
        {
            if (material.HasProperty(modeProp))
            {
                var mode = material.GetInt(modeProp);

                if (mode == 0)
                {
                    material.SetVector(valueProp, new Vector4(1, 0, 0, 0));
                }
                else if (mode == 1)
                {
                    material.SetVector(valueProp, new Vector4(0, 1, 0, 0));
                }
                else if (mode == 2)
                {
                    material.SetVector(valueProp, new Vector4(0, 0, 1, 0));
                }
                else if (mode == 3)
                {
                    material.SetVector(valueProp, new Vector4(0, 0, 0, 1));
                }
            }
        }

        public static void SetMaterialOptions(Material material, string modeProp, string valuePropA, string valuePropB)
        {
            if (material.HasProperty(modeProp))
            {
                var mode = material.GetInt(modeProp);

                if (mode == 0)
                {
                    material.SetVector(valuePropA, new Vector4(1, 0, 0, 0));
                    material.SetVector(valuePropB, Vector4.zero);
                }
                else if (mode == 1)
                {
                    material.SetVector(valuePropA, new Vector4(0, 1, 0, 0));
                    material.SetVector(valuePropB, Vector4.zero);
                }
                else if (mode == 2)
                {
                    material.SetVector(valuePropA, new Vector4(0, 0, 1, 0));
                    material.SetVector(valuePropB, Vector4.zero);
                }
                else if (mode == 3)
                {
                    material.SetVector(valuePropA, new Vector4(0, 0, 0, 1));
                    material.SetVector(valuePropB, Vector4.zero);
                }
                else if (mode == 4)
                {
                    material.SetVector(valuePropA, Vector4.zero);
                    material.SetVector(valuePropB, new Vector4(1, 0, 0, 0));
                }
                else if (mode == 5)
                {
                    material.SetVector(valuePropA, Vector4.zero);
                    material.SetVector(valuePropB, new Vector4(0, 1, 0, 0));
                }
                else if (mode == 6)
                {
                    material.SetVector(valuePropA, Vector4.zero);
                    material.SetVector(valuePropB, new Vector4(0, 0, 1, 0));
                }
                else if (mode == 7)
                {
                    material.SetVector(valuePropA, Vector4.zero);
                    material.SetVector(valuePropB, new Vector4(0, 0, 0, 1));
                }
            }
        }

        public static void SetMaterialBackface(Material material, string modeProp, string valueProp)
        {
            if (material.HasProperty(modeProp))
            {
                // None 0 / Flip 1 / Mirror 2
                var mode = material.GetInt(modeProp);

                if (mode == 0)
                {
                    material.SetVector(valueProp, new Vector4(1, 1, 1, 0));
                }
                else if (mode == 1)
                {
                    material.SetVector(valueProp, new Vector4(-1, -1, -1, 0));
                }
                else if (mode == 2)
                {
                    material.SetVector(valueProp, new Vector4(1, 1, -1, 0));
                }
            }
        }

        public static void SetMaterialBackfaceLegacy(Material material, string modeProp, string valueProp)
        {
            if (material.HasProperty(modeProp))
            {
                // Flip 0 / Mirror 1 / None 2
                var mode = material.GetInt(modeProp);

                if (mode == 0)
                {
                    material.SetVector(valueProp, new Vector4(-1, -1, -1, 0));
                }
                else if (mode == 1)
                {
                    material.SetVector(valueProp, new Vector4(1, 1, -1, 0));
                }
                else if (mode == 2)
                {
                    material.SetVector(valueProp, new Vector4(1, 1, 1, 0));
                }
            }
        }

        public static void SetMaterialReciprocal(Material material, string valueProp)
        {
            if (material.HasProperty(valueProp))
            {
                var value = material.GetVector(valueProp);

                material.SetVector(valueProp, new Vector4(value.x, value.y, 1 / (value.y - value.x), value.w));
            }
        }

        public static void SetMaterialKeyword(Material material, string keyword, bool enable)
        {
            if (enable)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }

        public static void SetMaterialKeyword(Material material, string keyword, float enable)
        {
            if (enable > 0)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }

        public static void SetMaterialKeyword(Material material, string property, string keyword)
        {
            if (material.HasFloat(property))
            {
                var mode = material.GetFloat(property);

                if (mode == 0)
                {
                    material.DisableKeyword(keyword);
                }
                else
                {
                    material.EnableKeyword(keyword);
                }
            }
        }

        public static void SetMaterialKeyword(Material material, string property, string[] keywords)
        {
            if (material.HasFloat(property))
            {
                var mode = material.GetFloat(property);

                for (int i = 0; i < keywords.Length; i++)
                {
                    if (i == mode)
                    {
                        material.EnableKeyword(keywords[i]);
                    }
                    else
                    {
                        material.DisableKeyword(keywords[i]);
                    }
                }
            }
            else
            {
                material.EnableKeyword(keywords[0]);

                for (int i = 1; i < keywords.Length; i++)
                {
                    material.DisableKeyword(keywords[i]);
                }
            }
        }

        public static void SetMaterialKeyword(Material material, string property, string[] keywords, int[] indices)
        {
            if (material.HasFloat(property))
            {
                var mode = material.GetFloat(property);

                for (int i = 0; i < keywords.Length; i++)
                {
                    if (indices[i] == mode)
                    {
                        material.EnableKeyword(keywords[i]);
                    }
                    else
                    {
                        material.DisableKeyword(keywords[i]);
                    }
                }
            }
            else
            {
                material.EnableKeyword(keywords[0]);

                for (int i = 1; i < keywords.Length; i++)
                {
                    material.DisableKeyword(keywords[i]);
                }
            }
        }

        public static void SetMaterialKeyword(Material material, string parent, string property, string keyword)
        {
            if (material.HasFloat(parent) && material.HasFloat(property))
            {
                var parentMode = material.GetFloat(parent);

                if (parentMode > 0)
                {
                    var propertyMode = material.GetFloat(property);

                    if (propertyMode == 0)
                    {
                        material.DisableKeyword(keyword);
                    }
                    else
                    {
                        material.EnableKeyword(keyword);
                    }
                }
                else
                {
                    material.DisableKeyword(keyword);
                }
            }
        }

        public static void SetMaterialKeyword(Material material, string parent, string property, string[] keywords)
        {
            if (material.HasFloat(parent) && material.HasFloat(property))
            {
                var parentMode = material.GetFloat(parent);

                if (parentMode > 0)
                {
                    var propertyMode = material.GetFloat(property);

                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (i == propertyMode)
                        {
                            material.EnableKeyword(keywords[i]);
                        }
                        else
                        {
                            material.DisableKeyword(keywords[i]);
                        }
                    }
                }
                else
                {
                    material.EnableKeyword(keywords[0]);

                    for (int i = 1; i < keywords.Length; i++)
                    {
                        material.DisableKeyword(keywords[i]);
                    }
                }
            }
        }

        public static void SetMaterialKeyword(Material material, string parent, string property, string[] keywords, int[] indices)
        {
            if (material.HasFloat(parent) && material.HasFloat(property))
            {
                var parentMode = material.GetFloat(parent);

                if (parentMode > 0)
                {
                    var propertyMode = material.GetFloat(property);

                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (indices[i] == propertyMode)
                        {
                            material.EnableKeyword(keywords[i]);
                        }
                        else
                        {
                            material.DisableKeyword(keywords[i]);
                        }
                    }
                }
                else
                {
                    material.EnableKeyword(keywords[0]);

                    for (int i = 1; i < keywords.Length; i++)
                    {
                        material.DisableKeyword(keywords[i]);
                    }
                }
            }
        }

        public static void SetMaterialKeyword(Material material, bool allParentsOn, string[] parents, string property, string keyword)
        {
            bool parentsMode = false;
            float propertyMode = 0;

            if (allParentsOn)
            {
                int enableCount = 0;

                for (int i = 0; i < parents.Length; i++)
                {
                    var parent = parents[i];

                    if (material.HasProperty(parent))
                    {
                        if (material.GetFloat(parent) > 0)
                        {
                            enableCount++;
                        }
                    }
                }

                if (parents.Length == enableCount)
                {
                    parentsMode = true;
                }
            }
            else
            {
                float enableFloat = 0;

                for (int i = 0; i < parents.Length; i++)
                {
                    var parent = parents[i];

                    if (material.HasProperty(parent))
                    {
                        enableFloat += material.GetFloat(parent);
                    }
                }

                if (enableFloat > 0)
                {
                    parentsMode = true;
                }
            }

            if (material.HasProperty(property))
            {
                propertyMode = material.GetFloat(property);
            }

            if (parentsMode && propertyMode > 0)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }

        public static void SetMaterialKeyword(Material material, bool allParentsOn, string[] parents, string property, string[] keywords)
        {
            bool parentsMode = false;

            if (allParentsOn)
            {
                int enableCount = 0;

                for (int i = 0; i < parents.Length; i++)
                {
                    var parent = parents[i];

                    if (material.HasProperty(parent))
                    {
                        if (material.GetFloat(parent) > 0)
                        {
                            enableCount++;
                        }
                    }
                }

                if (parents.Length == enableCount)
                {
                    parentsMode = true;
                }
            }
            else
            {
                float enableFloat = 0;

                for (int i = 0; i < parents.Length; i++)
                {
                    var parent = parents[i];

                    if (material.HasProperty(parent))
                    {
                        enableFloat += material.GetFloat(parent);
                    }
                }

                if (enableFloat > 0)
                {
                    parentsMode = true;
                }
            }

            if (material.HasFloat(property))
            {
                if (parentsMode)
                {
                    var propertyMode = material.GetFloat(property);

                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (i == propertyMode)
                        {
                            material.EnableKeyword(keywords[i]);
                        }
                        else
                        {
                            material.DisableKeyword(keywords[i]);
                        }
                    }
                }
                else
                {
                    material.EnableKeyword(keywords[0]);

                    for (int i = 1; i < keywords.Length; i++)
                    {
                        material.DisableKeyword(keywords[i]);
                    }
                }
            }
        }

        public static void SetMaterialKeyword(Material material, bool allParentsOn, string[] parents, string property, string[] keywords, int[] indices)
        {
            bool parentsMode = false;

            if (allParentsOn)
            {
                int enableCount = 0;

                for (int i = 0; i < parents.Length; i++)
                {
                    var parent = parents[i];

                    if (material.HasProperty(parent))
                    {
                        if (material.GetFloat(parent) > 0)
                        {
                            enableCount++;
                        }
                    }
                }

                if (parents.Length == enableCount)
                {
                    parentsMode = true;
                }
            }
            else
            {
                float enableFloat = 0;

                for (int i = 0; i < parents.Length; i++)
                {
                    var parent = parents[i];

                    if (material.HasProperty(parent))
                    {
                        enableFloat += material.GetFloat(parent);
                    }
                }

                if (enableFloat > 0)
                {
                    parentsMode = true;
                }
            }

            if (material.HasFloat(property))
            {
                if (parentsMode)
                {
                    var propertyMode = material.GetFloat(property);

                    for (int i = 0; i < keywords.Length; i++)
                    {
                        if (indices[i] == propertyMode)
                        {
                            material.EnableKeyword(keywords[i]);
                        }
                        else
                        {
                            material.DisableKeyword(keywords[i]);
                        }
                    }
                }
                else
                {
                    material.EnableKeyword(keywords[0]);

                    for (int i = 1; i < keywords.Length; i++)
                    {
                        material.DisableKeyword(keywords[i]);
                    }
                }
            }
        }

        public static void SetMaterialKeyword(Material material, bool allParentsOn, string[] properties, string keyword)
        {
            bool parentMode = false;

            if (allParentsOn)
            {
                int enableCount = 0;

                for (int i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];

                    if (material.HasProperty(property))
                    {
                        if (material.GetFloat(property) > 0)
                        {
                            enableCount++;
                        }
                    }
                }

                if (properties.Length == enableCount)
                {
                    parentMode = true;
                }
            }
            else
            {
                float enableFloat = 0;

                for (int i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];

                    if (material.HasProperty(property))
                    {
                        enableFloat += material.GetFloat(property);
                    }
                }

                if (enableFloat > 0)
                {
                    parentMode = true;
                }
            }

            if (parentMode)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }

        public static void SetMaterialKeywordInverted(Material material, string property, string keyword)
        {
            if (material.HasFloat(property))
            {
                var mode = material.GetFloat(property);

                if (mode == 0)
                {
                    material.EnableKeyword(keyword);
                }
                else
                {
                    material.DisableKeyword(keyword);
                }
            }
        }

        public static void SetMaterialKeywordByTexture(Material material, string property, string keyword)
        {
            if (IsMaterialTextureUsed(material, property))
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }

        public static void SetMaterialTextureSpace(Material material, string texProp, string spaceProp)
        {
            var spaceMode = 0;

            if (material.HasTexture(texProp))
            {
                var texture = material.GetTexture(texProp);

                if (texture != null)
                {
                    if (texture.isDataSRGB)
                    {
                        spaceMode = 1;
                    }
                }
            }

            material.SetFloat(spaceProp, spaceMode);
        }

        public static float GetMaterialFloat(Material material, string property, float defaultValue)
        {
            float value = defaultValue;

            if (material.HasFloat(property))
            {
                value = material.GetFloat(property);
            }

            return value;
        }

        public static float GetMaterialFloat(Material material, string property)
        {
            return GetMaterialFloat(material, property, 0);
        }

        public static int GetMaterialInt(Material material, string property, int defaultValue)
        {
            int value = defaultValue;

            if (material.HasFloat(property))
            {
                value = material.GetInt(property);
            }

            return value;
        }

        public static int GetMaterialInt(Material material, string property)
        {
            return GetMaterialInt(material, property, 0);
        }

        public static Texture GetMaterialTexture(Material material, string property)
        {
            Texture value = null;

            if (material.HasTexture(property))
            {
                value = material.GetTexture(property);
            }

            return value;
        }

        public static bool IsMaterialTextureUsed(Material material, string property)
        {
            bool value = false;

            if (material.HasTexture(property))
            {
                if (material.GetTexture(property) != null)
                {
                    value = true;
                }
            }

            return value;
        }

        // Math Utils
        public static float MathRemap(float value, float minOld, float maxOld, float minNew, float maxNew)
        {
            return minNew + (value - minOld) * (maxNew - minNew) / (maxOld - minOld);
        }

        public static float MathRemap(float value, float minOld, float maxOld)
        {
            return (value - minOld) / (maxOld - minOld);
        }

        public static float MathVector2ToFloat(float x, float y)
        {
            Vector2 output;

            output.x = Mathf.Floor(x * (2048 - 1));
            output.y = Mathf.Floor(y * (2048 - 1));

            return (output.x * 2048) + output.y;
        }

        public static Vector2 MathFloatFromVector2(float input)
        {
            Vector2 output;

            output.y = input % 2048f;
            output.x = Mathf.Floor(input / 2048f);

            return output / (2048f - 1);
        }

        // Text Utils
        public static string FormatEnum(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            var sb = new StringBuilder(text.Length);

            for (int i = 0; i < text.Length; i++)
            {
                if (i < text.Length - 2)
                {
                    string token = text.Substring(i, 3);

                    switch (token)
                    {
                        case "MIN": sb.Append('-'); i += 2; continue;
                        case "OPA": sb.Append('('); i += 2; continue;
                        case "CPA": sb.Append(')'); i += 2; continue;
                    }
                }

                if (text[i] == '_')
                {
                    sb.Append(' ');
                    continue;
                }

                sb.Append(text[i]);
            }

            return sb.ToString();
        }
        public static string FormatTitle(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            var sb = new StringBuilder(text.Length);

            for (int i = 0; i < text.Length; i++)
            {
                if (i < text.Length - 2)
                {
                    string token = text.Substring(i, 3);

                    switch (token)
                    {
                        case "MIN": sb.Append('-'); i += 2; continue;
                        case "VER": sb.Append('|'); i += 2; continue;
                        case "OPA": sb.Append('('); i += 2; continue;
                        case "CPA": sb.Append(')'); i += 2; continue;
                    }
                }

                sb.Append(text[i]);
            }

            return sb.ToString();
        }
        public static string FormatMessage(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            var sb = new StringBuilder(text.Length);

            for (int i = 0; i < text.Length; i++)
            {
                if (i < text.Length - 2)
                {
                    string token = text.Substring(i, 3);

                    switch (token)
                    {
                        case "MIN": sb.Append('-'); i += 2; continue;
                        case "VER": sb.Append('|'); i += 2; continue;
                        case "PLU": sb.Append('+'); i += 2; continue;
                        case "NEW": sb.Append('\n'); i += 2; continue;
                        case "EXC": sb.Append('!'); i += 2; continue;
                        case "COL": sb.Append(':'); i += 2; continue;
                        case "APS": sb.Append('\''); i += 2; continue;
                        case "QUO": sb.Append('"'); i += 2; continue;
                        case "SLH": sb.Append('/'); i += 2; continue;
                        case "OPA": sb.Append('('); i += 2; continue;
                        case "CPA": sb.Append(')'); i += 2; continue;
                        case "LAR": sb.Append('<'); i += 2; continue;
                        case "RAR": sb.Append('>'); i += 2; continue;
                        case "EQU": sb.Append('='); i += 2; continue;
                        case "HAS": sb.Append('#'); i += 2; continue;
                        case "AST": sb.Append('*'); i += 2; continue;
                        case "BUL": sb.Append('◦'); i += 2; continue;
                    }
                }

                if (i < text.Length - 1)
                {
                    string token2 = text.Substring(i, 2);
                    if (token2 == "__") { sb.Append(','); i++; continue; }
                }

                sb.Append(text[i]);
            }

            return sb.ToString();
        }

        public static string FormatMessageReverse(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return "";
            }

            var sb = new StringBuilder(message.Length);

            foreach (char c in message)
            {
                switch (c)
                {
                    case '-': sb.Append("MIN"); break;
                    case '|': sb.Append("VER"); break;
                    case '+': sb.Append("PLU"); break;
                    case '\n': sb.Append("NEW"); break;
                    case '!': sb.Append("EXC"); break;
                    case ':': sb.Append("COL"); break;
                    case '\'': sb.Append("APS"); break;
                    case '"': sb.Append("QUO"); break;
                    case '/': sb.Append("SLH"); break;
                    case '(': sb.Append("OPA"); break;
                    case ')': sb.Append("CPA"); break;
                    case '<': sb.Append("LAR"); break;
                    case '>': sb.Append("RAR"); break;
                    case '=': sb.Append("EQU"); break;
                    case '#': sb.Append("HAS"); break;
                    case '*': sb.Append("AST"); break;
                    case '◦': sb.Append("BUL"); break;
                    case ',': sb.Append("__"); break;
                    default: sb.Append(c); break;
                }
            }

            return sb.ToString();
        }

#if UNITY_EDITOR
        public static float GetMaterialSerializedFloat(Material material, string internalName, float defaultValue)
        {
            float value = defaultValue;

            if (EditorUtility.IsPersistent(material))
            {
                var so = new SerializedObject(material);
                var itr = so.GetIterator();

                while (itr.Next(true))
                {
                    if (itr.displayName == internalName)
                    {
                        if (itr.hasChildren)
                        {
                            var itrC = itr.Copy();
                            itrC.Next(true); //Walk into child ("First")
                            itrC.Next(false); //Walk into sibling ("Second")

                            value = itrC.floatValue;
                        }
                    }
                }
            }

            return value;
        }

        public static Vector4 GetMaterialSerializedVector(Material material, string internalName, Vector4 defaultValue)
        {
            Vector4 value = defaultValue;

            if (EditorUtility.IsPersistent(material))
            {
                var so = new SerializedObject(material);
                var itr = so.GetIterator();

                while (itr.Next(true))
                {
                    if (itr.displayName == internalName)
                    {
                        if (itr.hasChildren)
                        {
                            var itrC = itr.Copy();
                            itrC.Next(true); //Walk into child ("First")
                            itrC.Next(false); //Walk into sibling ("Second")

                            value = itrC.colorValue;
                        }
                    }
                }
            }

            return value;
        }

        public static Vector4 GetMaterialSerializedVector(Material material, string internalNameMin, string internalNameMax, Vector4 defaultValue)
        {
            Vector4 value = defaultValue;

            value.x = GetMaterialSerializedFloat(material, internalNameMin, 0);
            value.y = GetMaterialSerializedFloat(material, internalNameMax, 1);

            if (value.x > value.y)
            {
                value.w = 1;
            }

            return value;
        }

        public static Texture2D GetMaterialSerializedTexture(Material material, string internalName, Texture2D defaultValue)
        {
            Texture2D value = defaultValue;

            if (EditorUtility.IsPersistent(material))
            {
                var so = new SerializedObject(material);
                var itr = so.GetIterator();

                while (itr.Next(true))
                {
                    if (itr.displayName == internalName)
                    {
                        if (itr.hasChildren)
                        {
                            var itrC = itr.Copy();
                            itrC.Next(true); //Walk into child ("First")
                            itrC.Next(false); //Walk into sibling ("Second")

                            if (itrC.hasChildren)
                            {
                                var itrT = itrC.Copy();
                                itrT.Next(true); //Walk into child ("First")
                                value = (Texture2D)itrT.objectReferenceValue;
                            }
                        }
                    }
                }
            }

            return value;
        }

        public static bool IsShaderGUIPropertyHidden(MaterialProperty property)
        {
#if UNITY_6000_2_OR_NEWER
            if (property.propertyFlags == UnityEngine.Rendering.ShaderPropertyFlags.HideInInspector)
#else
            if (property.flags == MaterialProperty.PropFlags.HideInInspector)
#endif
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsShaderGUIPropertyTexture(MaterialProperty property)
        {
#if UNITY_6000_2_OR_NEWER
            if (property.propertyType == UnityEngine.Rendering.ShaderPropertyType.Texture)
#else
            if (property.type == MaterialProperty.PropType.Texture)
#endif
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsShaderGUIPropertyFloat(MaterialProperty property)
        {
#if UNITY_6000_2_OR_NEWER
            if (property.propertyType == UnityEngine.Rendering.ShaderPropertyType.Float || property.propertyType == UnityEngine.Rendering.ShaderPropertyType.Int || property.propertyType == UnityEngine.Rendering.ShaderPropertyType.Range)
#else
            if (property.type == MaterialProperty.PropType.Float || property.type == MaterialProperty.PropType.Int || property.type == MaterialProperty.PropType.Range)
#endif
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsShaderGUIPropertyVector(MaterialProperty property)
        {
#if UNITY_6000_2_OR_NEWER
            if (property.propertyType == UnityEngine.Rendering.ShaderPropertyType.Vector || property.propertyType == UnityEngine.Rendering.ShaderPropertyType.Color)
#else
            if (property.type == MaterialProperty.PropType.Vector || property.type == MaterialProperty.PropType.Color)
#endif
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetShaderPropertyCount(Shader shader)
        {
#if UNITY_6000_2_OR_NEWER
            return shader.GetPropertyCount();
#else
            return ShaderUtil.GetPropertyCount(shader);
#endif
        }

        public static string GetShaderPropertyName(Shader shader, int index)
        {
#if UNITY_6000_2_OR_NEWER
            return shader.GetPropertyName(index);
#else
            return ShaderUtil.GetPropertyName(shader, index);
#endif
        }

        public static ShaderPropertyType GetShaderPropertyType(Shader shader, int index)
        {
#if UNITY_6000_2_OR_NEWER
            return shader.GetPropertyType(index);
#else
            return (ShaderPropertyType)ShaderUtil.GetPropertyType(shader, index);
#endif
        }


        public static string GetAssetFolder(string searchFile, string defaultPath)
        {

            var folder = FindAsset(searchFile);
            folder = folder.Replace("/" + searchFile, "");

            if (folder == "")
            {
                folder = defaultPath;
            }

            return folder;
        }

        public static string GetUserFolder()
        {
            if (!Directory.Exists(BoxoGlobals.userFolder))
            {
                string[] guids = AssetDatabase.FindAssets("t:DefaultAsset " + "BOXOPHOBIC+");

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    if (AssetDatabase.IsValidFolder(path) && Path.GetFileName(path) == "BOXOPHOBIC+")
                    {
                        BoxoGlobals.userFolder = path;
                    }
                }
            }

            return BoxoGlobals.userFolder;
        }

        public static string[] FindAssets(string filter, bool sort)
        {
            var assetPaths = AssetDatabase.FindAssets("glob:\"" + filter + "\"");

            if (sort)
            {
                assetPaths = assetPaths.OrderBy(f => new FileInfo(f).Name).ToArray();
            }

            for (int i = 0; i < assetPaths.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetPaths[i]);
            }

            return assetPaths;
        }

        public static string FindAsset(string filter)
        {
            var assetPath = "";

            var assetGUIDs = AssetDatabase.FindAssets("glob:\"" + filter + "\"");

            if (assetGUIDs != null && assetGUIDs.Length > 0)
            {
                assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDs[0]);
            }

            return assetPath;
        }

        public static void ImportPackage(string packagePath, bool interactive)
        {
#if UNITY_6000_6_OR_NEWER
            UnityEditor.AssetPackage.Package.Import(packagePath, interactive);
#else
            AssetDatabase.ImportPackage(packagePath, interactive);
#endif
        }

        public static void ExportPackage(string[] exportArr, string exportPath, string exportOrg, ExportPackageOptions exportOption)
        {
#if UNITY_6000_6_OR_NEWER
            var parameters = new UnityEditor.AssetPackage.ExportPackageParameters(exportArr, exportPath, exportOrg, exportOption);

            UnityEditor.AssetPackage.Package.Export(parameters);
#else
            AssetDatabase.ExportPackage(exportArr, exportPath, exportOption);
#endif
        }

        public static void SetDefineSymbol(string symbol)
        {
#if UNITY_2023_1_OR_NEWER
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
#else
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif

            if (!defineSymbols.Contains(symbol))
            {
                defineSymbols += ";" + symbol + ";";

#if UNITY_2023_1_OR_NEWER
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defineSymbols);
#else
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defineSymbols);
#endif
            }
        }

        public static void SetDefineSymbols(string[] symbols)
        {
#if UNITY_2023_1_OR_NEWER
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
#else
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif

            for (int i = 0; i < symbols.Length; i++)
            {
                var symbol = symbols[i];

                if (!defineSymbols.Contains(symbol))
                {
                    defineSymbols += ";" + symbol + ";";

#if UNITY_2023_1_OR_NEWER
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defineSymbols);
#else
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defineSymbols);
#endif
                }
            }
        }

        public static void SetDefineSymbol(string symbol, string version)
        {
#if UNITY_2023_1_OR_NEWER
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
#else
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif

            var defineSymbolsSplit = defineSymbols.Split(";");

            var newDefineSymbols = "";

            var versionFormat = symbol + "_V";
            var versionSymbol = symbol + "_V" + version;

            for (int i = 0; i < defineSymbolsSplit.Length; i++)
            {
                var define = defineSymbolsSplit[i];

                if (define.Contains(versionFormat))
                {
                    defineSymbolsSplit[i] = versionSymbol;
                }

                newDefineSymbols = newDefineSymbols + defineSymbolsSplit[i] + ";";
            }

            if (!newDefineSymbols.Contains(symbol))
            {
                newDefineSymbols = newDefineSymbols + symbol + ";";
            }

            if (!newDefineSymbols.Contains(versionFormat))
            {
                newDefineSymbols = newDefineSymbols + versionSymbol + ";";
            }

#if UNITY_2023_1_OR_NEWER
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newDefineSymbols);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefineSymbols);
#endif
        }

        public static void RemoveDefineSymbol(string symbol)
        {
#if UNITY_2023_1_OR_NEWER
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
#else
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif

            defineSymbols = defineSymbols.Replace(symbol + ";", "");
            defineSymbols = defineSymbols.Replace(symbol, ""); // define symbol is the last

#if UNITY_2023_1_OR_NEWER
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defineSymbols);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defineSymbols);
#endif

        }

        public static void RemoveDefineSymbols(string[] symbols)
        {
#if UNITY_2023_1_OR_NEWER
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
#else
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif

            for (int i = 0; i < symbols.Length; i++)
            {
                var symbol = symbols[i];

                defineSymbols = defineSymbols.Replace(symbol + ";", "");
                defineSymbols = defineSymbols.Replace(symbol, ""); // define symbol is the last

#if UNITY_2023_1_OR_NEWER
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defineSymbols);
#else
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defineSymbols);
#endif
            }
        }

        public static bool HasDefineSymbol(string symbol)
        {
            bool hasSymbol = false;

#if UNITY_2023_1_OR_NEWER
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
#else
            var defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif

            if (defineSymbols.Contains(symbol))
            {
                hasSymbol = true;
            }

            return hasSymbol;
        }
#endif

        //Misc Utils
        public static void DestryObject(UnityEngine.Object objectToDestory)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(objectToDestory);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(objectToDestory);
            }
#else
            UnityEngine.Object.Destroy(objectToDestory);
#endif
        }

        public static bool DisableServerExecution()
        {
#if UNITY_SERVER
            return true;
#else
            return Application.isBatchMode ||
                   SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
#endif
        }

        public static Light[] FindAllLights()
        {
#if UNITY_6000_6_OR_NEWER
            return UnityEngine.Object.FindObjectsByType<Light>();
#elif UNITY_2023_1_OR_NEWER
            return UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
#else
            return UnityEngine.Object.FindObjectsOfType<Light>();
#endif
        }

        public static GameObject[] FindAllGameObjects()
        {
#if UNITY_6000_6_OR_NEWER
            return UnityEngine.Object.FindObjectsByType<GameObject>();
#elif UNITY_2023_1_OR_NEWER
            return UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
#else
            return UnityEngine.Object.FindObjectsOfType<GameObject>();
#endif
        }

        public class BoxoGlobals
        {
            public static string userFolder = "Assets/Skybox_TimeApe+";
        }

        [System.Serializable]
        public class ProjectData
        {
            public string pipeline = "";
            public string minimum = "";
            public string package = "";
            public bool isSupported = true;
            public bool isTechRelease = false;
            public bool isAlphaOrBetaRelease = false;

            public ProjectData()
            {

            }
        }
    }
}
