using System;
using System.Drawing;

using UIKit;
using CoreGraphics;

namespace KnobControl.iOS
{
    public class UICircleView : UIView
    {
        CGPoint _center;
        nfloat _radius;

        public UICircleView(CGRect rect, CGPoint center, nfloat radius)
            : base(rect)
        {
            _center = center;
            _radius = radius;

            BackgroundColor = UIColor.Clear;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            using (CGContext g = UIGraphics.GetCurrentContext())
            {
                // Set up drawing attributes
                g.SetLineWidth(1);
                UIColor.Red.SetStroke();

                // Set up the drawing path
                var path = new CGPath();
                path.AddArc(_center.X, _center.Y, _radius, 0.0F, 2 * ((float)Math.PI), false);

                // Add path to context and draw
                g.AddPath(path);
                g.DrawPath(CGPathDrawingMode.Stroke);
            }
        }
    }
}

