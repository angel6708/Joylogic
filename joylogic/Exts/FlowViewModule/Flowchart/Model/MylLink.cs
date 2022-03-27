using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Aga.Diagrams.Controls;
using Aga.Diagrams;
using System.Windows.Controls;
using System.Windows.Media;

namespace FlowViewModule.Flowchart
{
    public class MyLink : SegmentLink
    {
        public MyLink()
        {
        }

        public override void UpdatePath()
        {

            var points = this.GetEndPoinds();
            if (points != null)
            {
                UpdateEdges(points);
            }
            if (points == null) { return; }

            double x1 = points[0].X;
            double y1 = points[0].Y;
            double x4 = points[1].X;
            double y4 = points[1].Y;

            double x2 = x1;
            double y2 = y1 + 100;

            double x3 = x4;
            double y3 = y4 - 100;

            if (y1 > y4)
            {
                y2 += 100;
                y3 -= 100;
            }

            PathFigure subShape = new PathFigure();
            PathSegmentCollection inLine = new PathSegmentCollection();
            PathFigureCollection shapeList = new PathFigureCollection();
            PathGeometry shape = new PathGeometry();
            subShape.StartPoint = new Point(x1, y1);
            subShape.Segments = inLine;
            shapeList.Add(subShape);
            shape.Figures = shapeList;



            this.PathGeometry = shape;
            //pathObject.Stroke = System.Windows.Media.Brushes.Black;
            //pathObject.StrokeThickness = 1;



            BezierSegment b = new BezierSegment();

            b.Point1 = new Point(x2, y2);
            b.Point2 = new Point(x3, y3);
            b.Point3 = new Point(x4, y4);



            inLine.Add(b);
            inLine.Add(new LineSegment() { Point = new Point() { Y = y4 - 10, X = x4 + 5 } });
            inLine.Add(new LineSegment() { Point = new Point() { Y = y4 - 10, X = x4 + 4 } });
            inLine.Add(new LineSegment() { Point = new Point() { Y = y4, X = x4 } });
            inLine.Add(new LineSegment() { Point = new Point() { Y = y4 - 10, X = x4 - 5 } });
            inLine.Add(new LineSegment() { Point = new Point() { Y = y4 - 10, X = x4 - 6 } });




            LabelPosition = new Point(x1 + (x4 - x1) / 2, y4 - (y4 - y1) / 2);

            //Canvas.SetLeft(activityPath.标签, x1 + (x4 - x1) / 2);
            // Canvas.SetTop(activityPath.标签, y4 - (y4 - y1) / 2);
        }

    }
}
