using Core.Infrastructure.Services;
using Core.Infrastructure.Workflow;
using FlowViewModule.Flowchart;
using FlowViewModule.Flowchart.Model;
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

namespace FlowViewModule
{
	public partial class WorkflowView : Window
	{
        private FlowchartModel _model;
        private Stack<ComposedActivity> NavStack = new Stack<ComposedActivity>();
        public WorkflowView()
		{
            
			InitializeComponent();
            var evt = ContainerService.Current.GetInstance<IEventAggregator>().GetEvent<PubSubEvent<Tuple<ComposedActivity>>>();
            if (evt != null) { evt.Subscribe(GoIntoEventHandler); }

            _model = new FlowchartModel();
            if (_editor != null)
            {
                _editor.Controller = new Controller(_editor, _model);
                _editor.Selection.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Selection_PropertyChanged);
                 
                _editor.DragDropTool = new DragDropTool(_editor, _model);
                _editor.DragTool = new CustomMoveResizeTool(_editor, _model)
                {
                    MoveGridCell = _editor.GridCellSize
                };
                _editor.LinkTool = new CustomLinkTool(_editor);
                
            }
        }
         
         

        void Selection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var p = _editor.Selection.Primary;
            _propertiesView.SelectedObject = p != null ? p.ModelElement : null;
        }


        private ComposedActivity Activity { get; set; }

   

        private void Back(object sender, RoutedEventArgs e)
        {
            GoBack();
        }
         

        private void GoIntoEventHandler(Tuple<ComposedActivity> obj)
        { 
            NavStack.Push(Activity);
            if (!Load(obj.Item1))
            {
                GoBack();
            }
        }
       
        public void GoBack()
        {
          
            if (NavStack.Count >= 1)
            {
               
                var activity= NavStack.Pop(); 
                Load(activity);
            }
        }

        public bool Load(ComposedActivity  activity)
        {
            try
            {   
                Activity = activity;
               
                _model.Links.Clear();
                _model.Nodes.Clear();
                //return;
                foreach (var ac in Activity.Activities)
                { 
                    _model.Nodes.Add(ac);
                    ac.UpdateContent = ActivityHelper.UpdateNode;
                }
                foreach (var link in Activity.Links)
                {
                    _model.Links.Add(link);
                }

                activityname.Text = activity.Name;
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
