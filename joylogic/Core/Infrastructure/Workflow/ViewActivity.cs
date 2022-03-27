
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel;
using System.Diagnostics;
using Core.Infrastructure.Services;
using Core.Infrastructure.Controls;

namespace Core.Infrastructure.Workflow
{
    [DataContract]
    public class ViewActivity : Activity
    {
        [Browsable(false)]
        public Intent Intent { get; set; }

        private IViewCheckSession _session;
        private IViewCheckSession ViewCheckSession
        {

            get
            {
                if (_session == null)
                {
                    if (string.IsNullOrEmpty(ViewCheckSessionName)) { return null; }
                    _session = ContainerService.Current.GetInstance(Type.GetType(ViewCheckSessionName)) as IViewCheckSession;

                }
                return _session;
            }
        }
        private string _viewCheckSessionName;
        [DataMember]
        public string ViewCheckSessionName
        {
            get { return _viewCheckSessionName; }
            set
            {
                _viewCheckSessionName = value;
                OnPropertyChanged("ViewCheckSessionName");
            }
        }

        public override void Start(bool fromCancel = false)
        {
            this.IsProcessing = true;
            Debug.WriteLine($"{this.Text}  starting at :  {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff")},fromCancel:{fromCancel} ");
            Debug.WriteLine(string.Format("ViewName:{0}", this.ViewName));
            ISysNavigationService navigationService = ContainerService.Current.GetInstance<ISysNavigationService>();
            Intent.IsFromCancel = fromCancel;
            navigationService.ChangeMainRegion(Intent);
        }

        public override void Config()
        {
            this.Intent = new Intent()
            {
                ViewName = this.ViewName,
                NavigationBehavior = this.NavigationBehavior,
                ViewShownName = this.ViewShowName
            };

            if (this.Intent == null) { throw new Exception(" Error Loaded,Intent null"); }
            this.Intent.Finshed = this.Finish;
            this.Intent.Canceled = this.Cancel;
        }

        public override void Finish()
        {
            if (this.ViewCheckSession == null || this.ViewCheckSession.FinshCheck())
            {
                base.Finish();
            }
        }

        public override void Cancel()
        {
            if (this.ViewCheckSession == null || this.ViewCheckSession.CancelCheck())
            {
                base.Cancel();
            }

        }



        private ViewActivity(NodeKinds k)
            : base(k)
        {
        }

        public ViewActivity()
            : this(NodeKinds.View)
        {

        }

        private string _viewName { get; set; }
        [DataMember]
        public string ViewName
        {
            get
            {
                return _viewName;
            }
            set
            {
                _viewName = value;
                OnPropertyChanged("ViewName");
            }
        }

        private string _viewShowName { get; set; }
        [DataMember]
        public string ViewShowName
        {
            get
            {
                return _viewShowName;
            }
            set
            {
                _viewShowName = value;
                OnPropertyChanged("ViewShowName");
            }
        }


        private NavigationBehavior _navigationBehavior;
        [DataMember]
        public NavigationBehavior NavigationBehavior
        {
            get { return _navigationBehavior; }
            set
            {
                _navigationBehavior = value;
                OnPropertyChanged("NavigationBehavior");
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

            var ui = new Border();
            ui.BorderBrush = Brushes.Black;
            ui.BorderThickness = new Thickness(1);
            ui.Background = Brushes.LightBlue;

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

            if (!ActivityGlobalConfig.IsEditMode)
            {
                var menu = new ContextMenu();
                var mi = new MenuItem();
                mi.Header = "Start Here";
                mi.Click += (s, e) => { this.Start(true); };
                menu.Items.Add(mi);
                ui.ContextMenu = menu;
            }

            ui.Child = grid;
            return ui;
        }

    }
}
