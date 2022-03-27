using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Resources.usercontrol
{
    public class HintTextBox:TextBox
    {
        public HintTextBox() 
        {
           DefaultStyleKey = typeof(HintTextBox);
            
        }

        public static DependencyProperty HintProperty =
         DependencyProperty.Register("Hint", typeof(string), typeof(HintTextBox), new UIPropertyMetadata("用户名"));
        public string Hint
        {
            get { return (string)GetValue(HintProperty); }

            set { SetValue(HintProperty, value);
            }
            

        }

    }
}
