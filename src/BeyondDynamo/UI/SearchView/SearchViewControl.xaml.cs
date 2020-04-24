using Dynamo.Graph.Nodes;
using Dynamo.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Controls;
using Dynamo.Wpf.ViewModels;

namespace BeyondDynamo
{
    /// <summary>
    /// Interaction logic for TextBoxWindow.xaml
    /// </summary>
    public partial class SearchViewControl : ContentControl
    {
        //These are all the nodes in the dynamo model
        private List<NodeModel> nodes { get; set; }

        //These are all the Node names
        private List<string> nodeNames { get; set; }

        //These are the nodes which match the search term
        private List<NodeModel> foundNodes { get; set; }

        //This it the current Dynamo Model
        private DynamoViewModel dynamoViewModel { get; set; }

        /// <summary>
        /// The Text Editor Window
        /// </summary>
        /// <param name="startText"></param>
        public SearchViewControl(DynamoViewModel DynamoViewModel)
        {
            IEnumerator<NodeSearchElementViewModel> enumerator = DynamoViewModel.SearchViewModel.FilteredResults.GetEnumerator();

            InitializeComponent();

            //Set the current Dynamo view model
            dynamoViewModel = DynamoViewModel;
            //Get the nodes in the current workspace
            nodes = (List<NodeModel>)DynamoViewModel.CurrentSpace.Nodes;

            //Create new lists for the names and the nodes
            this.nodeNames = new List<string>();
            this.foundNodes = new List<NodeModel>();

            //Start looping over the nodes and get the name and node. Add them to the right lists.
            foreach (NodeModel node in nodes)
            {
                string name = node.Name;
                this.foundNodes.Add(node);
                nodeNames.Add(name);
            }

            //Add all the names of the nodes to the listview control 
            foreach (string name in nodeNames)
            {
                this.listView.Items.Add(name);
            }

            //Register events for adding and removing nodes to the current workspace
            DynamoViewModel.CurrentSpace.NodeAdded += (obj) =>
            {
                this.Refresh();
            };
            DynamoViewModel.CurrentSpace.NodeRemoved += (obj) =>
            {
                this.Refresh();
            };
            DynamoViewModel.Model.WorkspaceAdded += (obj) =>
            {
                this.Refresh();
            };
            DynamoViewModel.Model.WorkspaceRemoved += (obj) =>
            {
                this.Refresh();
            };
            DynamoViewModel.Model.WorkspaceOpening += (obj) =>
            {
                this.Refresh();
            };
        }

        /// <summary>
        /// This methode in triggered when the text in the searchbar changes. It activates the searching algorithm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = searchBox.Text;

            this.listView.Items.Clear();
            this.foundNodes.Clear();
            if (searchTerm != "")
            {
                for (int i = 0; i < nodeNames.Count; i++)
                {
                    string name = nodeNames[i];
                    if (name.ToUpper().Contains(searchTerm.ToUpper()))
                    {
                        this.listView.Items.Add(name);
                        this.foundNodes.Add(nodes[i]);
                    }
                }
            }
            else
            {
                this.foundNodes.Clear();
                this.foundNodes.AddRange(this.nodes);
                foreach (string name in nodeNames)
                {
                    this.listView.Items.Add(name);
                }
            }
                 
        }

        /// <summary>
        /// This methodes is triggered when the user clicks on one of the Node names in the List view. It zooms in to one of the selected Nodes
        /// </summary>
        /// <param name="node"></param>
        /// <param name="dynamoViewModel"></param>
        private void ZoomToNode(NodeModel node, DynamoViewModel dynamoViewModel)
        {
            //Run the Command twice for a better Result. 
            dynamoViewModel.CurrentSpaceViewModel.FindByIdCommand.Execute(node.GUID);
            dynamoViewModel.FitViewCommand.Execute(dynamoViewModel);
            dynamoViewModel.CurrentSpaceViewModel.FindByIdCommand.Execute(node.GUID);
            dynamoViewModel.FitViewCommand.Execute(dynamoViewModel);
        }

        /// <summary>
        /// This methode is triggered when the user clicks on one of the Node names in the List view 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="dynamoViewModel"></param>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listView.SelectedItem != null)
            {
                int nodeIndex = this.listView.SelectedIndex;
                NodeModel foundNode = foundNodes[nodeIndex];
                ZoomToNode(foundNode, this.dynamoViewModel);
            }
        }

        /// <summary>
        /// This methodes reloads all the Nodes and names from the current Workspace.
        /// </summary>
        private void Refresh()
        {
            DynamoViewModel DynamoViewModel = this.dynamoViewModel;
            nodes = (List<NodeModel>)DynamoViewModel.CurrentSpace.Nodes;
            this.nodeNames.Clear();
            this.foundNodes.Clear();
            this.listView.Items.Clear();
            foreach (NodeModel node in nodes)
            {
                string name = node.Name;
                this.foundNodes.Add(node);
                nodeNames.Add(name);
            }
            foreach (string name in nodeNames)
            {
                this.listView.Items.Add(name);
            }
        }
    }
}
