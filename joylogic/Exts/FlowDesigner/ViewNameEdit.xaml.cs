using Core.Infrastructure.Consts;
using Core.Infrastructure.Controls;
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
    public partial class ViewNameEdit : Window
    {
        object selectedObject;
        public ViewNameEdit(TextBox edit, object obj)
        {
            InitializeComponent();
            this.selectedObject = obj;
            this.Editor = edit;
            lst.Items.Clear();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var baseType = typeof(BaseView);
            var caseType = typeof(ICaseView);
            var isCaseView = obj.GetType() == typeof(CaseViewActivity);
            foreach (var assebly in assemblies)
            {
                foreach (var t in assebly.DefinedTypes)
                {
                    bool sign = caseType.IsAssignableFrom(t);
                    if (!isCaseView) { sign = !sign; }
                    if (t != baseType && baseType.IsAssignableFrom(t) && sign)
                    {
                        var attrs = t.GetCustomAttributes(false);
                        var attr = attrs.FirstOrDefault() as DescriptionAttribute;
                        string name = t.Name;
                        if (attr != null)
                        {
                            name = attr.Description;
                        }
                        var model = new Model { Name = name, Value = t.Name, Type = t };
                        lst.Items.Add(model);
                        models.Add(model);
                    }
                }
            }

            lst.SelectedItem = lst.Items.OfType<Model>().FirstOrDefault(a => a.Value == edit.Text);


        }

        private List<Model> models = new List<Model>();

        class Model
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public Type Type { get; set; }
        }


        public TextBox Editor { get; set; }
        private void okbutton_Click(object sender, RoutedEventArgs e)
        {

            if (lst.SelectedItem != null)
            {
                var model = lst.SelectedItem as Model;
                Editor.Text = model.Value as string;
                Editor.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                var isCaseView = selectedObject.GetType() == typeof(CaseViewActivity);
                if (isCaseView)
                {

                    var viewAct = selectedObject as CaseViewActivity;
                    var viewType = model.Type;
                    var baseType = typeof(ICaseView);
                    foreach (var t in viewType.GetInterfaces())
                    {
                        if (t.IsGenericType && baseType.IsAssignableFrom(t))
                        {
                            
                            viewAct.CaseSessionName = t.GetGenericArguments()[0].AssemblyQualifiedName;
                        }
                    }

                }
            }


            this.Hide();
        }

        private void cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filtered = models;

            if (!string.IsNullOrEmpty(search.Text))
            {
                filtered = new List<Model>(models.Where(a => a.Name.Contains(search.Text) || a.Value.Contains(search.Text)));
            }
            lst.Items.Clear();
            foreach (var item in filtered)
            {
                lst.Items.Add(item);
            }

            lst.SelectedItem = lst.Items.OfType<Model>().FirstOrDefault(a => a.Value == Editor.Text);

        }
    }
}
