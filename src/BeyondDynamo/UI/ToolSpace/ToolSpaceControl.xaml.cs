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

namespace BeyondDynamo
{
    /// <summary>
    /// Interaction logic for TextBoxWindow.xaml
    /// </summary>
    public partial class ToolSpaceControl : ContentControl
    {

        private List<NodeModel> completedNodes = new List<NodeModel>();
        private List<NodeModel> warningNodes = new List<NodeModel>();
        private List<NodeModel> errorNodes = new List<NodeModel>();
        public ToolSpaceControl()
        {
            InitializeComponent();
            DynamoViewModel viewmodel = BeyondDynamo.Utils.DynamoVM;
            //viewmodel.Model.RefreshCompleted += EvaluateGraph;
        }

        private async void EvaluateGraph(HomeWorkspaceModel model)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.completedNodes = new List<NodeModel>();
                this.warningNodes = new List<NodeModel>();
                this.errorNodes = new List<NodeModel>();
                WarningStacker.Children.Clear();
                WorkspaceModel currentWorkSpace = BeyondDynamo.Utils.DynamoVM.CurrentSpace;
                foreach (NodeModel node in currentWorkSpace.Nodes)
                {
                    if (node.State == ElementState.Active)
                    {
                        completedNodes.Add(node);
                    }
                    else if (node.State == ElementState.Error)
                    {
                        errorNodes.Add(node);
                    }
                    else if (node.State == ElementState.Warning || node.State == ElementState.PersistentWarning)
                    {
                        warningNodes.Add(node);
                        string warningText = BeyondDynamoFunctions.GetNodeViewModel(node).ErrorBubble.FullContent;
                        Button warningButton = new Button()
                        {
                            Style = (Style)this.Resources["WarningButton"],
                            Content = warningText
                        };
                        warningButton.Click += WarningButton_Click; ;
                        WarningStacker.Children.Add(warningButton);
                    }
                    else
                    {

                    }
                }
            }));
           
        }

        private void WarningButton_Click(object sender, RoutedEventArgs e)
        {
            Button current = (Button)sender;
            int index = this.WarningStacker.Children.IndexOf(current);
            NodeModel node = warningNodes[index];
            DynamoViewModel dynamoViewModel = BeyondDynamo.Utils.DynamoVM;

            //Run the Command twice for a better Result.
            dynamoViewModel.CurrentSpaceViewModel.FindByIdCommand.Execute(node.GUID);
            dynamoViewModel.FitViewCommand.Execute(dynamoViewModel);
            dynamoViewModel.CurrentSpaceViewModel.FindByIdCommand.Execute(node.GUID);
            dynamoViewModel.FitViewCommand.Execute(dynamoViewModel);
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            DynamoViewModel VM = BeyondDynamo.Utils.DynamoVM;
            WorkspaceViewModel currentViewModel = VM.CurrentSpaceViewModel;
            List<AnnotationViewModel> selectedGroups = new List<AnnotationViewModel>();
            int count = 0;
            foreach (AnnotationViewModel group in currentViewModel.Annotations)
            {
                Button button = (Button)sender;
                if (group.AnnotationModel.IsSelected)
                {
                    selectedGroups.Add(group);
                    SolidColorBrush color = (SolidColorBrush)button.Foreground;
                    group.Background = color.Color;
                    count++;
                }
            }
            if (count == 0)
            {
                MessageBox.Show("No group Selected");
            }
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
            WorkspaceViewModel workspaceView = BeyondDynamo.Utils.DynamoVM.CurrentSpaceViewModel;
            string configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dynamo\\BeyondDynamoSettings\\beyondDynamo2Config.json");
            BeyondDynamoConfig config = new BeyondDynamoConfig(configFilePath);
            BeyondDynamoFunctions.ChangeGroupColor(workspaceView, config);
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            DynamoModel dynamoModel = BeyondDynamo.Utils.DynamoVM.Model;
            PreviewNodesCommand.PreviewNodes();
        }

        private void ScrollViewer_Unloaded(object sender, RoutedEventArgs e)
        {
            DynamoViewModel viewmodel = BeyondDynamo.Utils.DynamoVM;
            //viewmodel.Model.RefreshCompleted -= EvaluateGraph;
        }
    }
}
