using Dynamo.Graph.Nodes;
using Dynamo.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Controls;
using Dynamo.Wpf.ViewModels;
using System.Windows.Media;
using System.Windows.Input;
using Dynamo.Models;
using System.IO;
using Dynamo.Graph;
using Dynamo.Graph.Workspaces;
using Dynamo.UI.Commands;
using Dynamo.Graph.Annotations;

namespace BeyondDynamo.UI
{
    /// <summary>
    /// Interaction logic for TextBoxWindow.xaml
    /// </summary>
    public partial class ToolSpaceControl : ContentControl
    {

        private List<NodeModel> completedNodes = new List<NodeModel>();
        private List<NodeModel> warningNodes = new List<NodeModel>();
        private List<NodeModel> errorNodes = new List<NodeModel>();


        //These are all the nodes in the dynamo model
        private List<NodeModel> nodes { get; set; }

        //These are all the Node names
        private List<SearchNodeLabel> nodeLabels { get; set; }

        //These are the nodes which match the search term
        private List<NodeModel> foundNodes { get; set; }

        public ToolSpaceControl()
        {
            InitializeComponent();
            DynamoViewModel viewmodel = BeyondDynamo.Utils.DynamoVM;
            //viewmodel.Model.RefreshCompleted += EvaluateGraph;

            //Get the nodes in the current workspace
            nodes = (List<NodeModel>)viewmodel.CurrentSpace.Nodes;

            //Create new lists for the names and the nodes
            this.nodeLabels = new List<SearchNodeLabel>();
            this.foundNodes = new List<NodeModel>();

            //Start looping over the nodes and get the name and node. Add them to the right lists.
            foreach (NodeModel node in nodes)
            {
                node.PropertyChanged += (sender, args) => { this.Refresh(); };
                SearchNodeLabel label = new SearchNodeLabel(node);
                this.foundNodes.Add(node);
                nodeLabels.Add(label);
            }

            //Add all the names of the nodes to the nodeStacker control 
            foreach (SearchNodeLabel label in nodeLabels)
            {
                this.nodeStacker.Children.Add(label);
            }

            //Register events for adding and removing nodes to the current workspace
            viewmodel.CurrentSpace.NodeAdded += (obj) =>
            {
                NodeModel node = (NodeModel)obj;
                node.PropertyChanged += (sender, args) => { this.Refresh(); };
                this.Refresh();
            };
            viewmodel.CurrentSpace.NodeRemoved += (obj) =>
            {
                this.Refresh();
            };
            viewmodel.Model.WorkspaceAdded += (obj) =>
            {
                this.Refresh();
            };
            viewmodel.Model.WorkspaceRemoved += (obj) =>
            {
                this.Refresh();
            };
            viewmodel.Model.WorkspaceOpening += (obj) =>
            {
                this.Refresh();
            };
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            List<AnnotationModel> selectedGroups = BeyondDynamoFunctions.GetAllSelectedGroups();
            Button button = (Button)sender;
            SolidColorBrush color = (SolidColorBrush)button.Foreground;
            System.Drawing.Color newColor = System.Drawing.Color.FromArgb(color.Color.A,
                                                             color.Color.R,
                                                             color.Color.G,
                                                             color.Color.B);
            string colorString = System.Drawing.ColorTranslator.ToHtml(newColor);
            BeyondDynamo.BeyondDynamoFunctions.ChangeGroupColor(selectedGroups, colorString);
        }
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            Brush background = button.Background;
            Brush foreground = button.Foreground;
            if (foreground.IsFrozen)
            {
                foreground = foreground.Clone();
            }
            button.Foreground = background;
            button.Background = foreground;
        }
        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            Brush foreground = button.Background;
            if (foreground.IsFrozen)
            {
                foreground = foreground.Clone();
            }
            Brush background = button.Foreground;
            button.Foreground = foreground;
            button.Background = background;
        }

        private void FreezeButton_Click(object sender, RoutedEventArgs e)
        {
            DynamoModel dynamoModel = BeyondDynamo.Utils.DynamoVM.Model;
            FreezeNodesCommand.FreezeNodes();
        }

        private void PythonRenameButton_Click(object sender, RoutedEventArgs e)
        {
            BeyondDynamoFunctions.RenamePythonInputs();
        }

        private void ImportDYNGraphButton_Click(object sender, RoutedEventArgs e)
        {
            DynamoViewModel viewmodel = BeyondDynamo.Utils.DynamoVM;
            BeyondDynamoFunctions.ImportFromScript(viewmodel);
        }

        private void SpecialColorButton_Click(object sender, RoutedEventArgs e)
        {
            List<AnnotationModel> selectedGroups = BeyondDynamoFunctions.GetAllSelectedGroups();
            BeyondDynamoFunctions.ChangeGroupColor(selectedGroups);
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            DynamoModel dynamoModel = BeyondDynamo.Utils.DynamoVM.Model;
            PreviewNodesCommand.PreviewNodes();
        }

        /// <summary>
        /// This methode in triggered when the text in the searchbar changes. It activates the searching algorithm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();

        }


        private void Search()
        {
            string searchTerm = searchBox.Text;

            this.nodeStacker.Children.Clear();
            this.foundNodes.Clear();
            if (searchTerm != "")
            {
                for (int i = 0; i < nodeLabels.Count; i++)
                {
                    SearchNodeLabel label = nodeLabels[i];
                    string name = label.NodeModel.Name;
                    if (name.ToUpper().Contains(searchTerm.ToUpper()))
                    {
                        this.nodeStacker.Children.Add(label);
                        this.foundNodes.Add(nodes[i]);
                    }
                }
            }
            else
            {
                this.foundNodes.Clear();
                this.foundNodes.AddRange(this.nodes);
                foreach (SearchNodeLabel label in nodeLabels)
                {
                    this.nodeStacker.Children.Add(label);
                }
            }
        }
        


        /// <summary>
        /// This methodes reloads all the Nodes and names from the current Workspace.
        /// </summary>
        private void Refresh()
        {
            this.Dispatcher.Invoke(() =>
            {
                DynamoViewModel DynamoViewModel = BeyondDynamo.Utils.DynamoVM;
                nodes = (List<NodeModel>)DynamoViewModel.CurrentSpace.Nodes;
                this.nodeLabels.Clear();
                this.foundNodes.Clear();
                this.nodeStacker.Children.Clear();
                foreach (NodeModel node in nodes)
                {
                    SearchNodeLabel nodeLabel = new SearchNodeLabel(node);

                    this.foundNodes.Add(node);
                    nodeLabels.Add(nodeLabel);
                }
                foreach (SearchNodeLabel label in nodeLabels)
                {
                    this.nodeStacker.Children.Add(label);
                }
                this.searchBox.Text = this.searchBox.Text;
                Search();
            });
            
        }


    }
}
