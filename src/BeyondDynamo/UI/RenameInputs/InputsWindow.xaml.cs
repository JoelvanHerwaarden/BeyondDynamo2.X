using DSCore;
using Dynamo.Graph.Connectors;
using Dynamo.Graph.Nodes;
using ProtoCore.AST.AssociativeAST;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Forms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Dynamo.ViewModels;
using Dynamo.Utilities;

namespace BeyondDynamo.UI
{
    /// <summary>
    /// Interaction logic for InputsWindow.xaml
    /// </summary>
    public partial class InputsWindow : Window
    {
        private NodeViewModel NodeView { get; set; }
        private NodeModel Node { get; set; }
        private List<string> InputToolTips { get; set; }
        private List<PortModel> StartConnectorPorts { get; set; }
        private List<string> Inputs { get; set; }
        private string OutputToolTip { get; set; }
        private string Output { get; set; }
        private PortModel EndConnectorPort { get; set; }

        public InputsWindow(NodeViewModel nodeView)
        {
            InitializeComponent();
            this.Owner = BeyondDynamo.Utils.DynamoWindow;
            this.Title = "Rename Python Inputs for " + nodeView.NodeLogic.Name;
            this.Node = nodeView.NodeLogic;
            this.NodeView = nodeView;
            GetInOutputs(this.Node);
            PopulateWindow(this.Inputs, this.Output);
        }

        private void GetInOutputs(NodeModel node)
        {
            GetInOutConnectors(node);
            this.Inputs = new List<string>();
            this.InputToolTips = new List<string>();
            for(int i=0; i< node.InPorts.Count();i++)
            {
                PortModel portModel = node.InPorts[i];
                this.Inputs.Add(portModel.Name);
                string toolTip = portModel.ToolTip;
                if(!toolTip.Contains("You can reference"))
                {
                    toolTip += string.Format("\nYou can reference this input with IN[{0}] in the Python Script", i.ToString());
                }
                this.InputToolTips.Add(toolTip);
            }
            foreach(PortModel portModel in node.OutPorts)
            {
                this.Output = portModel.Name;
                string toolTip = portModel.ToolTip;
                if (!toolTip.Contains("You can reference"))
                {
                    toolTip += "\nYou can reference this input with OUT in the Python Script";
                }
                this.OutputToolTip = toolTip;
            }
        }

        private void GetInOutConnectors(NodeModel node)
        {
            this.StartConnectorPorts = new List<PortModel>();
            this.EndConnectorPort = null;
            foreach(PortModel portModel in node.InPorts)
            {
                if (portModel.Connectors.Count() != 0)
                {
                    StartConnectorPorts.Add(portModel.Connectors.Last().Start);
                }
                else
                {
                    StartConnectorPorts.Add(null);
                }
            }
            PortModel outport = node.OutPorts.Last();
            if(outport.Connectors.Count() != 0)
            {
                EndConnectorPort = outport.Connectors.Last().End;
            }
        }

        public void SetInOutput(NodeViewModel nodeView)
        {
            NodeModel node = nodeView.NodeModel;

            node.InPorts.RemoveAll((p) => { return true; });
            for(int i = 0; i < this.Inputs.Count; i++)
            {
                string inputName = this.Inputs[i];
                string inputToolTip = this.InputToolTips[i];

                PortData portData = new PortData(inputName, inputToolTip);
                PortModel inputPort = new PortModel(PortType.Input, node, portData);
                node.InPorts.Add(inputPort);
                PortModel startPort = StartConnectorPorts[i];
                if(startPort != null)
                {
                    inputPort.Connectors.Add(new ConnectorModel(startPort, node.InPorts.Last(), Guid.NewGuid()));
                }
            }

            node.OutPorts.RemoveAll((p) => { return true; });
            string outputName = this.Output;
            string outputToolTip = this.OutputToolTip;
            PortData portdata = new PortData(outputName, outputToolTip);
            PortModel outputPort = new PortModel(PortType.Output, node, portdata);
            node.OutPorts.Add(outputPort);
            if(EndConnectorPort != null)
            {
                outputPort.Connectors.Add(new ConnectorModel(node.OutPorts.Last(), EndConnectorPort, Guid.NewGuid()));
            }
            node.RegisterAllPorts();
        }

        public void PopulateWindow(List<string> inputs, string output)
        {
            Label label = this.stackPanel.Children[0] as Label;
            foreach (string input in inputs)
            {
                TextBox inputtextbox = new TextBox()
                {
                    BorderThickness = new System.Windows.Thickness(0),
                    Text = input,
                    FontSize = 14,
                    Foreground = label.Foreground,
                    Margin = new System.Windows.Thickness(10)
                };
                this.stackPanel.Children.Add(inputtextbox);
            }
            Label outputLabel = new Label()
            {
                Content = "Output",
                Foreground = label.Foreground,
                FontWeight = FontWeights.DemiBold,
                Margin = new System.Windows.Thickness(10)
            };
            this.stackPanel.Children.Add(outputLabel);

            TextBox inputbox = new TextBox()
            {
                BorderThickness = new System.Windows.Thickness(0),
                Text = output,
                FontSize = 14,
                Foreground = label.Foreground,
                Margin = new System.Windows.Thickness(10)
            };
            this.stackPanel.Children.Add(inputbox);
        }

        public void GetValuesFromWindow()
        {
            this.Inputs.Clear();
            bool lookingForInputs = false;
            foreach(UIElement uIElement in this.stackPanel.Children)
            {
                if (uIElement.GetType().ToString().Contains("Label"))
                {
                    if (lookingForInputs)
                    {
                        lookingForInputs = false;
                    }
                    else
                    {
                        lookingForInputs = true;
                    }
                }
                else
                {
                    TextBox inputField = (TextBox)uIElement;
                    string value = inputField.Text;
                    if (lookingForInputs)
                    {
                        this.Inputs.Add(value);
                    }
                    else
                    {
                        this.Output = value;
                    }
                }
            }
        }

        private void ReNameButton_Click(object sender, RoutedEventArgs e)
        {
            GetValuesFromWindow();
            SetInOutput(this.NodeView);
            this.Close();
        }
    }
}
