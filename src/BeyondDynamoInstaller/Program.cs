﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace BeyondDynamoInstaller
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Make sure that Dynamo / Revit / Civil3D / Autocad are all closed! Hit any key to continue");
            Console.ReadLine();
            GitHubRequests.DynamoLocations = GitHubRequests.GetDynamoCorePaths();
            Console.WriteLine("Installation Started");
            string version = GitHubRequests.RequestLatestVersion().Result;
            Console.WriteLine(version);
            GitHubRequests.GetAssets().Wait();
            List<string> filepaths = GitHubRequests.DownloadAssets();
            GitHubRequests.CopyAssetsToLocations(filepaths);
            Console.WriteLine("Installation Completed!");
            Console.ReadLine();
        }
    }

    public class GitHubRequests
    {
        public const string GitHubAPI = @"https://api.github.com";

        public const string Owner = "JoelvanHerwaarden";
        public const string Repo = "BeyondDynamo2.X";
        public static string ReleaseId = null;
        public static Dictionary<string, string> Assets = null;

        public static List<string> DynamoLocations = new List<string>()
        {
                @"C:\Program Files\Autodesk",
                @"C:\Program Files\Dynamo\Dynamo Core",
                @"D:\Program Files\Autodesk",
                @"D:\Program Files\Dynamo\Dynamo Core"
        };

        public async static Task<string> RequestLatestVersion()
        {
            Console.WriteLine("Start Version Request");
            string result = null;
            string requestUri = string.Format(@"{0}/repos/{1}/{2}/releases", GitHubAPI, Owner, Repo);
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;
                List<double> releasedVersions = new List<double>();

                HttpWebRequest webRequest = WebRequest.CreateHttp(requestUri);
                webRequest.ContentType = "application/json";
                webRequest.UserAgent = "Foo";
                webRequest.Accept = "application/json";
                webRequest.Method = "GET";

                WebResponse response = webRequest.GetResponse();
                Stream dataStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);
                string json = reader.ReadToEnd();

                JToken githubReleases = JToken.Parse(json);
                foreach (JObject release in githubReleases.Children())
                {
                    JToken versionNumber = release.GetValue("tag_name");
                    releasedVersions.Add((double)versionNumber);
                }
                releasedVersions.Sort();
                string latestVersionNumber = releasedVersions[releasedVersions.Count - 1].ToString();
                foreach (JObject release in githubReleases.Children())
                {
                    JToken versionNumber = release.GetValue("tag_name");
                    if(versionNumber.ToString() == latestVersionNumber)
                    {
                        JToken releaseId = release.GetValue("id");
                        ReleaseId = releaseId.ToString();
                        Console.WriteLine(ReleaseId);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            Console.WriteLine("Finished Version Request");
            return result;
        }

        public async static Task GetAssets()
        {
            Console.WriteLine("Start Get Assets");
            string result = null;
            string requestUri = string.Format(@"{0}/repos/{1}/{2}/releases/{3}/assets", GitHubAPI, Owner, Repo, ReleaseId);
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;
                List<double> releasedVersions = new List<double>();

                HttpWebRequest webRequest = WebRequest.CreateHttp(requestUri);
                Clipboard.SetText(requestUri);
                webRequest.ContentType = "application/json";
                webRequest.UserAgent = "Foo";
                webRequest.Accept = "application/json";
                webRequest.Method = "GET";

                WebResponse response = webRequest.GetResponse();
                Stream dataStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);
                string json = reader.ReadToEnd();

                List<JToken> assets = JsonConvert.DeserializeObject<List<JToken>>(json);
                Assets = new Dictionary<string, string>();
                foreach (JToken asset in assets)
                {
                    string key = asset["name"].ToString();
                    string value = asset["browser_download_url"].ToString();
                    Console.WriteLine(key + " - " + value);
                    Assets.Add(key, value);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(requestUri);
            }
            Console.WriteLine("Finished Get Assets");
        }

        public static List<string> DownloadAssets()
        {
            List<string> filePaths = new List<string>();
            Console.WriteLine("Start Downloading Assets");
            using (var client = new WebClient())
            {
                string[] keys = new List<string> { "BeyondDynamo.dll", "BeyondDynamo_ViewExtensionDefinition.xml" }.ToArray();
                foreach (string key in keys)
                {
                    string filePath = Path.Combine(Path.GetTempPath(), key);
                    Console.WriteLine("Downloading " + key);
                    client.DownloadFile(Assets[key], filePath);
                    Console.WriteLine(filePath);
                    Console.WriteLine("Finished " + key);
                    filePaths.Add(filePath);
                }
            }
            Console.WriteLine("Finished Downloading Assets");
            return filePaths;
        }

        public static void CopyAssetsToLocations(List<string> filePaths)
        {
            Console.WriteLine("Started Copying Files");
            foreach (string location in DynamoLocations)
            {
                if (Directory.Exists(location))
                {
                    string source = filePaths[0];
                    string destPath = Path.Combine(location, Path.GetFileName(source));
                    try
                    {
                        File.Copy(source, destPath, true);
                        Console.WriteLine(string.Format("Copied {0} to {1}\n", Path.GetFileName(source), location));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(string.Format("Failed to Copy {0} to {1}", Path.GetFileName(source), location));
                        Console.WriteLine(e.Message + "\n");
                    }

                    source = filePaths[1];
                    string fileLocation = string.Format(@"{0}\viewExtensions", location);
                    destPath = Path.Combine(fileLocation, Path.GetFileName(source));
                    try
                    {
                        File.Copy(source, destPath, true);
                        Console.WriteLine(string.Format("Copied {0} to {1}\n", Path.GetFileName(source), fileLocation));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(string.Format("Failed to Copy {0} to {1}", Path.GetFileName(source), location));
                        Console.WriteLine(e.Message+"\n");
                    }
                }
            }
            Console.WriteLine("Finished Copying Files");
            Console.WriteLine("Start Deleting Download Files");
            foreach (string f in filePaths)
            {
                File.Delete(f);
            }
            Console.WriteLine("Finished Deleting Download Files");
        }

        public static List<string> GetDynamoCorePaths()
        {
            List<string> result = new List<string>();

            foreach(string path in DynamoLocations)
            {
                if (Directory.Exists(path))
                {
                    string[] folders = Directory.GetDirectories(path);
                    foreach (string folder in folders)
                    {
                        if (path.Split('\\').Last() == "Autodesk")
                        {
                            if (folder.Split('\\').Last().StartsWith("AutoCAD"))
                            {
                                string locationPath = Path.Combine(folder, @"C3D\Dynamo\Core");
                                if (Directory.Exists(locationPath))
                                {
                                    result.Add(locationPath);
                                }
                            }
                            else if(folder.Split('\\').Last().StartsWith("Revit"))
                            {
                                string locationPath = Path.Combine(folder, @"AddIns\DynamoForRevit");
                                if (Directory.Exists(locationPath))
                                {
                                    result.Add(locationPath);
                                }
                            }
                        }
                        else if (path.Split('\\').Last() == "Dynamo Core")
                        {
                            if (folder.Split('\\').Last().StartsWith("2"))
                            {
                                result.Add(folder);
                            }
                        }
                    }
                }
            }

            foreach(string p in result)
            {
                Console.WriteLine(p);
            }

            return result;
        }
    }
}
