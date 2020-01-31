using System;
using System.Collections.Generic;
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
using DirectoryContents.ViewModels;

namespace DirectoryContents.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : BaseUserControlView
    {
        #region Private Members

        private readonly SettingsViewModel m_ViewModel;

        #endregion

        #region constructor

        public SettingsView(MainWindowViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();

            m_ViewModel = new SettingsViewModel(viewModel);

            DataContext = m_ViewModel;
        }

        #endregion

        #region Private Methods

        private void NextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            m_ViewModel.ShowNextPage(Classes.Enumerations.PageControl.Directory, null, WpfPageTransitions.PageTransitionType.SlideAndFade);
        }

        #endregion
    }
}
