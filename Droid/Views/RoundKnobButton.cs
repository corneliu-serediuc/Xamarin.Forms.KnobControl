using System;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Content.Res;

namespace KnobControl.Droid
{
    public class RoundKnobButton : RelativeLayout, GestureDetector.IOnGestureListener
    {
		private GestureDetector gestureDetector;
		private float mAngleDown, mAngleUp;
		private ImageView m_RotorImaveView;
		private Bitmap m_BmpRotorOn, m_BmpRotorOff;
		private bool m_State = false, m_ShowTouchPath = false, m_IsEnabled = false, m_IsScrolling = false;
		private int m_Width = 0, m_Height = 0;
		private TextView m_CurrentValueTextView;
		private CircleView m_InnerCircle, m_OuterCircle;

		public Action<bool> OnStateChanged;
		public Action<float> OnRotate;

		public Action<float> RotationEnded;

		private float m_MaxValue = 100;
		private float m_MinValue = 0;
		private float m_CurrentValue;

		public float MaxValue
		{
			get
			{
				return m_MaxValue;
			}
			set
			{
				// Check if max > min
				if (value < m_MinValue)
				{
					// Reverse values
					m_MaxValue = m_MinValue;
					m_MinValue = value;
				}
				else
				{
					m_MaxValue = value;
				}

				// SetRotorPercentage((int)CurrentValue);
			}
		}

		public float MinValue
		{
			get { return m_MinValue; }
			set
			{ 
				// Check if max > min
				if (value > m_MaxValue)
				{
					// Reverse values
					m_MinValue = m_MaxValue;
					m_MaxValue = value;
				}
				else
				{
					m_MinValue = value;
				}

				// SetRotorPercentage((int)CurrentValue);
			}
		}

		public float CurrentValue
		{
			get
			{
				// Check if current value is set
				if (m_CurrentValue == float.MinValue)
					// Set to center
					return m_MinValue + (m_MaxValue - m_MinValue) / 2;
				else
					return m_CurrentValue;
			}
			set
			{
				m_CurrentValue = value;

				if (m_CurrentValueTextView != null)
				{
					m_CurrentValueTextView.Text = Convert.ToInt32(m_CurrentValue).ToString();
				}
					
				SetRotorPercentage((int)value);
			}
		}

		public bool ShowTouchPath
		{
			get { return m_ShowTouchPath; }
			set
			{
				m_ShowTouchPath = value;

				if (m_InnerCircle != null)
				{
					m_InnerCircle.Visibility = (m_ShowTouchPath) ? ViewStates.Visible : ViewStates.Invisible;
				}

				if (m_OuterCircle != null)
				{
					m_OuterCircle.Visibility = (m_ShowTouchPath) ? ViewStates.Visible : ViewStates.Invisible;
				}
			}
		}

		public bool IsEnabled
		{
			get
			{ 
				Console.WriteLine("RoundKnobButton :: IsEnabled :: get :: {0}", m_IsEnabled);
				return m_IsEnabled;
			}
			set
			{
				Console.WriteLine("RoundKnobButton :: IsEnabled :: set :: {0}", value);
				m_IsEnabled = value;
			}
		}

