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

            m_ViewModel = new MainWindowViewModel(pageTransitionControl);
            m_ViewModel.TitleText = Title;

            DataContext = m_ViewModel;
        }

        #endregion constructor

        #region Private Methods

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_ViewModel.ShowNextPage(Classes.Enumerations.PageControl.Settings);
        }

        #endregion Private Methods

    }
}