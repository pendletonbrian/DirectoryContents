﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using DirectoryContents.Classes;
using DirectoryContents.Models;

namespace DirectoryContents.ViewModels
{
    /// <summary>
    /// The binding knowledge came from:
    /// https://www.wpf-tutorial.com/treeview-control/treeview-data-binding-multiple-templates/
    /// Nifty TreeView info: https://www.codeproject.com/Articles/26288/Simplifying-the-WPF-TreeView-by-Using-the-ViewMode
    /// </summary>
    public class MainWindowViewModel : NotifyObject
    {
        #region Public Members

        public static RoutedCommand BrowseCommand = new RoutedCommand();
        public static RoutedCommand CollapseAllCommand = new RoutedCommand();
        public static RoutedCommand ExpandAllCommand = new RoutedCommand();
        public static RoutedCommand ExportCommand = new RoutedCommand();

        #endregion Public Members

        #region Private Members

        private string m_DirectoryToParse = string.Empty;
        private DirectoryItem m_RootNode;

        #endregion Private Members

        #region Public Properties

        public ObservableCollection<DirectoryItem> DirectoryItems { get; private set; }

        public string DirectoryToParse
        {
            get { return m_DirectoryToParse; }

            set
            {
                if (string.IsNullOrWhiteSpace(m_DirectoryToParse) ||
                    m_DirectoryToParse.Equals(value, StringComparison.OrdinalIgnoreCase) == false)
                {
                    m_DirectoryToParse = value;

                    RaisePropertyChanged(nameof(DirectoryToParse));

                    Parse();
                }
            }
        }

        public DirectoryItem RootNode
        {
            get { return m_RootNode; }

            private set
            {
                m_RootNode = value;

                RaisePropertyChanged(nameof(RootNode));
            }
        }

        #endregion Public Properties

        #region constructor

        public MainWindowViewModel()
        {
            DirectoryItems = new ObservableCollection<DirectoryItem>
            {
            };
        }

        #endregion constructor

        #region Public Methods

        internal bool CanGenerate()
        {
            return string.IsNullOrWhiteSpace(m_DirectoryToParse).Equals(false);
        }

        internal void CollapseAll()
        {
            m_RootNode.IsExpanded = true;

            foreach (DirectoryItem node in m_RootNode.Items)
            {
                SetIsExpanded(node, false);
            }
        }

        internal void ExpandAll()
        {
            m_RootNode.IsExpanded = true;

            foreach (DirectoryItem node in m_RootNode.Items)
            {
                SetIsExpanded(node, true);
            }
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

        #endregion Public Methods

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
    }
}