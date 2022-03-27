using Aga.Diagrams.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aga.Diagrams
{
    public static class VisualHelper
    {
        public static T FindParent<T>(this DependencyObject value) where T : DependencyObject
        {
            DependencyObject parent = value;
            while (parent != null && !(parent is T))
                parent = VisualTreeHelper.GetParent(parent);
            return parent as T;
        }

        public static DiagramItem FindChild(this DiagramItem value, Point p)
        {
            DiagramItem child = value;
            DoChild(child, p, ref child);
            return child;

        }

        private static void DoChild(Control child, Point p, ref DiagramItem set)
        {
            Control temp = child;


            var count = VisualTreeHelper.GetChildrenCount(child);
            for (int i = 0; i < count; i++)
            {
                temp = VisualTreeHelper.GetChild(child, i) as Control;
                if (temp == null) { continue; }
                var bounds = GetBounds(temp);
                if (bounds.Contains(p))
                {
                    if (temp is DiagramItem)
                    {
                        set = temp as DiagramItem;
                    }

                    p.X -= bounds.Left;
                    p.Y -= bounds.Top;

                    DoChild(temp, p, ref set);
                    break;
                }

            }

        }

        private static Rect GetBounds(Control v)
        {
            var x = Canvas.GetLeft(v);
            var y = Canvas.GetTop(v);
            return new Rect(x, y, v.ActualWidth, v.ActualHeight);
        }





        /*public static Point GetWindowPosition(this System.Windows.Input.MouseEventArgs e, DependencyObject relativeTo)
        {
            var parentWindow = Window.GetWindow(relativeTo);
            return e.GetPosition(parentWindow);
        }*/

        /*public static Point ClientToScreen(this UIElement value, Point point)
        {
            var parentWindow = Window.GetWindow(value);
            return value.TranslatePoint(point, parentWindow);
        }*/
    }
}
