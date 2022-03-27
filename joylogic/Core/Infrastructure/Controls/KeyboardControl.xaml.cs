using MouseKeyboardLibrary;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
    /// Interaction logic for KeyboardControl.xaml
    /// </summary>
    public partial class KeyboardControl : System.Windows.Controls.UserControl, INotifyPropertyChanged
    { 
        private KeyboardControl(Border placeHolder)
        { 
            PlaceHolder = placeHolder;
            Instance = this; 
            LanguageSwithCommand = new DelegateCommand<object>(para =>
            {
                var list = System.Windows.Forms.InputLanguage.InstalledInputLanguages;
              
                if (System.Windows.Forms.InputLanguage.CurrentInputLanguage.LayoutName == "中文(简体) - 美式键盘")
                {
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {

                            if (list[i].LayoutName != "中文(简体) - 美式键盘")
                            {
                                System.Windows.Forms.InputLanguage.CurrentInputLanguage = list[i];
                                Instance.TextCh.Visibility = Visibility.Visible;
                                Instance.TextEn.Visibility = Visibility.Collapsed;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {

                            if (list[i].LayoutName == "中文(简体) - 美式键盘")
                            {
                                System.Windows.Forms.InputLanguage.CurrentInputLanguage = list[i];
                                Instance.TextCh.Visibility = Visibility.Collapsed;
                                Instance.TextEn.Visibility = Visibility.Visible;
                                break;
                            }
                        }
                    }
                }
            });

            KeyCommand = new DelegateCommand<System.Windows.Forms.Keys?>(key =>
            { 

                bool sysKey = false;
                bool doubleSysKey = false;
                var k = key.GetValueOrDefault();
                if (k == System.Windows.Forms.Keys.PrintScreen)
                {
                    Hide();
                    sysKey = true;
                }
                if (k == System.Windows.Forms.Keys.LShiftKey || k == System.Windows.Forms.Keys.RShiftKey)
                {
                    if (ShiftFlag) { doubleSysKey = true; }
                    ShiftFlag = true;
                    sysKey = true;
                }
                if (k == System.Windows.Forms.Keys.LControlKey || k == System.Windows.Forms.Keys.RControlKey)
                {
                    if (CtrlFlag) { doubleSysKey = true; }
                    CtrlFlag = true;
                    sysKey = true;
                }
                if (k == System.Windows.Forms.Keys.Alt)
                {
                    if (AltFlag) { doubleSysKey = true; }
                    AltFlag = true;
                    sysKey = true;
                }

                if (sysKey && !doubleSysKey) { return; }

         
                if (ShiftFlag)
                {
                    KeyboardSimulator.KeyDown(System.Windows.Forms.Keys.Shift);

                }
                if (CtrlFlag)
                {
                    KeyboardSimulator.KeyDown(System.Windows.Forms.Keys.LControlKey);
                }

                if (AltFlag)
                {
                    KeyboardSimulator.KeyDown(System.Windows.Forms.Keys.Alt);
                }
                if (!doubleSysKey)
                {
                    KeyboardSimulator.KeyPress(k);
                     
                }
                if (ShiftFlag)
                {
                    KeyboardSimulator.KeyUp(System.Windows.Forms.Keys.Shift);
                    ShiftFlag = false;
                }
                if (CtrlFlag)
                {
                    KeyboardSimulator.KeyUp(System.Windows.Forms.Keys.LControlKey);
                    CtrlFlag = false;
                }

                if (AltFlag)
                {
                    KeyboardSimulator.KeyUp(System.Windows.Forms.Keys.Alt);
                    AltFlag = false;
                }
            });
            InitializeComponent();


        }

        public static void CreateSingle(Border placeHolder)
        {
            if (Instance == null)
                Instance = new KeyboardControl(placeHolder);
        }

        private static KeyboardControl Instance { get; set; }
        private static Border PlaceHolder { get; set; }

        private static TextBlock TextBlockCh { get; set; }

        private static TextBlock TextBlockEn { get; set; }

        private static Point LastPosition { get; set; }

        private static HorizontalAlignment KeybordHorizontalAlignment = HorizontalAlignment.Stretch;
        private static VerticalAlignment KeybordVerticalAlignment = VerticalAlignment.Stretch;


        private bool _ShiftFlag;

        public bool ShiftFlag
        {
            get { return _ShiftFlag; }
            set
            {
                _ShiftFlag = value;
                OnPropertyChanged("ShiftFlag");
            }
        }

        private bool _ctrlFlag;
        public bool CtrlFlag
        {
            get { return _ctrlFlag; }
            set
            {
                _ctrlFlag = value;
                OnPropertyChanged("CtrlFlag");
            }
        }

        public bool CapsLockFlag
        {

            get
            {
                return Console.CapsLock;
               
            }
        }

        private bool _altFlag;
        public bool AltFlag
        {
            get { return _altFlag; }
            set
            {
                _altFlag = value;
                OnPropertyChanged("AltFlag");
            }
        }

        public ICommand KeyCommand { get; set; }

        public ICommand LanguageSwithCommand { get; set; }

        public static bool GetKeyboardControl(DependencyObject obj)
        {
            return (bool)obj.GetValue(KeyboardControlProperty);
        }

        public static void SetKeyboardControl(DependencyObject obj, bool value)
        {
            obj.SetValue(KeyboardControlProperty, value);
        }

  
        public static readonly DependencyProperty KeyboardControlProperty =
            DependencyProperty.RegisterAttached("KeyboardControl", typeof(bool), typeof(KeyboardControl), new UIPropertyMetadata(default(bool), KeyboardControlPropertyChanged));

    

        static void KeyboardControlPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement host = sender as FrameworkElement;

            if (host != null)
            {
                host.GotFocus += OnGotFocus;
                host.LostFocus += OnLostFocus;
            }
        }

        public static readonly DependencyProperty KeyboardHorizontalAlignmentProperty =
           DependencyProperty.RegisterAttached("KeyboardHorizontalAlignment", typeof(HorizontalAlignment), typeof(KeyboardControl), new UIPropertyMetadata(HorizontalAlignment.Center, KeyboardHorizontalAlignmentPropertyChanged));

         static void KeyboardHorizontalAlignmentPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            KeybordHorizontalAlignment = (HorizontalAlignment)e.NewValue;
        }

        public static HorizontalAlignment GetKeyboardHorizontalAlignment(DependencyObject obj)
        {
            return (HorizontalAlignment)obj.GetValue(KeyboardHorizontalAlignmentProperty);
        }


        public static void SetKeyboardHorizontalAlignment(DependencyObject obj, HorizontalAlignment value)
        {
            obj.SetValue(KeyboardHorizontalAlignmentProperty, value);
        }



        public static void SetKeyboardVerticalAlignment(DependencyObject obj, VerticalAlignment value)
        {
            obj.SetValue(KeyboardVerticalAlignmentProperty, value);
        }

        public static VerticalAlignment GetKeyboardVerticalAlignment(DependencyObject obj)
        {
            return (VerticalAlignment)obj.GetValue(KeyboardVerticalAlignmentProperty);
        }

        public static readonly DependencyProperty KeyboardVerticalAlignmentProperty =
        DependencyProperty.RegisterAttached("KeyboardVerticalAlignment", typeof(VerticalAlignment), typeof(KeyboardControl), new UIPropertyMetadata(VerticalAlignment.Bottom, KeyboardVerticalAlignmentPropertyChanged));

        private static void KeyboardVerticalAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KeybordVerticalAlignment = (VerticalAlignment)e.NewValue;
        }


        static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement host = sender as FrameworkElement;
           // host.GotFocus -= OnGotFocus;
           // host.LostFocus -= OnLostFocus;
            Hide();
        }

        static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            
            Control host = sender as Control;
            KeybordHorizontalAlignment = (HorizontalAlignment)host.GetValue(KeyboardControl.KeyboardHorizontalAlignmentProperty);
            KeybordVerticalAlignment = (VerticalAlignment)host.GetValue(KeyboardControl.KeyboardVerticalAlignmentProperty);
            if (PlaceHolder != null)
            {
                var grid = PlaceHolder.Parent as Grid;
                var point = host.TranslatePoint(new Point(0, 0), grid);
                 
                Show(point);

                var list = System.Windows.Forms.InputLanguage.InstalledInputLanguages;
                if (host.Tag != null && host.Tag.ToString() == "Ch")
                {
                    Instance.TextCh.Visibility = Visibility.Visible;
                    Instance.TextEn.Visibility = Visibility.Collapsed;
               
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {

                            if (list[i].LayoutName != "中文(简体) - 美式键盘")
                            {
                                System.Windows.Forms.InputLanguage.CurrentInputLanguage = list[i];
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Instance.TextCh.Visibility = Visibility.Collapsed;
                    Instance.TextEn.Visibility = Visibility.Visible;

                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i].LayoutName == "中文(简体) - 美式键盘")
                            {
                                System.Windows.Forms.InputLanguage.CurrentInputLanguage = list[i];
                                break;
                            }
                        }
                    }

                }
            }
        }


        static double _heightResume = 100;

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

            var horizontalAlignment = KeybordHorizontalAlignment;
            if (horizontalAlignment == HorizontalAlignment.Right)
            {
                location.X = size.Width - Instance.Width;
            }
            else if (horizontalAlignment == HorizontalAlignment.Left) 
            {
                location.X = 0;
            }
            else if (horizontalAlignment == HorizontalAlignment.Center) 
            {
                location.X = (size.Width -Instance.Width) / 2;
            }


            var verticalAlignment = KeybordVerticalAlignment;
            if (verticalAlignment ==  VerticalAlignment.Bottom)
            {
                location.Y = size.Height - Instance.Height;
            }
            else if (verticalAlignment ==  VerticalAlignment.Top)
            {
                location.Y = 0;
            }
            else if (verticalAlignment == VerticalAlignment.Center)
            {
                location.Y = (size.Height-Instance.Height) / 2;
            }

           
           // .GetValue(KeyboardControl.HorizontalAlignmentProperty);

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
