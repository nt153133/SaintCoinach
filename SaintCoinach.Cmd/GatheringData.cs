using System;
using System.Collections.Generic;

namespace SaintCoinach.Cmd
{

    public class GatheringData
    {
        /// <summary>
        /// AKA NPCID
        /// </summary>

        public uint NodeId { get; set; }

    
        public uint Bonus { get; set; }

    
        public uint Level { get; set; }

        /// <summary>
        /// 0 -> Mining
        /// 1 -> Quarrying
        /// 2 -> Logging
        /// 3 -> Harrvesting
        /// 4 -> SpearFishing
        /// </summary>
     
        public byte Job { get; set; } //

      
        public uint RequiredBook { get; set; }

     
        public uint Territory { get; set; }
     
        public string TerritoryName { get; set; }

      
        public uint Place { get; set; }
 
        public string PlaceName { get; set; }

   
        public float PlaceX { get; set; }

  
        public float PlaceY { get; set; }


      
        public bool Timed { get; set; }

   
        public List<uint> Timer { get; set; }

    
        public uint Duration { get; set; }

    

        public Level Position { get; set; }
    }
    
    public class Level
    {
        
      
        public int ObjectId { get; set; }
      
        public uint Id { get; set; }

        public int Type { get; set; }
    
        public ushort ZoneId { get; set; }
     
        public float X { get; set; }
      
        public float Y { get; set; }
      
        public float Z { get; set; }

     
        public float Radius { get; set; }

        public T ToVec3<T>() where T : new()
        {
            return (T)Activator.CreateInstance(typeof(T), X, Y, Z);
        }
    }
}