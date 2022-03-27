using Core.Infrastructure.Controls;
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

namespace FlowDesigner
{
    /// <summary>
    /// Interaction logic for NavigationBehaviorEdit.xaml
    /// </summary>
    public partial class NavigationBehaviorEdit : Window
    {
        public NavigationBehaviorEdit(TextBox edit)
        {
            InitializeComponent();
            this.Editor = edit;
            lst.Items.Clear();
            foreach (var item in Enum.GetNames(typeof(NavigationBehavior)))
            {
                lst.Items.Add(item);
            }
            lst.SelectedItem = lst.Items.OfType<string>().FirstOrDefault(a => a == Editor.Text);
        }


        public TextBox Editor { get; set; }
        private void okbutton_Click(object sender, RoutedEventArgs e)
        {
            if (lst.SelectedItem != null)
            {
                Editor.Text = lst.SelectedItem.ToString();
                Editor.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
            this.Hide();
        }

        private void cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
