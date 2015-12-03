using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using KnobControl;
using KnobControl.iOS;
using CoreGraphics;
using UIKit;

[assembly:ExportRenderer(typeof(Knob), typeof(KnobRenderer))]

namespace KnobControl.iOS
{
    public class KnobRenderer : ViewRenderer<Knob, UIKnobControl>
    {
        public KnobRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Knob> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || this.Element == null)
                return;

            Console.WriteLine("OnElementChanged :: Element.Width" + Element.Width);
            Console.WriteLine("OnElementChanged :: Element.Height" + Element.Height);
            Console.WriteLine("OnElementChanged :: Element.Bounds.Width" + Element.Bounds.Width);
            Console.WriteLine("OnElementChanged :: Element.Bounds.Height" + Element.Bounds.Height);

            var mainScreen = UIScreen.MainScreen.Bounds;

            var frame = new CGRect((nfloat)Element.Bounds.X, (nfloat)Element.Bounds.Y, (nfloat)mainScreen.Height/2-20, (nfloat)mainScreen.Height/2-20);
            var knob = new UIKnobControl(frame, Element.Min, Element.Max, Element.Current, Element.ShowTouchPath);

            knob.RotationEnded += (nfloat obj) =>
            {
                if(Element != null)
                {
                    Element.Current = (float)obj;
                }
            };

            SetNativeControl(knob);
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control == null || Element == null)
                return;

            if (e.PropertyName == Knob.MaxProperty.PropertyName)
            {
                Control.MaxValue = Element.Max;
            }
            else if (e.PropertyName == Knob.MinProperty.PropertyName)
            {
                Control.MinValue = Element.Min;
            }
            else if (e.PropertyName == Knob.CurrentProperty.PropertyName)
            {
                Control.CurrentValue = Element.Current;
            }
            else if (e.PropertyName == Knob.BoundsProperty.PropertyName)
            {
                var frame = new CGRect((nfloat)Element.Bounds.X, (nfloat)Element.Bounds.Y, (nfloat)Element.Bounds.Width, (nfloat)Element.Bounds.Height);
                Control.Bounds = frame;
            }
        }
    }
}