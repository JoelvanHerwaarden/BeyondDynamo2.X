using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Dynamo.Wpf.Extensions;
using Dynamo.ViewModels;
using BeyondDynamo.UI.About;
using System.Xml;
using Forms = System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace BeyondDynamo
{
    public class BeyondDynamoExtension : IViewExtension
    {
        /// <summary>
        /// Head Menu Item
        /// </summary>
        private MenuItem BDmenuItem;

        /// <summary>
        /// Change Group Color Menu Item
        /// </summary>
        private MenuItem GroupColor;

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
        /// Unfreeze Multiple Nodes Menu Item
        /// </summary>
        private MenuItem UnfreezeNodes;

        /// <summary>
        /// Order Player Inputs Menu Item
        /// </summary>
        private MenuItem OrderPlayerInput;

        /// <summary>
        /// About Window Menu Item
        /// </summary>
        private MenuItem AboutItem;

        #region Functions which have to be inplemented for the IViewExtension interface
        public void Dispose() { }

        public void Startup(ViewStartupParams p) { }
        
        private void CurrentSpaceViewModel_WorkspacePropertyEditRequested(Dynamo.Graph.Workspaces.WorkspaceModel workspace)
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
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
                return "Remove Trace Data";
            }
        }
        #endregion

        /// <summary>
        /// Function Which gets Called on Loading the Plug-In
        /// </summary>
        /// <param name="p">Parameters</param>
        public void Loaded(ViewLoadedParams p)
        {
            BDmenuItem = new MenuItem { Header = "Beyond Dynamo" };
            DynamoViewModel VM = p.DynamoWindow.DataContext as DynamoViewModel;

            //This can be run anytime
            #region

            OrderPlayerInput = new MenuItem { Header = "Order Input/Output Nodes" };
            OrderPlayerInput.Click += (sender, args) =>
            {
                //Open a FileBrowser Dialog so the user can select a Dynamo Graph
                System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
                fileDialog.Filter = "Dynamo Files (*.dyn)|*.dyn";
                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if(BeyondDynamoFunctions.IsFileOpened(VM, fileDialog.FileName))
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

            AboutItem = new MenuItem { Header = "About Beyond Dynamo" };
            AboutItem.Click += (sender, args) =>
            {
                //Show the About dialog
                About about = new About();
                about.Show();
            };
            # endregion

            //These can only run inside a Graph
            #region

            GroupColor = new MenuItem { Header = "Change Group Color" };
            GroupColor.Click += (sender, args) =>
            {
                BeyondDynamo.BeyondDynamoFunctions.ChangeGroupColor(VM.CurrentSpaceViewModel);
            };

            ScriptImport = new MenuItem { Header = "Import From Script" };
            ScriptImport.Click += (sender, args) =>
            {
                BeyondDynamoFunctions.ImportFromScript(VM);
            };

            EditNotes = new MenuItem { Header = "Edit Note Text" };
            EditNotes.Click += (sender, args) =>
            {
                //Check if we are in an active graph
                if (VM.Workspaces.Count < 1)
                {
                    Forms.MessageBox.Show("This command can only run in an active graph.\nPlease open a Dynamo Graph to use this function.");
                    return;
                }

                BeyondDynamoFunctions.CallTextEditor(VM.Model);
            };

            FreezeNodes = new MenuItem { Header = "Freeze Multiple Nodes" };
            FreezeNodes.Click += (sender, args) =>
            {
                BeyondDynamoFunctions.FreezeNodes(VM.Model);
            };

            UnfreezeNodes = new MenuItem { Header = "Unfreeze Multiple Nodes" };
            UnfreezeNodes.Click += (sender, args) =>
            {
                BeyondDynamoFunctions.UnfreezeNodes(VM.Model);
            };
            #endregion

            #region
            //App Extensions
            BDmenuItem.Items.Add(OrderPlayerInput);
            BDmenuItem.Items.Add(new Separator());
            BDmenuItem.Items.Add(new Separator());

            //Graph Extensions
            BDmenuItem.Items.Add(GroupColor);
            BDmenuItem.Items.Add(ScriptImport);
            BDmenuItem.Items.Add(EditNotes);
            BDmenuItem.Items.Add(FreezeNodes);
            BDmenuItem.Items.Add(UnfreezeNodes);

            //Main Extension
            BDmenuItem.Items.Add(new Separator());
            BDmenuItem.Items.Add(new Separator());
            BDmenuItem.Items.Add(AboutItem);
            p.dynamoMenu.Items.Add(BDmenuItem);
            #endregion
        }

    }
}
