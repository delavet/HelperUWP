using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace HelperUWP.Lib.Storage
{
    class Editor
    {
        private static String TAG = "pkuhelper";

        public static String getString(String key, String def_value)
        {

            var rootContainer = ApplicationData.Current.LocalSettings;
            Object temp_value;
            if (rootContainer.Values.TryGetValue(key, out temp_value))
            {
                if (!(temp_value is String)) return def_value;
                return temp_value as String;
            }
            return def_value;
        }
        public static String getString(String key)
        {
            return getString(key, "");
        }

        public static Int32 getInt(String key, Int32 def_value)
        {
            var rootContainer = ApplicationData.Current.LocalSettings;
            Object temp_value;
            if (rootContainer.Values.TryGetValue(key, out temp_value))
            {
                if (!(temp_value is Int32)) return def_value;
                return (Int32)temp_value;
            }
            return def_value;
        }
        public static Int32 getInt(String key)
        {
            return getInt(key, 0);
        }

        public static Int64 getLong(String key, Int64 def_value)
        {
            var rootContainer = ApplicationData.Current.LocalSettings;
            Object temp_value;
            if (rootContainer.Values.TryGetValue(key, out temp_value))
            {
                if (!(temp_value is Int64)) return def_value;
                return (Int64)temp_value;
            }
            return def_value;
        }
        public static Int64 getLong(String key)
        {
            return getLong(key, 0);
        }

        public static Boolean getBoolean(String key, Boolean def_value)
        {
            var rootContainer = ApplicationData.Current.LocalSettings;
            Object temp_value;
            if (rootContainer.Values.TryGetValue(key, out temp_value))
            {
                if (!(temp_value is Boolean)) return def_value;
                return (Boolean)temp_value;
            }
            return def_value;
        }
        public static Boolean getBoolean(String key)
        {
            return getBoolean(key, false);
        }
        public static void putBoolean(String key, Boolean value)
        {
            var rootContainer = ApplicationData.Current.LocalSettings;
            rootContainer.Values[key] = value;
        }

        public static void putString(String key, String value)
        {
            var rootContainer = ApplicationData.Current.LocalSettings;
            rootContainer.Values[key] = value;
        }

        public static void putInt(String key, Int32 value)
        {
            var rootContainer = ApplicationData.Current.LocalSettings;
            rootContainer.Values[key] = value;
        }

        public static void putLong(String key, Int64 value)
        {
            var rootContainer = ApplicationData.Current.LocalSettings;
            rootContainer.Values[key] = value;
        }

        public static void remove(String key)
        {
            Object temp;
            var rootContainer = ApplicationData.Current.LocalSettings;
            if (rootContainer.Values.TryGetValue(key, out temp))
            {
                rootContainer.Values.Remove(key);
            }
        }

        public static void clear()
        {
            var rootContainer = ApplicationData.Current.LocalSettings;
            rootContainer.Values.Clear();
        }
    }
}
