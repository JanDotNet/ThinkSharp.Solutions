using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ThinkSharp.Solutions.TemplateDefinition
{
    [XmlRoot]
    public class TemplateDefinition
    {
        [XmlArray("Placeholders")]
        [XmlArrayItem("Placeholder")]
        public Placeholder[] Placeholders { get; set; }
    }

    public class Placeholder
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Description { get; set; }
        [XmlAttribute]
        public string SuggestionList { get; set; }
        [XmlAttribute]
        public string DefaultValue { get; set; }
    }
}
