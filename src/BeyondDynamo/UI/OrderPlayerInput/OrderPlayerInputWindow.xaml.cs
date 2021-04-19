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
        public Dictionary<string, string> _InputNodes { get; set; }

        /// <summary>
        /// This List contains the Names of the Input Nodes
        /// </summary>
        public List<string> InputNodeNames { get; set; }

        public List<string> InputNodeIds { get; set; }

        /// <summary>
        /// The List with Input Nodes
        /// </summary>
        public Dictionary<string, string> _OutputNodes { get; set; }

        /// <summary>
        /// This List contains the Names of the Output Nodes
        /// </summary>
        public List<string> OutputNodeNames { get; set; }
        public List<string> OutputNodeIds { get; set; }

        /// <summary>
        /// Initiates the OrderPlayerInputWindow Class
        /// </summary>
        /// <param name="inputNodeNames"></param>
        /// <param name="outputNodeNames"></param>
        public OrderPlayerInputWindow(Dictionary<string, string> inputNodes, Dictionary<string,string> outputNodes)
        {
            InitializeComponent();
            Owner = BeyondDynamo.Utils.DynamoWindow;
            this._InputNodes = inputNodes;
            this._OutputNodes = outputNodes;

            InputNodeNames = new List<string>(this._InputNodes.Values);
            OutputNodeNames = new List<string>(this._OutputNodes.Values);
            InputNodeIds = new List<string>(this._InputNodes.Keys);
            OutputNodeIds= new List<string>(this._OutputNodes.Keys);

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
                    var shadowId = this.InputNodeIds[selectedIndex];

                    //Remove the Selected Item from the current Position
                    this.InputNodeNames.RemoveAt(selectedIndex);
                    this.InputNodeIds.RemoveAt(selectedIndex);

                    //Insert the Selected Item in the New Position
                    this.InputNodeNames.Insert(selectedIndex - 1, itemToMoveUp);
                    this.InputNodeIds.Insert(selectedIndex - 1, shadowId);

                    // Add the New Items to the Listbox
                    this.InputNodesListBox.Items.Clear();
                    foreach (string name in InputNodeNames)
                    {
                        this.InputNodesListBox.Items.Add(name);
                    }

                    //Change the Selected Item
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
                    var shadowId = this.InputNodeIds[selectedIndex];

                    //Remove the Selected Item from the current Position
                    this.InputNodeNames.RemoveAt(selectedIndex);
                    this.InputNodeIds.RemoveAt(selectedIndex);

                    //Insert the Selected Item in the New Position
                    this.InputNodeNames.Insert(selectedIndex + 1, itemToMoveDown);
                    this.InputNodeIds.Insert(selectedIndex + 1, shadowId);

                    // Add the New Items to the Listbox
                    this.InputNodesListBox.Items.Clear();
                    foreach (string name in InputNodeNames)
                    {
                        this.InputNodesListBox.Items.Add(name);
                    }

                    //Change the Selected Item
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
                    var shadowId = this.OutputNodeIds[selectedIndex];

                    //Remove the Selected Item from the current Position
                    this.OutputNodeNames.RemoveAt(selectedIndex);
                    this.OutputNodeIds.RemoveAt(selectedIndex);

                    //Insert the Selected Item in the New Position
                    this.OutputNodeNames.Insert(selectedIndex - 1, itemToMoveUp);
                    this.OutputNodeIds.Insert(selectedIndex - 1, shadowId);

                    // Add the New Items to the Listbox
                    this.OutputNodesListBox.Items.Clear();
                    foreach (string name in OutputNodeNames)
                    {
                        this.OutputNodesListBox.Items.Add(name);
                    }

                    //Change the Selected Item
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
                    var shadowId = this.OutputNodeIds[selectedIndex];

                    //Remove the Selected Item from the current Position
                    this.OutputNodeNames.RemoveAt(selectedIndex);
                    this.OutputNodeIds.RemoveAt(selectedIndex);

                    //Insert the Selected Item in the New Position
                    this.OutputNodeNames.Insert(selectedIndex + 1, itemToMoveDown);
                    this.OutputNodeIds.Insert(selectedIndex + 1, shadowId);

                    // Add the New Items to the Listbox
                    this.OutputNodesListBox.Items.Clear();
                    foreach (string name in OutputNodeNames)
                    {
                        this.OutputNodesListBox.Items.Add(name);
                    }

                    //Change the Selected Item
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
            #region GET THE ORDERED IDS

            foreach (string id in InputNodeIds)
            {
                sortedIds.Add(id);
            }
            foreach (string id in OutputNodeIds)
            {
                sortedIds.Add(id);
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
