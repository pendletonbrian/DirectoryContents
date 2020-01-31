using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DirectoryContents.Classes;

namespace DirectoryContents.ViewModels
{
    public class BaseUserControlViewModel : NotifyObject
    {
        #region Public Members

        /// <summary>
        /// Command to go back to the previous page.
        /// </summary>
        public static RoutedCommand BackCommand = new RoutedCommand();

        #endregion Public Members

        #region Private Members

        private readonly MainWindowViewModel m_ViewModel;

        #endregion

        #region constructor

        public BaseUserControlViewModel(MainWindowViewModel viewModel)
        {
            m_ViewModel = viewModel;
        }

        #endregion

        #region Public Properties

        public string BackButtonToolTipText
        {
            get { return "Back"; }
        }

        #endregion

        #region Public Methods

        public void ShowPreviousPage(WpfPageTransitions.PageTransitionType transitionType = WpfPageTransitions.PageTransitionType.SlideBackAndFade)
        {
            m_ViewModel.ShowPreviousPage(transitionType);
        }

        /// <summary>
        /// Writes the message to the log file, prepending a timestamp (if 
        /// desired) and appending a new line. Calls flush after logging
        /// the message.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="prependTimeStamp"></param>
        public void Log(string msg, bool prependTimeStamp = true)
        {
            MainWindowViewModel.Log(msg, prependTimeStamp);
        }

        public void ShowNextPage(Enumerations.PageControl pageControl,
            object additionalData = null,
            WpfPageTransitions.PageTransitionType transitionType = WpfPageTransitions.PageTransitionType.SlideBackAndFade)
        {
            m_ViewModel.ShowNextPage(pageControl, additionalData, transitionType);
        }

        #endregion
    }
}
