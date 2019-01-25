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
        /// Mark input nodes Menu Item
        /// </summary>
        private MenuItem MarkInputNodes;

        /// <summary>
        /// Unmark input nodes Menu Item
        /// </summary>
        private MenuItem UnmarkInputNodes;

        /// <summary>
        /// Mark output nodes Menu Item
        /// </summary>
        private MenuItem MarkOutputNodes;

        /// <summary>
        /// Unmark output nodes Menu Item
        /// </summary>
        private MenuItem UnmarkOutputNodes;

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
                return "Beyond Dynamo 2.0";
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

            #region THIS CAN BE RUN ANYTIME

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
            BDmenuItem.Items.Add(OrderPlayerInput);

            BDmenuItem.Items.Add(new Separator());
            BDmenuItem.Items.Add(new Separator());
            #endregion


            # region THESE FUNCTION ONLY WORK INSIDE A SCRIPT

            GroupColor = new MenuItem { Header = "Change Group Color" };
            GroupColor.Click += (sender, args) =>
            {
                BeyondDynamo.BeyondDynamoFunctions.ChangeGroupColor(VM.CurrentSpaceViewModel);
            };
            BDmenuItem.Items.Add(GroupColor);

            ScriptImport = new MenuItem { Header = "Import From Script" };
            ScriptImport.Click += (sender, args) =>
            {
                BeyondDynamoFunctions.ImportFromScript(VM);
            };
            BDmenuItem.Items.Add(ScriptImport);

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
            BDmenuItem.Items.Add(EditNotes);

            FreezeNodes = new MenuItem { Header = "Freeze Multiple Nodes" };
            FreezeNodes.Click += (sender, args) =>
            {
                BeyondDynamoFunctions.FreezeNodes(VM.Model);
            };
            BDmenuItem.Items.Add(FreezeNodes);

            UnfreezeNodes = new MenuItem { Header = "Unfreeze Multiple Nodes" };
            UnfreezeNodes.Click += (sender, args) =>
            {
                BeyondDynamoFunctions.UnfreezeNodes(VM.Model);
            };
            BDmenuItem.Items.Add(UnfreezeNodes);

            //MarkInputNodes = new MenuItem { Header = "Mark Multiple Nodes as Input " };
            //MarkInputNodes.Click += (sender, args) =>
            //{
            //    BeyondDynamo.BeyondDynamoFunctions.MarkAsInput(VM.Model.CurrentWorkspace);
            //};
            //BDmenuItem.Items.Add(MarkInputNodes);

            //UnmarkInputNodes = new MenuItem { Header = "Unmark Multiple Nodes as Input " };
            //UnmarkInputNodes.Click += (sender, args) =>
            //{
            //    BeyondDynamo.BeyondDynamoFunctions.UnMarkAsInput(VM.Model.CurrentWorkspace);
            //};
            //BDmenuItem.Items.Add(UnmarkInputNodes);

            //MarkOutputNodes = new MenuItem { Header = "Mark Multiple Nodes as Output" };
            //MarkOutputNodes.Click += (sender, args) =>
            //{
            //    BeyondDynamo.BeyondDynamoFunctions.MarkAsOutput(VM.Model.CurrentWorkspace);
            //};
            //BDmenuItem.Items.Add(MarkOutputNodes);

            //UnmarkOutputNodes = new MenuItem { Header = "Unmark Multiple Nodes as Output" };
            //UnmarkOutputNodes.Click += (sender, args) =>
            //{
            //    BeyondDynamo.BeyondDynamoFunctions.UnMarkAsOutput(VM.Model.CurrentWorkspace);
            //};
            //BDmenuItem.Items.Add(UnmarkOutputNodes);
            #endregion 
            
            AboutItem = new MenuItem { Header = "About Beyond Dynamo" };
            AboutItem.Click += (sender, args) =>
            {
                //Show the About dialog
                About about = new About();
                about.Show();
            };
            BDmenuItem.Items.Add(new Separator());
            BDmenuItem.Items.Add(new Separator());
            BDmenuItem.Items.Add(AboutItem);

            p.dynamoMenu.Items.Add(BDmenuItem);
            
        }

    }
}
