using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Forms = System.Windows.Forms;

namespace BeyondDynamo
{
    /// <summary>
    /// This class represents the Beyond Dynamo Settings
    /// </summary>
    public class BeyondDynamoConfig
    {
        private string ConfigFilePath { get; set; }

        public int[] customColors { get; set; }

        public BeyondDynamoConfig(string configFilePath)
        {
            ConfigFilePath = configFilePath;
            if (File.Exists(ConfigFilePath))
            {
                string content = File.ReadAllText(ConfigFilePath);
                if(content != String.Empty)
                {
                    JToken config = JToken.Parse(content);
                    customColors = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(config["customColors"].ToString());
                }
            }
            else
            {
                File.Create(ConfigFilePath);
            }
        }

        /// <summary>
        /// Saves the current Content to Json format
        /// </summary>
        public void Save()
        {
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(this.ConfigFilePath))
            {
                file.WriteLine(jsonString);
            }
        }

    }
}
