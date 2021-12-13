using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreBaseClass
{
   public class ConfigurationHelper
    {
        #region AppSettings

        /// <summary>
        /// 获取一个类型为<typeparam name="T">T</typeparam>的配置值
        /// 如果<paramref name="appSettingKey"/>的配置值为null、string.Empty或则不是有效的类型<typeparam name="T">T</typeparam>的字符串值，将抛出异常<seealso cref="System.ArgumentException"/>
        /// </summary>
        /// <param name="appSettingKey">Name of the appSettings key.</param>
        /// <returns></returns>
        public static T GetAppSetting<T>(string appSettingKey) where T : IConvertible
        {
            string value = ConfigurationManager.AppSettings[appSettingKey];

            if (value == null)
            {
                throw new ArgumentException(string.Format("'{0}' not defined in appSettings.", appSettingKey));
            }
            else
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    throw new ArgumentException(string.Format("appSettings '{0}' can not convert to {1}.", appSettingKey, typeof(T).FullName));
                }
            }
        }

        /// <summary>
        /// 获取一个类型为<typeparam name="T">T</typeparam>的配置值
        /// 如果<paramref name="appSettingKey"/>的配置值为null、string.Empty或则不是有效的类型<typeparam name="T">T</typeparam>的字符串值，将返回 <paramref name="defaultValue"/>
        /// </summary>
        /// <param name="appSettingKey">Name of the appSettings key.</param>
        /// <param name="defaultValue">The default value returned if the appSetting has not been defined.</param>
        /// <returns></returns>
        public static T GetAppSetting<T>(string appSettingKey, T defaultValue) where T : IConvertible
        {
            try
            {
                return GetAppSetting<T>(appSettingKey);
            }
            catch
            {
                return defaultValue;
            }
        }



        #endregion

        #region ConnectionString
        public static ConnectionStringSettings GetConnectionSection(string key)
        {
            return ConfigurationManager.ConnectionStrings[key];
        }

        public static string GetConnectionValue(string key, string defaultValue)
        {
            string d = ConfigurationManager.ConnectionStrings[key].ConnectionString;

            if (d == null)
            {
                return defaultValue;
            }
            else
            {
                return d;
            }
        }

        public static string GetConnectionString(string connectionStringName)
        {
            if (connectionStringName == null)
                throw new ArgumentNullException("connectionStringName");

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (settings == null)
                throw new Exception(string.Format("No connection string settings with the name '{0}'.", connectionStringName));

            return settings.ConnectionString;
        }
        #endregion

        #region Section
        public static Dictionary<string, string> GetSection(string sectionName)
        {
            Hashtable configs = ConfigurationManager.GetSection(sectionName) as Hashtable;
            Dictionary<string, string> contents = new Dictionary<string, string>();

            if (configs != null)
            {
                foreach (DictionaryEntry de in configs)
                {
                    contents.Add(de.Key.ToString(), de.Value.ToString());
                }
            }

            return contents;
        }

        /// <summary>
        /// 获取KeyValue类型配置节点段
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static NameValueCollection GetSectionGroup(string sectionName)
        {
            return (NameValueCollection)ConfigurationManager.GetSection(sectionName);
        }

        /// <summary>
        /// 获取KeyValue类型配置节点段中 <paramref name="sectionName"/> 对应的节点值
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string GetSectionGroupValue(string sectionName, string keyName)
        {
            return GetSectionGroup(sectionName)[keyName];
        }
        #endregion

        #region Other
        public static void AddConfigurationProperties(ConfigurationPropertyCollection collection, IEnumerable<ConfigurationProperty> properties)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (properties == null)
                throw new ArgumentNullException("properties");

            foreach (ConfigurationProperty property in properties)
            {
                collection.Add(property);
            }
        }
        #endregion
    }
}
