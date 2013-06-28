using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace QuotesOfWisdom.Common
{
    public class IncrementalSource<T, K> : ObservableCollection<K>, ISupportIncrementalLoading
        where T: IPagedSource<K>, new()
    {
        private string Query { get; set; }
        public int VirtualCount { get; set; }
        private int CurrentPage { get; set; }
        private IPagedSource<K> Source { get; set; }

        public IncrementalSource(string query)
        {
            this.Source = new T();
            this.VirtualCount = int.MaxValue;
            this.CurrentPage = 0;
            this.Query = query;
        }

        #region ISupportIncrementalLoading
        
        public bool HasMoreItems
        {
            get {
                //if (sessionData.isSearchClicked)
                //{
                //    sessionData.isSearchClicked = false;
                //    return false;
                //}
                return this.VirtualCount > this.CurrentPage * 12; }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            CoreDispatcher dispatcher = Window.Current.Dispatcher;

            return Task.Run<LoadMoreItemsResult>(
                async () =>
                {
                    
                    //sessionData.curPageCount = this.CurrentPage;
                    IPagedResponse<K> result;

                    if (sessionData.isSearchClicked)
                    {
                        result = null;
                        sessionData.isSearchClicked = false;
                    }
                    result = await this.Source.GetPage(this.Query, ++this.CurrentPage, 12);
                    
                    this.VirtualCount = result.VirtualCount;

                    await dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        () =>
                        {
                            foreach (K item in result.Items)
                                this.Add(item);
                        });

                    return new LoadMoreItemsResult() { Count = (uint)result.Items.Count() };

                }).AsAsyncOperation<LoadMoreItemsResult>();
        } 

        #endregion
    }
}