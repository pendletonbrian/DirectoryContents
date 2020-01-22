﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DirectoryContents.Classes;
using DirectoryContents.Models;

namespace DirectoryContents.ViewModels
{
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

            m_RootNode = new DirectoryItem(dirInfo.Name);

            DirectoryItems.Add(m_RootNode);

            ParseDirectory(m_RootNode, dirInfo.FullName);

            RaisePropertyChanged(nameof(DirectoryItems));
        }

        #endregion Public Methods

        #region Private Methods

        private void ParseDirectory(DirectoryItem node, string directoryPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);

            foreach (FileInfo file in dirInfo.GetFiles())
            {
                node.Items.Add(new DirectoryItem(file.Name));
            }

            foreach (DirectoryInfo directory in dirInfo.GetDirectories())
            {
                DirectoryItem directoryNode = new DirectoryItem(directory.Name);
                node.Items.Add(directoryNode);

                ParseDirectory(directoryNode, directory.FullName);
            }
        }

        #endregion Private Methods
    }
}
