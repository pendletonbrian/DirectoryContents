﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using DirectoryContents.Classes;
using DirectoryContents.Models;

namespace DirectoryContents.ViewModels
{
    public class DirectoryViewModel : BaseUserControlViewModel
    {
        #region Public Members

        /// <summary>
        /// Main menu command to browse for the target directory.
        /// </summary>
        public static RoutedCommand BrowseCommand = new RoutedCommand();

        /// <summary>
        /// Context menu command to collapse all nodes of the tree.
        /// </summary>
        public static RoutedCommand CollapseAllCommand = new RoutedCommand();

        /// <summary>
        /// Context menu command to expand al nodes of the tree.
        /// </summary>
        public static RoutedCommand ExpandAllCommand = new RoutedCommand();

        /// <summary>
        /// Main menu command to export the directory structure to a file.
        /// </summary>
        public static RoutedCommand ExportCommand = new RoutedCommand();

        /// <summary>
        /// Context menu command to open a file explorer at the selected
        /// file's location, with the file selected.
        /// </summary>
        public static RoutedCommand ViewInFileExplorerCommand = new RoutedCommand();

        /// <summary>
        /// Main menu command to show the settings page.
        /// </summary>
        public static RoutedCommand ViewSettingsCommand = new RoutedCommand();

        /// <summary>
        /// Context menu command to show the generate file hash page.
        /// </summary>
        public static RoutedCommand GenerateFileHashCommand = new RoutedCommand();

        #endregion Public Members

        #region Private Members

        private DirectoryItem m_RootNode;
        private DirectoryItem m_SelectedItem;
        private string m_DirectoryToParse = string.Empty;

        #endregion

        #region constructor

        public DirectoryViewModel(MainWindowViewModel viewModel) : base(viewModel)
        {
            DirectoryItems = new ObservableCollection<DirectoryItem>
            {
            };
        }

        #endregion

        #region Public Properties

        public ObservableCollection<DirectoryItem> DirectoryItems { get; private set; }

        public DirectoryItem RootNode
        {
            get { return m_RootNode; }

            private set
            {
                m_RootNode = value;

                RaisePropertyChanged(nameof(RootNode));
            }
        }

        public DirectoryItem SelectedItem
        {
            get { return m_SelectedItem; }
            set
            {
                if (m_SelectedItem is null ||
                    m_SelectedItem.Equals(value).Equals(false))
                {
                    Log($"Updating {nameof(SelectedItem)} from \"{m_SelectedItem}\" to \"{value}\".");

                    m_SelectedItem = value;

                    RaisePropertyChanged(nameof(SelectedItem));
                }
            }
        }

        public string DirectoryToParse
        {
            get { return m_DirectoryToParse; }

            set
            {
                if (string.IsNullOrWhiteSpace(m_DirectoryToParse) ||
                    m_DirectoryToParse.Equals(value, StringComparison.OrdinalIgnoreCase) == false)
                {
                    Log($"Updating {nameof(DirectoryToParse)} from \"{m_DirectoryToParse}\" to \"{value}\".");

                    m_DirectoryToParse = value;

                    RaisePropertyChanged(nameof(DirectoryToParse));

                    Parse();
                }
            }
        }

        #endregion

        #region Private Methods

        private void ExportNode(StringBuilder sb, DirectoryItem node)
        {
            // Depth first

            /*

                // get the file attributes for file or directory
                FileAttributes attr = File.GetAttributes(@"c:\Temp");

                //detect whether its a directory or file
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {}
            */

            string tabs = new string('\t', node.Depth);

            foreach (DirectoryItem childNode in node.Items)
            {
                sb.AppendLine($"{tabs}|\t{childNode.ItemName}");

                if (childNode.HasChildren)
                {
                    ExportNode(sb, childNode);

                    continue;
                }
            }
        }

        private void ParseDirectory(DirectoryItem node, string directoryPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);

            DirectoryItem fileNode;

            foreach (DirectoryInfo directory in dirInfo.GetDirectories())
            {
                DirectoryItem directoryNode = new DirectoryItem(directory)
                {
                    Depth = node.Depth + 1
                };

                node.Items.Add(directoryNode);

                ParseDirectory(directoryNode, directory.FullName);
            }

            foreach (FileInfo file in dirInfo.GetFiles())
            {
                fileNode = new DirectoryItem(file)
                {
                    Depth = node.Depth + 1
                };

                node.Items.Add(fileNode);
            }
        }

