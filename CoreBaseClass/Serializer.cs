using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreBaseClass
{
    public class Serializer
    {
        //public static T DeserializeFromStrng<T>(string value)
        //{
        //    return JsonConvert.DeserializeObject<T>(value);
        //}


        //public static string SerializeToString<T>(T value,bool is_indented = false)
        //{
        //    JsonSerializerSettings settings = new JsonSerializerSettings();
        //    settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        //    if (is_indented)
        //    {
        //        return JsonConvert.SerializeObject(value, Formatting.Indented, settings);
        //    }
            
        //    return JsonConvert.SerializeObject(value, Formatting.None, settings);
        //}

        public static string xml_SerializeToString<T>(T obj)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (var strWriter = new System.IO.StringWriter())
            {
                var xmlWriter = System.Xml.XmlWriter.Create(strWriter, new System.Xml.XmlWriterSettings { OmitXmlDeclaration = true });
                xmlSerializer.Serialize(xmlWriter, obj, new System.Xml.Serialization.XmlSerializerNamespaces(new[] { System.Xml.XmlQualifiedName.Empty }));
                return strWriter.ToString();
                
            }
        }

        public static T xml_DeserializeFromStrng<T>(string str)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                return (T)xmlSerializer.Deserialize(ms);
            }
        }


    }
}
