using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aga.Diagrams;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using Aga.Diagrams.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;
using FlowDesigner.Flowchart.Model;
using Core.Infrastructure.Workflow;

namespace FlowDesigner.Flowchart
{
    class Controller : IDiagramController
    {
        private class UpdateScope : IDisposable
        {
            private Controller _parent;
            public bool IsInprogress { get; set; }

            public UpdateScope(Controller parent)
            {
                _parent = parent;
            }

            public void Dispose()
            {
                IsInprogress = false;
                _parent.UpdateView();
            }
        }

        private DiagramView _view;
        private FlowchartModel _model;
        private UpdateScope _updateScope;

        public Controller(DiagramView view, FlowchartModel model)
        {
            _view = view;
            _model = model;
            _model.Nodes.CollectionChanged += NodesCollectionChanged;
            _model.Links.CollectionChanged += LinksCollectionChanged;
            _updateScope = new UpdateScope(this);

            foreach (var t in _model.Nodes)
                t.PropertyChanged += NodePropertyChanged;

            UpdateView();
        }

        void NodesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (var t in e.NewItems.OfType<INotifyPropertyChanged>())
                    t.PropertyChanged += NodePropertyChanged;

            if (e.OldItems != null)
                foreach (var t in e.OldItems.OfType<INotifyPropertyChanged>())
                    t.PropertyChanged -= NodePropertyChanged;
            UpdateView();
        }

        void LinksCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateView();
        }

        void NodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var fn = sender as Activity;
            var n = _view.Children.OfType<Node>().FirstOrDefault(p => p.ModelElement == fn);
            if (fn != null && n != null)
                //UpdateNode(fn, n);
                ActivityHelper.UpdateNode(fn, false);
        }

        private void UpdateView()
        {
            if (!_updateScope.IsInprogress)
            {
                _view.Children.Clear();

                foreach (var node in _model.Nodes)
                    _view.Children.Add(ActivityHelper.UpdateNode(node, true));

                foreach (var link in _model.Links)
                    _view.Children.Add(CreateLink(link));


            }
        }



        private Control CreateLink(Link link)
        {
            // var item = new OrthogonalLink();
            var item = new MyLink();
            item.ModelElement = link;
            item.EndCap = true;
            item.Source = link.Source.FindPort(_view, link.SourcePort);
            item.Target = link.Target.FindPort(_view, link.TargetPort);

            var b = new Binding("Text");
            b.Source = link;
            item.SetBinding(LinkBase.LabelProperty, b);

            return item;
        }



        private void DeleteSelection()
        {
            using (BeginUpdate())
            {
                var nodes = _view.Selection.Select(p => p.ModelElement as Activity).Where(p => p != null);
                foreach (var n in nodes)
                {
                    n.IsDelete = true;
                }
                var links = _view.Selection.Select(p => p.ModelElement as Link).Where(p => p != null);
                _model.Nodes.RemoveRange(p => nodes.Contains(p));
                _model.Links.RemoveRange(p => links.Contains(p));
                _model.Links.RemoveRange(p => nodes.Contains(p.Source) || nodes.Contains(p.Target));
            }
        }

        private IDisposable BeginUpdate()
        {
            _updateScope.IsInprogress = true;
            return _updateScope;
        }

        #region IDiagramController Members

        public void UpdateItemsBounds(Aga.Diagrams.Controls.DiagramItem[] items, Rect[] bounds)
        {
            for (int i = 0; i < items.Length; i++)
            {
                var node = items[i].ModelElement as Activity;
                if (node != null)
                {
                    node.Location = bounds[i].Location;
                    node.Size = bounds[i].Size;

                }

            }
        }

        public void UpdateLink(LinkInfo initialState, Aga.Diagrams.Controls.ILink link)
        {
            using (BeginUpdate())
            {
                var sourcePort = link.Source as PortBase;
                var source = VisualHelper.FindParent<Node>(sourcePort);
                var targetPort = link.Target as PortBase;
                var target = VisualHelper.FindParent<Node>(targetPort);

                _model.Links.Remove((link as LinkBase).ModelElement as Link);
                if (target != null && source != null)
                {
                    _model.Links.Add(
                        new Link((Activity)source.ModelElement, (PortKinds)sourcePort.Tag,
                            (Activity)target.ModelElement, (PortKinds)targetPort.Tag)
                            );
                }




            }
        }

        public void ExecuteCommand(System.Windows.Input.ICommand command, object parameter)
        {
            if (command == ApplicationCommands.Delete)
                DeleteSelection();
        }

        public bool CanExecuteCommand(System.Windows.Input.ICommand command, object parameter)
        {
            if (command == ApplicationCommands.Delete)
                return true;
            else
                return false;
        }

        #endregion
    }
}
