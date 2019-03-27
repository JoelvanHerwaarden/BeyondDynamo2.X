﻿using System;
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
    public partial class TextBoxWindow : Window
    {
        /// <summary>
        /// The Typed Text in the Test Editor Window
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// The Text Editor Window
        /// </summary>
        /// <param name="startText"></param>
        public TextBoxWindow(string startText)
        {
            text = startText;
            InitializeComponent();
            textBox.Text = text;
        }

        /// <summary>
        /// Sets the Typed Text to the Text Property and closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            text = textBox.Text;
            this.Close();
        }
    }
}