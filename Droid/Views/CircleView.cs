using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Views;

namespace KnobControl.Droid
{
    public class CircleView : View
    {
        private readonly ShapeDrawable _shape;

        public CircleView(Context context, int width, int height, int offset): base(context)
        {
            var paint = new Paint();
            paint.Color = Color.Red;
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = 3;

            _shape = new ShapeDrawable(new OvalShape());
            _shape.Paint.Set(paint);

            _shape.SetBounds(offset, offset, width-offset, height-offset);
        }

        protected override void OnDraw(Canvas canvas)
        {
            _shape.Draw(canvas);
        }
    }
}