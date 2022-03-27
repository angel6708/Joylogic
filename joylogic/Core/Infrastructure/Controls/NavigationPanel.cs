using Core.Infrastructure.Events;
using Core.Infrastructure.Services;
using Core.Infrastructure.ViewModels;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Core.Infrastructure.Controls
{
    public class NavigationPanel : Grid
    {


        private readonly TranslateTransform NavigationPanelTranslateTransform;
        private readonly ScaleTransform NavigationPanelScaleTransform; 

        private static readonly TimeSpan DURATION = TimeSpan.FromMilliseconds(700);

        private static readonly int FrameCount = 20;
        private readonly double Zoom = 0.97;

        private readonly ScrollViewer TempNavigatingContainer;


        public NavigationPanel()
            : base()
        {

            NavigationPanelTranslateTransform = new TranslateTransform();
            NavigationPanelScaleTransform = new ScaleTransform(Zoom,Zoom);
            // The Temp Container When navigating.
            TempNavigatingContainer = new ScrollViewer();
            TempNavigatingContainer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            TempNavigatingContainer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RenderTransform = NavigationPanelTranslateTransform;

            //the left element
            Border border = new Border();
            border.BorderBrush = Brushes.LightBlue;
            border.BorderThickness = new Thickness(2); 
            border.Margin = new Thickness(0, 10, 0, 10);
            border.SetValue(Grid.ColumnProperty, 0);
            border.RenderTransform = NavigationPanelScaleTransform;
            grid.Children.Add(border);

            //the right element
            border = new Border();
            border.BorderBrush = Brushes.LightBlue;
            border.BorderThickness = new Thickness(2);
            border.Margin = new Thickness(0, 10, 0, 10);
            border.SetValue(Grid.ColumnProperty, 1);
            border.RenderTransform = NavigationPanelScaleTransform;
            grid.Children.Add(border);

            TempNavigatingContainer.Content = grid; 

        }


        public void Next(FrameworkElement e)
        {
            this.Navigate(e, true);
        }

        public void Replace(FrameworkElement e)
        {
            this.Children.Clear();
            this.Children.Add(e);
        }

        public void Previous(FrameworkElement e)
        {
            this.Navigate(e, false);
        }

        private void Navigate(FrameworkElement e, bool next)
        {


            var count = this.Children.Count;
            if (this.Children.Contains(e)) { return; }
            if (count == 0)
            {
                this.Children.Clear();
                this.Children.Add(e);
                return;
            }
            ContainerService.Current.GetInstance<IEventAggregator>()
            .GetEvent<ShowLoadingEvent>()
            .Publish(new ShowLoadingEventArg { Caption = "Navigating...", Status = System.Windows.Visibility.Visible });


            var theOld = this.Children[0] as FrameworkElement;
            var theNew = e;

            var width = this.ActualWidth;
            var height = this.ActualHeight;

            theOld.Height = height * Zoom;
            theOld.Width = width * Zoom;

            theNew.Width = width * Zoom;
            theNew.Height = height * Zoom;

            if (next)
            {
                FrameworkElement temp = theOld;
                theOld = theNew;
                theNew = temp;
            }
            this.Children.Clear();

            var grid = TempNavigatingContainer.Content as Grid;
            var borderLeft = grid.Children[0] as Border;
            borderLeft.Child = theOld;

            var borderRight = grid.Children[1] as Border;
            borderRight.Child = theNew;
            this.Children.Add(TempNavigatingContainer);

            if (count != 0)
            {
                double start = width * (1 - Zoom) / 2;
                double end = -width * Zoom + start;
                if (next)
                {
                    var temp = start;
                    start = end;
                    end = temp;

                }
                AnimationChildren(next, e, start, end);
            }

        }
         

         
        private void AnimationChildren(bool forward, FrameworkElement e, double start, double end)
        {
            DoubleAnimation da = new DoubleAnimation(start, end, DURATION);

            da.SetValue(Timeline.DesiredFrameRateProperty, FrameCount);
            da.EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut };
            RenderOptions.SetBitmapScalingMode(da, BitmapScalingMode.Linear);

            da.Completed += (o1, e1) =>
            {
                var grid = ((this.Children[0] as ScrollViewer).Content as Grid);
                var boders = grid.Children;
                foreach (Border bd in boders) 
                {
                    bd.Child = null;
                } 
                this.Children.Clear();
                e.Width = double.NaN;
                e.Height = double.NaN;
                this.Children.Add(e);
                e.RenderTransform = null;
                
                ContainerService.Current.GetInstance<IEventAggregator>()
                    .GetEvent<ShowLoadingEvent>()
                    .Publish(new ShowLoadingEventArg { Caption = "", Status = System.Windows.Visibility.Collapsed });

            };



            NavigationPanelTranslateTransform.BeginAnimation(TranslateTransform.XProperty, da);
        }


    }

}
