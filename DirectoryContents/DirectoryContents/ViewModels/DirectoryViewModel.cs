using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DirectoryContents.Classes;
using DirectoryContents.Classes.Checksums;
using DirectoryContents.Classes.ExportFiles;
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
        /// Context menu command to show the generate file hash page.
        /// </summary>
        public static RoutedCommand GenerateFileHashCommand = new RoutedCommand();

        /// <summary>
        /// Context menu command to open a file explorer at the selected file's
        /// location, with the file selected.
        /// </summary>
        public static RoutedCommand ViewInFileExplorerCommand = new RoutedCommand();

        /// <summary>
        /// Main menu command to show the settings page.
        /// </summary>
        public static RoutedCommand ViewSettingsCommand = new RoutedCommand();

        /// <summary>
        /// Main menu command to show the search bar.
        /// </summary>
        public static RoutedCommand SearchCommand = new RoutedCommand();

        #endregion Public Members

        #region Private Members

        private const string m_FmtInt = "###,###,###,##0";
        private readonly Hasher m_Hasher = null;
        private int m_DirectoryCount;
        private string m_DirectoryToParse = string.Empty;
        private int m_FileCount;
        private DirectoryItem m_RootNode;
        private DirectoryItem m_SelectedItem;
        private Enumerations.ExportFileStructure m_SelectedExportStructure = Enumerations.ExportFileStructure.None;

        #endregion Private Members

        #region constructor

        public DirectoryViewModel(MainWindowViewModel viewModel, IHashAlgorithim hashAlgorithim) : base(viewModel)
        {
            DirectoryItems = new ObservableCollection<DirectoryItem>();

            ExportFileStructureList = new SortedDictionary<Enumerations.ExportFileStructure, string>();

            foreach (Enumerations.ExportFileStructure val in Enum.GetValues(typeof(Enumerations.ExportFileStructure)))
            {
                if (val.Equals(Enumerations.ExportFileStructure.None) == false)
                {
                    ExportFileStructureList.Add(val, val.GetDescription());
                }
            }

            RaisePropertyChanged(nameof(ExportFileStructureList));

            SelectedExportStructure = Enumerations.ExportFileStructure.TextFile;

            if (hashAlgorithim is null)
            {
                return;
            }

            m_Hasher = new Hasher(hashAlgorithim);
        }

        #endregion constructor

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
                    Log($"Updating {nameof(DirectoryToParse)} from \"{m_DirectoryToParse}\" to \"{value}\".");

                    m_DirectoryToParse = value;

                    RaisePropertyChanged(nameof(DirectoryToParse));
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

        public Enumerations.ExportFileStructure SelectedExportStructure
        {
            get { return m_SelectedExportStructure; }

            set
            {
                if (m_SelectedExportStructure.Equals(value) == false)
                {
                    Log($"{nameof(SelectedExportStructure)} is changing from \"{m_SelectedExportStructure}\" to \"{value}\".");

                    m_SelectedExportStructure = value;

                    RaisePropertyChanged(nameof(SelectedExportStructure));
                }
            }
        }

        public SortedDictionary<Enumerations.ExportFileStructure, string> ExportFileStructureList { get; private set; }

        #endregion Public Properties

        #region Private Methods

        private void ParseDirectory(DirectoryItem node, string directoryPath)
        {
            ShowStatusMessage($"Parsing \"{directoryPath}\".");

            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);

            DirectoryItem fileNode;

            foreach (DirectoryInfo directory in dirInfo.GetDirectories())
            {
                m_DirectoryCount++;

                DirectoryItem directoryNode = new DirectoryItem(directory)
                {
                    Depth = node.Depth + 1
                };

                node.Items.Add(directoryNode);

                ParseDirectory(directoryNode, directory.FullName);
            }

            foreach (FileInfo file in dirInfo.GetFiles())
            {
                m_FileCount++;

                fileNode = new DirectoryItem(file)
                {
                    Depth = node.Depth + 1
                };

                if (m_Hasher != null)
                {
                    Log($"  Hashing \"{file.Name}\".");

                    bool? result = m_Hasher.TryGetFileChecksum(file.FullName, out string checksum);

                    if (result.HasValue &&
                        result.Value)
                    {
                        fileNode.Checksum = checksum.Replace("-", string.Empty);

                        Log($"    {fileNode.Checksum}");
                    }
                    else
                    {
                        LogError($"    Could not compute the checksum.");
                    }
                }

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

        #region debug variables

        private bool m_PrevIsItemSelected = false;
        private bool m_IsItemSelected = true;

        #endregion

        /// <summary>
        /// Checks the Directory FileAttribute to see if the given
        /// filepath is a file and not a directory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal static bool IsItemFile(string fullyQualifiedFilePath)
        {
            FileAttributes attr = File.GetAttributes(fullyQualifiedFilePath);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return false;
            }

            return true;
        }

        internal bool IsLoaded()
        {
            return m_RootNode != null;
        }

        internal void CollapseAll()
        {
            Log($"{nameof(DirectoryViewModel)}.{nameof(CollapseAll)}");

            RootNode.IsExpanded = true;

            foreach (DirectoryItem node in RootNode.Items)
            {
                SetIsExpanded(node, false);
            }
        }

        internal void ExpandAll()
        {
            Log($"{nameof(DirectoryViewModel)}.{nameof(ExpandAll)}");

            RootNode.IsExpanded = true;

            foreach (DirectoryItem node in RootNode.Items)
            {
                SetIsExpanded(node, true);
            }
        }

        internal async Task ExportAsync(string fullyQualifiedPath)
        {
            Log($"{nameof(DirectoryViewModel)}.{nameof(ExportAsync)} => Filepath: \"{fullyQualifiedPath}\"");

            if (m_RootNode is null)
            {
                throw new ArgumentException("Root node is null.", nameof(fullyQualifiedPath));
            }

            StringBuilder sb = new StringBuilder();

            if (m_Hasher is null)
            {
                sb.AppendLine("No hashing selected.");
            }
            else
            {
                sb.AppendLine($"Hashing algorithm: {m_Hasher.AlgorithimName}");
            }

            IFileExport exporter = FileExporterFactory.Get(SelectedExportStructure);

            await Task.Run(() => exporter.Export(m_RootNode, fullyQualifiedPath, sb));

            Log($"File written to: \"{fullyQualifiedPath}\".");
        }

        /// <summary>
        /// Check to see if anything is selected.
        /// </summary>
        /// <returns></returns>
        internal bool IsItemSelected()
        {
            m_PrevIsItemSelected = m_IsItemSelected;
            m_IsItemSelected = SelectedItem != null;

            if (m_PrevIsItemSelected != m_IsItemSelected)
            {
                Debug.WriteLine($"{nameof(DirectoryViewModel)}.{nameof(IsItemSelected)}: {m_IsItemSelected}");
            }

            return m_IsItemSelected;
        }

        /// <summary>
        /// Check to see if the selected item is a file.
        /// </summary>
        /// <returns>Null, if nothing is selected.</returns>
        internal bool? IsSelectedItemFile()
        {
            if (SelectedItem is null)
            {
                return null;
            }

            return IsItemFile(SelectedItem.FullyQualifiedFilename);
        }

        /// <summary>
        /// Parse the directory.
        /// </summary>
        internal async Task ParseAsync()
        {
            Log($"{nameof(DirectoryViewModel)}.{nameof(ParseAsync)}: Start");

            try
            {
                if (string.IsNullOrWhiteSpace(m_DirectoryToParse))
                {
                    LogError($"{nameof(ParseAsync)} => {nameof(m_DirectoryToParse)} is empty/null.  Returning.");

                    return;
                }

                m_DirectoryCount = 0;
                m_FileCount = 0;

                DirectoryItems.Clear();

                if (IsItemFile(m_DirectoryToParse))
                {
                    FileInfo fi = new FileInfo(m_DirectoryToParse);

                    m_RootNode = new DirectoryItem(fi)
                    {
                        Depth = 0
                    };

                    m_FileCount = 1;
                }
                else
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(m_DirectoryToParse);

                    m_RootNode = new DirectoryItem(dirInfo)
                    {
                        Depth = 0
                    };

                    DirectoryItems.Add(m_RootNode);

                    m_DirectoryCount = 0;
                    m_FileCount = 0;

                    Stopwatch timer = Stopwatch.StartNew();

                    await Task.Run(() => ParseDirectory(m_RootNode, dirInfo.FullName));

                    timer.Stop();

                    RaisePropertyChanged(nameof(DirectoryItems));

                    m_RootNode.IsExpanded = true;

                    ShowStatusMessage($"Time to parse {m_DirectoryCount.ToString(m_FmtInt)} directories and {m_FileCount.ToString(m_FmtInt)} files: {timer.Elapsed.GetTimeFromTimeSpan()}");

                }

            }
            catch (Exception ex)
            {
                LogError($"  Exception: {ex.ToString()}");
            }
            finally
            {
                Log($"{nameof(DirectoryViewModel)}.{nameof(ParseAsync)}: End");
            }
        }

        internal void ShowSelectedItemInFileExplorer()
        {
            if (SelectedItem is null)
            {
                LogError($"{nameof(SelectedItem)} is null.");

                return;
            }

            Log($"{nameof(DirectoryViewModel)}.{nameof(ShowSelectedItemInFileExplorer)}: \"{SelectedItem}\"");

            string fullyQualifiedPath = SelectedItem.FullyQualifiedFilename;

            if (string.IsNullOrWhiteSpace(fullyQualifiedPath))
            {
                LogError($"{SelectedItem.FullyQualifiedFilename} is empty/null.");

                return;
            }

            if (SelectedItem.IsDirectory)
            {
                if (Directory.Exists(fullyQualifiedPath).Equals(false))
                {
                    LogError($"The directory \"{fullyQualifiedPath}\" does not exist.");

                    return;
                }
            }
            else
            {
                if (File.Exists(fullyQualifiedPath).Equals(false))
                {
                    LogError($"The file \"{fullyQualifiedPath}\" does not exist.");

                    return;
                }
            }

            IntPtr intPtr = NativeMethods.ILCreateFromPathW(fullyQualifiedPath);

            if (intPtr == IntPtr.Zero)
            {
                LogError($"Couldn't get the pointer to the file.");

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

        #endregion Public Methods
    }
}