using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Core.Infrastructure.Controls
{
    public class ToolBarButton : RadioButton
    {
        public ToolBarButton()
        {
            this.Click += ToolBar_Checked;
            this.Unchecked += ToolBar_Unchecked;
        }

        private void ToolBar_Checked(object sender, RoutedEventArgs e)
        {
            if (IsSelectedDifferent)
            {
                this.HasSelected = true;
            }

            this.IsChecked = true;
        }

        private void ToolBar_Unchecked(object sender, RoutedEventArgs e)
        {
            this.HasSelected = false;
        }



        public static readonly DependencyProperty IsSelectedDifferentProperty =
           DependencyProperty.Register("IsSelectedDifferent", typeof(bool), typeof(ToolBarButton), new PropertyMetadata(false));

        public bool IsSelectedDifferent
        {
            get
            {
                return (bool)GetValue(IsSelectedDifferentProperty);
            }
            set
            {
                this.SetValue(IsSelectedDifferentProperty, value);
            }
        }

        public static readonly DependencyProperty HasSelectedProperty =
            DependencyProperty.Register("HasSelected", typeof(bool), typeof(ToolBarButton), new PropertyMetadata(false));

        public bool HasSelected
        {
            get
            {
                return (bool)GetValue(HasSelectedProperty);
            }
            set
            {
                this.SetValue(HasSelectedProperty, value);
            }
        }

      
    }
}
