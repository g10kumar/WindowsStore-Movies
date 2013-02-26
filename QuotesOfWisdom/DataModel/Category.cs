using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.IO;
using Windows.Data.Xml.Dom;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage;

namespace QuotesOfWisdom.Data
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class CategoryCommon : QuotesOfWisdom.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public CategoryCommon(int ct, String title, String imagePath)
        {
            this._ct = ct;
            this._title = title;
            //this._subtitle = subtitle;
            //this._description = description;
            this._imagePath = imagePath;
        }

        private int _ct = 0;
        public int Ct
        {
            get { return this._ct; }
            set { this.SetProperty(ref this._ct, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(CategoryCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }
    }

    /// <summary>
    /// Category Chars class
    /// </summary>
    public abstract class CategoryChars : QuotesOfWisdom.Common.BindableBase
    {
        public CategoryChars(String chars)
        {
            this._chars = chars;
        }

        private string _chars = string.Empty;
        public string Chars
        {
            get { return this._chars; }
            set { this.SetProperty(ref this._chars, value); }
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class CategoryItem : CategoryCommon
    {
        public CategoryItem(int ct, String title, String imagePath, CategoryGroup group)
            : base(ct, title, imagePath)
        {
            //this._content = content;
            this._group = group;
        }

        //private string _content = string.Empty;
        //public string Content
        //{
        //    get { return this._content; }
        //    set { this.SetProperty(ref this._content, value); }
        //}

        private CategoryGroup _group;
        public CategoryGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class CategoryGroup : CategoryChars
    {
        public CategoryGroup(String chars)
            : base(chars)
        {
        }

        private ObservableCollection<CategoryItem> _items = new ObservableCollection<CategoryItem>();
        public ObservableCollection<CategoryItem> Items
        {
            get { return this._items; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// </summary>
    public sealed class Category
    {
        private ObservableCollection<CategoryGroup> _itemGroups = new ObservableCollection<CategoryGroup>();
        public ObservableCollection<CategoryGroup> ItemGroups
        {
            get { return this._itemGroups; }
        }

        public Category()
        {
            GetCategories();
        }

        public async void GetCategories()
        {
            Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Xml");
            var sf = await storageFolder.GetFileAsync(@"Category.xml");
            var file = await sf.OpenAsync(FileAccessMode.Read);
            Stream inStream = file.AsStreamForRead();
            //XDocument xdoc = XDocument.Load(inStream);
            XElement xe = XElement.Load(inStream);


            string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            foreach(char c in Alphabet)
            {
                var cats = (from category in xe.Elements("cat")
                            where category.Value.Substring(0,1) == c.ToString()
                                select new
                                {
                                    ct = (int)category.Attribute("count"),
                                    title = category.Value,
                                 });


                var group1 = new CategoryGroup(c.ToString());
                foreach(var cat in cats)
                {
                    group1.Items.Add(new CategoryItem(cat.ct, cat.title, cat.title + ".jpg", group1));
                }
                this.ItemGroups.Add(group1);
            }

        
    //        if (ItemGroups == null) 
    //               ItemGroups = new ObservableCollection<CategoryGroup>();
    //          ItemGroups.Clear(); 
        }
    }

}
