using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace QuotesOfWisdom.Common
{
    class AppSettings
    {
        const string AutoPlayKeyName = "AutoPlay";
        const int AutoPlayDefault = 5;

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(Key))
            {
                // If the value has changed
                if (ApplicationData.Current.RoamingSettings.Values[Key] != value)
                {
                    // Store the new value
                    ApplicationData.Current.RoamingSettings.Values[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                ApplicationData.Current.RoamingSettings.Values[Key] = value;
                valueChanged = true;
            }

            return valueChanged;
        }


        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public valueType GetValueOrDefault<valueType>(string Key, valueType defaultValue)
        {
            valueType value;

            // If the key exists, retrieve the value.
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(Key))
            {
                value = (valueType)ApplicationData.Current.RoamingSettings.Values[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }

            return value;
        }

        public int AutoPlay
        {
            get
            {
                return GetValueOrDefault<int>(AutoPlayKeyName, AutoPlayDefault);
            }
            set
            {
                AddOrUpdateValue(AutoPlayKeyName, value);
                ApplicationData.Current.RoamingSettings.Values[AutoPlayKeyName] = AutoPlayDefault;
            }
        }
    }
}
