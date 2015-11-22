using System;
using Xamarin.Forms;
using KnobControl;
using KnobControl.Droid;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using System.ComponentModel;

[assembly:ExportRenderer(typeof(Knob), typeof(KnobRenderer))]

namespace KnobControl.Droid
{
    public class KnobRenderer: ViewRenderer<Knob, RoundKnobButton>
    {
		RoundKnobButton knob;
		static Bitmap bmpRotorOn, bmpRotorOff;

		protected override void OnElementChanged(ElementChangedEventArgs<Knob> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || this.Element == null)
				return;

			var context = Forms.Context;
			var metrics = Resources.DisplayMetrics;
			var w = metrics.HeightPixels / 2 - 20;
			var h = metrics.HeightPixels / 2 - 20;

			// Load pictures once
			if (bmpRotorOn == null || bmpRotorOff == null)
			{
				Console.WriteLine ("OnElementChanged :: Create Bitmaps");

				Bitmap srcon = BitmapFactory.DecodeResource (context.Resources, Resource.Drawable.knob_handle);

				float scaleWidth = ((float)w) / srcon.Width;
				float scaleHeight = ((float)h) / srcon.Height;

				Matrix matrix = new Matrix ();
				matrix.PostScale (scaleWidth, scaleHeight);

				bmpRotorOn = Bitmap.CreateBitmap (srcon, 0, 0, srcon.Width, srcon.Height, matrix, true);

				srcon.Recycle ();

				Bitmap srcoff = BitmapFactory.DecodeResource (context.Resources, Resource.Drawable.knob_handle);

				bmpRotorOff = Bitmap.CreateBitmap (srcoff, 0, 0, srcoff.Width, srcoff.Height, matrix, true);

				srcoff.Recycle ();
			}

			Console.WriteLine("KnobRenderer :: OnElementChanged :: Element.Width = " + Element.Width);
			Console.WriteLine("KnobRenderer :: OnElementChanged :: Element.Height = " + Element.Height);
			Console.WriteLine("KnobRenderer :: OnElementChanged :: Element.Bounds.Width = " + Element.Bounds.Width);
			Console.WriteLine("KnobRenderer :: OnElementChanged :: Element.Bounds.Height = " + Element.Bounds.Height);

			knob = new RoundKnobButton(context, Resource.Drawable.knob_bg, bmpRotorOn, bmpRotorOff, w, h);
			knob.ShowTouchPath = Element.ShowTouchPath;
			knob.MaxValue = Element.Max;
			knob.MinValue = Element.Min;
			knob.CurrentValue = Element.Current;

			knob.RotationEnded += (float obj) =>
			{
				Console.WriteLine( "knob.RotationEnded with {0} ", obj );

				if (Element != null && Element.RotationEnded != null)
				{
					Element.RotationEnded((float)obj);
				}
			};

			knob.IsEnabled = Element.IsEnabled;

			knob.OnRotate += ((float obj) =>
				{
					Console.WriteLine("percentage = {0}", obj);
				});

			SetNativeControl(knob);
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (Control == null || Element == null) return;

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
				// var frame = new Rect((nfloat)Element.Bounds.X, (nfloat)Element.Bounds.Y, (nfloat)Element.Bounds.Width, (nfloat)Element.Bounds.Height);
				// Control.Bounds = frame;
			}
			else if (e.PropertyName == Knob.IsEnabledProperty.PropertyName)
			{
				Control.IsEnabled = Element.IsEnabled;
			}
		}
    }
}