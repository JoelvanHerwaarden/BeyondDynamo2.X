using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Dynamo.Wpf.Extensions;
using Dynamo.ViewModels;
using BeyondDynamo.UI.About;
using BeyondDynamo.UI;
using Forms = System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using Dynamo.Models;
using Dynamo.Graph.Workspaces;
using Dynamo.Graph.Nodes;
using System.Windows;
using System.Windows.Input;
using Dynamo.Graph.Annotations;

namespace BeyondDynamo
{
    /// <summary>
    /// This Class is the Main Extension for Dynamo
    /// </summary>
    public class BeyondDynamoExtension : IViewExtension
    {
        /// <summary>
        /// FilePath for Config File
        /// </summary>
        private string configFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dynamo\\BeyondDynamoSettings");

        /// <summary>
        /// FilePath for Config File
        /// </summary>
        private string configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dynamo\\BeyondDynamoSettings\\beyondDynamo2Config.json");

        /// <summary>
        /// The Configurations for the Plug-in
        /// </summary>
        private BeyondDynamoConfig config;

        /// <summary>
        /// Request URL for the Releases
        /// </summary>
        private const string RequestUri = "https://api.github.com/repos/JoelvanHerwaarden/BeyondDynamo2.X/releases";

        /// <summary>
        /// This will get the Latest Version
        /// </summary>
        private MenuItem LatestVersion;

        /// <summary>
        /// Head Menu Item
        /// </summary>
        private MenuItem BDmenuItem;

        /// <summary>
        /// Change Node Colors Menuitem
        /// </summary>
        private MenuItem ChangeNodeColors;

        /// <summary>
        /// Remove Trace Data Menuitem
        /// </summary>
        private MenuItem BatchRemoveTraceData;

        /// <summary>
        /// Change Group Color Menu Item
        /// </summary>
        private MenuItem GroupColor;

        /// <summary>
        /// This Deselects the labels in the Background
        /// </summary>
        private MenuItem DeselectNodeLabels;

        /// <summary>
        /// Import Script Menu Item
        /// </summary>
        private MenuItem ScriptImport;

        /// <summary>
        /// Edit Notes Menu Item
        /// </summary>
        private MenuItem EditNotes;

        /// <summary>
        /// Freeze Multiple Nodes Menu Item
        /// </summary>
        private MenuItem FreezeNodes;


        /// <summary>
        /// Freeze Multiple Nodes Menu Item
        /// </summary>
        private MenuItem PreviewNodes;

        /// <summary>
        /// Order Player Inputs Menu Item
        /// </summary>
        private MenuItem OrderPlayerInput;

        /// <summary>
        /// Player script menu item
        /// </summary>
        private MenuItem PlayerScripts;

        /// <summary>
        /// Open Player Folderpath Menu Item
        /// </summary>
        private MenuItem OpenPlayerPath;

        /// <summary>
        /// Set Player Folderpath Menu Item
        /// </summary>
        private MenuItem SetPlayerPath;

        /// <summary>
        /// The Menu Item to remove the binding on the current graph
        /// </summary>
        private MenuItem RemoveBindingsCurrent;

        private MenuItem AutomaticPreviewOff;

        private MenuItem RenamePythonInputs;

        private MenuItem BDToolspace;

        /// <summary>
        /// About Window Menu Item
        /// </summary>
        private MenuItem AboutItem;

        /// <summary>
        /// Open Log File Menu Item
        /// </summary>
        private MenuItem OpenLog;
        
        public void Dispose() { }

