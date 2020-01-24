using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
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

        private void BrowseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

            e.Handled = true;
        }

        private void BrowseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (FolderBrowserDialog diag = new FolderBrowserDialog())
            {
                diag.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                diag.Description = "Select the folder to parse.";

                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    m_ViewModel.DirectoryToParse = diag.SelectedPath;
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

            treeView.Items.Clear();

            treeView.Items.Add(m_ViewModel.RootNode);

            treeView.UpdateLayout();
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

        #endregion Private Methods
    }
}