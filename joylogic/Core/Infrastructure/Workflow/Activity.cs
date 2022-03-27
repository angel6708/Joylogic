using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Diagnostics;

namespace Core.Infrastructure.Workflow
{
    [DataContract]
    public enum NodeKinds
    {
        [EnumMember]
        Start,
        [EnumMember]
        End,
        [EnumMember]
        View,
        [EnumMember]
        Service,
        [EnumMember]
        Condition,
        [EnumMember]
        Foreach,
        [EnumMember]
        Composed,
        [EnumMember]
        SwitchCase,
        [EnumMember]
        CaseView

    }

    [DataContract]
    public class Activity : INotifyPropertyChanged
    {

        private bool isProcessing;
        [Browsable(false)]
        public bool IsProcessing { get { return isProcessing; } set { isProcessing = value; OnPropertyChanged("IsProcessing"); } }


        #region Event And Handlers
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            this.NeedToSave = true;
        }
        #endregion

        #region Properties



        // [DataMember]
        [Browsable(false)]
        public ActivityEventHandler FinishEvent { get; private set; }


        // [DataMember]
        [Browsable(false)]
        public ActivityEventHandler CancelEvent { get; private set; }

        private Action _befforCancelHandler;

        [DataMember]
        [Browsable(false)]
        public Guid Id { get; set; }

        [Browsable(false)]
        [DataMember]
        public NodeKinds Kind { get; set; }

        [Browsable(false)]
        public object Item { get; set; }

        [Browsable(false)]
        public bool IsDelete { get; set; }

        [Browsable(false)]
        public bool NeedToSave { get; set; }

        private System.Windows.Point _location;
        [DataMember]
        public System.Windows.Point Location
        {
            get { return _location; }
            set
            {
                _location = value;
                OnPropertyChanged("Location");
            }
        }

        private System.Windows.Size _size;
        [DataMember]
        public System.Windows.Size Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged("Size");
            }
        }

        private string _text;
        [DataMember]
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        #endregion

        #region Constructors
        public Activity()
        {
            this.Init();
        }

        public Activity(NodeKinds kind)
            : this()
        {
            Id = Guid.NewGuid();
            Kind = kind;
            this.Text = kind.ToString();
        }

        #endregion

        #region AcitivityMethods

        public virtual void Config() { }

        public virtual void Start(bool fromCancel = false)
        {
            this.IsProcessing = true;
            Debug.WriteLine($"{this.Text}  starting at :  {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff")},fromCancel:{fromCancel} ");
            if (fromCancel)
            {
                this.Cancel();

            }
            else
            {
                this.Finish();
            }
        }
        public virtual void Init()
        {
            CancelEvent = new ActivityEventHandler();
            FinishEvent = new ActivityEventHandler();
        }

        public virtual void Finish()
        {
            Debug.WriteLine(this.Text + "  Finishing at : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff"));
            FinishEvent?.ActivityEvent();
            this.IsProcessing = false;

        }

        internal virtual void AddBefforCancelHandler(Action befforCancelHandler)
        {
            _befforCancelHandler += befforCancelHandler;
        }

        public virtual void Cancel()
        {
            Debug.WriteLine(this.Text + "  Canceling at : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff"));
            _befforCancelHandler?.Invoke();
            CancelEvent.ActivityEvent?.Invoke();
            this.IsProcessing = false;
        }

        public static Activity Create(NodeKinds k)
        {
            switch (k)
            {
                case NodeKinds.View:
                    return new ViewActivity();
                case NodeKinds.Condition:
                    return new ConditionActivity();
                case NodeKinds.End:
                    return new EndActivity();
                case NodeKinds.Foreach:
                    return new ForeachActivity();
                case NodeKinds.Start:
                    return new StartActivity();
                case NodeKinds.Service:
                    return new SaveServiceActivity();
                case NodeKinds.CaseView:
                    return new CaseViewActivity();
                case NodeKinds.Composed:
                    return new ComposedActivity();
                case NodeKinds.SwitchCase:
                    return new CaseflowActivity();
                default: throw new Exception("  ");
            }

        }
        #endregion

        #region UI Methods

        public virtual FrameworkElement CreateContent()
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

        public Func<Activity, bool, bool, bool, Control> UpdateContent;

        #endregion

    }


}
