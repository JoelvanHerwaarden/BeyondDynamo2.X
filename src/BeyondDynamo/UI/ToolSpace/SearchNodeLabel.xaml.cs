using BeyondDynamo.Utils;
using Dynamo.Graph.Nodes;
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

namespace BeyondDynamo.UI
{
    /// <summary>
    /// Interaction logic for SearchNodeLabel.xaml
    /// </summary>
    public partial class SearchNodeLabel : UserControl
    {
        public NodeModel NodeModel { get; }
        public SearchNodeLabel(NodeModel node)
        {
            InitializeComponent();
            NodeModel = node;
            this.ErrorDescriptionLabel.Visibility = Visibility.Hidden;
            if (node.State == ElementState.Warning | node.State == ElementState.PersistentWarning)
            {
                this.ErrorDescriptionLabel.Text = BeyondDynamo.BeyondDynamoFunctions.GetNodeViewModel(node).ErrorBubble.FullContent;
                this.ErrorDescriptionLabel.Visibility = Visibility.Visible;
                this.WarningIcon.Visibility = Visibility.Visible;
            }
            else if(node.State == ElementState.Error)
            {
                this.ErrorDescriptionLabel.Text = BeyondDynamo.BeyondDynamoFunctions.GetNodeViewModel(node).ErrorBubble.FullContent;
                this.ErrorDescriptionLabel.Visibility = Visibility.Visible;
                this.BrokenIcon.Visibility = Visibility.Visible;
            }
            this.NodeNameLabel.Content = node.Name;
        }

        private void ZoomToFit(object sender, MouseButtonEventArgs e)
        {
            //Run the Command twice for a better Result. 
            BeyondDynamoUtils.DynamoVM.CurrentSpaceViewModel.FindByIdCommand.Execute(this.NodeModel.GUID);
            BeyondDynamoUtils.DynamoVM.FitViewCommand.Execute(BeyondDynamoUtils.DynamoVM);
            BeyondDynamoUtils.DynamoVM.CurrentSpaceViewModel.FindByIdCommand.Execute(this.NodeModel.GUID);
            BeyondDynamoUtils.DynamoVM.FitViewCommand.Execute(BeyondDynamoUtils.DynamoVM);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            this.NodeNameLabel.Foreground = Brushes.Gray;
            this.ErrorDescriptionLabel.Foreground = Brushes.Gray;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.NodeNameLabel.Foreground = Brushes.White;
            this.ErrorDescriptionLabel.Foreground = Brushes.White;
        }
    }
}
