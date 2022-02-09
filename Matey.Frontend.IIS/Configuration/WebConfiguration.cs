using System.Xml.Serialization;

namespace Matey.Frontend.IIS.Configuration
{
    [XmlRoot(ElementName = "configuration")]
    public class WebConfiguration
    {
        [XmlElement(ElementName = "system.webServer")]
        public WebServer WebServer { get; set; } = new WebServer();
    }
}
