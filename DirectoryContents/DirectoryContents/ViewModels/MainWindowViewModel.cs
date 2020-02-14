using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows.Controls;
using DirectoryContents.Classes;
using DirectoryContents.Classes.WpfPageTransitions;
using DirectoryContents.Models;
using DirectoryContents.Views;
using static DirectoryContents.Classes.Enumerations;

namespace DirectoryContents.ViewModels
{
    /// <summary>
    /// The binding knowledge came from:
    /// https://www.wpf-tutorial.com/treeview-control/treeview-data-binding-multiple-templates/
    /// Nifty TreeView info: https://www.codeproject.com/Articles/26288/Simplifying-the-WPF-TreeView-by-Using-the-ViewMode
    /// </summary>
    public class MainWindowViewModel : NotifyObject
    {
        #region Private Members

        /// <summary>
        /// How long, in seconds, to display the status bar message.
        /// </summary>
        private const int MAX_STATUS_MSG_COUNT = 8;

        private static readonly Logger m_Logger;

        /// <summary>
        /// The list of the controls that have been used in the current path.
        /// </summary>
        private readonly Stack<UserControl> m_PageList = new Stack<UserControl>();

        private readonly PageTransition m_PageTransition;

        /// <summary>
        /// The timer for the status message.
        /// </summary>
        private readonly Timer m_StatusMsgTimer = new Timer();

        private StringBuilder m_DebugText = new StringBuilder();

        /// <summary>
        /// The current number of seconds for which the timer has been counting.
        /// </summary>
        private int m_MessageTimerCount;

        private bool m_ShowProgressBar = false;
        private string m_StatusText = string.Empty;

        #endregion Private Members

        #region Public Properties

        internal Enumerations.PageControl CurrentPage { get; private set; } = Enumerations.PageControl.None;

        public string DebugText
        {
            get { return m_DebugText.ToString(); }

            set
            {
                Debug.WriteLine(value);

                m_DebugText.AppendLine(value);

                RaisePropertyChanged(nameof(DebugText));
            }
        }

        public bool ShowProgressBar
        {
            get { return m_ShowProgressBar; }

            set
            {
                if (m_ShowProgressBar.Equals(value) == false)
                {
                    m_ShowProgressBar = value;

                    RaisePropertyChanged(nameof(ShowProgressBar));
                }
            }
        }

        public string StatusText
        {
            get { return m_StatusText; }

            set
            {
                if (string.IsNullOrWhiteSpace(m_StatusText) ||
                    m_StatusText.Equals(value, StringComparison.OrdinalIgnoreCase) == false)
                {
                    m_StatusText = value;

                    RaisePropertyChanged(nameof(StatusText));
                }
            }
        }

        public string TitleText { get; internal set; }

        #endregion Public Properties

        #region constructors

        static MainWindowViewModel()
        {
            try
            {
                string startupDirectory = AppDomain.CurrentDomain.BaseDirectory;

                m_Logger = new Logger(Path.Combine(startupDirectory, $"log_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.log"));

                Log($"{nameof(MainWindowViewModel)} static constructor.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in static constructor:{Environment.NewLine}{ex.ToString()}");
            }
        }

        public MainWindowViewModel(PageTransition pageTransition)
        {
            Log($"{nameof(MainWindowViewModel)} public constructor.");
            DebugText = $"{nameof(MainWindowViewModel)} public constructor.";

            m_PageTransition = pageTransition;

            m_StatusMsgTimer.Elapsed += StatusMsgTimer_Elapsed;
            m_StatusMsgTimer.Interval = 1000;
        }

        #endregion constructors

        #region Private Methods

        private UserControl GetPageUserControl(Enumerations.PageControl pageControl, object additionalData = null)
        {
            UserControl newPage = null;

            switch (pageControl)
            {
                case Enumerations.PageControl.None:
                    break;

                case Enumerations.PageControl.Directory:
                    newPage = new DirectoryView(this, null);
                    break;

                case Enumerations.PageControl.Settings:
                    newPage = new SettingsView(this);
                    break;

                case Enumerations.PageControl.FileChecksum:

                    if (additionalData is null)
                    {
                        throw new ArgumentException($"The {nameof(FileChecksumView)} requires the {nameof(Enumerations.ChecksumAlgorithim)} parameter.");
                    }

                    DirectoryItem item = additionalData as DirectoryItem;

                    if (item is null)
                    {
                        throw new ArgumentException($"The additional data wasn't of type {nameof(DirectoryItem)}.");
                    }

                    newPage = new FileChecksumView(this, item);

                    break;

                default:
                    throw new DirectoryContentsException($"Unhandled PageControl enumeration: {pageControl}");
            }

            return newPage;
        }

