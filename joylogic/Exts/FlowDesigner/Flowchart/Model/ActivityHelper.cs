using Aga.Diagrams;
using Aga.Diagrams.Controls;
using Core.Infrastructure.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FlowDesigner.Flowchart.Model
{
    public static class ActivityHelper
    {

        public static void CreatePorts(this Activity thiz, Node item)
        {
            foreach (var kind in thiz.GetPorts())
            {
                var port = new Aga.Diagrams.Controls.RectPort();
                port.Width = 30;
                port.Height = 20;
                if (kind == PortKinds.BottomLeft)
                {
                    port.Text = "确定";
                }
                else if (kind == PortKinds.Top)
                {
                    port.Text = "输入";
                }
                else if (kind == PortKinds.TopLeft)
                {
                    port.Text = "完成";
                }
                else if (kind == PortKinds.TopRight)
                {
                    port.Text = "终止";
                }
                else if (kind == PortKinds.Bottom)
                {
                    port.Text = "输出";
                }
                else if (kind == PortKinds.BottomRight)
                {
                    port.Text = "取消";
                }
                port.Margin = new Thickness(-5);
                port.Visibility = Visibility.Visible;

                port.VerticalAlignment = thiz.ToVerticalAligment(kind);
                port.HorizontalAlignment = thiz.ToHorizontalAligment(kind);
                port.CanAcceptIncomingLinks = thiz.ToVerticalAligment(kind) == VerticalAlignment.Top;
                port.CanAcceptOutgoingLinks = !port.CanAcceptIncomingLinks;
                port.Tag = kind;
                port.Cursor = Cursors.Cross;
                port.CanCreateLink = true;
                item.Ports.Add(port);
            }
        }

        public static Aga.Diagrams.Controls.IPort FindPort(this Activity thiz, DiagramView _view, PortKinds portKind)
        {
            var inode = _view.Items.FirstOrDefault(p => p.ModelElement == thiz) as Aga.Diagrams.Controls.INode;
            if (inode == null)
                return null;
            var port = inode.Ports.OfType<FrameworkElement>().FirstOrDefault(
                p => p.VerticalAlignment == thiz.ToVerticalAligment(portKind)
                    && p.HorizontalAlignment == thiz.ToHorizontalAligment(portKind)
                );
            return (Aga.Diagrams.Controls.IPort)port;
        }

        public static Node UpdateNode(Activity thiz, bool reset, bool inner = false, bool createContent = false)
        {
            if (reset) { thiz.Item = null; }
            var item = thiz.Item as Node;
            if (thiz.Item == null)
            {
                thiz.Item = new Node();
                item = thiz.Item as Node;
                item.ModelElement = thiz;
                if (!inner)
                {
                    CreatePorts(thiz, item);
                }
                item.Content = thiz.CreateContent();
            }
            else
            {
                item = thiz.Item as Node;
                if (createContent)
                {
                    if (!inner)
                    {
                        CreatePorts(thiz, item);
                    }
                    item.Content = thiz.CreateContent();
                }
            }
            item = thiz.Item as Node;
            item.Width = thiz.Size.Width;
            item.Height = thiz.Size.Height;

            item.CanResize = true;
            item.SetValue(Canvas.LeftProperty, thiz.Location.X);
            item.SetValue(Canvas.TopProperty, thiz.Location.Y);


            return item;
        }




        public static VerticalAlignment ToVerticalAligment(this Activity thiz, PortKinds kind)
        {
            if (kind == PortKinds.Top || kind == PortKinds.TopLeft || kind == PortKinds.TopRight)
                return VerticalAlignment.Top;
            if (kind == PortKinds.Bottom || kind == PortKinds.BottomLeft || kind == PortKinds.BottomRight)
                return VerticalAlignment.Bottom;
            else
                return VerticalAlignment.Center;
        }

        public static HorizontalAlignment ToHorizontalAligment(this Activity thiz, PortKinds kind)
        {
            if (kind == PortKinds.Left || kind == PortKinds.BottomLeft || kind == PortKinds.TopLeft)
                return HorizontalAlignment.Left;
            if (kind == PortKinds.Right || kind == PortKinds.BottomRight || kind == PortKinds.TopRight)
                return HorizontalAlignment.Right;
            else
                return HorizontalAlignment.Center;
        }


        public static IEnumerable<PortKinds> GetPorts(this Activity thiz)
        {
            switch (thiz.Kind)
            {
                case NodeKinds.Start:
                    yield return PortKinds.Bottom;
                    break;
                case NodeKinds.End:
                    yield return PortKinds.TopLeft;
                    yield return PortKinds.TopRight;
                    break;
                case NodeKinds.View: 
                case NodeKinds.Service: 
                case NodeKinds.Condition: 
                case NodeKinds.SwitchCase:
                case NodeKinds.Foreach:
                case NodeKinds.CaseView:
                case NodeKinds.Composed:
                    yield return PortKinds.Top;
                    yield return PortKinds.BottomLeft;
                    yield return PortKinds.BottomRight;
                    break;
            }
        }

        public static Activity Create(NodeKinds k)
        {
            var activity = Activity.Create(k);
            activity.UpdateContent = UpdateNode;
            return activity;
        }



    }
}
