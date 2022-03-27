 
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
using System.Windows.Threading;
using System.Threading; 
using System.Diagnostics;
using Core.Infrastructure.Services;
using Core.Infrastructure.Utils;
using Core.Infrastructure.Events;
using Prism.Events;

namespace Core.Infrastructure.Workflow
{
    [DataContract]
    public class SaveServiceActivity : Activity
    {
        [Browsable(false)]
        public Intent Intent { get; set; }

        private ISaveService _service;
        private ISaveService SaveService
        {

            get
            {
                if (_service == null)
                {
                    if (string.IsNullOrEmpty(SaveServiceName)) { return null; }
                    _service = ContainerService.Current.GetInstance(Type.GetType(SaveServiceName)) as ISaveService;

                }
                return _service;
            }
        }
        private string _SaveServiceName;
        [DataMember]
        public string SaveServiceName
        {
            get { return _SaveServiceName; }
            set
            {
                _SaveServiceName = value;
                OnPropertyChanged("SaveServiceName");
            }
        }

        public override void Start(bool fromCancel = false)
        {
            this.IsProcessing = true;
            Debug.WriteLine($"{this.Text}  starting at :  {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff")},fromCancel:{fromCancel} ");
            Debug.WriteLine($"SaveSession :  {SaveServiceName}");
            
            if (SaveService.IsInUiThread)
            {
                var dispatcher = UIDispatcher.Dispatcher;
                dispatcher.BeginInvoke(() => {
                    if (SaveService.ExecSave())
                    {
                        this.Finish();
                    }
                    else
                    {
                        this.Cancel();
                    }
                }); 
            }
            else
            {
                var dispatcher = UIDispatcher.Dispatcher;
                dispatcher.Invoke(() => {
                    IEventAggregator eventAggregator = ContainerService.Current.CreateInstance<IEventAggregator>();
                    eventAggregator.GetEvent<ShowLoadingEvent>().Publish(new ShowLoadingEventArg()
                    {
                        Status = Visibility.Visible,
                        Caption = SaveService.SavingDesc,
                    });

                });
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    bool ret = SaveService.ExecSave();

                    dispatcher.BeginInvoke((Action<bool>)delegate(bool r)
                    {
                        IEventAggregator eventAggregator = ContainerService.Current.CreateInstance<IEventAggregator>();
                        eventAggregator.GetEvent<ShowLoadingEvent>().Publish(new ShowLoadingEventArg()
                        {
                            Status = Visibility.Collapsed,
                            Caption = SaveService.SavingDesc,
                        });
                        if (r)
                        {
                            this.Finish();
                        }
                        else
                        {
                            this.Cancel();
                        }
                    }, ret);
                }, null);
            }
            
        }

        private delegate void RunHandle();


        public override void Config()
        {
            this.Intent = new Intent()
            {
                IntentName = "SaveService"
            };
            this.Intent.Finshed = this.Finish;
            this.Intent.Canceled = this.Cancel;
        }


        private SaveServiceActivity(NodeKinds k)
            : base(k)
        {
        }

        public SaveServiceActivity()
            : this(NodeKinds.Service)
        {

        }



        private string _ServiceShowName;
        [DataMember]
        public string ServiceShowName
        {
            get
            {
                return _ServiceShowName;
            }
            set
            {
                _ServiceShowName = value;
                OnPropertyChanged("ServiceShowName");
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

            ui.Child = grid;

            if (!ActivityGlobalConfig.IsEditMode)
            {
                var menu = new ContextMenu();
                var mi = new MenuItem();
                mi.Header = "Start Here";
                mi.Click += (s, e) => {  this.Start(true); };
                menu.Items.Add(mi);
                ui.ContextMenu = menu;
            }

            return ui;
        }

    }
}
