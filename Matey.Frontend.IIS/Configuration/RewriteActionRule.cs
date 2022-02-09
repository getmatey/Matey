using System.Xml.Serialization;

namespace Matey.Frontend.IIS.Configuration
{
    public class RewriteActionRule
    {
        [XmlAttribute(AttributeName = "type")]
        public string? Type { get; set; }

        [XmlAttribute(AttributeName = "url")]
        public string? Url { get; set; }
    }
}
