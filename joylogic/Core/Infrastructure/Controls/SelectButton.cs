using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Core.Infrastructure.Controls
{
    public class SelectButton : Button
    {
        public static readonly DependencyProperty HasSelectedProperty =
            DependencyProperty.Register("HasSelected", typeof(bool), typeof(SelectButton), new PropertyMetadata(false));

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
