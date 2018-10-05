using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.IMGUI.Controls;

namespace AssetStoreImporter
{
    internal static class CustomUI
    {
        private static GUIStyle m_TableListStyle;

        private static GUIStyle CreateTableListStyle()
        {
            var style = new GUIStyle("CN Box");
            style.margin.top = 0;
            style.padding.left = 3;
            return style;
        }

        public static void RenderTable(TreeView treeView, ref Vector2 scroll)
        {
            if (m_TableListStyle == null)
            {
                m_TableListStyle = CreateTableListStyle();
            }

            EditorGUILayout.BeginVertical(m_TableListStyle);
            GUILayout.Space(2f);
            scroll = EditorGUILayout.BeginScrollView(scroll, new GUILayoutOption[]
            {
                // GUILayout.ExpandWidth(true),
                // GUILayout.MaxWidth(2000f)
                // GUILayout.Width(250f),
                // GUILayout.ExpandHeight(true),
            });
            var controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[]
            {
                GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(true),
            });

            treeView?.OnGUI(controlRect);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}
