using Dynamo.Graph.Nodes;
using Dynamo.UI.Commands;
using Dynamo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeyondDynamo
{
    /// <summary>
    /// Interaction logic for TextBoxWindow.xaml
    /// </summary>
    public partial class NodeCollectorWindow : Window
    {
        private List<NodeModel> nodes { get; set; }

        private List<string> nodeNames { get; set; }

        private List<NodeModel> foundNodes { get; set; }


        private DynamoViewModel dynamoViewModel { get; set; }



        /// <summary>
        /// The Text Editor Window
        /// </summary>
        /// <param name="startText"></param>
        public NodeCollectorWindow(List<dynamic> InputNodes, DynamoViewModel DynamoViewModel)
        {
            dynamoViewModel = DynamoViewModel;

            nodes = InputNodes[0];
            nodeNames = InputNodes[1];
            InitializeComponent();
            foreach(string name in nodeNames)
            {
                this.listView.Items.Add(name);
            }
            this.foundNodes = new List<NodeModel>();
            foreach(NodeModel node in InputNodes[0])
            {
                this.foundNodes.Add(node);
            }
        }
        

        /// <summary>
        /// Sets the Typed Text to the Text Property and closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = searchBox.Text;
            this.listView.Items.Clear();
            this.foundNodes.Clear();
            if (searchTerm != "")
            {
                for (int i = 0; i < nodeNames.Count; i++ )
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
                this.foundNodes = nodes;
                foreach (string name in nodeNames)
                {
                    this.listView.Items.Add(name);
                }
            }
        }

        private void ZoomToNode(NodeModel node, DynamoViewModel dynamoViewModel)
        {
            //Run the Command twice for a better Result. 
            dynamoViewModel.CurrentSpaceViewModel.FindByIdCommand.Execute(node.GUID);
            dynamoViewModel.FitViewCommand.Execute(dynamoViewModel);
            dynamoViewModel.CurrentSpaceViewModel.FindByIdCommand.Execute(node.GUID);
            dynamoViewModel.FitViewCommand.Execute(dynamoViewModel);
        }


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listView.SelectedItem != null)
            {
                int nodeIndex = this.listView.SelectedIndex;
                NodeModel foundNode = foundNodes[nodeIndex];
                ZoomToNode(foundNode, this.dynamoViewModel);
            }
        }
        
    }
}
