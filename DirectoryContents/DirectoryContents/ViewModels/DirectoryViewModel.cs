using DirectoryContents.Classes;
using DirectoryContents.Classes.Checksums;
using DirectoryContents.Classes.ExportFiles;
using DirectoryContents.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

        public static RoutedCommand GenerateAllChecksumsCommand = new RoutedCommand();

        /// <summary>
        /// Context menu command to show the generate file hash page.
        /// </summary>
        public static RoutedCommand GenerateFileHashCommand = new RoutedCommand();

        /// <summary>
        /// Main menu command to show the search bar.
        /// </summary>
        public static RoutedCommand SearchCommand = new RoutedCommand();

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
        /// Context menu command to copy the checksum to the clipboard.
        /// </summary>
        public static RoutedCommand CopyChecksumCommand = new RoutedCommand();

        #endregion Public Members

        #region Private Members

        private const string m_FmtInt = "###,###,###,##0";
        private int m_DirectoryCount;
        private string m_DirectoryToParse = string.Empty;
        private int m_FileCount;
        private DirectoryItem m_RootNode;
        private string m_SearchString = string.Empty;
        private Enumerations.ExportFileStructure m_SelectedExportStructure = Enumerations.ExportFileStructure.None;
        private DirectoryItem m_SelectedItem;

        #endregion Private Members

        #region constructor

        public DirectoryViewModel(MainWindowViewModel viewModel) : base(viewModel)
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

        public SortedDictionary<Enumerations.ExportFileStructure, string> ExportFileStructureList { get; private set; }

        public DirectoryItem RootNode
        {
            get { return m_RootNode; }

            private set
            {
                m_RootNode = value;

                RaisePropertyChanged(nameof(RootNode));
            }
        }

        public string SearchString
        {
            get { return m_SearchString; }

            set
            {
                if (string.IsNullOrWhiteSpace(m_SearchString) ||
                    m_SearchString.Equals(value, StringComparison.OrdinalIgnoreCase) == false)
                {
                    m_SearchString = value;

                    RaisePropertyChanged(nameof(SearchString));

                    if (string.IsNullOrWhiteSpace(m_SearchString))
                    {
                        ResetSearch();
                    }
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

        public IHashAlgorithim HashAlgorithim { get; set; }

        #endregion Public Properties

        #region Private Methods

        private static void ResetSearch(DirectoryItem node)
        {
            SetFont(node, false);

            foreach (DirectoryItem childNode in node.Items)
            {
                ResetSearch(childNode);
            }
        }

        private static void SetFont(DirectoryItem node, bool isFound)
        {
            if (isFound)
            {
                node.FontStyle = FontStyles.Italic;
                node.FontWeight = FontWeights.Bold;
            }
            else
            {
                node.FontStyle = FontStyles.Normal;
                node.FontWeight = FontWeights.Normal;
            }
        }

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

            Hasher hasher = null;
            if (HashAlgorithim != null)
            {
                hasher = new Hasher(HashAlgorithim);
            }

            foreach (FileInfo file in dirInfo.GetFiles())
            {
                m_FileCount++;

                fileNode = new DirectoryItem(file)
                {
                    Depth = node.Depth + 1
                };

                if (hasher != null)
                {
                    Log($"  Hashing \"{file.Name}\".");

                    bool? result = hasher.TryGetFileChecksum(file.FullName, out string checksum);

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

        private bool Search(DirectoryItem node, string term)
        {
            /*
             * If the term is found in a directory, we'll need to know so that
             * the parent directory can be expanded.
            */
            bool found = false;

            if (node.ItemName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                node.FullyQualifiedFilename.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                Log($"Search found term in: {node.FullyQualifiedFilename}.");

                node.IsExpanded = true;

                SetFont(node, true);

                found = true;
            }

            foreach (DirectoryItem childNode in node.Items)
            {
                if (Search(childNode, term) &&
                    found.Equals(false))
                {
                    found = true;

                    node.IsExpanded = true;
                }
            }

            return found;
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

        private bool m_IsItemSelected = true;
        private bool m_PrevIsItemSelected = false;

        #endregion debug variables

        /// <summary>
        /// Checks the Directory FileAttribute to see if the given filepath is a
        /// file and not a directory.
        /// </summary>
        /// <param name="item">
        /// </param>
        /// <returns>
        /// </returns>
        internal static bool IsItemFile(string fullyQualifiedFilePath)
        {
            FileAttributes attr = File.GetAttributes(fullyQualifiedFilePath);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return false;
            }

            return true;
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

            if (RootNode is null)
            {
                throw new ArgumentException("Root node is null.", nameof(fullyQualifiedPath));
            }

            StringBuilder sb = new StringBuilder();

            if (HashAlgorithim is null)
            {
                sb.AppendLine("No hashing selected.");
            }
            else
            {
                sb.AppendLine($"Hashing algorithm: {HashAlgorithim.AlgorithimName}");
            }

            IFileExport exporter = FileExporterFactory.Get(SelectedExportStructure);

            await Task.Run(() => exporter.Export(RootNode, fullyQualifiedPath, sb));

            Log($"File written to: \"{fullyQualifiedPath}\".");
        }

        /// <summary>
        /// Check to see if anything is selected.
        /// </summary>
        /// <returns>
        /// </returns>
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

        internal bool IsLoaded()
        {
            return RootNode != null;
        }

        /// <summary>
        /// Check to see if the selected item is a file.
        /// </summary>
        /// <returns>
        /// Null, if nothing is selected.
        /// </returns>
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
                if (string.IsNullOrWhiteSpace(DirectoryToParse))
                {
                    LogError($"{nameof(ParseAsync)} => {nameof(DirectoryToParse)} is empty/null.  Returning.");

                    return;
                }

                m_DirectoryCount = 0;
                m_FileCount = 0;

                DirectoryItems.Clear();

                if (IsItemFile(DirectoryToParse))
                {
                    FileInfo fi = new FileInfo(DirectoryToParse);

                    RootNode = new DirectoryItem(fi)
                    {
                        Depth = 0
                    };

                    m_FileCount = 1;
                }
                else
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(DirectoryToParse);

                    RootNode = new DirectoryItem(dirInfo)
                    {
                        Depth = 0
                    };

                    DirectoryItems.Add(RootNode);

                    m_DirectoryCount = 0;
                    m_FileCount = 0;

                    Stopwatch timer = Stopwatch.StartNew();

                    await Task.Run(() => ParseDirectory(RootNode, dirInfo.FullName));

                    timer.Stop();

                    RaisePropertyChanged(nameof(DirectoryItems));

                    RootNode.IsExpanded = true;

                    ShowStatusMessage($"Time to parse " +
                        $"{m_DirectoryCount.ToString(m_FmtInt)} directories and" +
                        $" {m_FileCount.ToString(m_FmtInt)} files: " +
                        $"{timer.Elapsed.GetTimeFromTimeSpan()}");
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

        internal void ResetSearch()
        {
            foreach (DirectoryItem node in RootNode.Items)
            {
                ResetSearch(node);
            }
        }

        internal void Search()
        {
            Log($"Search for: \"{SearchString}\".");

            Stopwatch timer = Stopwatch.StartNew();

            foreach (DirectoryItem node in RootNode.Items)
            {
                Search(node, SearchString);
            }

            timer.Stop();

            ShowStatusMessage($"Time to search {m_DirectoryCount.ToString(m_FmtInt)} directories and {m_FileCount.ToString(m_FmtInt)} files: {timer.Elapsed.GetTimeFromTimeSpan()}");
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