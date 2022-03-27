using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aga.Diagrams.Tools;
using Aga.Diagrams.Controls;
using System.Windows;
using Aga.Diagrams;
using System.Windows.Controls;

namespace FlowViewModule.Flowchart
{
    class CustomLinkTool : LinkTool
    {
        public CustomLinkTool(DiagramView view)
            : base(view)
        {
        }

        public override bool CanDrop()
        {
            return false;
        }

        protected override ILink CreateNewLink(IPort port)
        {
            return null; 
        }

        protected override void UpdateLink(Point point, IPort port)
        { 
        }
    }
}
