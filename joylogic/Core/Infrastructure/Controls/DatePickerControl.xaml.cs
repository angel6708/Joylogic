using MouseKeyboardLibrary;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Core.Infrastructure.Controls
{
    /// <summary>
    /// Interaction logic for DatePickerControl.xaml
    /// </summary>
    public partial class DatePickerControl : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {

        public class BindingDay : INotifyPropertyChanged
        {
            public int Mounth { get; set; }
            public DateTime Day { get; set; }
            public bool IsInCurrentMounth { get { return Day.Month == Mounth; } }
            private bool _isSelected;
            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    _isSelected = value; OnPropertyChanged("IsSelected");
                }
            }

            protected void OnPropertyChanged(string proName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(proName));
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
        }

        public class BindingMounth : INotifyPropertyChanged
        {
            public int Mounth { get; set; }


            private bool _isSelected;
            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    _isSelected = value; OnPropertyChanged("IsSelected");
                }
            }

            protected void OnPropertyChanged(string proName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(proName));
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
        }

        private DatePickerControl(Border placeHolder)
        {

            PlaceHolder = placeHolder;
            Instance = this;

            ContentCommand1 = new DelegateCommand<object>(obj =>
            {
                if (Level == 2)
                {
                    this.InitYears(this.CurrentMounth.Year);
                }
                else if (Level == 1)
                {
                    this.InitMounths(this.CurrentMounth.Year, this.CurrentMounth.Month);
                }

            });



            DayCommand = new DelegateCommand<DateTime?>(day =>
            {
                if (day != null)
                {
                    foreach (var d in BindingDays)
                    {
                        if (day.Value == d.Day)
                        {
                            d.IsSelected = true;
                            SelectedDate = d.Day;
                            if (!d.IsInCurrentMounth)
                            {
                                this.InitDays(day.Value.Year, day.Value.Month, day.Value.Day);
                            }
                        }
                        else { d.IsSelected = false; }
                    }
                }


            });

            MounthCommand = new DelegateCommand<int?>(mounth =>
            {
                if (mounth != null)
                {
                    foreach (var m in BindingMounths)
                    {
                        if (mounth.Value == m.Mounth)
                        {
                            m.IsSelected = true;

                        }
                        else { m.IsSelected = false; }
                    }
                    this.InitDays(this.CurrentMounth.Year, mounth.Value, 1);
                }
            });

            YearCommand = new DelegateCommand<int?>(year =>
            {
                if (year != null)
                {
                    this.InitMounths(year.Value, this.CurrentMounth.Month);
                }
            });



            ForewordCommand1 = new DelegateCommand<object>(obj =>
            {
                if (Level == 1)
                {
                    var m = CurrentMounth.AddMonths(1);
                    this.InitDays(m.Year, m.Month, 1);
                }
                else if (Level == 2)
                {
                    var m = CurrentMounth.AddMonths(12);
                    this.InitMounths(m.Year, m.Month);
                }
                else if (Level == 3)
                {
                    var m = CurrentMounth.AddYears(10);
                    this.InitYears(m.Year);
                }
            });

            BackwordCommand1 = new DelegateCommand<object>(obj =>
            {
                if (Level == 1)
                {
                    if (CurrentMounth.Year != 1 || CurrentMounth.Month != 1)
                    {
                        var m = CurrentMounth.AddMonths(-1);
                        this.InitDays(m.Year, m.Month, 1);
                    }
                }
                else if (Level == 2)
                {
                    if (CurrentMounth.Year - 1 > 0)
                    {
                        var m = CurrentMounth.AddMonths(-12);
                        this.InitMounths(m.Year, m.Month);
                    }
                }
                else if (Level == 3)
                {
                    if (CurrentMounth.Year - 10 > 0)
                    {
                        var m = CurrentMounth.AddYears(-10);
                        this.InitYears(m.Year);
                    }
                }
            });

            TodayCommand = new DelegateCommand<object>(obj =>
            {
                this.SelectedDate = DateTime.Today;
                SetSelectedDate(_hostTextBox, this.SelectedDate);

                hideFocus.Focus();
                Hide();
            });

            NullCommand = new DelegateCommand<object>(obj =>
            {
                this.SelectedDate = null;
                SetSelectedDate(_hostTextBox, this.SelectedDate);
                hideFocus.Focus();
                Hide();
            });

            OkCommand = new DelegateCommand<object>(obj =>
            {
                // this.SelectedDate = DateTime.Today;
                SetSelectedDate(_hostTextBox, this.SelectedDate);
                hideFocus.Focus();
                Hide();
             

            });

            CancelCommand = new DelegateCommand<object>(obj =>
            {
                // this.SelectedDate = DateTime.Today;
                // SetSelectedDate(_hostTextBox, this.SelectedDate); 
                hideFocus.Focus();
                Hide();

            });


            InitializeComponent();
            //InitDays(2014, 1, 3);
            this.DataContext = this;
        }

        private DateTime? _selectedDate = DateTime.Now;
        private static TextBox _hostTextBox;

        public DateTime? SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                OnPropertyChanged("SelectedDate");
            }
        }

        private DateTime _currentMounth;
        public DateTime CurrentMounth
        {
            get { return _currentMounth; }
            set
            {
                _currentMounth = value;
                OnPropertyChanged("CurrentMounth");
            }
        }

        private int _Hour;
        public int Hour
        {
            get { return _Hour; }
            set
            {
                _Hour = value;
                OnPropertyChanged("Hour");

                this.CurrentMounth = new DateTime(this.CurrentMounth.Year, this.CurrentMounth.Month,
                    this.CurrentMounth.Day, this.CurrentMounth.Hour, this.CurrentMounth.Minute,0);
            }
        }

        private int _Minute;
        public int Minute
        {
            get { return _Minute; }
            set
            {
                _Minute = value;
                OnPropertyChanged("Minute");
                this.CurrentMounth = new DateTime(this.CurrentMounth.Year, this.CurrentMounth.Month,
                    this.CurrentMounth.Day, this.CurrentMounth.Hour, this.CurrentMounth.Minute, 0);
            }
        }


        private int Level = 1;


        public List<BindingDay> BindingDays { get; set; }

        public List<BindingMounth> BindingMounths { get; set; }

        private void InitDays(int year, int month, int day)
        {
            this.weekPanel.Visibility = System.Windows.Visibility.Visible;
            okButton.Visibility = System.Windows.Visibility.Visible;
            this.Level = 1;
            level1_days.Visibility = System.Windows.Visibility.Visible;
            level2_contents.Visibility = System.Windows.Visibility.Collapsed;
            SelectedDate = new DateTime(year, month, day);
            CurrentMounth = new DateTime(year, month, 1);
            var firstDay = new DateTime(year, month, 1);

            if (firstDay.Year != 1 && firstDay.Month != 1)
            {
                while (firstDay.DayOfWeek != DayOfWeek.Sunday)
                {
                    firstDay = firstDay.AddDays(-1);
                }
            }
            var lastDay = firstDay.AddDays(41);

            List<BindingDay> days = new List<BindingDay>();
            level1_days.Children.Clear();
            for (var d = firstDay; d <= lastDay; d = d.AddDays(1))
            {

                var bingdingDay = new BindingDay() { IsSelected = false, Mounth = month, Day = d };
                if (d.Month == month && d.Day == day) { bingdingDay.IsSelected = true; }
                days.Add(bingdingDay);
                Button b = new Button();
                b.Style = this.FindResource("InformButton") as Style;
                b.Focusable = false;
                b.Command = this.DayCommand;
                b.CommandParameter = d;
                Binding bingding = new Binding("IsSelected");
                bingding.Source = bingdingDay;
                b.SetBinding(Button.IsDefaultProperty, bingding);

                b.Content = bingdingDay.Day.Day;
                b.Tag = bingdingDay.IsInCurrentMounth.ToString().ToLower();


                Grid.SetColumn(b, d.Subtract(firstDay).Days % 7);
                Grid.SetRow(b, d.Subtract(firstDay).Days / 7);

                level1_days.Children.Add(b);
            }

            BindingDays = days;


        }

        private void InitMounths(int year, int mounth)
        {
            CurrentMounth = new DateTime(year, mounth, 1);
            okButton.Visibility = System.Windows.Visibility.Collapsed;
            this.Level = 2;
            this.weekPanel.Visibility = System.Windows.Visibility.Collapsed;
            level1_days.Visibility = System.Windows.Visibility.Collapsed;
            level2_contents.Visibility = System.Windows.Visibility.Visible;
            this.level2_contents.Children.Clear();
            BindingMounths = new List<BindingMounth>();
            for (var m = 1; m <= 12; m++)
            {

                var bingdingMounth = new BindingMounth() { IsSelected = false, Mounth = m, };
                if (m == mounth) { bingdingMounth.IsSelected = true; }

                Button b = new Button();
                b.Style = this.FindResource("InformButton") as Style;
                b.Focusable = false;
                b.Command = this.MounthCommand;
                b.CommandParameter = m;
                Binding bingding = new Binding("IsSelected");
                bingding.Source = bingdingMounth;
                b.SetBinding(Button.IsDefaultProperty, bingding);

                b.Content = bingdingMounth.Mounth;

                Grid.SetColumn(b, (m - 1) % 4);
                Grid.SetRow(b, (m - 1) / 4);

                level2_contents.Children.Add(b);
                BindingMounths.Add(bingdingMounth);
            }
        }

        private void InitYears(int year)
        {
            this.Level = 3;
            this.weekPanel.Visibility = System.Windows.Visibility.Collapsed;
            this.level2_contents.Children.Clear();
            CurrentMounth = new DateTime(year, CurrentMounth.Month, 1);

            for (var y = (year / 10) * 10 - 1; y <= (year / 10) * 10 + 10; y++)
            {

                var bingdingMounth = new BindingMounth() { IsSelected = false, Mounth = y, };
                if (y == year) { bingdingMounth.IsSelected = true; }

                Button b = new Button();
                if (y <= 0) { b.IsEnabled = false; }
                b.Style = this.FindResource("InformButton") as Style;
                b.Focusable = false;
                b.Command = this.YearCommand;
                b.CommandParameter = y;
                Binding bingding = new Binding("IsSelected");
                bingding.Source = bingdingMounth;
                b.SetBinding(Button.IsDefaultProperty, bingding);

                b.Content = bingdingMounth.Mounth;

                Grid.SetColumn(b, (y - (year / 10) * 10 + 1) % 4);
                Grid.SetRow(b, (y - (year / 10) * 10 + 1) / 4);

                level2_contents.Children.Add(b);

            }
        }

        public static void CreateSingle(Border placeHolder)
        {
            if (Instance == null)
                Instance = new DatePickerControl(placeHolder);
        }

        private static DatePickerControl Instance { get; set; }
        private static Border PlaceHolder { get; set; }

        private static Point LastPosition { get; set; }

        public ICommand KeyCommand { get; set; }

        public ICommand DayCommand { get; set; }
        public ICommand MounthCommand { get; set; }
        public ICommand YearCommand { get; set; }

        public ICommand ForewordCommand1 { get; set; }
        public ICommand BackwordCommand1 { get; set; }

        public ICommand ContentCommand1 { get; set; }

        public ICommand OkCommand { get; set; }
        public ICommand TodayCommand { get; set; }

        public ICommand NullCommand { get; set; }
        public ICommand CancelCommand { get; set; }




        public static bool GetDatePickerControl(DependencyObject obj)
        {
            return (bool)obj.GetValue(DatePickerControlProperty);
        }

        public static void SetDatePickerControl(DependencyObject obj, bool value)
        {
            obj.SetValue(DatePickerControlProperty, value);
        }

        public static readonly DependencyProperty DatePickerControlProperty =
            DependencyProperty.RegisterAttached("DatePickerControl", typeof(bool), typeof(DatePickerControl), new UIPropertyMetadata(default(bool), DatePickerControlPropertyChanged));


        static void DatePickerControlPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement host = sender as FrameworkElement;
            if (host != null)
            {

                host.GotFocus += OnGotFocus;
                host.LostFocus += OnLostFocus;
            }


        }


        public static bool GetDateInputNullable(DependencyObject obj)
        {
            return (bool)obj.GetValue(DateInputNullableProperty);
        }

        public static void SetDateInputNullable(DependencyObject obj, bool value)
        {
            obj.SetValue(DateInputNullableProperty, value);
        }

        public static readonly DependencyProperty DateInputNullableProperty =
            DependencyProperty.RegisterAttached("DateInputNullable", typeof(bool), typeof(DatePickerControl), new UIPropertyMetadata(default(bool), DateInputNullablePropertyChanged));


        static void DateInputNullablePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }



        static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            Control host = sender as Control;
            _hostTextBox = host as TextBox;
            if (PlaceHolder != null)
            {
                var grid = PlaceHolder.Parent as Grid;
                var point = host.TranslatePoint(new Point(0, 0), grid);
                // Instance.InitDays(2014, 1, 1);
                Show(point);
            }
            var date = GetSelectedDate(host).GetValueOrDefault(DateTime.Today);
            if (GetDateInputNullable(host))
            {
                //Instance.CancelCommand
                Instance.btnNull.Visibility = Visibility.Visible;
            }
            else
            {
                Instance.btnNull.Visibility = Visibility.Collapsed;
            }
            Instance.InitDays(date.Year, date.Month, date.Day);
        }


        public static DateTime? GetSelectedDate(DependencyObject obj)
        {
            return (DateTime?)obj.GetValue(SelectedDateProperty);
        }

        public static void SetSelectedDate(DependencyObject obj, DateTime? value)
        {
            obj.SetValue(SelectedDateProperty, value);
        }

        public static readonly DependencyProperty SelectedDateProperty =
          DependencyProperty.RegisterAttached("SelectedDate", typeof(DateTime?), typeof(DatePickerControl), new UIPropertyMetadata(DateTime.Today, SelectedDatePropertyChanged));
        static void SelectedDatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }


        static double _heightResume = 30;

        private static void Show(Point location)
        {
            var grid = PlaceHolder.Parent as Grid;
            Size size = new Size(grid.ActualWidth, grid.ActualHeight);
            if (size.Height - (location.Y + _heightResume) < Instance.Height)
            {
                location.Y -= Instance.Height;
            }
            else
            {
                location.Y += _heightResume;
            }

            if (size.Width - location.X < Instance.Width)
            {
                location.X = size.Width - Instance.Width;
            }

            var trans = PlaceHolder.RenderTransform as TranslateTransform;

            DoubleAnimation pax = new DoubleAnimation(trans.X, location.X, new Duration(TimeSpan.FromTicks(0)));
            DoubleAnimation pay = new DoubleAnimation(trans.Y, location.Y, new Duration(TimeSpan.FromTicks(0)));
            PlaceHolder.Visibility = System.Windows.Visibility.Visible;
            PlaceHolder.Child = Instance;
            trans.BeginAnimation(TranslateTransform.XProperty, pax);
            trans.BeginAnimation(TranslateTransform.YProperty, pay);

        }

        public static void Show()
        {
            Show(LastPosition);
        }

        private static void Hide()
        {
            PlaceHolder.Visibility = System.Windows.Visibility.Collapsed;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            var propChanged = PropertyChanged;
            if (propChanged != null)
            {
                propChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private static bool DateControlFocused { get; set; }

        private void Button_GotFocus(object sender, RoutedEventArgs e)
        {
            DateControlFocused = true;
        }
    }

}