        public void Startup(ViewStartupParams p)
        {
            Utils.SetupLog();
            Utils.LogMessage("Get Latest version Started...");
            try
            {

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                       | SecurityProtocolType.Tls11
                       | SecurityProtocolType.Tls12
                       | SecurityProtocolType.Ssl3;
                List<double> releasedVersions = new List<double>();

                HttpWebRequest webRequest = WebRequest.CreateHttp(RequestUri);
                webRequest.ContentType = "application/json";
                webRequest.UserAgent = "Foo";
                webRequest.Accept = "application/json";
                webRequest.Method = "GET";

                WebResponse response = webRequest.GetResponse();
                Stream dataStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);
                string result = reader.ReadToEnd();

                JToken githubReleases = JToken.Parse(result);
                foreach (JObject release in githubReleases.Children())
                {
                    JToken version = release.GetValue("tag_name");
                    releasedVersions.Add((double)version);
                }
                releasedVersions.Sort();
                this.latestVersion = releasedVersions[releasedVersions.Count - 1];
                Utils.LogMessage("Get Latest verion Completed!");
            }
            catch(Exception e)
            {
                Utils.LogMessage("Get Latest verion Failed!\n" + e.Message);
                this.latestVersion = this.currentVersion;
            }
            Utils.LogMessage("Latest version = " + this.latestVersion.ToString()); ;

            Utils.LogMessage("Creating Configuration File Started...");
            Directory.CreateDirectory(configFolderPath);
            config = new BeyondDynamoConfig(this.configFilePath);
            BeyondDynamoConfig.Current = config;

            Utils.LogMessage("Creating Configuration File Completed!");
        }

