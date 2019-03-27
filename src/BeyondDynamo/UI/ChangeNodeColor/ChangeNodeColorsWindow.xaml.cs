using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using Forms = System.Windows.Forms;

namespace BeyondDynamo.UI
{
    /// <summary>
    /// Interaction logic for ChangeNodeColors.xaml
    /// </summary>
    public partial class ChangeNodeColorsWindow : Window
    {
        private System.Windows.ResourceDictionary dynamoNodeSettings { get; set; }

        private SolidColorBrush _ITitleText { get; set; }
        private SolidColorBrush _ITitleBackground { get; set; }
        private SolidColorBrush _ITitleBorder { get; set; }
        private SolidColorBrush _IBodyBackground { get; set; }
        private SolidColorBrush _IBodyBorder { get; set; }

        private SolidColorBrush _ATitleText { get; set; }
        private SolidColorBrush _ATitleBackground { get; set; }
        private SolidColorBrush _ATitleBorder { get; set; }
        private SolidColorBrush _ABodyBackground { get; set; }
        private SolidColorBrush _ABodyBorder { get; set; }

        private SolidColorBrush _WTitleText { get; set; }
        private SolidColorBrush _WTitleBackground { get; set; }
        private SolidColorBrush _WTitleBorder { get; set; }
        private SolidColorBrush _WBodyBackground { get; set; }
        private SolidColorBrush _WBodyBorder { get; set; }

        private SolidColorBrush _ETitleText { get; set; }
        private SolidColorBrush _ETitleBackground { get; set; }
        private SolidColorBrush _ETitleBorder { get; set; }
        private SolidColorBrush _EBodyBackground { get; set; }
        private SolidColorBrush _EBodyBorder { get; set; }

        
        /// <summary>
        /// Public constructor for making the Change node color window
        /// </summary>
        /// <param name="DynamoNodeSettings"></param>
        public ChangeNodeColorsWindow(System.Windows.ResourceDictionary DynamoNodeSettings)
        {
            InitializeComponent();
            this.dynamoNodeSettings = DynamoNodeSettings;
            this._ITitleText                      = (SolidColorBrush)dynamoNodeSettings["headerForegroundInactive"];
            this._ITitleBackground                = (SolidColorBrush)dynamoNodeSettings["headerBackgroundInactive"];
            this._ITitleBorder                    = (SolidColorBrush)dynamoNodeSettings["headerBorderInactive"];
            this._IBodyBackground                 = (SolidColorBrush)dynamoNodeSettings["bodyBackgroundInactive"];
            this._IBodyBorder                     = (SolidColorBrush)dynamoNodeSettings["outerBorderInactive"];
                                                  
            this._ATitleText                      = (SolidColorBrush)dynamoNodeSettings["headerForegroundActive"];
            this._ATitleBackground                = (SolidColorBrush)dynamoNodeSettings["headerBackgroundActive"];
            this._ATitleBorder                    = (SolidColorBrush)dynamoNodeSettings["headerBorderActive"];
            this._ABodyBackground                 = (SolidColorBrush)dynamoNodeSettings["bodyBackgroundActive"];
            this._ABodyBorder                     = (SolidColorBrush)dynamoNodeSettings["outerBorderActive"];
                                                  
            this._WTitleText                      = (SolidColorBrush)dynamoNodeSettings["headerForegroundWarning"];
            this._WTitleBackground                = (SolidColorBrush)dynamoNodeSettings["headerBackgroundWarning"];
            this._WTitleBorder                    = (SolidColorBrush)dynamoNodeSettings["headerBorderWarning"];
            this._WBodyBackground                 = (SolidColorBrush)dynamoNodeSettings["bodyBackgroundWarning"];
            this._WBodyBorder                     = (SolidColorBrush)dynamoNodeSettings["outerBorderWarning"];
                                                  
            this._ETitleText                      = (SolidColorBrush)dynamoNodeSettings["headerForegroundError"];
            this._ETitleBackground                = (SolidColorBrush)dynamoNodeSettings["headerBackgroundError"];
            this._ETitleBorder                    = (SolidColorBrush)dynamoNodeSettings["headerBorderError"];
            this._EBodyBackground                 = (SolidColorBrush)dynamoNodeSettings["bodyBackgroundError"];
            this._EBodyBorder                     = (SolidColorBrush)dynamoNodeSettings["outerBorderError"];

            this.ITitleText.Background            = _ITitleText;
            this.ITitleBackground.Background      = _ITitleBackground;
            this.ITitleBorder.Background          = _ITitleBorder;
            this.IBodyBackground.Background       = _IBodyBackground;
            this.IBodyBorder.Background           = _IBodyBorder;

            this.ATitleText.Background            = _ATitleText;
            this.ATitleBackground.Background      = _ATitleBackground;
            this.ATitleBorder.Background          = _ATitleBorder;
            this.ABodyBackground.Background       = _ABodyBackground;
            this.ABodyBorder.Background           = _ABodyBorder;

            this.WTitleText.Background            = _WTitleText;
            this.WTitleBackground.Background      = _WTitleBackground;
            this.WTitleBorder.Background          = _WTitleBorder;
            this.WBodyBackground.Background       = _WBodyBackground;
            this.WBodyBorder.Background           = _WBodyBorder;

            this.ETitleText.Background            = _ETitleText;
            this.ETitleBackground.Background      = _ETitleBackground;
            this.ETitleBorder.Background          = _ETitleBorder;
            this.EBodyBackground.Background       = _EBodyBackground;
            this.EBodyBorder.Background           = _EBodyBorder;
        }


