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
    /// Interaction logic for NumberInputControl.xaml
    /// </summary>
    public partial class NumberInputControl : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {

        private NumberInputControl(Border placeHolder)
        {
            PlaceHolder = placeHolder;
            Instance = this;
            KeyCommand = new DelegateCommand<System.Windows.Forms.Keys?>(key =>
            { 
                var k = key.GetValueOrDefault();
                if (k == System.Windows.Forms.Keys.PrintScreen)
                {
                    Hide();

                }
                else if (k == System.Windows.Forms.Keys.Space)
                {
                    //System.Windows.Forms.Keys.Oemcomma

                    KeyboardSimulator.KeyPress(System.Windows.Forms.Keys.D0);
                    KeyboardSimulator.KeyPress(System.Windows.Forms.Keys.D0);
                }
                else
                {
                    KeyboardSimulator.KeyPress(k);
                }
                InputMethod.Current.ImeConversionMode = InputMethod.Current.ImeConversionMode & ~ImeConversionModeValues.Roman;
               

            });
            InitializeComponent();


        }

        public static void CreateSingle(Border placeHolder)
        {
            if (Instance == null)
                Instance = new NumberInputControl(placeHolder);
        }

        private static NumberInputControl Instance { get; set; }
        private static Border PlaceHolder { get; set; }

        private static Point LastPosition { get; set; }

        public ICommand KeyCommand { get; set; }



        public static bool GetNumberInputControl(DependencyObject obj)
        {
            return (bool)obj.GetValue(NumberInputControlProperty);
        }

        public static void SetNumberInputControl(DependencyObject obj, bool value)
        {
            obj.SetValue(NumberInputControlProperty, value);
        }

        public static readonly DependencyProperty NumberInputControlProperty =
            DependencyProperty.RegisterAttached("NumberInputControl", typeof(bool), typeof(NumberInputControl), new UIPropertyMetadata(default(bool), NumberInputControlPropertyChanged));



        static void NumberInputControlPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement host = sender as FrameworkElement;
            if (host != null)
            {

                host.GotFocus += OnGotFocus;
                host.LostFocus += OnLostFocus;
            }


        }

        static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            Control host = sender as Control;
            var textBox = host as TextBox;
            textBox.SetCurrentValue(InputMethod.IsInputMethodEnabledProperty, false);
             
            if (PlaceHolder != null)
            {
                var grid = PlaceHolder.Parent as Grid;
                var point = host.TranslatePoint(new Point(0, 0), grid);
                Show(point);
            }
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
    }

}
