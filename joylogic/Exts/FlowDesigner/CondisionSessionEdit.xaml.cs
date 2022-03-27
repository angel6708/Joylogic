
using Core.Infrastructure.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class CondisionSessionEdit : Window
    {
        public CondisionSessionEdit(TextBox edit)
        {
            InitializeComponent();

            this.Editor = edit;
            lst.Items.Clear();


            var regableType = typeof(IConditionSession);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach(var assembly in assemblies)
            { 
                foreach (var t in assembly.DefinedTypes)
                {
                    if (t.IsInterface && t.ImplementedInterfaces.Contains(regableType))
                    {
                        var attrs = t.GetCustomAttributes(false);
                        var attr = attrs.FirstOrDefault() as DescriptionAttribute;
                        string name = t.Name;
                        if (attr != null)
                        {
                            name = attr.Description;
                        } 
                        var model = new Model { Name = name, Value = t };
                        lst.Items.Add(model);
                        models.Add(model);
                    }
                }
            }
            lst.SelectedItem = lst.Items.OfType<Model>().FirstOrDefault(a => a.Value.AssemblyQualifiedName == Editor.Text);
        }

        class Model
        {
            public string Name { get; set; }
            public Type Value { get; set; }
        }


        public TextBox Editor { get; set; }
        private void okbutton_Click(object sender, RoutedEventArgs e)
        {
            if (lst.SelectedItem != null)
            {
                var model = lst.SelectedItem as Model;
                Editor.Text = model.Value.AssemblyQualifiedName;
                Editor.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            }
            this.Hide();
        }

        private void cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
        private List<Model> models = new List<Model>();
        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filtered = models;

            if (!string.IsNullOrEmpty(search.Text))
            {
                filtered = new List<Model>(models.Where(a => a.Name.Contains(search.Text) || a.Value.AssemblyQualifiedName.Contains(search.Text)));
            }
            lst.Items.Clear();
            foreach (var item in filtered)
            {
                lst.Items.Add(item);
            }

            lst.SelectedItem = lst.Items.OfType<Model>().FirstOrDefault(a => a.Value.AssemblyQualifiedName == Editor.Text);

        }
    }
}
