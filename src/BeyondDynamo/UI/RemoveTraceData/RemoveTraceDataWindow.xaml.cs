using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using Dynamo.ViewModels;
using System;

namespace BeyondDynamo.UI
{
    /// <summary>
    /// Interaction logic for RemoveTraceDataWindow.xaml
    /// </summary>
    public partial class RemoveTraceDataWindow : Window
    {
        /// <summary>
        /// Dynamo Viewmodel for the RemoveTraceDataWindow Class
        /// </summary>
        public DynamoViewModel viewModel { get; set; }

        /// <summary>
        /// The Selected Directory of the RemoveTraceDataWindow Class
        /// </summary>
        private string selectedDirectory { get; set; }

        /// <summary>
        /// This Constructor gets called on initiating the Class
        /// </summary>
        public RemoveTraceDataWindow()
        {
            InitializeComponent();
            selectedDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            this.selectedDirectoryLabel.Text = selectedDirectory;
        }

        /// <summary>
        /// Lets the User open a Directory Browser and select a Directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog();
            browserDialog.ShowNewFolderButton = false;
            browserDialog.SelectedPath = null;
            if (browserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (browserDialog.SelectedPath != null)
                {
                    selectedDirectory = browserDialog.SelectedPath;
                    selectedDirectoryLabel.Text = selectedDirectory;
                }
            }
        }

        /// <summary>
        /// This Calls the RemoveTraceData Function on each Selected File from the Project Directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveTraceDataButton_Click(object sender, RoutedEventArgs e)
        {
            //Create empty Lists for Log Strings
            List<string> succesLines = new List<string>();
            List<string> emptyLines = new List<string>();


            foreach(string fileName in FileListBox.Items)
            {
                //Combine name if the File and the Selected Directory into a File Path
                string filePath = Path.Combine(selectedDirectory, fileName);

                //Check is one of the selected files is not open
                if(BeyondDynamoFunctions.IsFileOpen(this.viewModel, filePath))
                {
                    System.Windows.Forms.MessageBox.Show("Please close the File before opening it", "Close file");
                    continue;
                }

                string coreLanguage = BeyondDynamoFunctions.DynamoCoreLanguage(filePath);
                bool succes = false;
                if (coreLanguage == "XML")
                {
                    succes = BeyondDynamoFunctions.RemoveSessionTraceData(filePath);
                }
                else if(coreLanguage == "JSON")
                {
                    succes = BeyondDynamoFunctions.RemoveBindings(filePath);
                }

                if (succes)
                {
                    //Log if there was trace data removed
                    string messageLine = "Session Trace Data Removed From " + fileName + "\n";
                    succesLines.Add(messageLine);
                }
                else
                {
                    //Log If there was no trace data to remove
                    string messageLine = "No Session Trace Data Found in " + fileName + "\n";
                    emptyLines.Add(messageLine);
                }
            }
            // Close the Window
            this.Close();

            // Concatnate all messages and show them
            List<string> messageLines = new List<string>();
            messageLines.AddRange(succesLines);
            messageLines.Add("\n");
            messageLines.AddRange(emptyLines);
            System.Windows.Forms.MessageBox.Show(string.Concat(messageLines), "Remove Trace Data");
        }

        /// <summary>
        /// Fills the List Box with the Dynamo Files everytime the SelectedFolder Changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FillFileListBox(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                selectedDirectory = this.selectedDirectoryLabel.Text;
                string directory = selectedDirectory;
                string[] files = Directory.GetFiles(directory, "*.dyn");
                this.FileListBox.Items.Clear();
                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    FileListBox.Items.Add(fileName);
                }
                FileListBox.SelectAll();
            }
            catch
            {

            }
        }
    }


}
