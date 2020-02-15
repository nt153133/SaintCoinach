using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Generate.Xml
{
    //[XmlRoot("Profile")]
    public class Profile
    {

        
        [XmlElement("Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlElement("KillRadius")]
        [DefaultValue(25f)]
        public float KillRadius
        {
            get;
            set;
        }
        
        [XmlArray("GrindAreas")]
        public List<GrindArea> GrindAreas
        {
            get;
            set;
        }
        
        [XmlElement("Order")]
        public string Order
        {
            get;
            set;
        }
    }
}