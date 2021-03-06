﻿using System.Windows.Input;
using DirectoryContents.ViewModels;
using static DirectoryContents.Classes.Enumerations;

namespace DirectoryContents.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : BaseUserControlView
    {
        #region Private Members

        private readonly SettingsViewModel m_ViewModel;

        #endregion Private Members

        #region constructor

        public SettingsView(MainWindowViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();

            m_ViewModel = new SettingsViewModel(viewModel);

            DataContext = m_ViewModel;
        }

        #endregion constructor

        #region Private Methods

        private void NextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            m_ViewModel.ShowNextPage(PageControl.Directory, m_ViewModel.SelectedAlgorithim, PageTransitionType.SlideAndFade);
        }

        #endregion Private Methods
    }
}
