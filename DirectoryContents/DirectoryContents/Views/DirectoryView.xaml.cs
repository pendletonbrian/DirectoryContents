using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DirectoryContents.Models;
using DirectoryContents.ViewModels;

namespace DirectoryContents.Views
{
    /// <summary>
    /// Interaction logic for DirectoryView.xaml
    /// </summary>
    public partial class DirectoryView : BaseUserControlView
    {
        #region Private Members

        private readonly DirectoryViewModel m_ViewModel;

        #endregion

        #region constructor

        public DirectoryView(MainWindowViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();

            m_ViewModel = new DirectoryViewModel(viewModel);

            DataContext = m_ViewModel;
        }

        #endregion

        #region Private Methods

        private void ViewSettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (m_ViewModel is null)
            {
                return;
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

            using (System.Windows.Forms.FolderBrowserDialog diag = new System.Windows.Forms.FolderBrowserDialog())
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

        }

        private void GenerateFileHashCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            CheckItemIsSelected(e);
        }

        private void GenerateFileHashCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {

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

        private void TreeView_DragEnter(object sender, DragEventArgs e)
        {
            bool isValidData = e.Data.GetDataPresent(DataFormats.FileDrop);

            if (isValidData == false)
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            string[] filenameList = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (filenameList is null ||
                filenameList.Length.Equals(0))
            {
                return;
            }

            LoadDirectory(filenameList[0]);
        }

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
                MessageBox.Show($"Exception: {ex.Message}", TitleText, MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

            using (System.Windows.Forms.SaveFileDialog diag = new System.Windows.Forms.SaveFileDialog())
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

                MessageBoxResult result = MessageBox.Show(
                    "File exported!  Would you like to view the result?",
                    TitleText,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Process.Start(filepath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}", TitleText, MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        #endregion
    }
}