        /// <summary>
        /// This event gets called when pressing a Color Button in the Change Node Color Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeColorButton_Click(object sender, RoutedEventArgs e)
        {
            Button changeColorButton = (Button)sender;
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                System.Windows.Media.Color newColor = new Color()
                {
                    A = color.A,
                    R = color.R,
                    G = color.G,
                    B = color.B
                };
                changeColorButton.Background = new SolidColorBrush(newColor);
            }
        }

        /// <summary>
        /// This Cancels and Closes the Change node Color Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// This will open a File Save Dialog. The user can save the Template somewhere easy to find for later copy pasting in the template folder. Use Open_Click to get the template folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {

            //Give a Warning 
            if(Forms.MessageBox.Show("Always make a Backup of your current settings.\n Are you sure you want to Contiue?", "Save Template", Forms.MessageBoxButtons.OKCancel) == Forms.DialogResult.OK)
            {
                // The chosen colors to the properties
                applyColors();

                // Get the Source path of the resource dictionary
                string filePath = dynamoNodeSettings.Source.AbsolutePath;

                // Create a Save file Dialog
                System.Windows.Forms.SaveFileDialog fileDialog = new Forms.SaveFileDialog();

                // Set the Right file name for the Template file
                fileDialog.FileName = Path.GetFileName(filePath.Replace("%20", " "));
                fileDialog.DefaultExt = "*.xaml";

                // Show dialog
                if (fileDialog.ShowDialog() == Forms.DialogResult.OK)
                {
                    // Make a XML Write to write the data to the saved file
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    XmlWriter writer = XmlWriter.Create(fileDialog.FileName, settings);
                    XamlWriter.Save(dynamoNodeSettings, writer);

                    // Show instructions
                    Forms.MessageBox.Show("Copy and Paste the File in the 'Dynamo Themes Directory'", "Restart Dynamo");
                }
                this.Close();
            }
            else
            {
                this.Close();
            }
        }
        
        /// <summary>
        /// Applies the chose colors to the Resource dictionary file
        /// </summary>
        private void applyColors()
        {
            dynamoNodeSettings["headerForegroundInactive"] = this.ITitleText.Background;     
            dynamoNodeSettings["headerBackgroundInactive"] = this.ITitleBackground.Background;    
            dynamoNodeSettings["headerBorderInactive"] = this.ITitleBorder.Background;     
            dynamoNodeSettings["bodyBackgroundInactive"] = this.IBodyBackground.Background;       
            dynamoNodeSettings["outerBorderInactive"] = this.IBodyBorder.Background;      
                                                                  
            dynamoNodeSettings["headerForegroundActive"] = this.ATitleText.Background;
            dynamoNodeSettings["headerBackgroundActive"] = this.ATitleBackground.Background;
            dynamoNodeSettings["headerBorderActive"] = this.ATitleBorder.Background;
            dynamoNodeSettings["bodyBackgroundActive"] = this.ABodyBackground.Background;
            dynamoNodeSettings["outerBorderActive"]  = this.ABodyBorder.Background;
                                                                   
            dynamoNodeSettings["headerForegroundWarning"] = this.WTitleText.Background;
            dynamoNodeSettings["headerBackgroundWarning"] = this.WTitleBackground.Background;
            dynamoNodeSettings["headerBorderWarning"] = this.WTitleBorder.Background;
            dynamoNodeSettings["bodyBackgroundWarning"] = this.WBodyBackground.Background;
            dynamoNodeSettings["outerBorderWarning"] = this.WBodyBorder.Background;   
                                                                 
            dynamoNodeSettings["headerForegroundError"]          = this.ETitleText.Background;    
            dynamoNodeSettings["headerBackgroundError"]          = this.ETitleBackground.Background;    
            dynamoNodeSettings["headerBorderError"]              = this.ETitleBorder.Background;   
            dynamoNodeSettings["bodyBackgroundError"]            = this.EBodyBackground.Background;    
            dynamoNodeSettings["outerBorderError"]               = this.EBodyBorder.Background;
        }


        /// <summary>
        /// Give a Messagebox with instructions on how to use this function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HowToUse_Click(object sender, RoutedEventArgs args)
        {
            string helpMessage = "HOW TO USE 'CHANGE NODE COLORS':" +
                "\n" +
                "\n" +
                "1. Adjust the Node Colors to your liking." +
                "\n" +
                "\n" +
                "2. Click 'Save Template'" +
                "\n" +
                "\n" +
                "3. Save the File somewhere using the given name: 'DynamoColorsAndBrushes.xaml'" +
                "\n" +
                "\n" +
                "4. Browse to C:\\Program Files\\Dynamo\\Dynamo Core\\1.3\\UI\\Themes\\Modern or C:\\Program Files\\Dynamo\\Dynamo Core\\2.0\\UI\\Themes\\Modern" +
                "\n" +
                "\n" +
                "5. Copy and Paste the Template file there to override the Current Settings" +
                "\n" +
                "\n" +
                "6. Restart Dynamo" +
                "\n" +
                "\n" +
                "ENJOY!";

            Forms.MessageBox.Show(helpMessage, "How to use Change Node Color");
        }

        /// <summary>
        /// Opens the directory where the Template files have to be stored.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            string path = Path.GetDirectoryName(dynamoNodeSettings.Source.AbsolutePath.Replace("%20", " "));
            System.Diagnostics.Process.Start(path);
        }
    }
} 