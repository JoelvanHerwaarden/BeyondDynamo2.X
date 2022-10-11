using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dynamo.Graph.Annotations;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Notes;
using Dynamo.Graph;
using Dynamo.Models;
using Dynamo.ViewModels;
using Newtonsoft.Json.Linq;
using Forms = System.Windows.Forms;
using BeyondDynamo.Utils;

namespace BeyondDynamo
{
    /// <summary>
    /// This class represents the Beyond Dynamo Settings
    /// </summary>
    public class BeyondDynamoConfig
    {
        private string ConfigFilePath { get; set; }

        public int[] customColors { get; set; }

        public bool hideNodePreview { get; set; }

        public static BeyondDynamoConfig Current { get; set; }

        public BeyondDynamoConfig(string configFilePath)
        {
            ConfigFilePath = configFilePath;
            if (File.Exists(ConfigFilePath))
            {
                string content = File.ReadAllText(ConfigFilePath);
                if (content != String.Empty)
                {
                    JToken config = JToken.Parse(content);
                    customColors = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(config["customColors"].ToString());
                    try
                    {
                        string hidePreview = config["hideNodePreview"].ToString();
                        BeyondDynamoUtils.LogMessage(hidePreview);
                        if (Boolean.Parse(hidePreview))
                        {
                            hideNodePreview = true;
                        }
                        else
                        {
                            hideNodePreview = false;
                        }
                    }
                    catch(Exception exception)
                    {
                        BeyondDynamoUtils.LogMessage("Error Hide Node Previews: " + exception.Message);
                        hideNodePreview = false;
                    }
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
