using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using System.Diagnostics;
using Core.Infrastructure.Services;

namespace Core.Infrastructure.Workflow
{
    [DataContract]
    public class ForeachActivity : Activity
    {
        private IForeachSession _session = null;
        private bool _isChildCanceled = false;
        private IForeachSession ForeachSession
        {
            get
            {
                if (_session == null)
                {
                    if (string.IsNullOrEmpty(ForeachSessionName)) { return null; }
                    _session = ContainerService.Current.GetInstance(Type.GetType(ForeachSessionName)) as IForeachSession;

                }
                return _session;
            }
        }
        [DataMember]
        [Browsable(false)]
        public Activity InnerActivity { get; set; }

        [DataMember]
        public string ForeachSessionName { get; set; }


        public override void Start(bool fromCancel = false)
        {
            this.IsProcessing = true;
            Debug.WriteLine($"{this.Text}  starting at :  {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff")},fromCancel:{fromCancel} ");
            Debug.WriteLine(string.Format("ForeachSessionName：{0}", this.ForeachSessionName));
            var foreachInfo = new ForeachInfo();
            //foreachInfo.TotalCount = ForeachSession.ForeachEnumerator.ForeachList.Count;
            //foreachInfo.CurrentCount = 0;
            ForeachInfoStack.Instance.Push(foreachInfo);

            Continue();
        }

        private void InnerStart()
        {
            if (InnerActivity != null)
            {
                InnerActivity.Start();

            }
        }
        public override void Init()
        {
            base.Init();
            if (InnerActivity != null)
            {
                InnerActivity.Init();
            }
        }

        public override void Config()
        {
            if (ForeachSession == null)
            {
                throw new Exception("ForeachEnumerator Must be set");
            }
            if (InnerActivity == null)
            {
                throw new Exception("InnerActivity Must be set");
            }
            InnerActivity.Config();
            InnerActivity.AddBefforCancelHandler(() => { _isChildCanceled = true; });
            InnerActivity.CancelEvent.ActivityEvent = this.InnerCancel;
            InnerActivity.FinishEvent.ActivityEvent = this.Continue;

        }


        private void Continue()
        {
            if (_isChildCanceled)
            {
                this.InnerCancel();
            }
            else
            { 
                if (ForeachSession.ForeachEnumerator.MoveNext())
                {
                    Debug.WriteLine(string.Format($"{ForeachSessionName} Moved Next,CurrentCount/TotalCount:{ForeachInfoStack.Instance.CurrentForeachInfo.CurrentCount}/{ForeachInfoStack.Instance.CurrentForeachInfo.TotalCount}" ));
                    this.InnerStart(); 
                }
                else
                {
                    ForeachSession.ForeachEnumerator.Reset();
                    this.Finish();
                }
            }
        }

        public override void Cancel()
        {
            ForeachInfoStack.Instance.Pop();
            base.Cancel();
        }
        public override void Finish()
        {
            ForeachInfoStack.Instance.Pop();
            base.Finish();
        }

        private void InnerCancel()
        {
            _isChildCanceled = false;
            this.Cancel();

        }

        private ForeachActivity(NodeKinds k)
            : base(k)
        {

        }

        public ForeachActivity()
            : this(NodeKinds.Foreach)
        {

        }


