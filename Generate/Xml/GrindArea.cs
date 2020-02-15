using System.Collections.Generic;
using System.Xml.Serialization;

namespace Generate.Xml
{


    //[XmlRoot("GrindArea")]
    public class GrindArea
    {

        [XmlAttribute("Name")] public string Name { get; set; }

        [XmlElement("MinLevel")] public int MinLevel { get; set; }

        [XmlElement("MaxLevel")] public int MaxLevel { get; set; }

        //[XmlElement("Hotspots")]
        [XmlArray("Hotspots")]
        public List<HotSpot> Hotspots { get; set; }

        [XmlArray("TargetMobs", IsNullable = false)] public List<TargetMob> TargetMobs { get; set; }

       // [XmlElement("AvoidMobs")] public List<AvoidMob> AvoidMobs { get; set; }
    }

    //[XmlElement("Grind")]
    public class GrindTag //: ProfileBehavior
    {
        [XmlAttribute("grindRef")] public string GrindRef { get; set; }


        [XmlAttribute("While")]
        public string WhileCondition { get; set; }
    }
}
