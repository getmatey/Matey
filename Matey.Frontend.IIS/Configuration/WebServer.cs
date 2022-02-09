using System.Xml.Serialization;

namespace Matey.Frontend.IIS.Configuration
{

    public class WebServer
    {
        [XmlElement(ElementName = "rewrite")]
        public Rewrite? Rewrite { get; set; }
    }
}