        private void StatusMsgTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (MAX_STATUS_MSG_COUNT <= (m_MessageTimerCount++))
            {
                m_StatusMsgTimer.Stop();

                StatusText = string.Empty;
            }
        }

        /// <summary>
        /// Given a control, attempt to discern the enumerated type that is
        /// associated with it.
        /// </summary>
        /// <param name="pageControl">
        /// </param>
        /// <returns>
        /// </returns>
        internal static Enumerations.PageControl GetPageEnumerationType(UserControl pageControl)
        {
            Enumerations.PageControl currentPage = Enumerations.PageControl.None;

            if (pageControl is DirectoryView)
            {
                currentPage = Enumerations.PageControl.Directory;
            }
            else if (pageControl is SettingsView)
            {
                currentPage = Enumerations.PageControl.Settings;
            }
            else if (pageControl is FileChecksumView)
            {
                currentPage = Enumerations.PageControl.FileChecksum;
            }
            else
            {
                throw new DirectoryContentsException($"Unhandled page type: \"{pageControl.GetType().ToString()}\".");
            }

            return currentPage;
        }

        #endregion Private Methods

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
        internal static void Log(string msg, bool prependTimeStamp = true)
        {
            Debug.WriteLine(msg);

            if (m_Logger is null)
            {
                return;
            }

            m_Logger.Log(msg, prependTimeStamp);
        }

        /// <summary>
        /// Writes the message to the error stream, then calls the Log method.
        /// </summary>
        /// <param name="msg">
        /// </param>
        /// <param name="prependTimeStamp">
        /// </param>
        internal static void LogError(string msg, bool prependTimeStamp = true)
        {
            Console.Error.WriteLine(msg);

            if (m_Logger is null)
            {
                return;
            }

            Log(msg, prependTimeStamp);
        }

        internal void ShowNextPage(Enumerations.PageControl pageControl,
            object additionalData = null,
            PageTransitionType transitionType = PageTransitionType.SlideAndFade)
        {
            Log($"{nameof(MainWindowViewModel)}.{nameof(ShowNextPage)}: Start");

            if (pageControl.Equals(Enumerations.PageControl.None))
            {
                Log($"  The page control type \"{pageControl}\" is unhandled.");
            }

            UserControl newPageControl = GetPageUserControl(pageControl, additionalData);

            if (newPageControl is null)
            {
                Log($"  The user control came back null for page type: {pageControl}");

                return;
            }

            Log($"  The page type is \"{pageControl}\".");

            if (CurrentPage.Equals(pageControl) == true)
            {
                return;
            }

            CurrentPage = pageControl;

            // The stack is only really used for going back to a previous page.
            m_PageList.Push(newPageControl);

            m_PageTransition.TransitionType = transitionType;
            m_PageTransition.ShowPage(newPageControl);

            Log($"{nameof(MainWindowViewModel)}.{nameof(ShowNextPage)}: End");

        }

        /// <summary>
        /// Shows the previous page with the given transition.
        /// </summary>
        /// <param name="transitionType">
        /// </param>
        internal void ShowPreviousPage(PageTransitionType transitionType = PageTransitionType.SlideBackAndFade)
        {
            Log($"{nameof(MainWindowViewModel)}.{nameof(ShowPreviousPage)}: Start");

            UserControl newPageControl = null;

            if (m_PageList.Count > 0)
            {
                newPageControl = m_PageList.Pop();

                Log($"  Removed {newPageControl} from stack.");

                if (m_PageList.Count == 1)
                {
                    Log("  There is now 1 item in the stack.");
                }
                else
                {
                    Log($"  There are now {m_PageList.Count} items in the stack.");
                }

            }
            else
            {
                Log($"  The page list stack is empty.");
            }

            if (m_PageList.Count > 0)
            {
                newPageControl = m_PageList.Peek();

                Log($"  New page is {newPageControl}.");
            }

            // Set back button text.

            if (newPageControl is null)
            {
                newPageControl = new DirectoryView(this, null);
            }

            CurrentPage = GetPageEnumerationType(newPageControl);

            m_PageTransition.TransitionType = transitionType;
            m_PageTransition.ShowPage(newPageControl);

            Log($"{nameof(MainWindowViewModel)}.{nameof(ShowPreviousPage)}: End");

        }

        internal void ShowStatusMessage(string msg, bool autoRemove = true)
        {
            Log(msg);

            StatusText = msg;

            if (autoRemove)
            {
                m_MessageTimerCount = 0;

                m_StatusMsgTimer.Stop();

                m_StatusMsgTimer.Start();
            }
        }

        #endregion Public Methods
    }
}