using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using System.Runtime.Serialization;
using Windows.Storage.Streams;
using System.Xml.Linq;

namespace Marketing2
{
    class LocalStorage
    {
        private static List<FeedData> _data = new List<FeedData>();

        public static List<FeedData> Data
        {
            get { return _data; }
            set { _data = value; }
        }

        private const string filename = "FeedDetails.xml";

        static async public Task SaveAsync<T>(List<FeedData> _data)
        {
            StorageFile sessionFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            IRandomAccessStream sessionRandomAccess = await sessionFile.OpenAsync(FileAccessMode.ReadWrite);
            IOutputStream sessionOutputStream = sessionRandomAccess.GetOutputStreamAt(0);
            var sessionSerializer = new DataContractSerializer(typeof(List<FeedData>), new Type[] { typeof(T) });
            sessionSerializer.WriteObject(sessionOutputStream.AsStreamForWrite(), _data);
            await sessionOutputStream.FlushAsync();
        }

        static async public Task<List<FeedData>> RestoreAsync<T>()
        {
            //StorageFile sessionFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
            try
            {
                StorageFile sessionFile = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
                if (sessionFile == null)
                {
                    return null;
                }
                IInputStream sessionInputStream = await sessionFile.OpenReadAsync();
                var sessionSerializer = new DataContractSerializer(typeof(List<FeedData>), new Type[] { typeof(T) });
                _data = (List<FeedData>)sessionSerializer.ReadObject(sessionInputStream.AsStreamForRead());

                return _data;

            }
            catch (FileNotFoundException fex)
            {
                return null;
            }
        }
    }
}
