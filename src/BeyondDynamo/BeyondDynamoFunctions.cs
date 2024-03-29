﻿using Dynamo.Graph.Nodes;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Dynamo.ViewModels;
using Dynamo.Graph.Workspaces;
using Dynamo.Graph.Annotations;
using Dynamo.Graph.Notes;
using Dynamo.Graph.Connectors;
using System;
using Dynamo.Models;
using Newtonsoft.Json.Linq;
using Forms = System.Windows.Forms;
using Controls = System.Windows.Controls;
using Dynamo.UI.Commands;
using Dynamo.Engine;
using BeyondDynamo.UI;
using System.Windows.Input;
using Dynamo.Graph;
using Dynamo.Core;
using Dynamo.Extensions;
using Dynamo.UI.Controls;
using System.Reflection;
using BeyondDynamo.Utils;
using Dynamo.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace BeyondDynamo
{
    /// <summary>
    /// This Class Contains all the Functions used in the BeyondDynamoExtension Class
    /// </summary>
    public class BeyondDynamoFunctions
    {

        /// <summary>
        /// This function calls Both RemoveSessionTraceData and RemoveBindings, based on the Graph Version (1.3, 2.0)
        /// </summary>
        /// <param name="filePath"></param>
        public static void RemoveTraceData(string filePath)
        {
            string version = BeyondDynamoUtils.DynamoCoreLanguage(filePath);

            if (version == "XML")
            {
                RemoveSessionTraceData(filePath);
            }
            else if( version == "JSON")
            {
                RemoveBindings(filePath);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Removes Session Trace Data from a given Filepath. Only for Dynamo 1.3 scripts because of the XML Structure
        /// </summary>
        /// <param name="filePath">The Filepath to the Dynamo 1.3 File</param>
        /// <returns></returns>
        public static bool RemoveSessionTraceData(string filePath)
        {
            bool succes = false;
            string xmlPath = Path.ChangeExtension(filePath, ".xml");
            File.Move(filePath, xmlPath);
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);

            XmlNode sessionTraceDataNode = null;
            XmlNode parentNode = null;

            foreach (XmlNode xmlNode in doc.DocumentElement)
            {
                if (xmlNode.Name == "SessionTraceData")
                {
                    sessionTraceDataNode = xmlNode;
                    parentNode = sessionTraceDataNode.ParentNode;
                }
            }
            try
            {
                parentNode.RemoveChild(sessionTraceDataNode);
                doc.Save(xmlPath);
                succes = true;
            }
            catch
            {

            }
            finally
            {
                string newFilePath = Path.ChangeExtension(xmlPath, ".dyn");
                File.Move(xmlPath, newFilePath);
                File.Delete(xmlPath);
            }
            return succes;
        }

        /// <summary>
        /// Removes the Bindings from a given Filepath. This one is made for Dynamo 2.0
        /// </summary>
        /// <param name="DynamoFilepath"></param>
        /// <returns></returns>
        public static bool RemoveBindings(string DynamoFilepath)
        {
            bool succes = false;

            //Convert the Dynamo File to a Json Text
            string jsonString = File.ReadAllText(DynamoFilepath);

            //Create a JObject from the Json Text
            JObject dynamoGraph = JObject.Parse(jsonString);


            JToken bindings = dynamoGraph.SelectToken("Bindings");
            JContainer bindingsParent = null;
            foreach (JToken child in bindings.Children())
            {
                bindingsParent = child.Parent;
            }
            if (bindingsParent != null)
            {
                bindingsParent.RemoveAll();
                succes = true;
            }
            //Write that string 
            File.WriteAllText(DynamoFilepath, dynamoGraph.ToString());

            return succes;
        }

        /// <summary>
        /// Import a Dynamo script in the current graph
        /// </summary>
        /// <param name="viewModel"></param>
        public static void ImportFromScript(DynamoViewModel viewModel)
        {
            WorkspaceModel model = viewModel.Model.CurrentWorkspace;
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Filter = "Dynamo Files (*.dyn)|*.dyn";
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Get the selected filePath
                string DynamoFilepath = fileDialog.FileName;

                string version = BeyondDynamoUtils.DynamoCoreLanguage(DynamoFilepath);
                if (version == "XML")
                {
                    ImportXMLDynamo(viewModel, DynamoFilepath);
                }
                else if (version == "Json")
                {
                    ImportJsonDynamo(viewModel, DynamoFilepath);
                }
                else
                {
                    Forms.MessageBox.Show("The Selected File is not Supported", "Beyond Dynamo");
                    return;
                }

                //Zoom to the imported Script
                DelegateCommand fitView = viewModel.FitViewCommand;
                fitView.Execute(viewModel.CurrentSpaceViewModel);
            }
        }

        /// <summary>
        /// Imports a Dynamo 1.3 File into the current graph
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="DynamoFilePath"></param>
        private static void ImportXMLDynamo(DynamoViewModel viewModel, string DynamoFilePath)
        {
            WorkspaceViewModel workspaceViewModel = viewModel.CurrentSpaceViewModel;
            WorkspaceModel model = viewModel.Model.CurrentWorkspace;

            //Create two lists for the Selection of the imported model
            List<string> guidList = new List<string>();
            List<dynamic> selectionList = new List<dynamic>();

            //Load a XML Document from the Dynamo File\
            
            XmlDocument doc = new XmlDocument();
            doc.Load(DynamoFilePath);

            //Loop over the XML Elements in the Document
            foreach (XmlElement node in doc.DocumentElement)
            {
                try
                {
                    foreach (XmlElement element in node.ChildNodes)
                    {
                        model.CreateModel(element);
                        XmlAttributeCollection attributes = element.Attributes;
                        foreach (XmlAttribute attribute in attributes)
                        {
                            string attributeName = attribute.Name;
                            string attributeValue = attribute.Value;
                            if (attributeName == "guid")
                            {
                                guidList.Add(attributeValue);
                            }
                        }
                    }
                }
                catch
                {

                }
            }

            # region MAKE ALL IMPORTED ITEMS THE NEW SELECTION
            //Check Groups
            #region
            foreach (AnnotationViewModel groupView in workspaceViewModel.Annotations)
            {
                AnnotationModel group = groupView.AnnotationModel;
                string itemGuid = group.GUID.ToString();
                foreach (string guid in guidList)
                {
                    if (itemGuid == guid)
                    {
                        selectionList.Add(group);
                    }
                }
            }
            #endregion

            //Check Notes
            #region
            foreach (Dynamo.Graph.Notes.NoteModel note in model.Notes)
            {
                string itemGuid = note.GUID.ToString();
                foreach (string guid in guidList)
                {
                    if (itemGuid == guid)
                    {
                        selectionList.Add(note);
                    }
                }
            }
            #endregion

            //Check Nodes
            #region
            foreach (Dynamo.Graph.Nodes.NodeModel node in model.Nodes)
            {
                string itemGuid = node.GUID.ToString();
                foreach (string guid in guidList)
                {
                    if (itemGuid == guid)
                    {
                        selectionList.Add(node);
                    }
                }
            }
            #endregion

            foreach (dynamic item in selectionList)
            {
                viewModel.Model.AddToSelection(item);
            }
            #endregion
        }

        /// <summary>
        /// Imports a Dynamo 2.0 File into the current graph
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="DynamoFilepath"></param>
        private static void ImportJsonDynamo(DynamoViewModel viewModel, string DynamoFilepath)
        {
            WorkspaceModel model = viewModel.Model.CurrentWorkspace;

            //Create two lists for the Selection of the imported model
            List<string> guidList = new List<string>();
            List<dynamic> selectionList = new List<dynamic>();

            //Things we need to Build the Imported Model
            Dynamo.Engine.EngineController engineController = viewModel.EngineController;
            Dynamo.Engine.LibraryServices librearyServices = engineController.LibraryServices;
            Dynamo.Scheduler.DynamoScheduler scheduler = viewModel.Model.Scheduler;
            Dynamo.Graph.Nodes.NodeLoaders.NodeFactory nodeFactory = model.NodeFactory;
            Dynamo.Core.CustomNodeManager manager = viewModel.Model.CustomNodeManager;

            //Create an empty dummy XML Document
            XmlDocument doc = new XmlDocument();

            //Convert the Dynamo File to a Json Text
            string jsonString = File.ReadAllText(DynamoFilepath);

            //Create a Workspace model from the Json string
            WorkspaceModel importedModel = WorkspaceModel.FromJson(jsonString, librearyServices, engineController, scheduler, nodeFactory, false, true, manager);

            //Create extra WorkspaceViewInfo from the Json string
            ExtraWorkspaceViewInfo workspaceViewInfo = WorkspaceViewModel.ExtraWorkspaceViewInfoFromJson(jsonString);

            //Add the WorkspaceViewInfo to the Imported WorkspaceModel
            importedModel.UpdateWithExtraWorkspaceViewInfo(workspaceViewInfo);

            //Create a new WorkspaceViewModel from the Imported WorkspaceModel
            WorkspaceViewModel importedViewModel = new WorkspaceViewModel(importedModel, viewModel);

            #region LOOP OVER THE NODES, CONNECTORS, NOTES AND GROUPS & SERIALIZE THEM TO XML. THEN CREATE MODELS FROM THE XMLELEMENTS
            //Serialize all Components from the importedModel 
            Dynamo.Graph.SaveContext saveContext = Dynamo.Graph.SaveContext.None;

            //Loop over all the NodeModels from the importedModel
            List<string> errorNodeNames = new List<string>
            {
                "The following nodes are not supported and will not be imported:\n\n",
            };
            foreach (NodeModel node in importedModel.Nodes)
            {
                node.GUID = Guid.NewGuid();
                foreach (PortModel port in node.InPorts)
                {
                    port.GUID = Guid.NewGuid();
                }
                foreach (PortModel port in node.OutPorts)
                {
                    port.GUID = Guid.NewGuid();
                }
                guidList.Add(node.GUID.ToString());
                XmlElement nodeAsXML = node.Serialize(doc, saveContext);
                NodeViewModel nodeView = new NodeViewModel(viewModel.CurrentSpaceViewModel, node);
                try
                {
                    model.CreateModel(nodeAsXML);
                }
                catch
                {
                    errorNodeNames.Add(node.Name);
                    errorNodeNames.Add("\n");
                }
            }
            if (errorNodeNames.Count > 1)
            {
                string message = string.Concat(errorNodeNames);
                Forms.MessageBox.Show(text: message, caption: "Import Warning", icon: MessageBoxIcon.Warning, buttons: MessageBoxButtons.OK);
            }

            //Loop over all ConnectorModels from the importedModel
            foreach (ConnectorModel connector in importedModel.Connectors)
            {
                try
                {
                    XmlElement connectorAsXML = connector.Serialize(doc, saveContext);
                    model.CreateModel(connectorAsXML);
                }
                catch
                {

                }
            }

            //Loop over all Notes from the imported Script
            foreach (NoteModel note in importedModel.Notes)
            {
                note.GUID = Guid.NewGuid();
                guidList.Add(note.GUID.ToString());
                XmlElement noteAsXML = note.Serialize(doc, saveContext);
                model.CreateModel(noteAsXML);
            }

            //Loop over all AnnotationViewModels from the importedViewModel
            foreach (AnnotationViewModel annotationView in importedViewModel.Annotations)
            {
                AnnotationModel annotation = annotationView.AnnotationModel;
                annotation.GUID = Guid.NewGuid();
                guidList.Add(annotation.GUID.ToString());
                XmlElement annotationAsXML = annotation.Serialize(doc, saveContext);
                model.CreateModel(annotationAsXML);
            }
            #endregion

            #region MAKE ALL IMPORTED ITEMS THE NEW SELECTION
            //Check Groups
            #region
            foreach (AnnotationViewModel groupView in viewModel.CurrentSpaceViewModel.Annotations)
            {
                AnnotationModel group = groupView.AnnotationModel;
                string itemGuid = group.GUID.ToString();
                foreach (string guid in guidList)
                {
                    if (itemGuid == guid)
                    {
                        selectionList.Add(group);
                    }
                }
            }
            #endregion

            //Check Notes
            #region
            foreach (Dynamo.Graph.Notes.NoteModel note in model.Notes)
            {
                string itemGuid = note.GUID.ToString();
                foreach (string guid in guidList)
                {
                    if (itemGuid == guid)
                    {
                        selectionList.Add(note);
                    }
                }
            }
            #endregion

            //Check Nodes
            #region
            foreach (Dynamo.Graph.Nodes.NodeModel node in model.Nodes)
            {
                string itemGuid = node.GUID.ToString();
                foreach (string guid in guidList)
                {
                    if (itemGuid == guid)
                    {
                        selectionList.Add(node);
                    }
                }
            }
            #endregion

            foreach (dynamic item in selectionList)
            {
                viewModel.Model.AddToSelection(item);
            }
            #endregion
        }

        /// <summary>
        /// Changes the Colors of the Selected Groups in the Workspace Model by using a Color Picker UI
        /// </summary>
        /// <param name="model">The Current Dynamo Model</param>
        /// <param name="config">The Beyond Dynamo Settings</param>
        public static BeyondDynamoConfig ChangeGroupColor(List<AnnotationModel> selectedGroups, string color=null)
        {
            BeyondDynamoConfig config = BeyondDynamoConfig.Current;
            if (selectedGroups.Count > 0)
            {
                if (color == null)
                {
                    System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
                    if (config.customColors != null)
                    {
                        colorDialog.CustomColors = config.customColors;
                    }
                    if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        foreach (AnnotationModel group in selectedGroups)
                        {
                            string colorString = System.Drawing.ColorTranslator.ToHtml(colorDialog.Color);
                            group.Background = colorString;
                        }
                        config.customColors = colorDialog.CustomColors;
                    }
                }
                else
                {
                    foreach(AnnotationModel group in selectedGroups)
                    {
                        group.Background = color;
                    }
                }
                
            }
            else
            {
                System.Windows.MessageBox.Show("No Groups selected", "Beyond Dynamo");
            }
            return config;
        }


        /// <summary>
        /// Opens a Text editor window for each selected Note
        /// </summary>
        /// <param name="model"></param>
        public static void CallTextEditor(DynamoModel model)
        {
            WorkspaceModel workspaceModel = model.CurrentWorkspace;
            
            //Check if there are any Notes selected
            List<Dynamo.Graph.Notes.NoteModel> selectedNotes = new List<Dynamo.Graph.Notes.NoteModel>();
            foreach (Dynamo.Graph.Notes.NoteModel note in workspaceModel.Notes)
            {
                if (note.IsSelected)
                {
                    selectedNotes.Add(note);
                }
            }

            if (selectedNotes.Count == 0)
            {
                System.Windows.MessageBox.Show("No Notes Selected", "Beyond Dynamo");
                BeyondDynamoUtils.KeepSelection(model);
                return;
            }

            IEnumerable<Dynamo.Graph.Notes.NoteModel> notes = workspaceModel.Notes;
            foreach (Dynamo.Graph.Notes.NoteModel note in notes)
            {
                if (note.IsSelected)
                {
                    NoteTextBoxWindow textBox = new NoteTextBoxWindow(note);
                    textBox.Show();
                }
            }
            BeyondDynamoUtils.KeepSelection(model);
        }

        /// <summary>
        /// Opens a Text editor window for each selected Note
        /// </summary>
        /// <param name="model"></param>
        public static List<NodeModel> GetSelectedNodes(WorkspaceModel workspaceModel)
        {

            //Check if there are any Notes selected
            List<NodeModel> selectedNodes = new List<NodeModel>();
            foreach (NodeModel node in workspaceModel.Nodes)
            {
                if (node.IsSelected)
                {
                    selectedNodes.Add(node);
                }
            }

            if (selectedNodes.Count == 0)
            {
                System.Windows.MessageBox.Show("No Nodes Selected", "Beyond Dynamo");
                return null;
            }
            else
            {
                return selectedNodes;
            }
        }
        /// <summary>
        /// Opens a Text editor window for each selected Note
        /// </summary>
        /// <param name="model"></param>
        public static List<NodeViewModel> GetSelectedNodeViewModels(WorkspaceViewModel workspaceModel)
        {

            //Check if there are any Notes selected
            List<NodeViewModel> selectedNodes = new List<NodeViewModel>();
            foreach (NodeViewModel node in workspaceModel.Nodes)
            {
                if (node.IsSelected)
                {
                    selectedNodes.Add(node);
                }
            }

            if (selectedNodes.Count == 0)
            {
                System.Windows.MessageBox.Show("No Nodes Selected", "Beyond Dynamo");
                return null;
            }
            else
            {
                return selectedNodes;
            }
        }

        /// <summary>
        /// Sorts Input and Output nodes from a selected Dynamo File
        /// </summary>
        /// <param name="DynamoFilepath"></param>
        public static void SortInputOutputNodesJson(string DynamoFilepath)
        {
            //Create a new Empty List for the Input Node Names
            Dictionary<string, string> inputNodes = new Dictionary<string, string>();

            //Create a new Empty List for the Output Node Names
            Dictionary<string, string> outputNodes = new Dictionary<string, string>();

            //Convert the Dynamo File to a Json Text
            string jsonString = File.ReadAllText(DynamoFilepath);

            //Create a JObject from the Json Text
            JObject dynamoGraph = JObject.Parse(jsonString);

            foreach (JToken child in dynamoGraph.SelectToken("View").SelectToken("NodeViews").Children())
            {
                if (child.Value<bool>("IsSetAsInput"))
                {
                    string value = child.Value<string>("Name");
                    string key = child.Value<string>("Id");
                    inputNodes.Add(key, value);
                }

                else if (child.Value<bool>("IsSetAsOutput"))
                {
                    string value = child.Value<string>("Name");
                    string key = child.Value<string>("Id");
                    outputNodes.Add(key, value);
                }
            }
            
            //Create a new Player Order Window
            OrderPlayerInputWindow orderPlayerInput = new OrderPlayerInputWindow(inputNodes, outputNodes);

            //Set the Properties of the Class and Show the Window
            orderPlayerInput.dynamoJsonGraph = dynamoGraph;
            orderPlayerInput.dynamoGraphPath = DynamoFilepath;
            orderPlayerInput.dynamoVersion = "Json";
            orderPlayerInput.Show();
        }

        /// <summary>
        /// Lets you Sort the Input Nodes for a Dynamo Script by a given Filepath 
        /// </summary>
        /// <param name="DynamoFilepath">The Filepath to the Dynamo 1.3 File</param>
        public static void SortInputOutputNodesXML(string DynamoFilepath)
        {
            Dictionary<string, string> inputNodes = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.Load(DynamoFilepath);
            foreach (XmlElement child in doc.DocumentElement)
            {
                if (child.Name == "Elements")
                {
                    foreach (XmlElement Node in child.ChildNodes)
                    {
                        XmlAttributeCollection attributes = Node.Attributes;
                        try
                        {
                            if (attributes["isSelectedInput"].Value == "True")
                            {
                                string key = attributes["nickname"].Value;
                                string value = attributes["guid"].Value;
                                inputNodes.Add(key, value);
                            }
                        }
                        catch { }
                    }
                }
            }

            Dictionary<string, string> outputNodes = new Dictionary<string, string>();
            OrderPlayerInputWindow orderPlayerInput = new OrderPlayerInputWindow(inputNodes, outputNodes);
            if (inputNodes.Count <= 0)
            {
                System.Windows.MessageBox.Show("No input nodes found");
                return;
            }

            orderPlayerInput.dynamoXMLGraph = doc;
            orderPlayerInput.dynamoVersion = "XML";
            orderPlayerInput.dynamoGraphPath = DynamoFilepath;
            orderPlayerInput.Show();
        }

        public static void ChangePreviewStateOfSelectedNodes(bool hide)
        {
            WorkspaceModel currentWorkspace = BeyondDynamoUtils.DynamoVM.CurrentSpace;
            foreach(NodeModel node in currentWorkspace.Nodes)
            {
                if (node.IsSelected)
                {
                    if (hide)
                    {
                        AutoNodePreview(node);
                    }
                    else
                    {
                        AutoNodePreview(node, hide);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves all player scripts from a Given folder and creates menuItems for them.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="model"></param>
        /// <param name="filePath"></param>
        /// <param name="extraItems"></param>
        public static void RetrievePlayerFiles(Controls.MenuItem owner, DynamoViewModel viewModel, string filePath, List<Controls.MenuItem> extraItems)
        {
            DynamoModel model = viewModel.Model;

            // Check if the Filepath is not Empty
            if (filePath != "")
            {
                // Get all the Files from the given Directory Path, which comes out of the Configuration file
                string[] filePaths = Directory.GetFiles(filePath);
                foreach (string path in filePaths)
                {
                    //Check if the File is a Dynamo File
                    if (Path.GetExtension(path) == ".dyn")
                    {
                        // Get the File Name
                        string fileName = Path.GetFileNameWithoutExtension(path);

                        //Make a Menu Item for each File
                        Controls.MenuItem item = new Controls.MenuItem { Header = fileName };
                        item.ToolTip = new Controls.ToolTip { Content = path };

                        //Create a clicking Event
                        item.Click += (sender, args) =>
                        {
                            //Things we need to Build the Imported Model
                            EngineController engineController = viewModel.EngineController;

                            //Check if the Current Model need Saving
                            if (model.CurrentWorkspace.HasUnsavedChanges)
                            {
                                //Show Prompt Window for Saving
                                Forms.DialogResult result = Forms.MessageBox.Show("Do you want to Save the current Workspace?", "Save", Forms.MessageBoxButtons.YesNoCancel);
                                if (result == Forms.DialogResult.Yes)
                                {
                                    //Check if the model already has a Filepath
                                    if (model.CurrentWorkspace.FileName == "")
                                    {
                                        //If there is no Filepath, show a Save as dialog
                                        Forms.SaveFileDialog dialog = new Forms.SaveFileDialog();
                                        dialog.FileName = "Home";
                                        dialog.AddExtension = true;
                                        dialog.DefaultExt = "dyn";
                                        dialog.Filter = "Dynamo Files (*.dyn)|*.dyn";
                                        if (Forms.DialogResult.OK == dialog.ShowDialog())
                                        {
                                            viewModel.SaveAsCommand.Execute(dialog.FileName);
                                        }
                                    }
                                    //If there is a Filepath, Save the File
                                    else
                                    {
                                        //Save the File
                                        viewModel.SaveCommand.Execute(viewModel);
                                    }
                                }
                                else if (result == Forms.DialogResult.Cancel)
                                {
                                    return;
                                }
                            }

                            //If there are No unsaved changes, Open the File
                            model.OpenFileFromPath(path, true);
                            model.CurrentWorkspace.HasUnsavedChanges = true;

                            //Convert the Dynamo File to a Json Text
                            string jsonString = File.ReadAllText(path);

                            //Create extra WorkspaceViewInfo from the Json string
                            ExtraWorkspaceViewInfo workspaceViewInfo = WorkspaceViewModel.ExtraWorkspaceViewInfoFromJson(jsonString);

                            model.CurrentWorkspace.UpdateWithExtraWorkspaceViewInfo(workspaceViewInfo);
                        };

                        //Add the Item to the Player scripts MenuItem
                        owner.Items.Add(item);
                    }
                }

                // Add the Extra Menu items
                owner.Items.Add(new Controls.Separator());
                foreach (Controls.MenuItem item in extraItems)
                {
                    owner.Items.Add(item);
                }
            }
            // If the Filepath is empty, Only add the "Set Player Path" Menu Item 
            else
            {
                owner.Items.Add(extraItems[0]);
            }
        }

        public static void RenamePythonListInputs()
        {
            DynamoViewModel VM = BeyondDynamoUtils.DynamoVM;
            //Check if we are in an active graph
            if (VM.Workspaces.Count < 1)
            {
                Forms.MessageBox.Show("This command can only run in an active graph.\nPlease open a Dynamo Graph to use this function.", "Beyond Dynamo");
                return;
            }
            WorkspaceViewModel currentWorkspaceView = VM.CurrentSpaceViewModel;

            WorkspaceModel currentWorkspace = currentWorkspaceView.Model;
            int selected = 0;
            foreach (NodeViewModel nodeView in currentWorkspaceView.Nodes)
            {
                if (nodeView.NodeModel.IsSelected)
                {
                    string type = nodeView.NodeModel.GetType().Name;

                    if(type == "PythonNode" | type == "PythonStringNode")
                    {
                        InputsWindow inputsWindow = new InputsWindow(nodeView, true);
                        inputsWindow.Show();
                        selected += 1;
                    }
                    else if (type == "CreateList")
                    {
                        InputsWindow inputsWindow = new InputsWindow(nodeView, false);
                        inputsWindow.Show();
                        selected += 1;
                    }
                }
            }
            if (selected == 0)
            {
                MessageBox.Show("No Valid Nodes Selected\nSelect a Python or List Create node to use this function");
            }
        }

        //public static void AutoRenamePythonListInputs()
        //{
        //    DynamoViewModel VM = BeyondDynamo.BeyondDynamoUtils.DynamoVM;
        //    //Check if we are in an active graph
        //    if (VM.Workspaces.Count < 1)
        //    {
        //        Forms.MessageBox.Show("This command can only run in an active graph.\nPlease open a Dynamo Graph to use this function.", "Beyond Dynamo");
        //        return;
        //    }
        //    WorkspaceViewModel currentWorkspaceView = VM.CurrentSpaceViewModel;

        //    WorkspaceModel currentWorkspace = currentWorkspaceView.Model;
        //    int selected = 0;
        //    foreach (NodeViewModel nodeView in currentWorkspaceView.Nodes)
        //    {
        //        if (nodeView.NodeModel.IsSelected)
        //        {
        //            string type = nodeView.NodeModel.GetType().Name;

        //            if (type == "PythonNode" | type == "PythonStringNode"| type == "CreateList")
        //            {
        //                selected += 1;
        //            }
        //        }
        //    }
        //    if (selected == 0)
        //    {
        //        Forms.MessageBox.Show("This command can only run in an active graph.\nPlease open a Dynamo Graph to use this function.", "Beyond Dynamo");
        //    }
        //}

        public static void AutoNodePreview(NodeModel node, bool hide = true)
        {
            NodeViewModel nodeView = GetNodeViewModel(node);
            Forms.MessageBox.Show(node.ShouldDisplayPreview.ToString());
            if (nodeView.ShowExecutionPreview && hide)
            {
                nodeView.ToggleIsVisibleCommand.Execute(node);
            }
            else if(!nodeView.ShowExecutionPreview && !hide)
            {
                nodeView.ToggleIsVisibleCommand.Execute(node);
            }
            BeyondDynamoUtils.LogMessage("Turning Node preview off for " + node.Name);
        }
        public static void AutoNodePreviewOff(NodeModel node)
        {
            NodeViewModel nodeView = GetNodeViewModel(node);
            nodeView.ToggleIsVisibleCommand.Execute(node);
            BeyondDynamoUtils.LogMessage("Turning Node preview off for " + node.Name);
        }
        public static NodeViewModel GetNodeViewModel(NodeModel obj)
        {
            NodeViewModel nodeViewModel = null;
            foreach(NodeViewModel nodeView in BeyondDynamoUtils.DynamoVM.CurrentSpaceViewModel.Nodes)
            {
                if(nodeView.NodeModel.GUID == obj.GUID)
                {
                    nodeViewModel = nodeView;
                    break;
                }
            }
            return nodeViewModel;
        }

        public static void ShowNodeList(ObservableCollection<NodeViewModel> nodeViewModels)
        {

            Dictionary<string, NodeView> nodeViews = BeyondDynamoUtils.NodeViewDictionary();
            foreach (NodeViewModel nodeViewModel in nodeViewModels)
            {
                //nodeViewModel.DynamoViewModel.ShowPreviewBubbles = false;
                nodeViewModel.PreviewPinned = !nodeViewModel.PreviewPinned;
                NodeView nodeView = nodeViews[nodeViewModel.NodeLogic.GUID.ToString()];
                
                PreviewControl previewControl = BeyondDynamoUtils.GetPrivateInteralField(nodeView, "previewControl");
                previewControl.TransitionToState(PreviewControl.State.Hidden);
                previewControl.BeginInit();
                previewControl.EndInit();

            }
        }
    }

    public class FreezeNodesCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            FreezeNodes();
        }

        /// <summary>
        /// Freezes a selection of nodes
        /// </summary>
        /// <param name="model"></param>
        public static void FreezeNodes()
        {
            DynamoModel model = BeyondDynamoUtils.DynamoVM.Model;
            WorkspaceModel workspace = model.CurrentWorkspace;
            List<dynamic> result = ShouldFreeze(workspace.Nodes);
            bool shouldFreeze = result[0];
            List<NodeModel> nodes = result[1];
            foreach (Dynamo.Graph.Nodes.NodeModel node in nodes)
            {
                if (shouldFreeze)
                {
                    node.IsFrozen = true;
                }
                else
                {
                    node.IsFrozen = false;
                }
            }
            BeyondDynamoUtils.KeepSelection(model);
        }

        public static List<dynamic> ShouldFreeze(IEnumerable<NodeModel> nodes)
        {
            List<dynamic> result = new List<dynamic>();
            List<NodeModel> frozenNodes = new List<NodeModel>();
            List<NodeModel> unfrozenNodes = new List<NodeModel>();
            foreach (NodeModel node in nodes)
            {
                if (node.IsSelected)
                {
                    if (node.IsFrozen)
                    {
                        frozenNodes.Add(node);
                    }
                    else
                    {
                        unfrozenNodes.Add(node);
                    }
                }
            }
            if (frozenNodes.Count == 0)
            {
                result = new List<dynamic>()
                {
                    true,
                    unfrozenNodes
                };
            }
            else if (unfrozenNodes.Count == 0)
            {
                result = new List<dynamic>()
                {
                    false,
                    frozenNodes
                };
            }
            else if (frozenNodes.Count > unfrozenNodes.Count)
            {
                result = new List<dynamic>()
                {
                    true,
                    unfrozenNodes
                };
            }
            else
            {
                result = new List<dynamic>()
                {
                    false,
                    frozenNodes
                };
            }
            return result;
        }
    }
    public class PreviewNodesCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            PreviewNodes();
        }

        /// <summary>
        /// Freezes a selection of nodes
        /// </summary>
        /// <param name="model"></param>
        public static void PreviewNodes()
        {
            DynamoModel model = BeyondDynamoUtils.DynamoVM.Model;
            WorkspaceModel workspace = model.CurrentWorkspace;
            List<NodeModel> nodes = ShouldPreview(workspace.Nodes);
            foreach (Dynamo.Graph.Nodes.NodeModel node in nodes)
            {
                NodeViewModel nodeView = BeyondDynamoFunctions.GetNodeViewModel(node);
                nodeView.ToggleIsVisibleCommand.Execute(node);
                BeyondDynamoUtils.LogMessage("Changed Node preview for " + node.Name);
            }
            BeyondDynamoUtils.KeepSelection(model);
        }

        public static List<NodeModel> ShouldPreview(IEnumerable<NodeModel> nodes)
        {
            List<NodeModel> result = new List<NodeModel>();
            List<NodeModel> shownNodes = new List<NodeModel>();
            List<NodeModel> hiddenNodes = new List<NodeModel>();
            foreach (NodeModel node in nodes)
            {
                if (node.IsSelected)
                {
                    if (node.IsVisible)
                    {
                        shownNodes.Add(node);
                    }
                    else
                    {
                        hiddenNodes.Add(node);
                    }
                }
            }
            if (shownNodes.Count == 0)
            {
                result = hiddenNodes;
            }
            else if (hiddenNodes.Count == 0)
            {
                result = shownNodes;
            }
            else if (shownNodes.Count > hiddenNodes.Count)
            {
                result = hiddenNodes;
            }
            else
            {
                result = shownNodes;
            }
            return result;
        }
    }
    public class ShowNodeLabelsCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            ShowNodeLabels();
        }

        /// <summary>
        /// Freezes a selection of nodes
        /// </summary>
        /// <param name="model"></param>
        public static void ShowNodeLabels()
        {
            DynamoModel model = BeyondDynamoUtils.DynamoVM.Model;
            WorkspaceModel workspace = model.CurrentWorkspace;
            List<NodeModel> nodes = ShouldShowLabels(workspace.Nodes, out bool shouldShowLabels);
            foreach (Dynamo.Graph.Nodes.NodeModel node in nodes)
            {
                NodeViewModel nodeView = BeyondDynamoFunctions.GetNodeViewModel(node);
                if (shouldShowLabels)
                {
                    node.DisplayLabels = false ;
                }
                else
                {
                    node.DisplayLabels = true;
                }
                BeyondDynamoUtils.LogMessage("Changed Node preview for " + node.Name);
            }
            BeyondDynamoUtils.KeepSelection(model);
        }

        public static List<NodeModel> ShouldShowLabels(IEnumerable<NodeModel> nodes, out bool shouldShowLabels)
        {
            List<NodeModel> result = new List<NodeModel>();
            List<NodeModel> shownNodes = new List<NodeModel>();
            List<NodeModel> hiddenNodes = new List<NodeModel>();
            foreach (NodeModel node in nodes)
            {
                if (node.IsSelected)
                {
                    if (node.DisplayLabels)
                    {
                        shownNodes.Add(node);
                    }
                    else
                    {
                        hiddenNodes.Add(node);
                    }
                }
            }
            if (shownNodes.Count == 0)
            {
                result = hiddenNodes;
                shouldShowLabels = false;
            }
            else if (hiddenNodes.Count == 0)
            {
                result = shownNodes;
                shouldShowLabels = true;
            }
            else if (shownNodes.Count > hiddenNodes.Count)
            {
                result = hiddenNodes;
                shouldShowLabels = false;
            }
            else
            {
                result = shownNodes;
                shouldShowLabels = true;
            }
            return result;
        }
    }

}
