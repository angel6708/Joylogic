using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aga.Diagrams.Tools;
using Aga.Diagrams;
using System.Windows;
using FlowViewModule.Flowchart.Model;
using Core.Infrastructure.Workflow;

namespace FlowViewModule.Flowchart
{
    class DragDropTool : IDragDropTool
    {
        DiagramView _view;
        FlowchartModel _model;


        public DragDropTool(DiagramView view, FlowchartModel model)
        {
            _view = view;
            _model = model;
        }

        public void OnDragEnter(System.Windows.DragEventArgs e)
        {
        }


        public void OnDragOver(System.Windows.DragEventArgs e)
        {
           
            e.Handled = true;
        }

        public void OnDragLeave(System.Windows.DragEventArgs e)
        {
        }

        public void OnDrop(System.Windows.DragEventArgs e)
        {
            
             
            e.Handled = true;
        }
    }
}
