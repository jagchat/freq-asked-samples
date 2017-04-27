using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Some.Common
{
    [XmlRoot("Behaviors")]
    public class Behaviors
    {
        public Behaviors() { Items = new List<Behavior>();}

        [XmlElement("Behavior")]
        public List<Behavior> Items { get; set; }
    }

    public class Behavior
    {
        public Behavior() { Methods = new List<Method>(); }

        [XmlAttribute("ClassType")]
        public string ClassType { get; set; }
        public List<Method> Methods { get; set; }
    }

    [XmlRoot("Method")]
    public class Method
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("Assembly")]
        public string Assembly { get; set; }
        [XmlAttribute("Class")]
        public string Class { get; set; }
        [XmlAttribute("Pre")]
        public string Pre { get; set; }
        [XmlAttribute("Post")]
        public string Post { get; set; }
        [XmlAttribute("Override")]
        public string Override { get; set; }
    }
}
