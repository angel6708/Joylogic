using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using System.Diagnostics;
using Core.Infrastructure.Services;

namespace Core.Infrastructure.Workflow
{
    [DataContract]
    public class ConditionActivity : Activity
    {
        [Browsable(false)]
        [DataMember]
        public Activity TrueActivity { get; set; }
        [Browsable(false)]
        [DataMember]
        public Activity FalseActivity { get; set; }

        private IConditionSession _condition;
        private IConditionSession Condition
        {
            get
            {
                if (_condition == null)
                {
                    if (string.IsNullOrEmpty(ConditionSessionName)) { return null; }
                    _condition = ContainerService.Current.GetInstance(Type.GetType(ConditionSessionName)) as IConditionSession;

                }
                return _condition;
            }

        }

        private string _session;
        [DataMember]
        public string ConditionSessionName
        {
            get { return _session; }
            set
            {
                _session = value;
                OnPropertyChanged("ConditionSessionName");
            }
        }

        public override void Start(bool fromCancel = false)
        {
            this.IsProcessing = true;
            Debug.WriteLine($"{this.Text}  starting at :  {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff")},fromCancel:{fromCancel} ");
            Debug.WriteLine(string.Format("ConditionSessionName:{0}", this.ConditionSessionName));

            var activity = this.TrueActivity;
            bool? condition = Condition?.Condition();
            if (condition.GetValueOrDefault() == false)
            {
                Debug.WriteLine(string.Format("Condition:{0}", false));
                activity = this.FalseActivity;
            }
            else
            {
                Debug.WriteLine(string.Format("Condition:{0}", true));
            }
            if (activity != null)
            {
                activity.Start(fromCancel);

            }
            else
            {
                if (fromCancel) { this.Cancel(); }
                else { this.Finish(); }
            }
        }

        public override void Init()
        {
            base.Init();
            if (this.TrueActivity != null)
            {

                TrueActivity.Init();
            }
            if (this.FalseActivity != null)
            {
                FalseActivity.Init();
            }
        }


        public override void Config()
        {
            if (TrueActivity == null) { throw new Exception(" "); }
            if (Condition == null) { throw new Exception(" "); }

            TrueActivity.CancelEvent.ActivityEvent = this.Cancel;
            TrueActivity.FinishEvent.ActivityEvent = this.Finish;
            TrueActivity.Config();
            if (FalseActivity != null)
            {
                FalseActivity.CancelEvent.ActivityEvent = this.Cancel;
                FalseActivity.FinishEvent.ActivityEvent = this.Finish;
                FalseActivity.Config();
            }

        }


        private ConditionActivity(NodeKinds k)
            : base(k)
        {
        }

        public ConditionActivity()
            : this(NodeKinds.Condition)
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

            leftg.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20) });
            leftg.RowDefinitions.Add(new RowDefinition());
            leftg.Children.Add(new TextBlock() { Text = "是", HorizontalAlignment = HorizontalAlignment.Center });

            Border leftb = new Border();
            leftb.BorderBrush = Brushes.LightGray;
            leftb.BorderThickness = new Thickness(1);
            leftb.SetValue(Grid.RowProperty, 1);
            leftb.Background = Brushes.Green;
            leftb.Tag = "是";
            leftb.Drop += Drop;
            leftb.DragEnter += DragEnter;
            leftb.DragLeave += DragLeave;
            leftb.ContextMenu = GetContextMenu(leftb, true);
            if (TrueActivity != null)
            {

                if (TrueActivity.IsDelete)
                {
                    TrueActivity = null;
                }
                else
                {

                    if (UpdateContent != null)
                    {
                        leftb.Child = UpdateContent(TrueActivity, true, true, false);
                    }

                }

            }

            leftg.Children.Add(leftb);
            leftg.SetValue(Grid.RowProperty, 1);


            Grid rightg = new Grid();

            rightg.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20) });
            rightg.RowDefinitions.Add(new RowDefinition());
            rightg.Children.Add(new TextBlock() { Text = "否", HorizontalAlignment = HorizontalAlignment.Center });

            Border rightb = new Border();
            rightb.BorderBrush = Brushes.LightGray;
            rightb.BorderThickness = new Thickness(1);
            rightb.SetValue(Grid.RowProperty, 1);
            rightb.Tag = "否";
            rightb.Background = Brushes.Green;
            rightb.Drop += Drop;
            rightb.DragEnter += DragEnter;
            rightb.DragLeave += DragLeave;
            rightb.ContextMenu = GetContextMenu(rightb, false);

            if (FalseActivity != null)
            {

                if (FalseActivity.IsDelete)
                {
                    FalseActivity = null;
                }
                else
                {

                    if (UpdateContent != null)
                    {
                        var item = UpdateContent(FalseActivity, true, true, false);

                        rightb.Child = item;
                    }
                }

            }

            rightg.Children.Add(rightb);


            rightg.SetValue(Grid.RowProperty, 1);
            rightg.SetValue(Grid.ColumnProperty, 1);

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            grid.Children.Add(textBlock);
            textBlock.SetValue(Grid.ColumnSpanProperty, 2);

            grid.Children.Add(leftg);
            grid.Children.Add(rightg);
            if (_theUi == null)
            {
                _theUi = new Border();
                _theUi.BorderBrush = Brushes.Black;
                _theUi.BorderThickness = new Thickness(1);
                _theUi.Background = Brushes.Lime;
            }

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

            _theUi.Child = grid;

            if (!ActivityGlobalConfig.IsEditMode)
            {
                var menu = new ContextMenu();
                var mi = new MenuItem();
                mi.Header = "Start Here";
                mi.Click += (s, e) => { this.Start(true); };
                menu.Items.Add(mi);
                _theUi.ContextMenu = menu;
            }
            return _theUi;
        }

        private Border _theUi;


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

                if (border.Tag.ToString() == "是")
                {
                    this.TrueActivity = node;
                }
                else
                {
                    this.FalseActivity = node;
                }

                border.Background = Brushes.Green;
            }
            e.Handled = true;
        }

        private ContextMenu GetContextMenu(Border border, bool isTrue)
        {
            if (!ActivityGlobalConfig.IsEditMode) { return null; }
            var contextMenu = new ContextMenu();
            foreach (NodeKinds kind in Enum.GetValues(typeof(NodeKinds)))
            {
                if (kind == NodeKinds.Start || kind == NodeKinds.End) { continue; }
                var item = new MenuItem() { Header = "Add " + kind };
                item.Tag = kind;
                item.Click += (s, e) => { CreateInnerActivity(border, (NodeKinds)(s as MenuItem).Tag, isTrue); };
                contextMenu.Items.Add(item);

            }
            return contextMenu;
        }

        protected void CreateInnerActivity(Border conntainer, NodeKinds nodeKind, bool isTrue)
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

            if (isTrue) this.TrueActivity = node;
            else this.FalseActivity = node;
            border.Background = Brushes.Green;
        }
    }
}