		public RoundKnobButton(Context context, int back, Bitmap bmpRotorOn, Bitmap bmpRotorOff, int w, int h)
            : base(context)
        {
			Console.WriteLine("RoundKnobButton :: ctor :: Start");

			// We won't wait for our size to be calculated, we'll just store our fixed size
			m_Width = w; 
			m_Height = h;

			m_BmpRotorOn = bmpRotorOn;
			m_BmpRotorOff = bmpRotorOff;

			//
			// Create stator
			//
			ImageView ivBack = new ImageView(context);
			ivBack.SetImageResource(back);
			RelativeLayout.LayoutParams lp_ivBack = new RelativeLayout.LayoutParams(w, h);
			lp_ivBack.AddRule(LayoutRules.CenterInParent);
			AddView(ivBack, lp_ivBack);

			// 
			// Create Rotor
			// 

			m_RotorImaveView = new ImageView(context);
			m_RotorImaveView.SetImageBitmap(bmpRotorOff);

			RelativeLayout.LayoutParams lp_ivKnob = new RelativeLayout.LayoutParams(w, h);//LayoutParams.WRAP_CONTENT, LayoutParams.WRAP_CONTENT);
			lp_ivKnob.AddRule(LayoutRules.CenterInParent);
			AddView(m_RotorImaveView, lp_ivKnob);

			SetState(true);

			//
			// Create Current Value Text
			//
			m_CurrentValueTextView = new TextView(context);
			m_CurrentValueTextView.SetTextSize(Android.Util.ComplexUnitType.Dip, 20);
			m_CurrentValueTextView.Gravity = GravityFlags.Center;
			RelativeLayout.LayoutParams lp_cvKnob = new RelativeLayout.LayoutParams(LayoutParams.WrapContent, h / 2);
			lp_cvKnob.AddRule(LayoutRules.CenterInParent);
			AddView(m_CurrentValueTextView, lp_cvKnob);

			// Border for TextView
			GradientDrawable gd = new GradientDrawable();
			gd.SetColor(20); // Changes this drawbale to use a single color instead of a gradient
			gd.SetCornerRadius(5);
			gd.SetStroke(1, Color.LightGray);
			// m_CurrentValueTextView.Background = gd;

			//
			// Circles
			//
			m_InnerCircle = new CircleView(context, w, h, w / 8);
			m_InnerCircle.Visibility = (m_ShowTouchPath) ? ViewStates.Visible : ViewStates.Invisible;
			RelativeLayout.LayoutParams lp_circle = new RelativeLayout.LayoutParams(w, h);
			lp_circle.AddRule(LayoutRules.CenterInParent);
			AddView(m_InnerCircle, lp_circle);

			m_OuterCircle = new CircleView(context, w, h, w / 3);
			m_OuterCircle.Visibility = (m_ShowTouchPath) ? ViewStates.Visible : ViewStates.Invisible;
			RelativeLayout.LayoutParams lp_circle2 = new RelativeLayout.LayoutParams(w, h);
			lp_circle2.AddRule(LayoutRules.CenterInParent);
			AddView(m_OuterCircle, lp_circle2);

			// Enable Gesture Detector
			gestureDetector = new GestureDetector(Context, this);
        }

		public void SetState(bool state)
		{
			m_State = state;
			m_RotorImaveView.SetImageBitmap(state ? m_BmpRotorOn : m_BmpRotorOff);
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (gestureDetector.OnTouchEvent(e) && m_IsEnabled)
				return true;

			if (e.Action == MotionEventActions.Up)
			{
				if (m_IsScrolling)
				{
					Console.WriteLine("OnTouchEvent :: ACTION_UP");
					m_IsScrolling = false;

					if (RotationEnded != null)
					{
						RotationEnded(m_CurrentValue);
					}
				}
			}
				
			return false;
		}

		public bool OnDown(MotionEvent e)
		{
			Console.WriteLine("OnDown :: Start");

			float x = e.GetX() / ((float)Width);
			float y = e.GetY() / ((float)Height);

			// 1- to correct our custom axis direction
			mAngleDown = CartesianToPolar(1 - x, 1 - y);

			Console.WriteLine("OnDown :: Start");

			return true;
		}

		public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			Console.WriteLine("OnFling :: Start");
			return false;
		}

		public void OnLongPress(MotionEvent e)
		{
			//
		}

		public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			Console.WriteLine("OnScroll :: Start :: -------------");

			m_IsScrolling = true;

			float x = e2.GetX() / ((float)Width);
			float y = e2.GetY() / ((float)Height);

			// 1- to correct our custom axis direction
			float rotDegrees = CartesianToPolar(1 - x, 1 - y);

