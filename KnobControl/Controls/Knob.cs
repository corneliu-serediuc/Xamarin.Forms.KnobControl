using System;
using Xamarin.Forms;
using System.Diagnostics;

namespace KnobControl
{
    public class Knob : View
    {
        public static readonly BindableProperty MinProperty = BindableProperty.Create<Knob,float> (p => p.Min, 0);
        public static readonly BindableProperty MaxProperty = BindableProperty.Create<Knob,float> (p => p.Max, 100);
        public static readonly BindableProperty CurrentProperty = BindableProperty.Create<Knob,float> (p => p.Current, 50);
        public static readonly BindableProperty BoundsProperty = BindableProperty.Create<Knob,Rectangle> (p => p.Bounds, new Rectangle(0,0,300,300));
        public static readonly BindableProperty ShowTouchPathProperty = BindableProperty.Create<Knob,bool> (p => p.ShowTouchPath, false);

        public float Min {
            get { return (float) GetValue(MinProperty); }
            set { SetValue (MinProperty, value); OnPropertyChanged(); }
        }

        public float Max {
            get { return (float) GetValue(MaxProperty); }
            set { SetValue (MaxProperty, value); OnPropertyChanged(); }
        }

        public float Current {
            get { return (float) GetValue(CurrentProperty); }
            set { SetValue (CurrentProperty, value); OnPropertyChanged();}
        }

		public new Rectangle Bounds {
			get { return (Rectangle)GetValue(BoundsProperty); }
			set { SetValue(BoundsProperty, value); }
		}

		public bool ShowTouchPath {
			get { return (bool)GetValue(ShowTouchPathProperty); }
			set { SetValue(ShowTouchPathProperty, value); }
		}
            
		public bool IsRotationEndedRegistered { get { return RotationEnded != null; }}

		public Action<float> RotationStarted;
		public Action<float> RotationEnded;

		public Knob(Rectangle frame, float min, float max, float current, bool showTouchPath = false)
		{
			SetValue (MinProperty, min);
			SetValue (MaxProperty, max);
			SetValue (CurrentProperty, current);
			SetValue (BoundsProperty, frame);
			SetValue (ShowTouchPathProperty, showTouchPath);
		}

		public Knob(Rectangle frame)
		{
			Bounds = frame;
		}
    }
}