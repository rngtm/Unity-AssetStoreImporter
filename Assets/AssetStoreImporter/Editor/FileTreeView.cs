using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetStoreImporter
{
    internal class FileTreeViewItem : TreeViewItem
    {
        public string FileName { get; set; } = "No Name";
        public string FilePath { get; set; } = "No Path";
        public string FileSize { get; set; } = "0.0";
    }

    internal class FileTreeView : TreeView
    {
        static readonly Vector2 ButtonSize = new Vector2(48f, 16f);
        static readonly float ButtonSpaceX = 3f;
        static readonly float ButtonPositionY = 2f;
        static readonly int RowHeight = 20;
        static readonly string SortedColumnIndexStateKey = "AssetStoreImporterTreeView_sortedColumnIndex";
        static readonly int DefaultSortedColumnIndex = 1;
        public IReadOnlyList<TreeViewItem> CurrentBindingItems;

        public FileTreeView() // constructer
            : this(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                new MultiColumnHeaderState.Column() { headerContent = new GUIContent(""), // button
                    autoResize = false,
                    canSort = false,
                    width = ButtonSize.x + ButtonSpaceX,
                    maxWidth = ButtonSize.x + ButtonSpaceX,
                    minWidth = ButtonSize.x + ButtonSpaceX,
                    },
                new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Package Name"), // name
                    autoResize = false,
                    width = 260f,
                    },
                new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Path"),
                    // width = 400f,
                }, 
                new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Size"),
                    // width = 400f,
                    autoResize = false,
                    width = 80f,
                    sortingArrowAlignment = TextAlignment.Right,
                },
                new MultiColumnHeaderState.Column() { // empty header (for reset TextAlignment)
                },
            })))
        {
        }

        public FileTreeView(TreeViewState state, MultiColumnHeader header) // constructer
            : base(state, header)
        {
            rowHeight = RowHeight;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            header.sortingChanged += Header_sortingChanged;

            header.ResizeToFit();
            Reload();

            header.sortedColumnIndex = SessionState.GetInt(SortedColumnIndexStateKey, DefaultSortedColumnIndex);
        }

        protected override void RowGUI(RowGUIArgs args) // draw gui
        {
            var item = args.item as FileTreeViewItem;
                
            for (var visibleColumnIndex = 0; visibleColumnIndex < args.GetNumVisibleColumns(); visibleColumnIndex++)
            {
                var rect = args.GetCellRect(visibleColumnIndex);
                var columnIndex = args.GetColumn(visibleColumnIndex);
                var labelStyle = args.selected ? EditorStyles.whiteLabel : EditorStyles.label;
                labelStyle.alignment = TextAnchor.MiddleLeft;
                
                switch (columnIndex)
                {
                    case 0:
                        rect.x += 1f;
                        rect.y += ButtonPositionY;
                        rect.size = ButtonSize;
                        if (GUI.Button(rect, "Import", EditorStyles.miniButton))
                        {
                            AssetDatabase.ImportPackage(item.FilePath, true);
                        }
                        break;
                    case 1:
                        EditorGUI.LabelField(rect, item.FileName, labelStyle);
                        break;
                    case 2:
                        EditorGUI.BeginDisabledGroup(true); // gray out
                        EditorGUI.LabelField(rect, item.FilePath, labelStyle);
                        EditorGUI.EndDisabledGroup();
                        break;
                    case 3:
                        labelStyle.alignment = TextAnchor.MiddleRight; 
                        rect.x -= 1f;
                        EditorGUI.BeginDisabledGroup(true); // gray out
                        EditorGUI.LabelField(rect, item.FileSize, labelStyle);
                        EditorGUI.EndDisabledGroup();
                        break;
                    case 4: // empty header
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, null);
                }
            }
        }


        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { depth = -1 };
            if (CurrentBindingItems == null || CurrentBindingItems.Count == 0)
            {
                var children = new List<TreeViewItem>();
                CurrentBindingItems = children;
            }

            root.children = CurrentBindingItems as List<TreeViewItem>;
            return root;
        }

        public void RegisterFiles(string[] filePaths)
        {
            var root = new TreeViewItem { depth = -1 };
            var children = new List<TreeViewItem>();
            for (int i = 0; i < filePaths.Length; i++)
            {
                var filePath = filePaths[i];

                if (!string.Equals(System.IO.Path.GetExtension(filePath), ".unitypackage")) // is unity package file?
                {
                    continue;
                }

                var fileInfo = new System.IO.FileInfo(filePath);
                
                children.Add(new FileTreeViewItem
                {
                    id = i,
                    FilePath = filePath,
                    FileName = System.IO.Path.GetFileNameWithoutExtension(filePath),
                    // FileSize = string.Format("{0} MB", fileInfo.Length / 1024 / 1024),
                    FileSize = string.Format("{0} KB", fileInfo.Length / 1024 ),
                });
            }

            CurrentBindingItems = children;
            root.children = CurrentBindingItems as List<TreeViewItem>;
            Reload();
        }

        private void Header_sortingChanged(MultiColumnHeader multiColumnHeader)
        {
            SessionState.SetInt(SortedColumnIndexStateKey, multiColumnHeader.sortedColumnIndex);
            var index = multiColumnHeader.sortedColumnIndex;
            var ascending = multiColumnHeader.IsSortedAscending(multiColumnHeader.sortedColumnIndex);
            var items = rootItem.children.Cast<FileTreeViewItem>();

            // sorting
            IOrderedEnumerable<FileTreeViewItem> orderedEnumerable;
            switch (index)
            {
                case 1:
                    orderedEnumerable = ascending ? items.OrderBy(item => item.FileName) : items.OrderByDescending(item => item.FileName);
                    break;
                case 2:
                    orderedEnumerable = ascending ? items.OrderBy(item => item.FilePath) : items.OrderByDescending(item => item.FilePath);
                    break;
                case 3:
                    orderedEnumerable = ascending ? items.OrderBy(item => item.FileSize) : items.OrderByDescending(item => item.FileSize);
                    break;
                case 4: // empty header
                    orderedEnumerable = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            CurrentBindingItems = rootItem.children = orderedEnumerable.Cast<TreeViewItem>().ToList();
            BuildRows(rootItem);
        }

        // double click
        protected override void DoubleClickedItem(int id)
        {
            var item = (FileTreeViewItem)GetRows()[id];
            var openPath = System.IO.Directory.GetParent(item.FilePath).FullName;
            System.Diagnostics.Process.Start(openPath);
        }
    }
}
