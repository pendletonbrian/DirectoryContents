using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using static DirectoryContents.Classes.Enumerations;

namespace DirectoryContents.Classes.WpfPageTransitions
{
    /// <summary>
    /// From http://www.codeproject.com/Articles/197132/Simple-WPF-Page-Transitions
    /// </summary>
    public partial class PageTransition : UserControl
    {
        private Stack<UserControl> pages = new Stack<UserControl>();

        private void HidePage_Completed(object sender, EventArgs e)
        {
            Storyboard hidePage = (Resources[string.Format("{0}Out", TransitionType.ToString())] as Storyboard).Clone();

            hidePage.Completed -= HidePage_Completed;

            contentPresenter.Content = null;

            ShowNextPage();
        }

        private void NewPage_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard showNewPage = Resources[string.Format("{0}In", TransitionType.ToString())] as Storyboard;

            showNewPage.Begin(contentPresenter);

            CurrentPage = sender as UserControl;
        }

        private void ShowNewPage()
        {
            Dispatcher.Invoke((Action)delegate
                {
                    if (contentPresenter.Content != null)
                    {
                        if (contentPresenter.Content is UserControl oldPage)
                        {
                            oldPage.Loaded -= NewPage_Loaded;

                            UnloadPage(oldPage);
                        }
                    }
                    else
                    {
                        ShowNextPage();
                    }
                });
        }

        private void ShowNextPage()
        {
            if (pages.Count == 0)
            {
                return;
            }

            UserControl newPage = pages.Pop();

            newPage.Loaded += NewPage_Loaded;

            contentPresenter.Content = newPage;
        }

        private void UnloadPage(UserControl page)
        {
            Storyboard hidePage = (Resources[string.Format("{0}Out", TransitionType.ToString())] as Storyboard).Clone();

            hidePage.Completed += HidePage_Completed;

            hidePage.Begin(contentPresenter);
        }

        public static readonly DependencyProperty TransitionTypeProperty = DependencyProperty.Register("TransitionType",
                                                            typeof(PageTransitionType),
            typeof(PageTransition),
            new PropertyMetadata(PageTransitionType.SlideAndFade));

        public PageTransition()
        {
            InitializeComponent();
        }

        public UserControl CurrentPage { get; set; }

        public PageTransitionType TransitionType
        {
            get
            {
                return (PageTransitionType)GetValue(TransitionTypeProperty);
            }

            set
            {
                SetValue(TransitionTypeProperty, value);
            }
        }

        public void ClearPage()
        {
            Dispatcher.Invoke((Action)delegate
                {
                    if (contentPresenter.Content != null)
                    {
                        if (contentPresenter.Content is UserControl oldPage)
                        {
                            oldPage.Loaded -= NewPage_Loaded;

                            UnloadPage(oldPage);
                        }
                    }
                });
        }

        public void ShowPage(UserControl newPage)
        {
            pages.Push(newPage);

            if (contentPresenter.Content != null)
            {
                if (contentPresenter.Content is UserControl oldPage)
                {
                    oldPage.Loaded -= NewPage_Loaded;
                    UnloadPage(oldPage);
                }
            }
            else
            {
                ShowNextPage();
            }
        }
    }
}