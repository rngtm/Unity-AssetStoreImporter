using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetStoreImporter
{
    public class ImporterWindow : EditorWindow
    {
        static readonly GUIContent ReloadContent = EditorGUIUtility.TrTextContent("Reload", "Reload UnityPackages", (Texture)null);
        static readonly GUIContent OpenAssetStoreContent = EditorGUIUtility.TrTextContent("Open AssetStore", "Open AssetStore Tab", (Texture)null);
        private FileTreeView m_TreeView;
        private Vector2 m_TableScroll = new Vector2(0f, 0f);


        [MenuItem("AssetTools/AssetStore Importer/Open Window", false, 80)]
        static void Open()
        {
            var window = GetWindow<ImporterWindow>();
            window.title = "Package Importer";
        }

        private void OnGUI()
        {
            if (m_TreeView == null)
            {
                CreateTreeView();
            }
            DrawHeader();

            CustomUI.RenderTable(m_TreeView, ref m_TableScroll);
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button(ReloadContent, EditorStyles.toolbarButton))
            {
                DoLoadFiles();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(OpenAssetStoreContent, EditorStyles.toolbarButton))
            {
                EditorApplication.ExecuteMenuItem("Window/General/Asset Store"); // open tab
                // System.Diagnostics.Process.Start("https://www.assetstore.unity3d.com"); // open link
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CreateTreeView()
        {
            m_TreeView = new FileTreeView();
        }

        private void DoLoadFiles()
        {
            string[] files = System.IO.Directory.GetFiles(AssetStore.GetAssetStoreDirectory(), "*", System.IO.SearchOption.AllDirectories);
            m_TreeView.RegisterFiles(files);
        }
    }
}
