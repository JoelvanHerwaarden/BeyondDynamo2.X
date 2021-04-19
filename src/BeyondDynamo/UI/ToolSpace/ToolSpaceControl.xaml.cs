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

namespace BeyondDynamo
{
    /// <summary>
    /// Interaction logic for TextBoxWindow.xaml
    /// </summary>
    public partial class ToolSpaceControl : ContentControl
    {
        public ToolSpaceControl()
        {
            InitializeComponent();
            if (BeyondDynamo.Utils.AutomaticHide)
            {

            }
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            DynamoViewModel VM = BeyondDynamo.Utils.DynamoVM;
            WorkspaceViewModel currentViewModel = VM.CurrentSpaceViewModel;
            foreach (AnnotationViewModel group in currentViewModel.Annotations)
            {
                Button button = (Button)sender;
                if (group.AnnotationModel.IsSelected)
                {
                    SolidColorBrush color = (SolidColorBrush)button.Foreground;
                    group.Background = color.Color;
                }
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
            BeyondDynamoFunctions.FreezeNodes(dynamoModel);
        }
        private void UnFreezeButton_Click(object sender, RoutedEventArgs e)
        {
            DynamoModel dynamoModel = BeyondDynamo.Utils.DynamoVM.Model;
            BeyondDynamoFunctions.UnfreezeNodes(dynamoModel);
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

        private void onPreview_Click(object sender, RoutedEventArgs e)
        {
            BeyondDynamoFunctions.ChangePreviewStateOfSelectedNodes(false);
        }

        private void previewoff_Click(object sender, RoutedEventArgs e)
        {
            BeyondDynamoFunctions.ChangePreviewStateOfSelectedNodes(true);
        }
    }
}
