using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace TracerLib
{
    public class Json_Serializer : ISerializer
    {
        private JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            Formatting = Formatting.Indented,
        };
        public string Serialize(object Object)
        {
            return JsonConvert.SerializeObject(Object, _settings);
        }
    }
    
    public class Xml_Serializer : ISerializer
    {
        private XmlSerializerNamespaces _xmlSerializerNamespaces = new XmlSerializerNamespaces(new []{ XmlQualifiedName.Empty });
        public string Serialize(object Object)
        {
            
        }
    }
}