			Console.WriteLine("OnScroll :: Start :: rotDegrees = {0}", rotDegrees);

			if (!float.IsNaN(rotDegrees))
			{
				// Instead of getting 0 -> 180, -180 -> 0, we go for 0 -> 360
				float posDegrees = rotDegrees;
				if (rotDegrees < 0)
					posDegrees = 360 + rotDegrees;

				// Deny full rotation, start and stop point, and get a linear scale
				if (posDegrees > 210 || posDegrees < 150)
				{
					// Rotate our imageview
					SetRotorPosAngle(posDegrees);

					// Get a linear scale
					float scaleDegrees = rotDegrees + 150; // given the current parameters, we go from 0 to 300

					Console.WriteLine("OnScroll :: scaleDegrees = {0}", scaleDegrees);

					// Get position percent
					float factor = 300 / (m_MaxValue - m_MinValue);
					m_CurrentValue = m_MinValue + scaleDegrees / factor;

					if (m_CurrentValueTextView != null)
						m_CurrentValueTextView.Text = Convert.ToInt32(m_CurrentValue).ToString ();

					if (OnRotate != null)
						OnRotate(m_CurrentValue);

					Console.WriteLine("OnScroll :: End :: true");
					return true; //consumed
				}
				else
				{
					Console.WriteLine("OnScroll :: End :: False Case 1");
					return false;
				}
			}
			else
			{
				Console.WriteLine("OnScroll :: End :: False :: Case 2");
				return false; // not consumed
			}
		}

		public void OnShowPress(MotionEvent e)
		{
			Console.WriteLine("OnShowPress :: Start");
		}

		public bool OnSingleTapUp(MotionEvent e)
		{
			Console.WriteLine("OnSingleTapUp :: Start");
			float x = e.GetX() / ((float)Width);
			float y = e.GetY() / ((float)Height);

			// 1- to correct our custom axis direction
			mAngleUp = CartesianToPolar(1 - x, 1 - y);

			// if we click up the same place where we clicked down, it's just a button press
			if (!float.IsNaN(mAngleDown) &&
				!float.IsNaN(mAngleUp) &&
				Math.Abs(mAngleUp - mAngleDown) < 10)
			{
				if (OnStateChanged != null)
					OnStateChanged(m_State);
			}

			Console.WriteLine("OnSingleTapUp :: End");
			return true;
		}

		private float CartesianToPolar(float x, float y)
		{
			return (float)-ToDegrees(Math.Atan2(x - 0.5f, y - 0.5f));
		}

		private double ToDegrees(double rad)
		{
			return (rad / (2 * Math.PI) * 360);
		}

		public void SetRotorPosAngle(float deg)
		{
			Console.WriteLine("posDegree = {0}", deg);

			if (deg >= 210 || deg <= 150)
			{
				if (deg > 180)
					deg = deg - 360;

				// Rotate handle
				Matrix matrix = new Matrix();
				m_RotorImaveView.SetScaleType(ImageView.ScaleType.Matrix);
				matrix.PostRotate((float)deg, m_Width / 2, m_Height / 2);
				m_RotorImaveView.ImageMatrix = matrix;
			}
		}

		public void SetRotorPercentage(float currentValue)
		{
			// Deal with exceptional values
			if (currentValue < m_MinValue)
				currentValue = m_MinValue;

			if (currentValue > m_MaxValue)
				currentValue = m_MaxValue;

			// if (m_CurrentValueTextView != null)
			// m_CurrentValueTextView.Text = currentValue.ToString() + m_Measure;

			float factor = 300 / (m_MaxValue - m_MinValue);

			int posDegree = (int)(currentValue * factor - 150 - m_MinValue * factor);
			if (posDegree < 0)
				posDegree = 360 + posDegree;

			SetRotorPosAngle(posDegree);
		}

		protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
		}
    }
}