using System.ComponentModel;
using System.Xml.Serialization;

namespace Generate.Xml
{

        //[XmlRoot("TargetMob")]
        public class TargetMob
        {
            public string Name
            {
                get;
                set;
            }

            [XmlAttribute("id")]
            public int Id
            {
                get;
                set;
            }

            [DefaultValue(1f)]
            [XmlIgnore]
            //[XmlAttribute("Weight")]
            public float Weight
            {
                get;
                set;
            }
        }
    
}