        private void SetIsExpanded(DirectoryItem node, bool isExpanded)
        {
            node.IsExpanded = isExpanded;

            foreach (DirectoryItem childNode in node.Items)
            {
                SetIsExpanded(childNode, isExpanded);
            }
        }

        #endregion Private Methods

        #region Public Methods

        internal bool CanGenerate()
        {
            Log($"{nameof(CanGenerate)}: \"{DirectoryToParse}\".");

            return string.IsNullOrWhiteSpace(DirectoryToParse).Equals(false);
        }

        internal void Export(TreeView tree, string fullyQualifiedPath)
        {
            if (tree is null)
            {
                throw new ArgumentNullException($"The TreeView is null.", nameof(tree));
            }

            if (string.IsNullOrEmpty(fullyQualifiedPath))
            {
                throw new ArgumentException("The export file name was not set.", nameof(fullyQualifiedPath));
            }

            DirectoryItem rootNode = tree.Items[0] as DirectoryItem;

            if (rootNode is null)
            {
                throw new ArgumentException("Root node is null.", nameof(fullyQualifiedPath));
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(rootNode.ItemName);

            foreach (DirectoryItem node in rootNode.Items)
            {
                sb.AppendLine($"|\t{node.ItemName}");

                if (node.HasChildren)
                {
                    ExportNode(sb, node);
                }
            }

            using (StreamWriter writer = new StreamWriter(fullyQualifiedPath))
            {
                writer.Write(sb.ToString());
                writer.Flush();
            }

            Debug.WriteLine($"File written to: \"{fullyQualifiedPath}\".");
        }

        internal bool ItemIsSelected()
        {
            return m_SelectedItem != null;
        }

        internal void ShowSelectedItemInFileExplorer()
        {
            if (SelectedItem is null)
            {
                Debug.WriteLine($"{nameof(SelectedItem)} is null.");

                return;
            }

            string fullyQualifiedPath = SelectedItem.FullyQualifiedFilename;

            if (string.IsNullOrWhiteSpace(fullyQualifiedPath))
            {
                Debug.WriteLine($"{SelectedItem.FullyQualifiedFilename} is empty/null.");

                return;
            }

            if (SelectedItem.IsDirectory)
            {
                if (Directory.Exists(fullyQualifiedPath).Equals(false))
                {
                    Debug.WriteLine($"The directory \"{fullyQualifiedPath}\" does not exist.");

                    return;
                }
            }
            else
            {
                if (File.Exists(fullyQualifiedPath).Equals(false))
                {
                    Debug.WriteLine($"The file \"{fullyQualifiedPath}\" does not exist.");

                    return;
                }
            }

            IntPtr intPtr = NativeMethods.ILCreateFromPathW(fullyQualifiedPath);

            if (intPtr == IntPtr.Zero)
            {
                Debug.WriteLine($"Couldn't get the pointer to the file.");

                return;
            }

            try
            {
                Marshal.ThrowExceptionForHR(NativeMethods.SHOpenFolderAndSelectItems(intPtr, 0, IntPtr.Zero, 0));
            }
            finally
            {
                NativeMethods.ILFree(intPtr);
            }
        }

        internal void CollapseAll()
        {
            RootNode.IsExpanded = true;

            foreach (DirectoryItem node in RootNode.Items)
            {
                SetIsExpanded(node, false);
            }
        }

        internal void ExpandAll()
        {
            RootNode.IsExpanded = true;

            foreach (DirectoryItem node in RootNode.Items)
            {
                SetIsExpanded(node, true);
            }
        }

        internal void Parse()
        {
            if (string.IsNullOrWhiteSpace(m_DirectoryToParse))
            {
                Debug.WriteLine($"{nameof(Parse)} => {nameof(m_DirectoryToParse)} is empty/null.  Returning.");

                return;
            }

            DirectoryItems.Clear();

            DirectoryInfo dirInfo = new DirectoryInfo(m_DirectoryToParse);

            m_RootNode = new DirectoryItem(dirInfo)
            {
                Depth = 0
            };

            DirectoryItems.Add(m_RootNode);

            ParseDirectory(m_RootNode, dirInfo.FullName);

            RaisePropertyChanged(nameof(DirectoryItems));

            m_RootNode.IsExpanded = true;
        }


        #endregion

    }
}