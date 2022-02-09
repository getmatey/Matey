using System.Xml.Serialization;

namespace Matey.Frontend.IIS.Configuration
{
    public class Rewrite
    {
        [XmlArray("rules"), XmlArrayItem("rule")]
        public List<RewriteRule> Rules { get; set; } = new List<RewriteRule>();
    }
}
