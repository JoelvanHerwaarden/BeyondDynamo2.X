﻿using System.Windows;
using System.Windows.Navigation;
using Dynamo.Extensions;
using Dynamo.Graph.Nodes;
using Dynamo.Wpf.Extensions;
using System.Diagnostics;
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
using Newtonsoft.Json;

namespace BeyondDynamo
{
    /// <summary>
    /// This Class Contains all the Functions used in the BeyondDynamoExtension Class
    /// </summary>
    public class BeyondDynamoFunctions
    {
        /// <summary>
        /// Sets the Current selection 
        /// </summary>
        /// <param name="model"></param>
        public static void KeepSelection(DynamoModel model)
        {
            foreach(dynamic item in model.CurrentWorkspace.CurrentSelection)
            {
                model.AddToSelection(item);
            }
        }

        /// <summary>
        /// Checks if the selected file is already opened
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsFileOpen(DynamoViewModel viewModel, string filePath)
        {
            if(viewModel.Model.CurrentWorkspace.FileName == filePath)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks in which Language the Core String of Dynamo Script is
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string DynamoCoreLanguage(string filePath)
        {
            string coreString = File.ReadAllText(filePath);
            if (coreString.StartsWith("<"))
            {
                return "XML";
            }
            else
            {
                return "Json";
            }
        }

        public static void RemoveTraceData(string filePath)
        {
            string version = DynamoCoreLanguage(filePath);

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

            //Loop over the Output Nodes and get their names
            JToken bindings = dynamoGraph.SelectToken("Bindings");
            JContainer bindingsParent = null;
            foreach(JToken child in bindings.Children())
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

                string version = DynamoCoreLanguage(DynamoFilepath);
                if (version == "XML")
                {
                    ImportXMLDynamo(viewModel, DynamoFilepath);
                }
                else if (version == "JSON")
                {
                    ImportJsonDynamo(viewModel, DynamoFilepath);
                }
                else
                {
                    return;
                }
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
            foreach (NodeModel node in importedModel.Nodes)
            {
                guidList.Add(node.GUID.ToString());
                XmlElement nodeAsXML = node.Serialize(doc, saveContext);
                NodeViewModel nodeView = new NodeViewModel(viewModel.CurrentSpaceViewModel, node);

                model.CreateModel(nodeAsXML);
            }

            //Loop over all ConnectorModels from the importedModel
            foreach (ConnectorModel connector in importedModel.Connectors)
            {
                XmlElement connectorAsXML = connector.Serialize(doc, saveContext);
                model.CreateModel(connectorAsXML);
            }

            //Loop over all Notes from the imported Script
            foreach (NoteModel note in importedModel.Notes)
            {
                guidList.Add(note.GUID.ToString());
                XmlElement noteAsXML = note.Serialize(doc, saveContext);
                model.CreateModel(noteAsXML);
            }

            //Loop over all AnnotationViewModels from the importedViewModel
            foreach (AnnotationViewModel annotationView in importedViewModel.Annotations)
            {
                AnnotationModel annotation = annotationView.AnnotationModel;
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
        public static BeyondDynamoConfig ChangeGroupColor(WorkspaceViewModel model, BeyondDynamoConfig config)
        {
            List<AnnotationModel> selectedGroups = new List<AnnotationModel>();
            foreach (AnnotationViewModel groupViewModel in model.Annotations)
            {
                AnnotationModel group = groupViewModel.AnnotationModel;
                if (group.IsSelected)
                {
                    selectedGroups.Add(group);
                }
            }
            if (selectedGroups.Count > 0)
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
                System.Windows.MessageBox.Show("No Groups selected");
            }

            return config;
        }

        /// <summary>
        /// Freezes a selection of nodes
        /// </summary>
        /// <param name="model"></param>
        public static void FreezeNodes(DynamoModel model)
        {
            WorkspaceModel workspace = model.CurrentWorkspace;
            foreach (Dynamo.Graph.Nodes.NodeModel node in workspace.Nodes)
            {
                if (node.IsSelected)
                {
                    if (node.IsFrozen == false)
                    {
                        node.IsFrozen = true;
                    }
                }
            }
            KeepSelection(model);
        }

        /// <summary>
        /// Unreezes a selection of nodes        
        /// </summary>
        /// <param name="model"></param>
        public static void UnfreezeNodes(DynamoModel model)
        {
            WorkspaceModel workspace = model.CurrentWorkspace;
            foreach (Dynamo.Graph.Nodes.NodeModel node in workspace.Nodes)
            {
                if (node.IsSelected)
                {
                    if (node.IsFrozen)
                    {
                        node.IsFrozen = false;
                    }
                }
            }
            KeepSelection(model);


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
                System.Windows.MessageBox.Show("No Notes Selected");
                KeepSelection(model);
                return;
            }

            IEnumerable<Dynamo.Graph.Notes.NoteModel> notes = workspaceModel.Notes;
            foreach (Dynamo.Graph.Notes.NoteModel note in notes)
            {
                if (note.IsSelected)
                {
                    TextBoxWindow textBox = new TextBoxWindow(note.Text);
                    textBox.Show();
                    textBox.Closed += (send, arg) =>
                    {
                        note.Text = textBox.text;
                    };
                }
            }
            KeepSelection(model);
        }

        /// <summary>
        /// Sorts Input and Output nodes from a selected Dynamo File
        /// </summary>
        /// <param name="DynamoFilepath"></param>
        public static void SortInputOutputNodesJson(string DynamoFilepath)
        {
            //Create a new Empty List for the Input Node Names
            List<string> inputNodeNames = new List<string>();

            //Create a new Empty List for the Input Node Names
            List<string> outputNodeNames = new List<string>();

            //Convert the Dynamo File to a Json Text
            string jsonString = File.ReadAllText(DynamoFilepath);

            //Create a JObject from the Json Text
            JObject dynamoGraph = JObject.Parse(jsonString);

            //Loop over the Input Nodes and get their names
            foreach (JToken child in dynamoGraph.SelectToken("Inputs").Children())
            {
                string name = child.Value<string>("Name");
                inputNodeNames.Add(name);
            }

            //Loop over the Output Nodes and get their names
            foreach (JToken child in dynamoGraph.SelectToken("Outputs").Children())
            {
                string name = child.Value<string>("Name");
                outputNodeNames.Add(name);
            }

            //Create a new Player Order Window
            OrderPlayerInputWindow orderPlayerInput = new OrderPlayerInputWindow(inputNodeNames, outputNodeNames);

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
            List<string> inputNodeNames = new List<string>();
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
                                inputNodeNames.Add(attributes["nickname"].Value);
                            }
                        }
                        catch { }
                    }
                }
            }

            List<string> outputNodeNames = new List<string>();
            OrderPlayerInputWindow orderPlayerInput = new OrderPlayerInputWindow(inputNodeNames, outputNodeNames);
            if (inputNodeNames.Count <= 0)
            {
                System.Windows.MessageBox.Show("No input nodes found");
                return;
            }

            orderPlayerInput.dynamoXMLGraph = doc;
            orderPlayerInput.dynamoVersion = "XML";
            orderPlayerInput.dynamoGraphPath = DynamoFilepath;
            orderPlayerInput.Show();
        }

        /// <summary>
        /// Mark all selected nodes as input nodes
        /// </summary>
        /// <param name="model"></param>
        public static void MarkAsInput(WorkspaceModel model)
        {
            foreach (NodeModel node in model.CurrentSelection)
            {
                node.IsSetAsInput = true;
            }
        }

        /// <summary>
        /// Mark all selected nodes as output nodes
        /// </summary>
        /// <param name="model"></param>
        public static void MarkAsOutput(WorkspaceModel model)
        {
            foreach (NodeModel node in model.CurrentSelection)
            {
                node.IsSetAsOutput = true;
            }
        }

        /// <summary>
        /// Unmark all selected nodes as input nodes
        /// </summary>
        /// <param name="model"></param>
        public static void UnMarkAsInput(WorkspaceModel model)
        {
            foreach (NodeModel node in model.CurrentSelection)
            {
               node.IsSetAsInput = false;
            }
        }
        
        /// <summary>
        /// Unmark all selected nodes as output nodes
        /// </summary>
        /// <param name="model"></param>
        public static void UnMarkAsOutput(WorkspaceModel model)
        {
            foreach (NodeModel node in model.CurrentSelection)
            {
                node.Description = "This is Description";
                node.ToolTipText = "This is the ToolTip";
                
            }
        }
    }
}
