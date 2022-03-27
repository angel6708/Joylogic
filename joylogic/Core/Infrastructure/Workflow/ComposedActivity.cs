using Core.Infrastructure.Services;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Core.Infrastructure.Workflow
{
    [DataContract]
    public class ComposedActivity : Activity
    {
        #region Properties

        [Browsable(false)]
        [DataMember]
        public List<Activity> Activities { get; set; }

        [Browsable(false)]
        [DataMember]
        public List<Link> Links { get; set; }

        [DataMember]
        public string Name
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        private ComposedActivity(NodeKinds k)
            : base(k)
        {
            Activities = new List<Activity>();
        }

        public ComposedActivity()
            : this(NodeKinds.Composed)
        {

        }

        #endregion

        #region Activity Overrides

        public override void Init()
        {
            base.Init();
            if (Activities == null) { Activities = new List<Activity>(); }
            if (Activities.Count == 0 && !ActivityGlobalConfig.IsEditMode)
            {
                if (!string.IsNullOrEmpty(this.Name))
                {
                    var act = ActivityLoader.Load(this.Name) as ComposedActivity;
                    this.Activities = act.Activities;
                    this.Links = act.Links;
                } 
            }
            foreach (var activity in this.Activities)
            {
                activity.Init();
            }
        }

        public override void Config()
        {
            this.UpdateEvents();
            foreach (var activity in this.Activities)
            {
                activity.Config();
            }
            var end = this.Activities.Where(a => a is EndActivity).FirstOrDefault();
            if (end != null)
            {
                end.FinishEvent.ActivityEvent = this.Finish;
                end.CancelEvent.ActivityEvent = this.Cancel;
            }

        }

        internal override void AddBefforCancelHandler(Action befforCancelHandler)
        {
            base.AddBefforCancelHandler(befforCancelHandler);
            foreach (var activity in this.Activities)
            {
                activity.AddBefforCancelHandler(befforCancelHandler);
            }
        }

        public override void Start(bool fromCancel = false)
        {
            this.IsProcessing = true;
            Debug.WriteLine($"{this.Text}  starting at :  {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff")},fromCancel:{fromCancel} ");
             Debug.WriteLine(string.Format("Name:{0}", this.Name));
            var start = this.Activities.Where(a => a is StartActivity).FirstOrDefault();
            if (start == null)
            {
                throw new Exception(string.Format("Inner Activity {0} is null", this.Name));
            }
            start.Start(fromCancel);

        }

       
        #endregion


        public void UpdateEvents()
        {

            if (Activities != null)
            {
               
                foreach (var activity in Activities)
                {
                    activity.CancelEvent.ActivityEvent = null;

                    activity.FinishEvent.ActivityEvent = null;

                    var composed = activity as ComposedActivity;

                    if (composed != null)
                    {
                        composed.UpdateEvents();
                    }
                }
            }
            if (Links == null) { return; }
            foreach (var link in Links)
            {
                if (link.Target != null && link.Source != null)
                {
                    switch (link.SourcePort)
                    {
                        case PortKinds.BottomRight:
                            link.Source.CancelEvent.ActivityEvent = () => { link.Target.Start(true); };
                            break;
                        default:
                            link.Source.FinishEvent.ActivityEvent = () => { link.Target.Start(false); };
                            break;
                    }

                }

            }
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
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            grid.Children.Add(textBlock);

            textBlock.SetValue(Grid.ColumnSpanProperty, 2);

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
            ui.Background = Brushes.Yellow;
            ui.CornerRadius = new CornerRadius(10);
            ui.Child = grid;

            ui.ContextMenu = new ContextMenu();
            var item = new MenuItem() { Header = "进入" };
            item.Click += GoInto;
            ui.ContextMenu.Items.Add(item);

            if (!ActivityGlobalConfig.IsEditMode)
            { 
                var mi = new MenuItem();
                mi.Header = "Start Here";
                mi.Click += (s, e) => { this.Start(true); };
                ui.ContextMenu.Items.Add(mi);
               
            }
           
           
            //ui.PreviewMouseRightButtonUp += Ui_PreviewMouseRightButtonUp;
            return ui;
        }

        private void GoInto(object sender, RoutedEventArgs e)
        {
            ContainerService.Current.GetInstance<IEventAggregator>().GetEvent<PubSubEvent<Tuple<ComposedActivity>>>().Publish(new Tuple<ComposedActivity>(this));
        }
    }
}