        public override FrameworkElement CreateContent()
        {

            var textBlock = new TextBlock()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            var b = new Binding("Text");
            b.Source = this;
            textBlock.SetBinding(TextBlock.TextProperty, b);
            textBlock.VerticalAlignment = VerticalAlignment.Bottom;



            Grid leftg = new Grid();

            leftg.RowDefinitions.Add(new RowDefinition());

            Border leftb = new Border();
            leftb.BorderBrush = Brushes.LightGray;
            leftb.BorderThickness = new Thickness(1);
            leftb.Background = Brushes.Green;
            leftb.Drop += Drop;
            leftb.DragEnter += DragEnter;
            leftb.DragLeave += DragLeave;
            leftb.ContextMenu = GetContextMenu(leftb);

            if (InnerActivity != null)
            {

                if (InnerActivity.IsDelete)
                {
                    InnerActivity = null;
                }
                else
                {

                    if (UpdateContent != null)
                    {
                        leftb.Child = UpdateContent(InnerActivity, true, true, false);
                    }

                }

            }

            leftg.Children.Add(leftb);
            leftg.SetValue(Grid.RowProperty, 1);

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            grid.Children.Add(textBlock);

            grid.Children.Add(leftg);

            Border runStatus = new Border();
            runStatus.Background = Brushes.Blue;
            runStatus.Width = 8;
            runStatus.Height = 8;
            runStatus.CornerRadius = new CornerRadius(4);
            runStatus.BorderThickness = new Thickness(1);
            runStatus.BorderBrush = Brushes.DarkBlue;
            runStatus.HorizontalAlignment = HorizontalAlignment.Left;
            runStatus.VerticalAlignment = VerticalAlignment.Top;
            runStatus.Margin = new Thickness(2);
            Binding bv = new Binding("IsProcessing");
            bv.Converter = new BooleanToVisibilityConverter();
            bv.Source = this;
            runStatus.SetBinding(Border.VisibilityProperty, bv);
            grid.Children.Add(runStatus);

            var ui = new Border();
            ui.BorderBrush = Brushes.Black;
            ui.BorderThickness = new Thickness(1);
            ui.Background = Brushes.Lime;
            ui.Child = grid;

            if (!ActivityGlobalConfig.IsEditMode)
            {
                var menu = new ContextMenu();
                var mi = new MenuItem();
                mi.Header = "Start Here";
                mi.Click += (s, e) => { this.Start(true); };
                menu.Items.Add(mi);
                ui.ContextMenu = menu;
            }
            return ui;
        }

        void item_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        void DragLeave(object sender, DragEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                border.Background = Brushes.Green;
            }
            e.Handled = true;
        }

        void DragEnter(object sender, DragEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                border.Background = Brushes.Lime;
            }
            e.Handled = true;
        }

        void Drop(object sender, DragEventArgs e)
        {

            var border = sender as Border;
            if (border != null)
            {
                var node = Activity.Create((NodeKinds)e.Data.GetData(typeof(NodeKinds)));
                node.UpdateContent = this.UpdateContent;
                node.Size = new Size(100, 50);
                var position = new Point();
                node.Location = position;

                if (UpdateContent != null)
                {
                    border.Child = UpdateContent(node, true, true, false);
                }
                this.InnerActivity = node;


                border.Background = Brushes.Green;
            }
            e.Handled = true;
        }

        private ContextMenu GetContextMenu(Border border)
        {
            if (!ActivityGlobalConfig.IsEditMode) { return null; }
            var contextMenu = new ContextMenu();
            foreach (NodeKinds kind in Enum.GetValues(typeof(NodeKinds)))
            {
                if (kind == NodeKinds.Start || kind == NodeKinds.End) { continue; }
                var item = new MenuItem() { Header = "Add " + kind };
                item.Tag = kind;
                item.Click += (s, e) => { CreateInnerActivity(border, (NodeKinds)(s as MenuItem).Tag); };
                contextMenu.Items.Add(item);

            }
            return contextMenu;
        }

        protected void CreateInnerActivity(Border conntainer, NodeKinds nodeKind)
        {
            var border = conntainer;
            var node = Activity.Create(nodeKind);
            node.UpdateContent = this.UpdateContent;
            node.Size = new Size(100, 50);
            var position = new Point();
            node.Location = position;

            if (UpdateContent != null)
            {
                border.Child = UpdateContent(node, true, true, false);
            }

           this.InnerActivity= node;
            
            border.Background = Brushes.Green;
        }

    }
}
