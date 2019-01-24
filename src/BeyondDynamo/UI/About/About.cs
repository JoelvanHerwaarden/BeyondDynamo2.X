using System;
using System.Drawing;
using System.Windows.Forms;

namespace BeyondDynamo.UI.About
{
    public partial class About : Form
    {
        public bool beingDragged { get; set; }
        public int mouseDownX { get; set; }
        public int mouseDownY { get; set; }

        public About()
        {
            InitializeComponent();
        }
        

        #region LinkedIn Label Events
        private void LinkedIn_Label_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(LinkedIn_Label.Text);
        }

        private void LinkedIn_Label_MouseEnter(object sender, EventArgs e)
        {
            LinkedIn_Label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFC8C8C8");
        }

        private void LinkedIn_Label_MouseLeave(object sender, EventArgs e)
        {
            LinkedIn_Label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
        }
        #endregion
                
        #region About window drag functionality
        private void ApplicationUI_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                beingDragged = true;
                mouseDownX = e.X;
                mouseDownY = e.Y;
            }

        }
        private void ApplicationUI_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                beingDragged = false;
            }
        }
        private void ApplicationUI_MouseMove(object sender, MouseEventArgs e)
        {
            if (beingDragged)
            {
                Point position = new Point();
                position.X = Location.X + (e.X - mouseDownX);
                position.Y = Location.Y + (e.Y - mouseDownY);
                Location = position;
            }
        }
        #endregion
        
    }
}
