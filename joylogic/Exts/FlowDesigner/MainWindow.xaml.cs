using Core.Infrastructure.Services;
using Core.Infrastructure.Workflow;
using FlowDesigner.Flowchart;
using FlowDesigner.Flowchart.Model;
using Microsoft.Win32;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
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
	public partial class MainWindow : Window
	{
        private ItemsControlDragHelper _dragHelper;
        private FlowchartModel _model;
        private Stack<string> NavStack = new Stack<string>();
        private string _path = null;
        public MainWindow()
		{
			InitializeComponent();
            var evt = ContainerService.Current.GetInstance<IEventAggregator>().GetEvent<PubSubEvent<Tuple<ComposedActivity>>>();
            if (evt != null) { evt.Subscribe(GoIntoEventHandler); }

            _model = new FlowchartModel();
            if (_editor != null)
            {
                _editor.Controller = new Controller(_editor, _model);
                _editor.DragDropTool = new DragDropTool(_editor, _model);
                _editor.DragTool = new CustomMoveResizeTool(_editor, _model)
                {
                    MoveGridCell = _editor.GridCellSize
                };
                _editor.LinkTool = new CustomLinkTool(_editor);
                _editor.Selection.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Selection_PropertyChanged);
                if (_toolbox != null)
                {
                    _dragHelper = new ItemsControlDragHelper(_toolbox, this);
                    FillToolbox();
                }
            }
        }
         

        private void FillToolbox()
        {
            foreach (NodeKinds nk in Enum.GetValues(typeof(NodeKinds)))
            {
                var node = ActivityHelper.Create(nk);
                node.Text = nk.ToString();
                var ui = node.CreateContent();
                ui.Width = 60;
                ui.Height = 30;
                ui.Margin = new Thickness(5);
                ui.Tag = nk;
                _toolbox.Items.Add(ui);
            }
        }



        void Selection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var p = _editor.Selection.Primary;
            _propertiesView.SelectedObject = p != null ? p.ModelElement : null;
        }


        private ComposedActivity Activity { get; set; }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_path)) { SaveAs(sender, e); return;  }
            ComposedActivity activity = new ComposedActivity();
            activity.Name = activityname.Text;

            activity.Activities = new List<Activity>(_model.Nodes);
            activity.Links = new List<Link>(_model.Links);

            var file = System.IO.Path.Combine(_path, activityname.Text+".xml");
            if(SaveFile(file, activity))
            foreach (var node in _model.Nodes) { node.NeedToSave = false; }

        }

        public void SaveAs(object sender, RoutedEventArgs e) 
        {
            SaveFileDialog dig = new SaveFileDialog();
            dig.DefaultExt = "xml";
            bool ret = dig.ShowDialog().GetValueOrDefault();
            if (ret)
            {
                var file = dig.FileName;
                var name = System.IO.Path.GetFileNameWithoutExtension(file);
                _path = System.IO.Path.GetDirectoryName(file);
                ComposedActivity activity = new ComposedActivity();
                activity.Name = name ;
                activityname.Text = name;
                activity.Activities = new List<Activity>(_model.Nodes);
                activity.Links = new List<Link>(_model.Links);


                if (SaveFile(file, activity))
                    foreach (var node in _model.Nodes) { node.NeedToSave = false; }
            }
        }

        private bool SaveFile(string file, ComposedActivity activity)
        {
            try
            {
               
                var dir = System.IO.Path.GetDirectoryName(file);
                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }

                if (activity.Activities == null) { activity.Activities = new List<Activity>(); }
                if (activity.Links == null) { activity.Links = new List<Link>(); }
                activity.Init();
                activity.Config();
                NetDataContractSerializer serializer = new NetDataContractSerializer();
                MemoryStream ms = new MemoryStream();
                StreamReader streamReader = new StreamReader(ms);
                serializer.WriteObject(ms, activity);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                string xml = streamReader.ReadToEnd();
                ms.Close();

                StreamWriter save = new StreamWriter(file, false, Encoding.UTF8);
                save.WriteLine(xml);
                save.Close();
                activity.NeedToSave = false;
                return true;
            }catch(Exception ex){ MessageBox.Show(ex.ToString());  }
            return false;
        }

        private void Open(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = "xml";
            bool ret = dialog.ShowDialog().GetValueOrDefault(); 
            if (ret)
            {
                _path = System.IO.Path.GetDirectoryName(dialog.FileName);
                var name = System.IO.Path.GetFileName(dialog.FileName).Replace(".xml", "");
                NavStack.Clear(); 
                Load(name);
            }
 

        }

        private void Back(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        public void AddActivity(object sender, RoutedEventArgs e) 
        {
            var mi = sender as MenuItem;
            var node = ActivityHelper.Create((NodeKinds)mi.Tag );
            node.Size = new Size(100, 50);
            var position = Mouse.GetPosition(_editor);
            node.Location = position;
            _model.Nodes.Add(node);
        }

        private void LoadAssembly(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            bool ret = dialog.ShowDialog().GetValueOrDefault();
            if (ret)
            {
                var path = dialog.FileName;
                var loader = ContainerService.Current.GetInstance<AssemblyLoader>();
                loader.LoadFromFile(path);
            }
        }

        private void GoIntoEventHandler(Tuple<ComposedActivity> obj)
        {
            var name = obj.Item1?.Name;
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please name this Activity and save current doc."); return;
            }
           
            foreach (var act in _model.Nodes)
            {
                if (act.NeedToSave) { MessageBox.Show("Please Save current doc before leave."); return; }
            }
            var file = System.IO.Path.Combine(_path, name + ".xml");
            if (!File.Exists(file))
            {
                var res = MessageBox.Show("SubActivity file not Exist. Do you want to create?", "NeetSave", MessageBoxButton.YesNo);

                if (res == MessageBoxResult.Yes)
                {
                    var act = obj.Item1; 
                    SaveFile(file, act);
                }
            }
            NavStack.Push(activityname.Text);
            if (!Load(name))
            {
                GoBack();
            }
        }
       

        public void GoBack()
        {
          
            if (NavStack.Count >= 1)
            {
                foreach (var act in _model.Nodes)
                {
                    if (act.NeedToSave) { MessageBox.Show("Please Save current doc before leave."); return; }
                }

                var name= NavStack.Pop(); 
                Load(name);
            }
        }

        private bool Load(string name)
        {
            try
            { 
                var fullname = System.IO.Path.Combine(_path, name + ".xml");
                StreamReader reader = new StreamReader(fullname, Encoding.UTF8);
                var xml = reader.ReadToEnd();
                reader.Close();

                NetDataContractSerializer serializer = new NetDataContractSerializer()  ;
                MemoryStream ms = new MemoryStream();
                StreamWriter writer = new StreamWriter(ms);
                writer.Write(xml);
                writer.Flush();
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                Object obj = serializer.ReadObject(ms);
                writer.Close(); 
               
                var activiy = obj as ComposedActivity;
                Activity = activiy;
                activiy.Init();
                activiy.Config();
                //Activity.UpdateLinks();
                _model.Links.Clear();
                _model.Nodes.Clear();
                //return;
                foreach (var activity in Activity.Activities)
                {
                    activity.NeedToSave = false;
                    _model.Nodes.Add(activity);
                    activity.UpdateContent = ActivityHelper.UpdateNode;
                }
                foreach (var link in Activity.Links)
                {
                    _model.Links.Add(link);
                }

                activityname.Text = name;
            }
            catch (Exception ex) 
            {
              
                //MessageBox.Show(ex.StackTrace, $"Error:{ex.Message}");
                MessageBox.Show( $"Error:{ex.Message}","Error");
                return false;
            }
            return true;
        }
    }
}