        public void Shutdown()
        {
            this.config.Save();
        }
        public string UniqueId
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }
        public string Name
        {
            get
            {
                return "Beyond Dynamo 2.0";
            }
        }

        /// <summary>
        /// Get the CUrrent Version of Beyond Dynamo for Dynamo 2.X
        /// </summary>
        private double currentVersion
        {
            get
            {
                return 1.4;
            }
        }

        /// <summary>
        /// The Latest version of Beyond Dynamo 2.X on Github
        /// </summary>
        private double latestVersion { get; set; }

        /// <summary>
        /// Function Which gets Called on Loading the Plug-In
        /// </summary>
        /// <param name="p">Parameters</param>
        public void Loaded(ViewLoadedParams p)
        {
            BDmenuItem = new MenuItem { Header = "Beyond Dynamo" };
            DynamoViewModel VM = p.DynamoWindow.DataContext as DynamoViewModel;
            BeyondDynamo.Utils.DynamoWindow = p.DynamoWindow;

            Utils.DynamoVM = VM;
            Utils.LogMessage("Loading Menu Items Started...");

            Utils.LogMessage("Loading Menu Items: Latest Version Started...");
            LatestVersion = new MenuItem { Header = "New version available! Download now!" };
            LatestVersion.Click += (sender, args) =>
            {
                System.Diagnostics.Process.Start("www.github.com/JoelvanHerwaarden/BeyondDynamo2.X/releases");
            };
            if (this.currentVersion < this.latestVersion)
            {
                BDmenuItem.Items.Add(LatestVersion);
            }
            else { Utils.LogMessage("Loading Menu Items: Latest Version is installed"); }

            Utils.LogMessage("Loading Menu Items: Latest Version Completed");

            #region THIS CAN BE RUN ANYTIME

            Utils.LogMessage("Loading Menu Items: Chang Node Colors Started...");
            ChangeNodeColors = new MenuItem { Header = "Change Node Color" };
            ChangeNodeColors.Click += (sender, args) =>
            {
                //Get the current Node Color Template
                System.Windows.ResourceDictionary dynamoUI = Dynamo.UI.SharedDictionaryManager.DynamoColorsAndBrushesDictionary;

                //Initiate a new Change Node Color Window
                ChangeNodeColorsWindow colorWindow = new ChangeNodeColorsWindow(dynamoUI, config)
                {
                    // Set the owner of the window to the Dynamo window.
                    Owner = BeyondDynamo.Utils.DynamoWindow
                };
                colorWindow.Left = colorWindow.Owner.Left + 400;
                colorWindow.Top = colorWindow.Owner.Top + 200;

                //Show the Color window
                colorWindow.Show();
            };
            ChangeNodeColors.ToolTip = new ToolTip()
            {
                Content = "This lets you change the Node Color Settings in your Dynamo nodes in In-Active, Active, Warning and Error state"
            };

            Utils.LogMessage("Loading Menu Items: Batch Remove Trace Data Started...");
            BatchRemoveTraceData = new MenuItem { Header = "Remove Session Trace Data from Dynamo Graphs" };
            BatchRemoveTraceData.Click += (sender, args) =>
            {
                //Make a ViewModel for the Remove Trace Data window

                //Initiate a new Remove Trace Data window
                RemoveTraceDataWindow window = new RemoveTraceDataWindow()
                {
                    Owner = p.DynamoWindow,
                    viewModel = VM
                };
                window.Left = window.Owner.Left + 400;
                window.Top = window.Owner.Top + 200;

                //Show the window
                window.Show();
            };
            BatchRemoveTraceData.ToolTip = new ToolTip()
            {
                Content = "Removes the Session Trace Data / Bindings from muliple Dynamo scripts in a given Directory" +
                "\n" +
                "\nSession Trace Data / Bindings is the trace data binding with the current Revit model and elements." +
                "\nIt can slow your scripts down if you run them because it first tries the regain the last session in which it was used."
            };

            Utils.LogMessage("Loading Menu Items: Order Player Nodes Started...");
            OrderPlayerInput = new MenuItem { Header = "Order Input/Output Nodes" };
            OrderPlayerInput.Click += (sender, args) =>
            {
                //Open a FileBrowser Dialog so the user can select a Dynamo Graph
                System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
                fileDialog.Filter = "Dynamo Files (*.dyn)|*.dyn";
                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (BeyondDynamoFunctions.IsFileOpen(VM, fileDialog.FileName))
                    {
                        Forms.MessageBox.Show("Please close the file before using this command", "Order Input/Output Nodes");
                        return;
                    }
                    //Get the selected filePath
                    string DynamoFilepath = fileDialog.FileName;
                    string DynamoString = File.ReadAllText(DynamoFilepath);
                    if (DynamoString.StartsWith("<"))
                    {
                        //Call the SortInputNodes Function
                        BeyondDynamoFunctions.SortInputOutputNodesXML(fileDialog.FileName);
                    }
                    else if (DynamoString.StartsWith("{"))
                    {
                        //Call the SortInputNodes Function
                        BeyondDynamoFunctions.SortInputOutputNodesJson(fileDialog.FileName);
                    }
                    else
                    {
                        return;
                    }
                }
            };
            OrderPlayerInput.ToolTip = new ToolTip()
            {
                Content = "Select a Dynamo file and it will let you sort the Nodes marked as 'Input'' & Output'" +
                "\n" +
                "\n Only Watch nodes which are marked as Output are displayed in the Dynamo Player. " +
                "\nOther nodes will show up in Refinery"
            };

            Utils.LogMessage("Loading Menu Items: Player Scripts Started...");
            PlayerScripts = new MenuItem { Header = "Player Graphs" };
            OpenPlayerPath = new MenuItem { Header = "Open Player Path" };
            OpenPlayerPath.Click += (sender, args) =>
            {
                System.Diagnostics.Process.Start(this.config.playerPath);
            };

            SetPlayerPath = new MenuItem { Header = "Set Player Path" };
            List<MenuItem> extraMenuItems = new List<MenuItem> { SetPlayerPath, OpenPlayerPath };
            SetPlayerPath.Click += (sender, args) =>
            {
                Forms.FolderBrowserDialog browserDialog = new Forms.FolderBrowserDialog();
                if (this.config.playerPath != null)
                {
                    browserDialog.SelectedPath = this.config.playerPath;
                }
                if (browserDialog.ShowDialog() == Forms.DialogResult.OK)
                {
                    if (browserDialog.SelectedPath != null)
                    {
                        PlayerScripts.Items.Clear();
                        BeyondDynamoFunctions.RetrievePlayerFiles(PlayerScripts, VM, browserDialog.SelectedPath, extraMenuItems);
                        this.config.playerPath = browserDialog.SelectedPath;
                    }
                }
            };
            Utils.LogMessage("Playerpath = " + this.config.playerPath);
            if (this.config.playerPath != null | this.config.playerPath != string.Empty)
            {
                try
                {
                    if (Directory.Exists(Path.GetDirectoryName(this.config.playerPath)))
                    {
                        BeyondDynamoFunctions.RetrievePlayerFiles(PlayerScripts, VM, this.config.playerPath, extraMenuItems);
                    }
                }
                catch (Exception e)
                {
                    Utils.LogMessage("Loading Player Path Warning: " + e.Message);
                    PlayerScripts.Items.Add(SetPlayerPath);
                }

            }
            else
            {
                PlayerScripts.Items.Add(SetPlayerPath);
            }

            //BDmenuItem.Items.Add(ChangeNodeColors);
            //Utils.LogMessage("Loading Menu Items: Chang Node Colors Completed");
            //BDmenuItem.Items.Add(BatchRemoveTraceData);
            //Utils.LogMessage("Loading Menu Items: Batch Remove Trace Data Completed");
            BDmenuItem.Items.Add(OrderPlayerInput);
            Utils.LogMessage("Loading Menu Items: Order Player Nodes Completed");
            BDmenuItem.Items.Add(PlayerScripts);
            Utils.LogMessage("Loading Menu Items: Player Scripts Completed");

            BDmenuItem.Items.Add(new Separator());
            BDmenuItem.Items.Add(new Separator());
            #endregion

            # region THESE FUNCTION ONLY WORK INSIDE A SCRIPT
            RemoveBindingsCurrent = new MenuItem { Header = "Remove Bindings from Current Graph" };
            RemoveBindingsCurrent.Click += (sender, args) =>
            {
                //Check if the Graph is saved somewhere
                WorkspaceViewModel workspaceViewModel = VM.CurrentSpaceViewModel;
                WorkspaceModel workspaceModel = workspaceViewModel.Model;
                string filePath = workspaceModel.FileName;
                bool succes = false;
                if (filePath == string.Empty)
                {
                    Forms.MessageBox.Show("Save the File before running this command");
                    return;
                }

                //Warn the user for the Restart
                Forms.DialogResult warningBox = Forms.MessageBox.Show("The Dynamo Graph will restart without Session Trace Data. \n\n The current graph will be saved", "Dynamo Graph Restart", System.Windows.Forms.MessageBoxButtons.OKCancel);
                if (warningBox == Forms.DialogResult.Cancel)
                {
                    return;
                }

                //Save the Graph, Close the Graph, Try to Remove Trace Data, Open the Graph again.
                workspaceViewModel.DynamoViewModel.SaveAs(filePath);
                VM.Model.RemoveWorkspace(VM.Model.CurrentWorkspace);
                succes = BeyondDynamoFunctions.RemoveBindings(filePath);
                if (succes)
                {
                    Forms.MessageBox.Show("Bindings removed", "Remove Bindings");
                }
                else
                {
                    Forms.MessageBox.Show("The current graph doesn't contain any Bindings yet", "Remove Bindings");
                }

                //Open workspace again
                VM.Model.OpenFileFromPath(filePath, true);
            };
            RemoveBindingsCurrent.ToolTip = new ToolTip()
            {
                Content = "Removes the Bindings to the last Session in Revit. \nIt will keep the Elements placed in the last Run"
            };

            DeselectNodeLabels = new MenuItem { Header = "Deselect Node Data" };
            DeselectNodeLabels.Click += (sender, args) =>
            {
                foreach (NodeModel node in VM.CurrentSpace.Nodes)
                {
                    node.DisplayLabels = true;
                }
                VM.ShowPreviewBubbles = false;
                foreach (NodeModel node in VM.CurrentSpace.Nodes)
                {
                    node.DisplayLabels = false;
                }
                VM.ShowPreviewBubbles = true;
            };
            DeselectNodeLabels.ToolTip = new ToolTip()
            {
                Content = "If you have selected something in the preview bubble of a Node, it will be selected in the Dynamo background.\n\n" +
                "This deselects all the labels in the preview."
            };

            GroupColor = new MenuItem { Header = "Change Group Color" };
            GroupColor.Click += (sender, args) =>
            {
                List<AnnotationModel> selectedGroups = BeyondDynamo.BeyondDynamoFunctions.GetAllSelectedGroups();
                this.config = BeyondDynamo.BeyondDynamoFunctions.ChangeGroupColor(selectedGroups) ;
            };
            GroupColor.ToolTip = new ToolTip()
            {
                Content = "Gives a Color Picker Dialog in which you can select a color to use for the selected groups"
            };

            ScriptImport = new MenuItem { Header = "Import From Graph" };
            ScriptImport.Click += (sender, args) =>
            {
                //Set the Run Type on Manual
                VM.CurrentSpaceViewModel.RunSettingsViewModel.SelectedRunTypeItem = new Dynamo.Wpf.ViewModels.RunTypeItem(RunType.Manual);

                //Try to Import the Graph
                BeyondDynamoFunctions.ImportFromScript(VM);
            };
            ScriptImport.ToolTip = new ToolTip()
            {
                Content = "This lets you select a Dynamo File and it will import the contents of that script inside the current workspace"
            };

            RenamePythonInputs = new MenuItem { Header = "Rename Python Inputs" };
            RenamePythonInputs.Click += (sender, args) =>
            {
                BeyondDynamoFunctions.RenamePythonInputs();
            };
            RenamePythonInputs.ToolTip = new ToolTip() { Content = "Rename the Input names and the Output name for a Python Node to make them more descriptive" };


            EditNotes = new MenuItem { Header = "Edit Note Text" };
            EditNotes.Click += (sender, args) =>
            {
                //Check if we are in an active graph
                if (VM.Workspaces.Count < 1)
                {
                    Forms.MessageBox.Show("This command can only run in an active graph.\nPlease open a Dynamo Graph to use this function.", "Beyond Dynamo");
                    return;
                }

                BeyondDynamoFunctions.CallTextEditor(VM.Model);
            };
            EditNotes.ToolTip = new ToolTip()
            {
                Content = "A Resizable window to edit selected Text Notes"
            };

            FreezeNodes = new MenuItem { Header = "Freeze Multiple Nodes" };
            FreezeNodes.InputGestureText = "Ctrl+E";
            FreezeNodesCommand command = new FreezeNodesCommand();
            FreezeNodes.Click += (sender, args) =>
            {
                FreezeNodesCommand.FreezeNodes();
            };
            Utils.DynamoWindow.CommandBindings.Add(new CommandBinding(command));
            KeyGesture shortkey = new KeyGesture(Key.E, System.Windows.Input.ModifierKeys.Control); ;
            Utils.DynamoWindow.InputBindings.Add(new InputBinding(command, shortkey));
            FreezeNodes.ToolTip = new ToolTip()
            {
                Content = "Freezes or Unfreezes all selected nodes and groups"
            };


            PreviewNodes = new MenuItem { Header = "Preview Multiple Nodes" };
            PreviewNodes.InputGestureText = "Ctrl+Q";
            PreviewNodesCommand previewCommand = new PreviewNodesCommand();
            PreviewNodes.Click += (sender, args) =>
            {
                PreviewNodesCommand.PreviewNodes();
            };
            Utils.DynamoWindow.CommandBindings.Add(new CommandBinding(previewCommand));
            KeyGesture previewShortKey = new KeyGesture(Key.Q, System.Windows.Input.ModifierKeys.Control); ;
            Utils.DynamoWindow.InputBindings.Add(new InputBinding(previewCommand, previewShortKey));
            PreviewNodes.ToolTip = new ToolTip()
            {
                Content = "Toggle the preview on and off for multiple nodes"
            };

            MenuItem EvaluateGroup = new MenuItem { Header = "Eval Group" };
            EvaluateGroup.Click += (sender, args) =>
            {
                BeyondDynamoFunctions.GetAllSelectedUngroupedItems();
            };
            


            BDToolspace = new MenuItem { Header = "Beyond Dynamo Toolspace" };
            BDToolspace.Click += (sender, args) =>
            {
                BeyondDynamoToolSpaceView panelView = new BeyondDynamoToolSpaceView();
                ToolSpaceControl toolspacecontrol = new ToolSpaceControl();
                try
                {

                    p.AddToExtensionsSideBar(panelView, toolspacecontrol);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Could not execute this function because:\n" + e.Message);
                    panelView.Dispose();
                }
            };
            BDToolspace.ToolTip = new ToolTip()
            {
                Content = "A Toolspace panel with the most used Beyond Dynamo at your disposal!"
            };

            AutomaticPreviewOff = new MenuItem { Header = "Automatic Preview Off" };
            AutomaticPreviewOff.IsChecked = config.hideNodePreview;
            BeyondDynamo.Utils.AutomaticHide = AutomaticPreviewOff.IsChecked;
            if (AutomaticPreviewOff.IsChecked)
            {
                VM.CurrentSpace.NodeAdded += BeyondDynamoFunctions.AutoNodePreviewOff;
            }
            AutomaticPreviewOff.Click +=(sender, args) =>
            {
                if (AutomaticPreviewOff.IsChecked)
                {
                    config.hideNodePreview = false;
                    AutomaticPreviewOff.IsChecked = false;
                    VM.CurrentSpace.NodeAdded -= BeyondDynamoFunctions.AutoNodePreviewOff;
                }
                else
                {
                    config.hideNodePreview = true;
                    AutomaticPreviewOff.IsChecked = true;
                    VM.CurrentSpace.NodeAdded += BeyondDynamoFunctions.AutoNodePreviewOff;
                }
            };

            if (double.Parse(VM.Version.Substring(0,3)) >= 2.4)
            {
                BDmenuItem.Items.Add(BDToolspace);
            }
            BDmenuItem.Items.Add(ScriptImport);
            BDmenuItem.Items.Add(RemoveBindingsCurrent);
            BDmenuItem.Items.Add(PreviewNodes);
            BDmenuItem.Items.Add(FreezeNodes);
            BDmenuItem.Items.Add(RenamePythonInputs);
            BDmenuItem.Items.Add(GroupColor);
            BDmenuItem.Items.Add(EditNotes);
            BDmenuItem.Items.Add(AutomaticPreviewOff);
            //BDmenuItem.Items.Add(EvaluateGroup);
            //BDmenuItem.Items.Add(DeselectNodeLabels);

            #endregion

            BDmenuItem.Items.Add(new Separator());
            BDmenuItem.Items.Add(new Separator());

            AboutItem = new MenuItem { Header = "About Beyond Dynamo" };
            AboutItem.Click += (sender, args) =>
            {
                //Show the About dialog
                About about = new About(this.currentVersion.ToString());
                about.Show();
            };
            AboutItem.ToolTip = new ToolTip()
            {
                Content = "Shows all the information about Beyond Dynamo"
            };

            BDmenuItem.Items.Add(AboutItem);
            OpenLog = new MenuItem() { Header = "Open Beyond Dynamo Log" };
            OpenLog.Click += (sender, args) =>
            {
                Utils.OpenLog();
            };
            OpenLog.ToolTip = new ToolTip() { Content = "Opens the Log file for Beyond Dynamo. \nThis is where all the activities are logged for Beyond Dynamo." };
            //BDmenuItem.Items.Add(OpenLog);

            p.dynamoMenu.Items.Add(BDmenuItem);

            #region ADD GRAPH DESCRIPTION

            #endregion ADD GRAPH DESCRIPTION

            Utils.LogMessage("Loading all Menu Items Completed");
        }
    }
}