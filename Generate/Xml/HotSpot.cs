using System.Xml.Serialization;
using Clio.Utilities;

namespace Generate.Xml
{
   // [XmlRoot("Hotspot")]
    public class HotSpot
    {
        private Vector3 _Position;
        private float _radius;

        public HotSpot()
        {
        }

        public HotSpot(string name, Vector3 position, float radius)
        {
            _Position = position;
            _radius = radius;
            Name = name;
        }

        [XmlAttribute("XYZ")]
        public string XYZ
        {
            get
            {
                return Position.ToString().Trim(new []{'<','>'});
            }
            set => Position = new Vector3(value);
        }

        [XmlIgnore]
        public Vector3 Position
        {
            get { return _Position; }
            set { _Position = value; }
        }


        [XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("Radius")]
        public float Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
            }
        }
        
    }
}