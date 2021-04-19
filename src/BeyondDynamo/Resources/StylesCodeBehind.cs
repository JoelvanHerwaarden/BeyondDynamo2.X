using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BeyondDynamo.UI
{
    public partial class StylesCodeBehind
    {
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
    }
}
