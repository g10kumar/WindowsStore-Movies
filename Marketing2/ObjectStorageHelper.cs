using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.Storage.Streams;
using System.IO;

namespace Marketing2
{
    public class ObjectStorageHelper<T>
    {
        private ApplicationData appData = Windows.Storage.ApplicationData.Current;
        private XmlSerializer serializer;
        private StorageType storageType;


        public ObjectStorageHelper(StorageType StorageType)
        {
            serializer = new XmlSerializer(typeof(T));
            storageType = StorageType;
        }

        public async void DeleteASync(string FileName)
        {
            FileName = FileName + ".xml";
            try
            {
                StorageFolder folder = GetFolder(storageType);

                var file = await GetFileIfExistsAsync(folder, FileName);
                if (file != null)
                {
                    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async void SaveASync(T Obj, string FileName)
        {

            FileName = FileName + ".xml";
            try
            {
                if (Obj != null)
                {
                    StorageFile file = null;
                    StorageFolder folder = GetFolder(storageType);
                    file = await folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);

                    IRandomAccessStream writeStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                    Stream outStream = Task.Run(() => writeStream.AsStreamForWrite()).Result;
                    serializer.Serialize(outStream, Obj);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<T> LoadASync(string FileName)
        {
            FileName = FileName + ".xml";
            try
            {
                StorageFile file = null;
                StorageFolder folder = GetFolder(storageType);
                file = await folder.GetFileAsync(FileName);
                IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.Read);
                Stream inStream = Task.Run(() => readStream.AsStreamForRead()).Result;
                return (T)serializer.Deserialize(inStream);
            }
            catch (FileNotFoundException)
            {
                //file not existing is perfectly valid so simply return the default 
                return default(T);
                //throw;
            }
            catch (Exception)
            {
                //Unable to load contents of file
                throw;
            }
        }

        private StorageFolder GetFolder(StorageType storageType)
        {
            StorageFolder folder;
            switch (storageType)
            {
                case StorageType.Roaming:
                    folder = appData.RoamingFolder;
                    break;
                case StorageType.Local:
                    folder = appData.LocalFolder;
                    break;
                case StorageType.Temporary:
                    folder = appData.TemporaryFolder;
                    break;
                default:
                    throw new Exception(String.Format("Unknown StorageType: {0}", storageType));
            }
            return folder;
        }

        private async Task<StorageFile> GetFileIfExistsAsync(StorageFolder folder, string fileName)
        {
            try
            {
                return await folder.GetFileAsync(fileName);

            }
            catch
            {
                return null;
            }
        }
    }

    public enum StorageType
    {
        Roaming, Local, Temporary
    }
}
