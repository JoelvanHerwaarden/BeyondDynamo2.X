using System.Windows;
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
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace BeyondDynamo
{
    /// <summary>
    /// Interaction logic for RemoveTraceDataWindow.xaml
    /// </summary>
    public partial class OrderPlayerInputWindow : Window
    {
        /// <summary>
        /// The Dynamo Version of the File
        /// </summary>
        public string dynamoVersion { get; set; }

        /// <summary>
        /// The Object which represents the DynamoGraph as a Json Object
        /// </summary>
        public JObject dynamoJsonGraph { get; set; }

        /// <summary>
        /// The Object which represents the DynamoGraph as a XML Document
        /// </summary>
        public XmlDocument dynamoXMLGraph { get; set; }

        /// <summary>
        /// The FilePath to the Dynamo File
        /// </summary>
        public string dynamoGraphPath { get; set; }

        /// <summary>
        /// The List with Input Nodes
        /// </summary>
        public List<string> InputNodeNames { get; set; }

        /// <summary>
        /// The List with Input Nodes
        /// </summary>
        public List<string> OutputNodeNames { get; set; }

        /// <summary>
        /// Initiates the OrderPlayerInputWindow Class
        /// </summary>
        /// <param name="inputNodeNames"></param>
        /// <param name="outputNodeNames"></param>
        public OrderPlayerInputWindow(List<string> inputNodeNames, List<string> outputNodeNames)
        {
            InitializeComponent();
            InputNodeNames = inputNodeNames;
            OutputNodeNames = outputNodeNames;

            //Add the Input Node Names to The ListBox
            foreach (string nodeName in InputNodeNames)
            {
                InputNodesListBox.Items.Add(nodeName);
            }

            //Add the Output Node Names to The ListBox
            foreach (string nodeName in OutputNodeNames)
            {
                OutputNodesListBox.Items.Add(nodeName);
            }
        }

        //Input Button Commands
        #region INPUT TAB COMMANDS
        /// <summary>
        /// Moves the selected input name up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void input_up_click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIndex = this.InputNodesListBox.SelectedIndex;

                if (selectedIndex > 0)
                {
                    var itemToMoveUp = this.InputNodeNames[selectedIndex];
                    this.InputNodeNames.RemoveAt(selectedIndex);
                    this.InputNodeNames.Insert(selectedIndex - 1, itemToMoveUp);
                    this.InputNodesListBox.Items.Clear();
                    foreach (string name in InputNodeNames)
                    {
                        this.InputNodesListBox.Items.Add(name);
                    }
                    this.InputNodesListBox.SelectedIndex = selectedIndex - 1;
                }
            }
            catch (System.Exception)
            {
            }
        }

        /// <summary>
        /// Moves the selected input name down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void input_down_click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIndex = this.InputNodesListBox.SelectedIndex;

                if (selectedIndex + 1 < this.InputNodeNames.Count)
                {
                    var itemToMoveDown = this.InputNodeNames[selectedIndex];
                    this.InputNodeNames.RemoveAt(selectedIndex);
                    this.InputNodeNames.Insert(selectedIndex + 1, itemToMoveDown);
                    this.InputNodesListBox.Items.Clear();
                    foreach (string name in InputNodeNames)
                    {
                        this.InputNodesListBox.Items.Add(name);
                    }
                    this.InputNodesListBox.SelectedIndex = selectedIndex + 1;
                }
            }
            catch (System.Exception)
            {
            }
        }
        #endregion

        
        #region OUTPUT TAB COMMANDS
        /// <summary>
        /// Moves the selected output name uo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void output_up_click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIndex = this.OutputNodesListBox.SelectedIndex;

                if (selectedIndex > 0)
                {
                    var itemToMoveUp = this.OutputNodeNames[selectedIndex];
                    this.OutputNodeNames.RemoveAt(selectedIndex);
                    this.OutputNodeNames.Insert(selectedIndex - 1, itemToMoveUp);
                    this.OutputNodesListBox.Items.Clear();
                    foreach (string name in OutputNodeNames)
                    {
                        this.OutputNodesListBox.Items.Add(name);
                    }
                    this.OutputNodesListBox.SelectedIndex = selectedIndex - 1;
                }
            }
            catch (System.Exception)
            {
            }
        }

        /// <summary>
        /// Moves the selected output name down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void output_down_click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIndex = this.OutputNodesListBox.SelectedIndex;

                if (selectedIndex + 1 < this.OutputNodeNames.Count)
                {
                    var itemToMoveDown = this.OutputNodeNames[selectedIndex];
                    this.OutputNodeNames.RemoveAt(selectedIndex);
                    this.OutputNodeNames.Insert(selectedIndex + 1, itemToMoveDown);
                    this.OutputNodesListBox.Items.Clear();
                    foreach (string name in OutputNodeNames)
                    {
                        this.OutputNodesListBox.Items.Add(name);
                    }
                    this.OutputNodesListBox.SelectedIndex = selectedIndex + 1;
                }
            }
            catch (System.Exception)
            {
            }
        }
        #endregion

        //Clicking the OK Button
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if(this.dynamoVersion == "Json")
            {
                applySortToJson();
            }
            else if(dynamoVersion == "XML")
            {
                applySortToXML();
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Sorts the Nodes for a Dynamo 2.0 file
        /// </summary>
        private void applySortToJson()
        {
            //Count Number of input/output nodes
            int numberOfNodes = InputNodeNames.Count + OutputNodeNames.Count;

            //Make a List for the Node Id's
            List<string> sortedIds = new List<string>();

            //Input Actions
            #region INPUTS
            //Clear the Current Order of Input Names
            InputNodeNames.Clear();

            //Fill Input Node Name with the Ordered Values
            foreach (string name in InputNodesListBox.Items)
            {
                InputNodeNames.Add(name);
            }

            //Create a Input Node List with JTokens
            List<JToken> inputNodes = new List<JToken>();

            //Specify a Input Parent
            JContainer inputParent = null;

            //Start Looping over the Ordered Names 
            foreach (string name in InputNodeNames)
            {
                //Search for the Input Section on the Dynamo Graph
                JToken inputSection = dynamoJsonGraph.SelectToken("Inputs");

                //Start Looping over all the Input Nodes
                foreach (JToken inputNode in inputSection.Children())
                {
                    //Get the Name of the Current Input Node in the Input Section
                    string nodeName = inputNode.Value<string>("Name");

                    //Check if the Node Name is the Same as the Ordered Input Node Name
                    if (nodeName == name)
                    {
                        sortedIds.Add(inputNode.Value<string>("Id"));
                        inputNodes.Add(inputNode);
                        inputParent = inputNode.Parent;
                    }
                }
            }

            if (inputNodes.Count > 0)
            {
                //Clear the Inputs
                inputParent.RemoveAll();

                //Add The Ordered Nodes
                inputParent.Add(inputNodes);
            }

            #endregion

            #region OUTPUTS
            //Clear the Current Order of Output Names
            OutputNodeNames.Clear();

            //Fill Output Node Name with the Ordered Values
            foreach (string name in OutputNodesListBox.Items)
            {
                OutputNodeNames.Add(name);
            }

            //Create a Output Node List with JTokens
            List<JToken> outputNodes = new List<JToken>();

            //Specify a Output Parent
            JContainer outputParent = null;

            //Start Looping over the Ordered Names 
            foreach (string name in OutputNodeNames)
            {
                //Search for the Input Section on the Dynamo Graph
                JToken outputSection = dynamoJsonGraph.SelectToken("Outputs");

                //Start Looping over all the Input Nodes
                foreach (JToken outputNode in outputSection.Children())
                {
                    //Get the Name of the Current Input Node in the Input Section
                    string nodeName = outputNode.Value<string>("Name");

                    //Check if the Node Name is the Same as the Ordered Input Node Name
                    if (nodeName == name)
                    {
                        sortedIds.Add(outputNode.Value<string>("Id"));
                        outputNodes.Add(outputNode);
                        outputParent = outputNode.Parent;
                    }
                }
            }

            if (outputNodes.Count > 0)
            {
                //Clear the outputs
                outputParent.RemoveAll();

                //Add The Ordered Nodes
                outputParent.Add(outputNodes);
            }
            #endregion

            #region NODEMODELS
            //Specify Parent for NodeViewModels
            JContainer nodeParent = null;

            //Create a List which can hold the JTokens of the NodeViews
            JToken[] orderedNodes = new JToken[numberOfNodes];
            List<JToken> unOrderedNodes = new List<JToken>();

            //Get the Node View Section
            JToken nodeModelSection = dynamoJsonGraph.SelectToken("Nodes");

            //Loop over the Selected Input and Outputnames
            foreach (JToken nodeModel in nodeModelSection.Children())
            {
                string nodeModelId = nodeModel.Value<string>("Id");
                if (sortedIds.Contains(nodeModelId))
                {
                    int ind = sortedIds.IndexOf(nodeModelId);
                    nodeParent = nodeModel.Parent;
                    orderedNodes.SetValue(nodeModel, ind);
                }
                else
                {
                    unOrderedNodes.Add(nodeModel);
                }
            }

            //Add all Nodes in order
            if (orderedNodes.Length > 0)
            {
                nodeParent.RemoveAll();

                //Add The Ordered Nodes
                nodeParent.Add(orderedNodes);
                nodeParent.Add(unOrderedNodes);
            }
            #endregion

            #region VIEWMODELS
            //Specify Parent for NodeViewModels
            JContainer viewParent = null;

            //Create a List which can hold the JTokens of the NodeViews
            JToken[] orderedViews = new JToken[numberOfNodes];
            List<JToken> unOrderedViews = new List<JToken>();

            //Get the Node View Section
            JToken nodeViewSection = dynamoJsonGraph.SelectToken("View").SelectToken("NodeViews");

            //Loop over the Selected Input and Outputnames
            foreach (JToken nodeViewModel in nodeViewSection.Children())
            {
                string nodeModelId = nodeViewModel.Value<string>("Id");
                if (sortedIds.Contains(nodeModelId))
                {
                    int ind = sortedIds.IndexOf(nodeModelId);
                    viewParent = nodeViewModel.Parent;
                    orderedViews.SetValue(nodeViewModel, ind);
                }
                else
                {
                    unOrderedViews.Add(nodeViewModel);
                }
            }

            //Add all the Node View in Order
            if (orderedNodes.Length > 0)
            {
                viewParent.RemoveAll();

                //Add The Ordered Nodes
                viewParent.Add(orderedViews);
                viewParent.Add(unOrderedViews);
            }
            #endregion

            //Write that string 
            File.WriteAllText(dynamoGraphPath, dynamoJsonGraph.ToString());

            //Close the Order Input Window
            this.Close();

        }

        /// <summary>
        /// Sorts the Nodes for a Dynamo 1.3 file
        /// </summary>
        private void applySortToXML()
        {
            InputNodeNames.Clear();
            foreach (string name in InputNodesListBox.Items)
            {
                InputNodeNames.Add(name);
            }

            foreach (XmlElement child in dynamoXMLGraph.DocumentElement)
            {
                if (child.Name == "Elements")
                {
                    foreach (string name in this.InputNodeNames)
                    {
                        foreach (XmlElement Node in child.ChildNodes)
                        {
                            if (Node.Attributes["nickname"].Value == name)
                            {
                                child.RemoveChild(Node);
                                child.AppendChild(Node);
                            }
                        }
                    }
                }
            }
            dynamoXMLGraph.Save(dynamoGraphPath);
            this.Close();
        }
    }
}
