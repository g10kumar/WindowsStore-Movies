using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Foundation.Collections;
using System.IO;
using Windows.Storage;
using System.Xml;
using System.Xml.Linq;
using SQLite;

namespace QuotesOfWisdom.Data
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class QuotesCommon : QuotesOfWisdom.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");
        

        public QuotesCommon(String uniqueId, int ct, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._ct = ct;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
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

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(QuotesCommon._baseUri, this._imagePath));
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

    public abstract class SectionTitles : QuotesOfWisdom.Common.BindableBase
    {
        public SectionTitles(String title)
        {
            this._title = title;
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }
    }


    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class QuotesItem : QuotesCommon
    {
        public QuotesItem(String uniqueId, int ct, String title, String subtitle, String imagePath, String description, String content, QuotesGroup group, List<Quotations> Quotations)
            : base(uniqueId, ct, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
            this._quotations = Quotations;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private List<Quotations> _quotations;
        public List<Quotations> Quotations
        {
            get { return this._quotations; }
            set { this.SetProperty(ref this._quotations, value); }
        }

        private QuotesGroup _group;
        public QuotesGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class QuotesGroup : SectionTitles
    {
        public QuotesGroup(String title)
            : base(title)
        {
        }

        private ObservableCollection<QuotesItem> _items = new ObservableCollection<QuotesItem>();
        public ObservableCollection<QuotesItem> Items
        {
            get { return this._items; }
        }

        public IEnumerable<QuotesItem> TopItems
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed
            get { return this._items.Take(48); }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// </summary>
    public sealed class Quotes
    {
        private static Quotes _Quotes = new Quotes();
        public event EventHandler QuotesLoaded;

        private ObservableCollection<QuotesGroup> _allGroups = new ObservableCollection<QuotesGroup>();
        public ObservableCollection<QuotesGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        private ObservableCollection<QuotesGroup> _catGroups = new ObservableCollection<QuotesGroup>();
        public ObservableCollection<QuotesGroup> CatGroups
        {
            get { return this._catGroups; }
        }

        private ObservableCollection<QuotesGroup> _autGroups = new ObservableCollection<QuotesGroup>();

        public ObservableCollection<QuotesGroup> AuthorGroups
        {
            get { return this._autGroups; }
        }

        private ObservableCollection<QuotesGroup> _searchCatGroups = new ObservableCollection<QuotesGroup>();
        public ObservableCollection<QuotesGroup> SearchCatGroups
        {
            get { return this._searchCatGroups; }
        }

        private ObservableCollection<QuotesGroup> _searchAutGroups = new ObservableCollection<QuotesGroup>();
        public ObservableCollection<QuotesGroup> SearchAutGroups
        {
            get { return this._searchAutGroups; }
        }

        public static IEnumerable<QuotesGroup> GetGroups(string title)
        {
            if (!title.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _Quotes.AllGroups;
        }

        public static IEnumerable<QuotesGroup> GetCatGroups(string title)
        {
            if (!title.Equals("CatGroups")) throw new ArgumentException("Only 'CatGroups' is supported as a collection of groups");

            return _Quotes.CatGroups;
        }

        public static IEnumerable<QuotesGroup> GetAutGroups(string title)
        {
            if (!title.Equals("AuthorGroups")) throw new ArgumentException("Only 'AuthorGroups' is supported as a collection of groups");

            return _Quotes.AuthorGroups;
        }

        public static IEnumerable<QuotesGroup> GetSearchAutGroups(string title)
        {
            if (!title.Equals("SearchAutGroups")) throw new ArgumentException("Only 'SearchAutGroups' is supported as a collection of groups");

            return _Quotes.SearchAutGroups;
        }

        public static IEnumerable<QuotesGroup> GetSearchCatGroups(string title)
        {
            if (!title.Equals("SearchCatGroups")) throw new ArgumentException("Only 'SearchCatGroups' is supported as a collection of groups");

            return _Quotes.SearchCatGroups;
        }

        public static QuotesGroup GetGroup(string title)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _Quotes.AllGroups.Where((group) => group.Title.Equals(title));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static QuotesItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _Quotes.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static QuotesItem GetCatItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _Quotes.CatGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static QuotesItem GetAuthItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _Quotes.AuthorGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static QuotesItem GetSearchCatItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _Quotes.SearchCatGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static QuotesItem GetSearchAuthItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _Quotes.SearchAutGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public Quotes()
        {
            //var group1 = new QuotesGroup("Category");

            

            GetCategories();
            //group1.Items.Add(new QuotesItem(cat.ct.ToString(), cat.title, "", cat.title + ".jpg", "", "", group1));
            //group1.Items.Add(new QuotesItem(cat.ct.ToString(), cat.title, "", cat.title + ".jpg", "", "", group1));
            //group1.Items.Add(new QuotesItem(cat.ct.ToString(), cat.title, "", cat.title + ".jpg", "", "", group1));

            //var group1 = new QuotesGroup("Category");

            GetAuthors();

            //if (sessionData.searchWord != null)
            //{
            getSearchCategtories(sessionData.searchKeyWord);
            getSearchAuthors(sessionData.searchKeyWord);
            //}

        }

        public async void GetCategories()
        {
            List<Categories> listCategory = new List<Categories>();

            Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Xml");
            var sf = await storageFolder.GetFileAsync(@"Category.xml");
            var file = await sf.OpenAsync(FileAccessMode.Read);
            Stream inStream = file.AsStreamForRead();
            //XDocument xdoc = XDocument.Load(inStream);
            XElement xe = XElement.Load(inStream);

            var cats = (from category in xe.Elements("cat")
                        //where category.Value.Substring(0, 1) == c.ToString()
                        orderby (int)category.Attribute("count")
                        select new
                        {
                            ct = (int)category.Attribute("count"),
                            title = category.Value,
                        });

            cats = cats.AsEnumerable().Reverse();//.Take(48);

            var group1 = new QuotesGroup("Categories");

            #region Category Groups
            var groupCat = new QuotesGroup("Categories");

            var catGroups = (from category in xe.Elements("cat")
                             orderby category.Value
                             select new Categories
                             {
                                 category = category.Value,
                                 ct = (int)category.Attribute("count")
                             });

            var cGroups = catGroups.ToList();

            //loop the author details and binds to authorlist
            foreach (Categories c in catGroups)
            {
                listCategory.Add(c);
            }

            //sessionData.currentCategoryQuotes = listCategory;
            sessionData.currentCategories = listCategory;
            #endregion

            int cnt = 0;
            foreach (var cat in cats)
            {
                //List<Quotations> list = new List<Quotations>();

                // reads the quotes data based on category
                //list = LoadQuotes(cat.title, "Category");

                group1.Items.Add(new QuotesItem(cat.title, cat.ct, cat.title, "Category", "ms-appx:///Assets/category/" + cat.title.Replace("&", "") + ".jpg", "", "", group1, null));

                #region Category Groups
                groupCat.Items.Add(new QuotesItem(cGroups[cnt].category, cGroups[cnt].ct, cGroups[cnt].category, "Category", "", "", "", groupCat, null));
                cnt++;
                #endregion
            }
            this.AllGroups.Add(group1);

            #region Category Groups

            this.CatGroups.Add(groupCat);

            #endregion

            if (QuotesLoaded != null)
                QuotesLoaded(this, null);

        }

        public async void GetAuthors()
        {
            List<Authors> listAuthor = new List<Authors>();

            Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Xml");
            var sf = await storageFolder.GetFileAsync(@"Author.xml");
            var file = await sf.OpenAsync(FileAccessMode.Read);
            Stream inStream = file.AsStreamForRead();
            //XDocument xdoc = XDocument.Load(inStream);
            XElement xe = XElement.Load(inStream);



            var cats = (from author in xe.Elements("auth")
                        //where category.Value.Substring(0, 1) == c.ToString()
                        orderby (int)author.Attribute("count")
                        select new
                        {
                            ct = (int)author.Attribute("count"),
                            title = author.Value,
                        });
            cats = cats.AsEnumerable().Reverse();

            var group1 = new QuotesGroup("Authors");

            #region Author Groups

            var groupAut = new QuotesGroup("Authors");

            var autGroups = (from author in xe.Elements("auth")
                             where (int)author.Attribute("count") < 5
                             orderby author.Value
                             select new Authors
                             {
                                 Author = author.Value,
                                 ct = (int)author.Attribute("count")
                             });

            var aGroups = autGroups.ToList();

            //loop the author details and binds to authorlist
            foreach (Authors a in autGroups)
            {
                listAuthor.Add(a);
            }

            //sessionData.currentAuthorQuotes = listAuthor;
            sessionData.currentAuthors = listAuthor;

            #endregion

            int acnt = 0;
            foreach (var cat in cats)
            {
                //List<Quotations> list = new List<Quotations>();

                // reads the quotes data based on Author
                //list = LoadQuotes(cat.title, "Author");

                group1.Items.Add(new QuotesItem(cat.title, cat.ct, cat.title, "Author", "ms-appx:///Assets/author/" + cat.title + ".jpg", "", "", group1, null));

                #region Author Groups
                if (acnt != aGroups.Count())
                {
                    groupAut.Items.Add(new QuotesItem(aGroups[acnt].Author, aGroups[acnt].ct, aGroups[acnt].Author, "Author", "", "", "", groupAut, null));
                    acnt++;
                }
                #endregion
            }
            this.AllGroups.Add(group1);

            #region Authors Groups

            this.AuthorGroups.Add(groupAut);

            #endregion

            if (QuotesLoaded != null)
                QuotesLoaded(this, null);

        }

        private List<Quotations> LoadQuotes(string name, string type)
        {
            List<Quotations> list = new List<Quotations>();
            try
            {
                

                if (type == "Category")
                {
                    using (SQLiteConnection db = new SQLiteConnection("thefile2.db", SQLiteOpenFlags.ReadOnly))
                    {
                        var query = from q in db.Table<Quotations>()
                                    where q.Cat.Contains(name) == true
                                    select q;

                        list = query.ToList();
                    }
                }
                else if (type == "Author")
                {
                    using (SQLiteConnection db = new SQLiteConnection("thefile2.db", SQLiteOpenFlags.ReadOnly))
                    {
                        var query = from q in db.Table<Quotations>()
                                    where q.Author.Contains(name) == true
                                    select q;

                        list = query.ToList();
                    }
                }

                


            }
            catch (Exception ex)
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                dialog.ShowAsync();
            }

            return list;
        }

        public void getSearchCategtories(string queryText)
        {
            using (SQLiteConnection db = new SQLiteConnection("thefile2.db", SQLiteOpenFlags.ReadOnly))
            {
                var query = (from q in db.Table<Categories>()
                             where q.category.Contains(queryText) == true
                             select new Categories
                             {
                                 category = q.category.Replace("&amp;", "&"),
                                 ct = (int)q.ct
                             });

                var groupSearchCat = new QuotesGroup("Categories");

                var scGroups = query.ToList();
                int sccnt = 0;
                foreach (Categories q in query)
                {
                    groupSearchCat.Items.Add(new QuotesItem(scGroups[sccnt].category, scGroups[sccnt].ct, scGroups[sccnt].category, "Category", "", "", "", groupSearchCat, null));
                    sccnt++;
                }

                this.SearchCatGroups.Add(groupSearchCat);

                if (QuotesLoaded != null)
                    QuotesLoaded(this, null);
            }
        }

        public void getSearchAuthors(string queryText)
        {
            using (SQLiteConnection db = new SQLiteConnection("thefile2.db", SQLiteOpenFlags.ReadOnly))
            {
                var query = (from q in db.Table<Authors>()
                             where q.Author.Contains(queryText) == true
                             select new Authors
                             {
                                 Author = q.Author.Replace("&amp;", "&"),
                                 ct = (int)q.ct
                             });

                var saGroups = query.ToList();
                int sacnt = 0;

                var groupAut = new QuotesGroup("Authors");
                //loop the author details and binds to authorlist
                foreach (Authors a in query)
                {
                    groupAut.Items.Add(new QuotesItem(saGroups[sacnt].Author, saGroups[sacnt].ct, saGroups[sacnt].Author, "Author", "", "", "", groupAut, null));
                    sacnt++;
                }

                this.SearchAutGroups.Add(groupAut);

                if (QuotesLoaded != null)
                    QuotesLoaded(this, null);
            }
        }

        //public static void Shuffle<Quotations>(this IList<Quotations> list)
        //{
        //    Random rng = new Random();
        //    int n = list.Count;
        //    while (n > 1)
        //    {
        //        n--;
        //        int k = rng.Next(n + 1);
        //        Quotations value = list[k];
        //        list[k] = list[n];
        //        list[n] = value;
        //    }
        //}
       
    }
}
