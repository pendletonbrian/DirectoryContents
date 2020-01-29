using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using DirectoryContents.Models;
using DirectoryContents.ViewModels;

namespace DirectoryContents
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Members

        private readonly MainWindowViewModel m_ViewModel;

        #endregion Private Members

        #region constructor

        public MainWindow()
        {
            InitializeComponent();

            m_ViewModel = new MainWindowViewModel();
            DataContext = m_ViewModel;
        }

        #endregion constructor

        #region Private Methods

        private void CheckItemIsSelected(CanExecuteRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = m_ViewModel.ItemIsSelected();
            }

            e.Handled = true;
        }

        private void ViewSettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                return;
            }

        }
        private void LoadDirectory(string fullyQualifiedDirectoryPath)
        {
            m_ViewModel.DirectoryToParse = fullyQualifiedDirectoryPath;

            treeView.Items.Clear();

            treeView.Items.Add(m_ViewModel.RootNode);

            treeView.UpdateLayout();
        }

        private void ViewInFileExplorerCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            CheckItemIsSelected(e);
        }

        private void ViewInFileExplorerCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                m_ViewModel.ShowSelectedItemInFileExplorer();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(this, $"Exception: {ex.Message}", Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void BrowseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string directoryPath = null;

            using (FolderBrowserDialog diag = new FolderBrowserDialog())
            {
                diag.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                diag.Description = "Select the directory:";

                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    directoryPath = diag.SelectedPath;
                }
            }

            //string startupDir = AppDomain.CurrentDomain.BaseDirectory;
            //if (startupDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            //{
            //    startupDir = startupDir.Substring(0, startupDir.Length - 1);
            //}

            //startupDir = startupDir.Substring(0, startupDir.LastIndexOf(Path.DirectorySeparatorChar));
            //startupDir = startupDir.Substring(0, startupDir.LastIndexOf(Path.DirectorySeparatorChar));

            //if (Directory.Exists(startupDir) == false)
            //{
            //    throw new Exception($"Solution directory couldn't be found...  \"{startupDir}\".");
            //}

            //m_ViewModel.DirectoryToParse = Path.Combine(startupDir, @"Test\RootFolder");

            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                return;
            }

            LoadDirectory(directoryPath);
        }

        private void CollapseAllCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }

            e.Handled = true;
        }

        private void CollapseAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            m_ViewModel.CollapseAll();

            treeView.UpdateLayout();
        }

        private void ExpandAllCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = true;
            }

            e.Handled = true;
        }

        private void ExpandAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            m_ViewModel.ExpandAll();

            treeView.UpdateLayout();
        }

        private void ExportCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                e.CanExecute = false;
            }
            else
            {
                e.CanExecute = m_ViewModel.CanGenerate();
            }

            e.Handled = true;
        }

        private void ExportCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string filepath;

            using (SaveFileDialog diag = new SaveFileDialog())
            {
                diag.Title = "Export file as:";
                diag.OverwritePrompt = true;
                diag.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                diag.Filter = "Text Files | *.txt";
                diag.FilterIndex = 0;
                diag.DefaultExt = "txt";
                diag.AddExtension = true;

                if (diag.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                filepath = diag.FileName;
            }

            if (string.IsNullOrWhiteSpace(filepath))
            {
                return;
            }

            //filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "results.txt");

            try
            {
                m_ViewModel.Export(treeView, filepath);

                MessageBoxResult result = System.Windows.MessageBox.Show(
                    this,
                    "File exported!  Would you like to view the result?",
                    Title,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Process.Start(filepath);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(this, $"Exception: {ex.Message}", Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DirectoryItem item = e.NewValue as DirectoryItem;

            if (item is null)
            {
                return;
            }

            m_ViewModel.SelectedItem = item;
        }

        private void TreeView_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            bool isValidData = e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop);

            if (isValidData == false)
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
        }

        private void TreeView_Drop(object sender, System.Windows.DragEventArgs e)
        {
            string[] filenameList = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

            if (filenameList is null ||
                filenameList.Length.Equals(0))
            {
                return;
            }

            LoadDirectory(filenameList[0]);
        }

        private void GenerateFileHashCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            CheckItemIsSelected(e);
        }

        private void GenerateFileHashCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        #endregion Private Methods

    }
}