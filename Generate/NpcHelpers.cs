using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FileHelpers;



namespace Generate
{
    public class NpcHelpers
    {
        private FileHelperEngine<MappyNPC>  engine = new FileHelperEngine<MappyNPC>();
        //private string MappyFile;
        private MappyNPC[] mappyDataResult; 
        private static string filename = $@"G:\MappyFiles\{DateTime.Today.ToString("dd-MM-yyyy")}.cvs";

        public NpcHelpers()
        {
            if (!File.Exists(filename))
            {
                DownloadNewVersion();
            }

            mappyDataResult = engine.ReadFile(filename);
        }

        public IEnumerable<MappyNPC> GetNpcsByName(string name)
        {
            return mappyDataResult.Where(i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public MappyNPC GetNpcByName(string name)
        {
            return mappyDataResult.FirstOrDefault(i => i.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public MappyNPC GetNpcById(int id)
        {
            return mappyDataResult.FirstOrDefault(i => i.BNpcBaseID == id || i.BNpcNameID == id || i.ENpcResidentID == id);
        }
        
        public IEnumerable<MappyNPC> GetNpcsById(int id)
        {
            return mappyDataResult.Where(i => i.BNpcBaseID == id || i.BNpcNameID == id || i.ENpcResidentID == id);
        }
        
        public void DownloadNewVersion()
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("https://xivapi.com/download?data=map_data", filename);
            }
        }
        
        
    }
}