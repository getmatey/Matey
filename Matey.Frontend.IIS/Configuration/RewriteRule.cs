

using System.Xml.Serialization;

namespace Matey.Frontend.IIS.Configuration
{
    public class RewriteRule
    {
        [XmlAttribute(AttributeName = "name")]
        public string? Name { get; set; }

        [XmlAttribute(AttributeName = "stopProcessing")]
        public bool StopProcessing { get; set; }

        [XmlElement(ElementName = "match")]
        public RewriteMatchRule? Match { get; set; }

        [XmlElement(ElementName = "action")]
        public RewriteRuleAction? Action { get; set; }
    }
}
