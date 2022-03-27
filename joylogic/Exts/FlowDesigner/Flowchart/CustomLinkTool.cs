using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aga.Diagrams.Tools;
using Aga.Diagrams.Controls;
using System.Windows;
using Aga.Diagrams;
using System.Windows.Controls;

namespace FlowDesigner.Flowchart
{
    class CustomLinkTool : LinkTool
    {
        public CustomLinkTool(DiagramView view)
            : base(view)
        {
        }

        public override bool CanDrop()
        {
            bool can = base.CanDrop();
            if (can)
            {
                var target = this.Adorner.Port as PortBase;
                PortBase source = null;
                if (Thumb == LinkThumbKind.Target)
                {
                    source = Link.Source as PortBase;

                }
                else
                {
                    source = Link.Target as PortBase;

                }
                if (Thumb == LinkThumbKind.Target)
                {
                    if (source.Links.Count > 1) { return false; }
                }
                else
                {
                    if (target.Links.Count > 1) { return false; }
                }
                if (source.Links.Where(a => a.Target == target || a.Source == target).Count() > 1)
                {
                    return false;
                }


                return
                    target.HorizontalAlignment == HorizontalAlignment.Center ||
                    source.HorizontalAlignment == HorizontalAlignment.Center ||
                    target.HorizontalAlignment == source.HorizontalAlignment;
            }
            return true;
        }

        protected override ILink CreateNewLink(IPort port)
        {
            // var item = new OrthogonalLink();
            var item = new MyLink();
            BindNewLinkToPort(port, item);
            return item;
        }

        protected override void UpdateLink(Point point, IPort port)
        {
            base.UpdateLink(point, port);
        }
    }
}
