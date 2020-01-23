using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using DirectoryContents.Classes;
using DirectoryContents.Models;

namespace DirectoryContents.ViewModels
{
    /// <summary>
    /// The binding knowledge came from: https://www.wpf-tutorial.com/treeview-control/treeview-data-binding-multiple-templates/
    /// </summary>
    public class MainWindowViewModel : NotifyObject
    {
        #region Public Members

        public static RoutedCommand BrowseCommand = new RoutedCommand();
        public static RoutedCommand ExportCommand = new RoutedCommand();

        #endregion Public Members

        #region Private Members

        private string m_DirectoryToParse = string.Empty;
        private DirectoryItem m_RootNode;

        #endregion Private Members

        #region Public Properties

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

        #endregion Public Properties

        #region constructor

        public MainWindowViewModel() 
        {
            DirectoryItems = new ObservableCollection<DirectoryItem>
            {
            };
        }

        #endregion

        #region Public Methods

        internal bool CanGenerate()
        {
            return string.IsNullOrWhiteSpace(m_DirectoryToParse).Equals(false);
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

            m_RootNode = new DirectoryItem(dirInfo);
            m_RootNode.Depth = 0;

            DirectoryItems.Add(m_RootNode);

            ParseDirectory(m_RootNode, dirInfo.FullName);

            RaisePropertyChanged(nameof(DirectoryItems));

            m_RootNode.IsExpanded = true;
        }

        internal void Export(string fullyQualifiedPath)
        {
            if (string.IsNullOrEmpty(fullyQualifiedPath))
            {
                throw new ArgumentException("The export file name was not set.", nameof(fullyQualifiedPath));
            }

            DirectoryInfo dirInfo = new DirectoryInfo(m_DirectoryToParse);

            if (rootNode is null)
            {
                throw new ArgumentException("Root node is null.", nameof(fullyQualifiedPath));
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(rootNode.ItemName);

            foreach (DirectoryItem node in rootNode.Items)
            {
                sb.AppendLine($"|\t{node.ItemName}");

                ExportNode(sb, node);
            }

            using (StreamWriter writer = new StreamWriter(fullyQualifiedPath))
            {
                writer.Write(sb.ToString());
                writer.Flush();
            }

            Debug.WriteLine($"File written to: \"{fullyQualifiedPath}\".");
        }

        #endregion Public Methods

        #region Private Methods

        private void ParseDirectory(DirectoryItem node, string directoryPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);

            DirectoryItem fileNode;

            foreach (FileInfo file in dirInfo.GetFiles())
            {
                fileNode = new DirectoryItem(file)
                {
                    Depth = node.Depth + 1
                };

                node.Items.Add(fileNode);
            }

            foreach (DirectoryInfo directory in dirInfo.GetDirectories())
            {
                DirectoryItem directoryNode = new DirectoryItem(directory)
                {
                    Depth = node.Depth + 1
                };

                node.Items.Add(directoryNode);

                ParseDirectory(directoryNode, directory.FullName);
            }
        }

        private void ExportNode(StringBuilder sb, DirectoryItem parentNode)
        {
            string tabs = string.Empty;

            if (parentNode.Depth > 1)
            {
                tabs = new string('\t', parentNode.Depth - 1);
            }

            foreach (DirectoryItem node in parentNode.Items)
            {
                sb.AppendLine($"{tabs}|\t{node.ItemName}");

                ExportNode(sb, node);
            }
        }

        #endregion Private Methods
    }
}
