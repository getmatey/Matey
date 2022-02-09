using System.Xml.Serialization;

namespace Matey.Frontend.IIS.Configuration
{
    public class RewriteMatchRule
    {
        [XmlAttribute(AttributeName = "url")]
        public string? Url { get; set; }
    }
}
