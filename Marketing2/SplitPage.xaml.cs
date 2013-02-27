using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.ApplicationModel.Store;
// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace Marketing2
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for
    /// the currently selected item.
    /// </summary>
    public sealed partial class SplitPage : Marketing2.Common.LayoutAwarePage
    {
        public DataTransferManager datatransferManager;

        public SplitPage()
        {
            this.InitializeComponent();
            SettingsPane.GetForCurrentView().CommandsRequested += SplitPage_CommandsRequested;
            //Window.Current.Activated += OnWindowActivated;
            //Window.Current.SizeChanged += OnWindowSizeChanged;
            ShareSourceLoad();
            adMain.LoadCompleted += adMain_LoadCompleted;
            
        }

        //private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        //{
        //    if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
        //    {
        //        WebViewBrush b = new WebViewBrush();
        //        b.SourceName = "contentView";
        //        b.Redraw();
        //        contentViewRect.Fill = b;
        //        contentView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        contentView.Visibility = Windows.UI.Xaml.Visibility.Visible;
        //        contentViewRect.Fill = new SolidColorBrush(Windows.UI.Colors.Transparent);
        //        //Window.Current.Activated -= OnWindowActivated;
        //    }
        //}

        //private void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        //{
        //    string viewState = DetermineVisualState(ApplicationViewState)
        //        contentView.Visibility = Windows.UI.Xaml.Visibility.Visible;
        //        contentViewRect.Fill = new SolidColorBrush(Windows.UI.Colors.Transparent);
        //        Window.Current.Activated -= OnWindowActivated;
        //}
        void adMain_LoadCompleted(object sender, NavigationEventArgs e)
        {
            List<Uri> allowedUris = new List<Uri>();
            allowedUris.Add(new Uri("http://ads.daksatech.com"));
            adMain.AllowedScriptNotifyUris = allowedUris;
            adMain.ScriptNotify += adMain_ScriptNotify;
        }

        async void adMain_ScriptNotify(object sender, NotifyEventArgs e)
        {
            // Respond to the script notification.
            var success = await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Value.ToString()));
        }



        public void ShareSourceLoad()
        {
            datatransferManager = DataTransferManager.GetForCurrentView();
            datatransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
        }

        void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            FeedItem selectedItem = this.itemListView.SelectedItem as FeedItem;
            if (selectedItem != null && this.Frame != null)
            {
                e.Request.Data.Properties.Title = selectedItem.Title;
                e.Request.Data.Properties.Description = selectedItem.Title + "-" + selectedItem.Author + Environment.NewLine + selectedItem.Link.AbsoluteUri.ToString();
                e.Request.Data.SetText(selectedItem.Title + "-" + selectedItem.Author + Environment.NewLine + selectedItem.Link.AbsoluteUri.ToString());
                e.Request.Data.SetUri(selectedItem.Link);
                e.Request.Data.SetHtmlFormat(selectedItem.Title + "<br /> - " + selectedItem.Author + "<br /><br />" + selectedItem.Link.AbsoluteUri.ToString() + "<br /><br />" + "Shared from <a href='" + CurrentAppSimulator.LinkUri + "'>Marketing</a> app.");
                
            }

            
        }

        private void pageRoot_Unloaded_1(object sender, RoutedEventArgs e)
        {
            datatransferManager = DataTransferManager.GetForCurrentView();
            datatransferManager.DataRequested -= DataRequested;
        }

        
        void SplitPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            if (this.Frame.SourcePageType.FullName == "Marketing2.SplitPage")
            {
                WebViewBrush b = new WebViewBrush();
                b.SourceName = "contentView";
                b.Redraw();
                contentViewRect.Fill = b;
                contentView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;


                WebViewBrush a = new WebViewBrush();
                a.SourceName = "adMain";
                a.Redraw();
                adMainRect.Fill = a;
                adMain.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            }
        }

        //private void pageRoot_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    contentView.Visibility = Windows.UI.Xaml.Visibility.Visible;
        //    contentViewRect.Fill = new SolidColorBrush(Windows.UI.Colors.Transparent);
        //}

        private void ViewDetail_Click(object sender, RoutedEventArgs e)
        {
            FeedItem selectedItem = this.itemListView.SelectedItem as FeedItem;
            if (selectedItem != null && this.Frame != null)
            {
                string itemTitle = selectedItem.Title;
                this.Frame.Navigate(typeof(DetailPage), itemTitle);
            }
        }

        #region Page state management

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Run the PopInThemeAnimation 
            Windows.UI.Xaml.Media.Animation.Storyboard sb =
                this.FindName("PopInStoryboard") as Windows.UI.Xaml.Media.Animation.Storyboard;
            if (sb != null) sb.Begin();

            // TODO: Assign a bindable group to this.DefaultViewModel["Group"]
            // TODO: Assign a collection of bindable items to this.DefaultViewModel["Items"]
            string feedTitle = (string)navigationParameter;
            //FeedData feedData = FeedDataSource.GetFeed(feedTitle);
            FeedData feedData = (FeedData)sessionData.currentFeeds.Find((feed)=>feed.Title.Equals(feedTitle));            
            if (feedData != null)
            {
                this.DefaultViewModel["Feed"] = feedData;
                this.DefaultViewModel["Items"] = feedData.Items;
            }

            if (pageState == null)
            {
                // When this is a new page, select the first item automatically unless logical page
                // navigation is being used (see the logical page navigation #region below.)
                if (!this.UsingLogicalPageNavigation() && this.itemsViewSource.View != null)
                {
                    this.itemsViewSource.View.MoveCurrentToFirst();
                }
                else
                {
                    this.itemsViewSource.View.MoveCurrentToPosition(-1);
                }
            }
            else
            {
                // Restore the previously saved state associated with this page
                if (pageState.ContainsKey("SelectedItem") && this.itemsViewSource.View != null)
                {
                    // TODO: Invoke this.itemsViewSource.View.MoveCurrentTo() with the selected
                    //       item as specified by the value of pageState["SelectedItem"]
                    string itemTitle = (string)pageState["SelectedItem"];
                    FeedItem selectedItem = FeedDataSource.GetItem(itemTitle);
                    this.itemsViewSource.View.MoveCurrentTo(selectedItem);
                }
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            if (this.itemsViewSource.View != null)
            {
                var selectedItem = this.itemsViewSource.View.CurrentItem;
                // TODO: Derive a serializable navigation parameter and assign it to
                //       pageState["SelectedItem"]
                if (selectedItem != null)
                {
                    string itemTitle = ((FeedItem)selectedItem).Title;
                    pageState["SelectedItem"] = itemTitle;
                }
            }
        }

        #endregion

        #region Logical page navigation

        // Visual state management typically reflects the four application view states directly
        // (full screen landscape and portrait plus snapped and filled views.)  The split page is
        // designed so that the snapped and portrait view states each have two distinct sub-states:
        // either the item list or the details are displayed, but not both at the same time.
        //
        // This is all implemented with a single physical page that can represent two logical
        // pages.  The code below achieves this goal without making the user aware of the
        // distinction.

        /// <summary>
        /// Invoked to determine whether the page should act as one logical page or two.
        /// </summary>
        /// <param name="viewState">The view state for which the question is being posed, or null
        /// for the current view state.  This parameter is optional with null as the default
        /// value.</param>
        /// <returns>True when the view state in question is portrait or snapped, false
        /// otherwise.</returns>
        private bool UsingLogicalPageNavigation(ApplicationViewState? viewState = null)
        {
            if (viewState == null) viewState = ApplicationView.Value;
            return viewState == ApplicationViewState.FullScreenPortrait ||
                viewState == ApplicationViewState.Snapped;
        }

        /// <summary>
        /// Invoked when an item within the list is selected.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is Snapped)
        /// displaying the selected item.</param>
        /// <param name="e">Event data that describes how the selection was changed.</param>
        void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Invalidate the view state when logical page navigation is in effect, as a change
            // in selection may cause a corresponding change in the current logical page.  When
            // an item is selected this has the effect of changing from displaying the item list
            // to showing the selected item's details.  When the selection is cleared this has the
            // opposite effect.
            if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();

            // Add this code to populate the web view
            //  with the content of the selected blog post.
            Selector list = sender as Selector;
            FeedItem selectedItem = list.SelectedItem as FeedItem;
            if (selectedItem != null)
            {
                this.contentView.NavigateToString(selectedItem.Content);
            }
            else
            {
                this.contentView.NavigateToString("");
            }

            if (contentView.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
            {
                contentView.Visibility = Windows.UI.Xaml.Visibility.Visible;
                contentViewRect.Fill = new SolidColorBrush(Windows.UI.Colors.Transparent);
            }
            string visualState = DetermineVisualState(ApplicationView.Value);
            if (adMain.Visibility == Windows.UI.Xaml.Visibility.Collapsed & visualState != "Snapped")
            {
                adMain.Visibility = Windows.UI.Xaml.Visibility.Visible;
                adMainRect.Fill = new SolidColorBrush(Windows.UI.Colors.Transparent);

            }

        }

       


        /// <summary>
        /// Invoked when the page's back button is pressed.
        /// </summary>
        /// <param name="sender">The back button instance.</param>
        /// <param name="e">Event data that describes how the back button was clicked.</param>
        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
            if (this.UsingLogicalPageNavigation() && itemListView.SelectedItem != null)
            {
                // When logical page navigation is in effect and there's a selected item that
                // item's details are currently displayed.  Clearing the selection will return to
                // the item list.  From the user's point of view this is a logical backward
                // navigation.
                this.itemListView.SelectedItem = null;
            }
            else
            {
                // When logical page navigation is not in effect, or when there is no selected
                // item, use the default back button behavior.
                base.GoBack(sender, e);
            }

        }

        /// <summary>
        /// Invoked to determine the name of the visual state that corresponds to an application
        /// view state.
        /// </summary>
        /// <param name="viewState">The view state for which the question is being posed.</param>
        /// <returns>The name of the desired visual state.  This is the same as the name of the
        /// view state except when there is a selected item in portrait and snapped views where
        /// this additional logical page is represented by adding a suffix of _Detail.</returns>
        protected override string DetermineVisualState(ApplicationViewState viewState)
        {
            // Update the back button's enabled state when the view state changes
            var logicalPageBack = this.UsingLogicalPageNavigation(viewState) && this.itemListView.SelectedItem != null;
            var physicalPageBack = this.Frame != null && this.Frame.CanGoBack;
            this.DefaultViewModel["CanGoBack"] = logicalPageBack || physicalPageBack;

            // Determine visual states for landscape layouts based not on the view state, but
            // on the width of the window.  This page has one layout that is appropriate for
            // 1366 virtual pixels or wider, and another for narrower displays or when a snapped
            // application reduces the horizontal space available to less than 1366.
            if (viewState == ApplicationViewState.Filled ||
                viewState == ApplicationViewState.FullScreenLandscape)
            {
                var windowWidth = Window.Current.Bounds.Width;
                if (windowWidth >= 1366) return "FullScreenLandscapeOrWide";
                return "FilledOrNarrow";
            }

            // When in portrait or snapped start with the default visual state name, then add a
            // suffix when viewing details instead of the list
            var defaultStateName = base.DetermineVisualState(viewState);
            return logicalPageBack ? defaultStateName + "_Detail" : defaultStateName;
        }

        #endregion

        private void ContentView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            string errorString = "<p>Page could not be loaded.</p><p>Error is: " + e.WebErrorStatus.ToString() + "</p>";
            this.contentView.NavigateToString(errorString);
        }

        private void adMainRect_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //string visualState = DetermineVisualState(ApplicationView.Value);
            if (adMain.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
            {
                adMain.Visibility = Windows.UI.Xaml.Visibility.Visible;
                adMainRect.Fill = new SolidColorBrush(Windows.UI.Colors.Transparent);

            }
        }

        private void contentViewRect_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (contentView.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
            {
                contentView.Visibility = Windows.UI.Xaml.Visibility.Visible;
                contentViewRect.Fill = new SolidColorBrush(Windows.UI.Colors.Transparent);
            }
        }

       
    }
}
