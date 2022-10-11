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
using Dynamo.Controls;
using System.Windows.Media;
using System.Reflection;

namespace BeyondDynamo.Utils
{
    public static class BeyondDynamoUtils
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
                if (DynamoVM != null)
                {
                    DynamoVM.WriteToLogCmd.Execute(msg);
                }
            }
        }
        public static void OpenLog()
        {
            System.Diagnostics.Process.Start(filePath);
        }
        /// <summary>
        /// Sets the Current selection 
        /// </summary>
        /// <param name="model"></param>
        public static void KeepSelection(DynamoModel model)
        {
            foreach (dynamic item in model.CurrentWorkspace.CurrentSelection)
            {
                model.AddToSelection(item);
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
            else if (coreString.StartsWith("{"))
            {
                return "Json";
            }
            else
            {
                return "Unsupported";
            }
        }


        public static Dictionary<string, dynamic> GetAllSelectedUngroupedItems()
        {
            DynamoViewModel VM = BeyondDynamoUtils.DynamoVM;
            WorkspaceViewModel currentViewModel = VM.CurrentSpaceViewModel;
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            List<dynamic> selectedGroups = new List<dynamic>();
            List<ModelBase> ungroupedItems = new List<ModelBase>();
            List<NodeModel> ungroupedNodes = new List<NodeModel>();
            List<NoteModel> ungroupedNotes = new List<NoteModel>();
            List<Guid> GroupedItems = new List<Guid>();
            foreach (AnnotationViewModel group in currentViewModel.Annotations)
            {
                if (group.AnnotationModel.IsSelected)
                {
                    selectedGroups.Add(group.AnnotationModel);
                    foreach (ModelBase modelbase in group.Nodes)
                    {
                        GroupedItems.Add(modelbase.GUID);
                    }
                }
            }
            foreach (NodeViewModel node in currentViewModel.Nodes)
            {
                if (node.IsSelected && !GroupedItems.Contains(node.NodeModel.GUID))
                {
                    ungroupedItems.Add(node.NodeModel);
                    ungroupedNodes.Add(node.NodeModel);
                }
            }

            foreach (NoteViewModel note in currentViewModel.Notes)
            {
                if (note.IsSelected && !GroupedItems.Contains(note.Model.GUID))
                {
                    ungroupedItems.Add(note.Model);
                    ungroupedNotes.Add(note.Model);
                }
            }
            result.Add("SelectedGroups", selectedGroups);
            result.Add("UngroupedItems", ungroupedItems);
            result.Add("UngroupedNodes", ungroupedNodes);
            result.Add("UngroupedNotes", ungroupedNotes);

            return result;
        }

        public static List<AnnotationModel> GetAllSelectedGroups()
        {
            DynamoViewModel VM = BeyondDynamoUtils.DynamoVM;

            List<AnnotationModel> selectedGroups = new List<AnnotationModel>();
            Dictionary<string, dynamic> result = GetAllSelectedUngroupedItems();
            List<NodeModel> ungroupedNodes = (List<NodeModel>)result["UngroupedNodes"];
            List<NoteModel> ungroupedNotes = (List<NoteModel>)result["UngroupedNotes"];


            //if (ungroupedNodes.Count > 0 || ungroupedNotes.Count > 0)
            //{
            //    AnnotationModel newGroup = new AnnotationModel(ungroupedNodes, ungroupedNotes);
            //    VM.Model.ExecuteCommand(new DynamoModel.CreateAnnotationCommand(newGroup.GUID, newGroup.AnnotationText, newGroup.X, newGroup.Y, true));

            //    AnnotationViewModel newGroupViewModel = new AnnotationViewModel(VM.CurrentSpaceViewModel, newGroup);
            //    newGroupViewModel.AnnotationText = "<Click here to edit the group title>";
            //    VM.CurrentSpaceViewModel.Annotations.Add(newGroupViewModel);

            //    selectedGroups.Add(newGroup);
            //    foreach (NodeModel node in ungroupedNodes)
            //    {
            //        node.IsSelected = true;
            //        NodeViewModel nodeView = BeyondDynamoFunctions.GetNodeViewModel(node);
            //        if (VM.AddModelsToGroupModelCommand.CanExecute(node.GUID))
            //        {
            //            VM.AddModelsToGroupModelCommand.Execute(node.GUID);
            //        }
            //    }
            //}

            foreach (AnnotationViewModel viewGroup in VM.CurrentSpaceViewModel.Annotations)
            {
                if (viewGroup.AnnotationModel.IsSelected)
                {
                    selectedGroups.Add(viewGroup.AnnotationModel);
                }
            }
            return selectedGroups;

        }

        /// <summary>
        /// Checks if the selected file is already opened
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsFileOpen(DynamoViewModel viewModel, string filePath)
        {
            if (viewModel.Model.CurrentWorkspace.FileName == filePath)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Dictionary<string, NodeView> NodeViewDictionary()
        {
            Dictionary<string, NodeView> result = new Dictionary<string, NodeView>();   
            foreach (NodeView visualChild in FindVisualChildren<NodeView>(DynamoWindow))
            {
                result.Add(visualChild.ViewModel.NodeLogic.GUID.ToString(), visualChild);
            }
            return result;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                        yield return (T)child;
                    foreach (T visualChild in FindVisualChildren<T>(child))
                    {
                        T childOfChild = visualChild;
                        yield return childOfChild;
                        childOfChild = default(T);
                    }
                    child = (DependencyObject)null;
                }
            }
        }
        public static dynamic UsePrivateInternalMethod(Object instanceObject, string methodName, List<Object> parameters = null)
        {
            //Get the Internal Method from the ViewCropRegionManager Type Class using Reflection
            MethodInfo internalMethod = instanceObject.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (parameters == null)
            {
                parameters = new List<Object>();
            }

            dynamic result = internalMethod.Invoke(instanceObject, parameters.ToArray());
            return result;
        }
        public static dynamic GetPrivateInteralProperty(Object instanceObject, string propertyName)
        {
            PropertyInfo internalProperty = instanceObject.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            dynamic result = internalProperty.GetValue(instanceObject);
            return result;
        }
        public static dynamic GetPrivateInteralField(Object instanceObject, string propertyName)
        {
            FieldInfo internalField = instanceObject.GetType().GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            dynamic result = internalField.GetValue(instanceObject);
            return result;
        }
    }
}
