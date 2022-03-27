
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
using System.Collections.ObjectModel;

namespace Core.Infrastructure.Workflow
{
    [DataContract]
    public class CaseViewActivity : Activity
    {



        private ICaseSession _caseSession;
        private ICaseSession CaseSession
        {
            get
            {
                if (_caseSession == null)
                {
                    if (string.IsNullOrEmpty(CaseSessionName)) { return null; }
                    _caseSession = ContainerService.Current.GetInstance(Type.GetType(CaseSessionName)) as ICaseSession;

                }
                return _caseSession;
            }

        }

        private string _caseSessionName;
        [DataMember]
        public string CaseSessionName
        {
            get { return _caseSessionName; }
            set
            {
                _caseSessionName = value;
                OnPropertyChanged("CaseSessionName");

                OnSessionChange();
            }
        }


        [DataMember]
        [Browsable(false)]
        public ObservableCollection<ActivityCase> Cases { get; set; }

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


        private void OnSessionChange()
        {
            if (CaseSession == null) { return; }
            var type = CaseSession.CaseType;
            var values = Enum.GetNames(type);
            Cases = new ObservableCollection<ActivityCase>();
            foreach (var v in values)
            {
                Cases.Add(new ActivityCase { CaseValue = v });
            }
            CreateContent();

        }

        public override void Start(bool fromCancel = false)
        {
            this.IsProcessing = true;
            Debug.WriteLine(this.Text + "  Starting at : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff"));
            Debug.WriteLine(string.Format("ViewName:{0}", this.ViewName));
            ISysNavigationService navigationService = ContainerService.Current.GetInstance<ISysNavigationService>();
            Intent.IsFromCancel = fromCancel;
            navigationService.ChangeMainRegion(Intent);
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



        private CaseViewActivity(NodeKinds k)
            : base(k)
        {
        }

        public CaseViewActivity()
            : this(NodeKinds.CaseView)
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


        public override void Init()
        {
            base.Init();
            if (Cases != null)
                foreach (var c in Cases)
                {
                    if (c.Activity != null)
                    {
                        c.Activity.Init();
                    }
                }
        }


        private void InnerCaseStart()
        {
            Debug.WriteLine($"{this.Text}  starting at :  {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff")},fromCancel:{true} ");
            Debug.WriteLine($"CaseSession: {this.CaseSessionName }, CaseSession Value: {this.CaseSession.GetCaseValue()}");

            if (Cases != null)
            {
                var c = this.Cases.Where(a => a.CaseValue == CaseSession.GetCaseValue()).FirstOrDefault();
                if (c == null||c.Activity==null)
                {
                    this.Finish();
                }
                else
                {
                    c.Activity.Start(false);
                }
            }
        }

        private void InnerCaseStartFromCancel()
        {
            Debug.WriteLine($"{this.Text}  starting at :  {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss fff")},fromCancel:{true} ");
            Debug.WriteLine($"CaseSession: {this.CaseSessionName }, CaseSession Value: {this.CaseSession.GetCaseValue()}");

            if (Cases != null)
            {
                var c = this.Cases.Where(a => a.CaseValue == CaseSession.GetCaseValue()).FirstOrDefault();
                if (c == null)
                {
                    this.Cancel();
                }
                else
                {
                    c.Activity.Start(true);
                }
            }
        }

        public override void Config()
        {

            this.Intent = new Intent()
            {
                ViewName = this.ViewName,
                NavigationBehavior = this.NavigationBehavior,
                ViewShownName = this.ViewShowName
            };

            if (this.Intent == null) { throw new Exception("Intent should not be none"); }
            this.Intent.Finshed = this.InnerCaseStart;
            this.Intent.Canceled = this.Cancel;


            //////
            if (this.Cases == null)
            {
                throw new Exception("CaseFlow Must be set");
            }

            foreach (var c in Cases)
            {
                if (c.Activity != null)
                {
                    c.Activity.CancelEvent.ActivityEvent = this.Cancel;
                    c.Activity.FinishEvent.ActivityEvent = this.Finish;
                    c.Activity.Config();
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
            textBlock.VerticalAlignment = VerticalAlignment.Bottom;


            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition());
            if (Cases != null)
                foreach (var item in this.Cases)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                }

            grid.Children.Add(textBlock);
            textBlock.SetValue(Grid.ColumnSpanProperty, 8);

            int col = 0;
            if (Cases != null)
                foreach (var item in this.Cases)
                {


                    Grid leftg = new Grid();

                    leftg.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20) });
                    leftg.RowDefinitions.Add(new RowDefinition());
                    leftg.Children.Add(new TextBlock() { Text = item.CaseValue, HorizontalAlignment = HorizontalAlignment.Center });

                    Border leftb = new Border();
                    leftb.BorderBrush = Brushes.LightGray;
                    leftb.BorderThickness = new Thickness(1);
                    leftb.SetValue(Grid.RowProperty, 1);
                    leftb.Background = Brushes.Green;
                    leftb.Tag = item.CaseValue;
                    leftb.Drop += Drop;
                    leftb.DragEnter += DragEnter;
                    leftb.DragLeave += DragLeave;
                    leftb.ContextMenu = GetContextMenu(leftb, item);

                    if (item.Activity != null)
                    {

                        if (item.Activity.IsDelete)
                        {
                            item.Activity = null;
                        }
                        else
                        {

                            if (UpdateContent != null)
                            {
                                leftb.Child = UpdateContent(item.Activity, true, true, false);
                            }

                        }

                    }

                    leftg.Children.Add(leftb);
                    leftg.SetValue(Grid.RowProperty, 1);
                    leftg.SetValue(Grid.ColumnProperty, col);
                    col++;
                    grid.Children.Add(leftg);

                }


            if (_theUi == null)
            {
                _theUi = new Border();
                _theUi.BorderBrush = Brushes.Black;
                _theUi.BorderThickness = new Thickness(1);
                _theUi.Background = Brushes.LightBlue;
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

        private Border _theUi = null;



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

                foreach (var c in Cases)
                {
                    if (border.Tag as string == c.CaseValue)
                    {
                        c.Activity = node;
                    }

                }
                border.Background = Brushes.Green;
            }
            e.Handled = true;
        }

        private ContextMenu GetContextMenu(Border border, ActivityCase cs)
        {
            if (!ActivityGlobalConfig.IsEditMode) { return null; }
            var contextMenu = new ContextMenu();
            foreach (NodeKinds kind in Enum.GetValues(typeof(NodeKinds)))
            {
                if (kind == NodeKinds.Start || kind == NodeKinds.End) { continue; }
                var item = new MenuItem() { Header = "Add " + kind };
                item.Tag = kind;
                item.Click += (s, e) => { CreateInnerActivity(border, (NodeKinds)(s as MenuItem).Tag, cs); };
                contextMenu.Items.Add(item);

            }
            return contextMenu;
        }

        protected void CreateInnerActivity(Border conntainer, NodeKinds nodeKind, ActivityCase cs)
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

            cs.Activity = node;
            border.Background = Brushes.Green;
        }


    }
}
