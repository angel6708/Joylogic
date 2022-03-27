using MouseKeyboardLibrary;
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
    /// Interaction logic for DateInputControl.xaml
    /// </summary>
    public partial class DateInputControl : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
 
        private DateInputControl(Border placeHolder)
        {

            PlaceHolder = placeHolder;
            Instance = this;
             
            InitializeComponent();
            //InitDays(2014, 1, 3);
            this.DataContext = this;
        }


        private DateTime _selectedDate = DateTime.Today;
        private static TextBox _hostTextBox;

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                OnPropertyChanged("SelectedDate");
            }
        }
         
        public static void CreateSingle(Border placeHolder)
        {
            if (Instance == null)
                Instance = new DateInputControl(placeHolder);
        }

        private static DateInputControl Instance { get; set; }
        private static Border PlaceHolder { get; set; }

        private static Point LastPosition { get; set; }
         
          
        public static bool GetDateInputControl(DependencyObject obj)
        {
            return (bool)obj.GetValue(DateInputControlProperty);
        }

        public static void SetDateInputControl(DependencyObject obj, bool value)
        {
            obj.SetValue(DateInputControlProperty, value);
        }

        public static readonly DependencyProperty DateInputControlProperty =
            DependencyProperty.RegisterAttached("DateInputControl", typeof(bool), typeof(DateInputControl), new UIPropertyMetadata(default(bool), DateInputControlPropertyChanged));


        static void DateInputControlPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
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
            DependencyProperty.RegisterAttached("DateInputNullable", typeof(bool), typeof(DateInputControl), new UIPropertyMetadata(default(bool), DateInputNullablePropertyChanged));


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
           Instance.SelectedDate = GetSelectedDate(host).GetValueOrDefault(DateTime.Today);
            if (GetDateInputNullable(host))
            {
                //Instance.CancelCommand
                 Instance.Nullbtn.Visibility = Visibility.Visible;
            }
            else
            {
                Instance.Nullbtn.Visibility = Visibility.Collapsed;
            }
            Instance.RefreshShow();
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
          DependencyProperty.RegisterAttached("SelectedDate", typeof(DateTime?), typeof(DateInputControl), new UIPropertyMetadata(DateTime.Today, SelectedDatePropertyChanged));
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
                location.Y = _heightResume+40;
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

        private void RefreshShow() 
        {
            yearText.Text = SelectedDate.Year.ToString("0000");
            mounthText.Text = SelectedDate.Month.ToString("00");
            dayText.Text = SelectedDate.Day.ToString("00");
        }

        private void yearUpBtn_Click(object sender, RoutedEventArgs e)
        {
          SelectedDate=  this.SelectedDate.AddYears(1);
          RefreshShow();
        }

        private void yearDnBtn_Click(object sender, RoutedEventArgs e)
        {
           SelectedDate= this.SelectedDate.AddYears(-1);
           RefreshShow();
        }

        private void mounthUpBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDate.AddMonths(1).Year == SelectedDate.Year)
            {
                SelectedDate = SelectedDate.AddMonths(1);
                RefreshShow();
            }
        }

        private void mounthDnBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDate.AddMonths(-1).Year == SelectedDate.Year)
            {
                SelectedDate = SelectedDate.AddMonths(-1);
                RefreshShow();
            }
         
        }

        private void dayUpBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDate.AddDays(1).Month == SelectedDate.Month) 
            {
                SelectedDate = SelectedDate.AddDays(1);
                RefreshShow();
            } 
        }

        private void dayDnBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDate.AddDays(-1).Month == SelectedDate.Month)
            {
                SelectedDate = SelectedDate.AddDays(-1);
                RefreshShow();
            } 
        }

        private void okbtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedDate(_hostTextBox, SelectedDate);
            this.SelectedDate = DateTime.Today;
            cancelbtn.Focus();
        }

        private void cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            //SetSelectedDate(_hostTextBox, null);
            this.SelectedDate = DateTime.Today;
           
           
        }

        private void todayBtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedDate(_hostTextBox, DateTime.Today);
            cancelbtn.Focus();
            this.SelectedDate = DateTime.Today;
        }

        private void Nullbtn_Click(object sender, RoutedEventArgs e)
        {
            SetSelectedDate(_hostTextBox, null);
            cancelbtn.Focus();
            this.SelectedDate = DateTime.Today;
        }
    }

}
