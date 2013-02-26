using Syncfusion.UI.Xaml.Controls.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TopMovies.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class SampleLayout : Page
    {
        public SampleLayout()
        {
            this.InitializeComponent();
        }

        public event SelectionChangedEventHandler SelectionChanged;

        ///// <summary>
        ///// Invoked when this page is about to be displayed in a Frame.
        ///// </summary>
        ///// <param name="e">Event data that describes how this page was reached.  The Parameter
        ///// property is typically used to configure the page.</param>
        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //}

        //public string Header
        //{
        //    get { return (string)GetValue(HeaderProperty); }
        //    set { SetValue(HeaderProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty HeaderProperty =
        //    DependencyProperty.Register("Header", typeof(string), typeof(SampleLayout), new PropertyMetadata(null));



        //public SampleViews SampleViews
        //{
        //    get { return (SampleViews)GetValue(SampleViewsProperty); }
        //    set { SetValue(SampleViewsProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for SampleViews.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty SampleViewsProperty =
        //    DependencyProperty.Register("SampleViews", typeof(SampleViews), typeof(SampleLayout), new PropertyMetadata(null, new PropertyChangedCallback(OnSampleViewsChanged)));

        //private static void OnSampleViewsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    SampleLayout layout = sender as SampleLayout;
        //    if (layout != null)
        //    {
        //        layout.SampleViews.CollectionChanged -= layout.SampleViews_CollectionChanged;
        //        if (e.NewValue != null)
        //        {
        //            layout.SampleViews.CollectionChanged += layout.SampleViews_CollectionChanged;
        //            layout.AddItems(layout.SampleViews);
        //        }
        //    }
        //}

        //private void SampleViews_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    AddItems((IList<SampleView>)e.NewItems);
        //}

        //private void AddItems(IList<SampleView> views)
        //{
        //    foreach (SampleView element in views)
        //    {
        //        TabItem item = new TabItem();
        //        if (element.Header != null)
        //        {
        //            item.Header = element.Header;
        //            item.Content = element;
        //            samples.Items.Add(item);
        //        }
        //        element.SampleLayout = this;
        //    }
        //    if (samples.Items.Count > 0)
        //    {
        //        samples.SelectedIndex = 0;
        //    }
        //}
        //public TabControl SamplesTab
        //{
        //    get
        //    {
        //        return samples;
        //    }
        //}

        //public SampleView SelectedItem
        //{
        //    get
        //    {
        //        if (samples.SelectedItem != null)
        //            return ((TabItem)samples.SelectedItem).Content as SampleView;
        //        return null;
        //    }
        //}
    }

}
