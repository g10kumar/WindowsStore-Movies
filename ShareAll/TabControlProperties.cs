using Syncfusion.UI.Xaml.Controls;
using Syncfusion.UI.Xaml.Controls.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareAll.Common;

namespace ShareAll
{
    public class TabControlProperties:NotificationObject
    {
        public TabControlProperties()
        {
            Shares = new ObservableCollection<ShareSites>();
            Shares.Add(new ShareSites("Facebook", "FB title","FB subject","FB Message"));
            Shares.Add(new ShareSites("Twitter", "Twitter title", "Twitter subject", "Twitter Message"));
            Shares.Add(new ShareSites("Email", "Email title", "Email subject", "Email Message"));
        }

        private ObservableCollection<ShareSites> shares;

        public ObservableCollection<ShareSites> Shares
        {
            get { return shares; }
            set { shares = value; }
        }

        private TabStripPlacement tabPlacement = TabStripPlacement.Top;

        public TabStripPlacement TabPlacement
        {
            get { return tabPlacement; }
            set { tabPlacement = value; RaisePropertyChanged("TabPlacement"); }
        }

    }

    public class ShareSites
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        public ShareSites(string name, string title, string subject, string message)
        {
            Name = name;
            Title = title;
            Subject = subject;
            Message = message;
        }
    }
}
