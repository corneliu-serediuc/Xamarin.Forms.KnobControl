using System;
using System.Drawing;

using UIKit;
using CoreGraphics;

namespace KnobControl.iOS
{
    public class UIKnobControl : UIView
    {
        private nfloat _maxValue;
        private nfloat _minValue;
        private nfloat _currentValue;

        private nfloat _imageAngle;
        private UIImageView _handleImage;
        private UIImageView _bgImage;
        private RotationGestureRecognizer _gestureRecognizer;
        private UILabel _valueLabel;

        private CGPoint _midPoint;
        private nfloat _outRadius;
        private nfloat _innerRadius;
        private bool _showTouchPath;

        public Action<nfloat> RotationEnded;

        public nfloat MaxValue {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        public nfloat MinValue {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public nfloat CurrentValue {
            get { return _currentValue; }
            set { _currentValue = value; }
        }

        public bool ShowTouchPath {
            get { return _showTouchPath; }
        }

        public UIKnobControl(CGRect frame, nfloat minValue, nfloat maxValue, nfloat currentValue, bool showTouchPath = false) : base(frame)
        {
            _maxValue = maxValue;
            _minValue = minValue;
            _currentValue = currentValue;
            _showTouchPath = showTouchPath;

            Initialize();
            SetupGestureRecognizer();
        }

        private void Initialize ()
        {
            BackgroundColor = UIColor.Clear;

            Console.WriteLine("UIKnobControl.Bounds.Width = " + Bounds.Width);
            Console.WriteLine("UIKnobControl.Bounds.Height = " + Bounds.Height);

            _bgImage = new UIImageView(UIImage.FromFile("knob_bg.png"))
            {
                Frame = Bounds,
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            AddSubview(_bgImage);

            _handleImage = new UIImageView(UIImage.FromFile("knob_handle.png"))
            {
                Frame = Bounds,
                ContentMode = UIViewContentMode.ScaleAspectFit,
            };

            AddSubview(_handleImage);

            _valueLabel = new UILabel()
            {
                    Frame = Bounds,
                    Font = UIFont.SystemFontOfSize(22.0F),
                    TextColor = UIColor.Gray,
                    Text = _currentValue.ToString(),
                    TextAlignment = UITextAlignment.Center
            };

            AddSubview(_valueLabel);
        }

        public void SetupGestureRecognizer()
        {
            _midPoint = new CGPoint(_handleImage.Frame.X + _handleImage.Frame.Size.Width / 2, _handleImage.Frame.Y + _handleImage.Frame.Size.Height / 2);

            _outRadius = _handleImage.Frame.Size.Width / 3 + 12.0f;
            _innerRadius = _outRadius / 2;

            if (_showTouchPath)
            {
                var bigCircle = new UICircleView(Bounds, _midPoint, _outRadius);
                var smallCircle = new UICircleView(Bounds, _midPoint, _innerRadius);

                Add(bigCircle);
                Add(smallCircle);
            }

            _imageAngle = _currentValue.ToCurrentAngle(_minValue, _maxValue);
            _handleImage.Transform = CGAffineTransform.MakeRotation(_imageAngle * ((float)Math.PI) / 180);

            _gestureRecognizer = new RotationGestureRecognizer(_midPoint, _innerRadius, _outRadius);

            _gestureRecognizer.RotationStarted += ((nfloat cumulatedAngle) =>
            {
                    _imageAngle += cumulatedAngle;

                    if (_imageAngle < -145)
                        _imageAngle = -145;
                    else if (_imageAngle > 145)
                        _imageAngle = 145;

                    InvokeOnMainThread(() =>
                        {
                            // Rotate image and update text field
                            _handleImage.Transform = CGAffineTransform.MakeRotation(_imageAngle * ((float)Math.PI) / 180);
                            _currentValue = _imageAngle.ToCurrentValue(_minValue, _maxValue);
                            _valueLabel.Text = Convert.ToInt32(_currentValue).ToString();

                            if (RotationEnded != null)
                                RotationEnded(_currentValue);
                        });
            });

            _gestureRecognizer.Rotating += (nfloat angle) =>
                {
                    // Calculate rotation angle
                    _imageAngle += angle;

                    /*
                    // Use this setting if you want to go round and round endlessly
                    if (imageAngle > 360)
                        imageAngle -= 360;
                    else if (imageAngle < -360)
                        imageAngle += 360;
                    */

                    Console.WriteLine ("_imageAngle = {0}", _imageAngle);

                    if (_imageAngle < -145)
                        _imageAngle = -145;
                    else if (_imageAngle > 145)
                        _imageAngle = 145;

                    InvokeOnMainThread(() =>
                        {
                            // Rotate image and update text field
                            _handleImage.Transform = CGAffineTransform.MakeRotation(_imageAngle * ((float)Math.PI) / 180);
                            _currentValue = _imageAngle.ToCurrentValue(_minValue, _maxValue);
                            _valueLabel.Text = Convert.ToInt32(_currentValue).ToString();

                            if (RotationEnded != null)
                                RotationEnded(_currentValue);
                        });
                };

            _gestureRecognizer.RotationEnded += (nfloat obj) =>
                {
                    Console.WriteLine("CurrentValue = " + _imageAngle.ToCurrentValue(_minValue, _maxValue));
                    _currentValue = _imageAngle.ToCurrentValue(_minValue, _maxValue);
                    _valueLabel.Text = Convert.ToInt32(_currentValue).ToString();

                    if (RotationEnded != null)
                        RotationEnded(_currentValue);
                };
           
            AddGestureRecognizer(_gestureRecognizer);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _handleImage = null;
            _gestureRecognizer = null;
            _valueLabel = null;
        }
    }

    public static class UIKnobControlExtenstions {

        public static nfloat ToCurrentValue(this nfloat angle, nfloat min, nfloat max)
        {
            return (max-min) * (angle+145) / 290;
        }

        public static nfloat ToCurrentAngle(this nfloat currentValue, nfloat min, nfloat max)
        {
            return (290 * currentValue / (max-min)) - 145;
        }
    }
}