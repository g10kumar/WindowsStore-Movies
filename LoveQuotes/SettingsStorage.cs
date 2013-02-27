using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LoveQuotes
{
    class SettingsStorage
    {
        public static T GetLocalSetting<T>(string key)
        {
            T result = default(T);

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                result = (T)ApplicationData.Current.LocalSettings.Values[key];
            }

            return result;
        }

        public static void SetLocalSetting(string key, Object value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }
    }
}
