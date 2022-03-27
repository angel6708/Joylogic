using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aga.Diagrams.Tools;
using Aga.Diagrams;
using System.Windows;
using FlowDesigner.Flowchart.Model;
using Core.Infrastructure.Workflow;

namespace FlowDesigner.Flowchart
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
            e.Effects = DragDropEffects.None;
            if (e.Data.GetDataPresent(typeof(NodeKinds)))
            {
                e.Effects = e.AllowedEffects;
            }
            e.Handled = true;
        }

        public void OnDragLeave(System.Windows.DragEventArgs e)
        {
        }

        public void OnDrop(System.Windows.DragEventArgs e)
        {
            
            var node = ActivityHelper.Create((NodeKinds)e.Data.GetData(typeof(NodeKinds)));
            node.Size = new Size(100, 50);
            var position = e.GetPosition(_view);
            node.Location = position;
            _model.Nodes.Add(node);
            e.Handled = true;
        }
    }
}
