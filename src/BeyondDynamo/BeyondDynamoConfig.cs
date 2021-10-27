using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dynamo.ViewModels;
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

        public string playerPath { get; set; }

        public bool hideNodePreview { get; set; }

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
                        if (config["playerPath"].ToString() != "null")
                        {
                            playerPath = config["playerPath"].ToString();
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception exception)
                    {
                        Utils.LogMessage("Error loading Player path: " + exception.Message);
                        playerPath = null;
                    }
                    try
                    {
                        string hidePreview = config["hideNodePreview"].ToString();
                        Utils.LogMessage(hidePreview);
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
                        Utils.LogMessage("Error Hide Node Previews: " + exception.Message);
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

    public class Utils
    {
        public static Window DynamoWindow = null;
        public static DynamoViewModel DynamoVM = null;
        public static bool AutomaticHide = false;
        private static string fileName = "BeyondDynamo.Log";
        private static string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Dynamo\BeyondDynamoSettings");
        private static string filePath = Path.Combine(folderPath, fileName);

        public static void SetupLog(string FileName = null)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (FileName != null)
            {
                fileName = FileName;
                filePath = Path.Combine(folderPath, fileName);
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            LogMessage("New Log file created");
        }
        public static void LogMessage(string message)
        {
            using (StreamWriter streamWriter = new StreamWriter(filePath, true))
            {
                string time = DateTime.Now.ToString("HH:mm:ss");
                string msg = time + ": " + message;
                streamWriter.WriteLine(msg);
                if(DynamoVM != null)
                {
                    DynamoVM.WriteToLogCmd.Execute(msg);
                }
            }
        }
        public static void OpenLog()
        {
            System.Diagnostics.Process.Start(filePath);
        }

    }
}
