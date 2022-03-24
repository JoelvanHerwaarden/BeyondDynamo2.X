using Dynamo.Graph.Notes;
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
    public partial class NoteTextBoxWindow : Window
    {
        private NoteModel Note { get; set; }

        /// <summary>
        /// The Typed Text in the Test Editor Window
        /// </summary>
        public string InitialText { get; set; }

        private bool Accepted { get; set; }

        /// <summary>
        /// The Text Editor Window
        /// </summary>
        /// <param name="startText"></param>
        public NoteTextBoxWindow(NoteModel note)
        {
            this.Note = note;
            InitialText = Note.Text;
            InitializeComponent();
            this.Owner = Utils.DynamoWindow;
            textBox.Text = InitialText;
            Accepted = false;
        }

        /// <summary>
        /// Sets the Typed Text to the Text Property and closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Accepted = true;
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Note.Text = this.textBox.Text;
        }

        private void Change_Text_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Accepted)
            {
                this.Note.Text = InitialText;
            }
        }
    }
}
