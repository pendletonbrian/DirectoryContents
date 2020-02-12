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

        #endregion Private Members

        #region constructor

        public BaseUserControlViewModel(MainWindowViewModel viewModel)
        {
            m_ViewModel = viewModel;
        }

        #endregion constructor

        #region Public Properties

        public string BackButtonToolTipText
        {
            get { return "Back"; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Writes the message to the Debug stream, then writes the message to
        /// the log file, prepending a timestamp (if desired) and appending a
        /// new line. Calls flush after logging the message.
        /// </summary>
        /// <param name="msg">
        /// </param>
        /// <param name="prependTimeStamp">
        /// </param>
        public void Log(string msg, bool prependTimeStamp = true)
        {
            MainWindowViewModel.Log(msg, prependTimeStamp);
        }

        /// <summary>
        /// Writes the message to the error stream, then calls the Log method.
        /// </summary>
        /// <param name="msg">
        /// </param>
        /// <param name="prependTimeStamp">
        /// </param>
        public void LogError(string msg, bool prependTimeStamp = true)
        {
            MainWindowViewModel.LogError(msg, prependTimeStamp);
        }

        /// <summary>
        /// Show a given page.
        /// </summary>
        /// <param name="pageControl">The enumerated value for the new page.</param>
        /// <param name="additionalData">Any additional data that the new page might need.</param>
        /// <param name="transitionType">Defaults to sliding to the left.</param>
        public void ShowNextPage(Enumerations.PageControl pageControl,
            object additionalData = null,
            WpfPageTransitions.PageTransitionType transitionType = WpfPageTransitions.PageTransitionType.SlideAndFade)
        {
            if (m_ViewModel is null)
            {
                return;
            }

            m_ViewModel.ShowNextPage(pageControl, additionalData, transitionType);
        }

        /// <summary>
        /// Show the previous page.
        /// </summary>
        /// <param name="transitionType">Defaults to sliding to the right.</param>
        public void ShowPreviousPage(WpfPageTransitions.PageTransitionType transitionType = WpfPageTransitions.PageTransitionType.SlideBackAndFade)
        {
            if (m_ViewModel is null)
            {
                return;
            }

            m_ViewModel.ShowPreviousPage(transitionType);
        }

        public void ShowStatusMessage(string msg, bool autoRemove = true)
        {
            Log(msg);

            if (m_ViewModel is null)
            {
                return;
            }

            m_ViewModel.ShowStatusMessage(msg, autoRemove);
        }

        #endregion Public Methods
    }
}