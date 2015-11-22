using System;
using System.Drawing;

using UIKit;
using Foundation;
using CoreGraphics;

namespace KnobControl.iOS
{
    public class RotationGestureRecognizer : UIGestureRecognizer
    {
        CGPoint _midPoint;
        nfloat _innerRadius;
        nfloat _outerRadius;
        nfloat _cumulatedAngle;

        public Action<nfloat> RotationStarted;
        public Action<nfloat> Rotating;
        public Action<nfloat> RotationEnded;

        public RotationGestureRecognizer(CGPoint midPoint, nfloat innerRadius, nfloat outerRadius)
        {
            _midPoint = midPoint;
            _innerRadius = innerRadius;
            _outerRadius = outerRadius;
        }

        public override void Reset()
        {
            Console.WriteLine("RotationGestureRecognizer :: Reset");
            base.Reset();

            if (RotationEnded != null)
                RotationEnded(_cumulatedAngle);

            _cumulatedAngle = 0;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            Console.WriteLine("RotationGestureRecognizer :: TouchesBegan");
            base.TouchesBegan(touches, evt);

            if (touches.Count != 1)
            {
                this.State = UIGestureRecognizerState.Failed;
                return;
            }
            else
            {
                CGPoint nowPoint = (touches.AnyObject as UITouch).LocationInView(View);
                CGPoint prevPoint = (touches.AnyObject as UITouch).PreviousLocationInView(View);

                // Get distance
                var distance = DistanceBetweenPoints(_midPoint, nowPoint);

                // Make sure the point is within the area
                if (_innerRadius <= distance && distance <= _outerRadius)
                {
                    var angle = AngleBetweenLinesInDegrees(_midPoint, prevPoint, _midPoint, nowPoint);

                    if (angle > 180)
                    {
                        angle -= 360;
                    }
                    else if (angle < -180)
                    {
                        angle += 360;
                    }

                    if (RotationStarted != null)
                    {
                        RotationStarted(angle);
                    }
                }
                else
                {
                    this.State = UIGestureRecognizerState.Failed;
                    return;
                }
            }
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            Console.WriteLine("RotationGestureRecognizer :: TouchesMoved");
            base.TouchesMoved(touches, evt);

            if (this.State == UIGestureRecognizerState.Failed)
                return;

            CGPoint nowPoint = (touches.AnyObject as UITouch).LocationInView(View);
            CGPoint prevPoint = (touches.AnyObject as UITouch).PreviousLocationInView(View);

            // Get distance
            var distance = DistanceBetweenPoints(_midPoint, nowPoint);

            // Make sure the new point is within the area
            if (_innerRadius <= distance && distance <= _outerRadius)
            {
                // Calculate rotation angle between two points
                var angle = AngleBetweenLinesInDegrees(_midPoint, prevPoint, _midPoint, nowPoint);

                // Fix value, if the 12 o'clock position is between prevPoint and nowPoint
                if (angle > 180)
                {
                    angle -= 360;
                }
                else if (angle < -180)
                {
                    angle += 360;
                }

                // Sum up single steps
                _cumulatedAngle += angle;
                Console.WriteLine("_cumulatedAngle = {0}", _cumulatedAngle);

                // Call delegate
                if (Rotating != null)
                {
                    Rotating(angle);
                }
            }
            else
            {
                // Finger moved outside the area
                this.State = UIGestureRecognizerState.Failed;
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            Console.WriteLine("RotationGestureRecognizer :: TouchesEnded");
            base.TouchesEnded(touches, evt);

            if (this.State == UIGestureRecognizerState.Possible)
            {
                this.State = UIGestureRecognizerState.Recognized;
            }
            else
            {
                this.State = UIGestureRecognizerState.Failed;
            }

            _cumulatedAngle = 0;
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            Console.WriteLine("RotationGestureRecognizer :: TouchesCancelled");
            base.TouchesCancelled(touches, evt);

            this.State = UIGestureRecognizerState.Failed;

            _cumulatedAngle = 0;
        }

        private nfloat DistanceBetweenPoints(CGPoint point1, CGPoint point2)
        {
            var dx = point1.X - point2.X;
            var dy = point1.Y - point2.Y;

            return (nfloat) Math.Sqrt(dx*dx + dy*dy);
        }

        private nfloat AngleBetweenLinesInDegrees(CGPoint beginLineA, CGPoint endLineA, CGPoint beginLineB, CGPoint endLineB)
        {
            nfloat a = endLineA.X - beginLineA.X;
            nfloat b = endLineA.Y - beginLineA.Y;
            nfloat c = endLineB.X - beginLineB.X;
            nfloat d = endLineB.Y - beginLineB.Y;

            nfloat atanA = (nfloat) Math.Atan2(a, b);
            nfloat atanB = (nfloat) Math.Atan2(c, d);
            nfloat pi = (nfloat) Math.PI;

            // Convert radiants to degrees
            return (atanA - atanB) * 180 / pi;
        }
    }
}