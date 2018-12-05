using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace TerrorWindows.Helper
{
    public enum Direction
    {
        None,
        Up,
        Down
    }

    public class TouchHelper
    {
        private GestureSample prev;
        private GestureSample now;
        public static GestureSample zeroGesture = new GestureSample(GestureType.None, TimeSpan.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero);
        private int fingerSize = 10;

        private bool debug = false;

        public TouchHelper()
        {
            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.VerticalDrag | GestureType.Flick;
        }

        public void Update()
        {
            prev = now;
            if (!TouchPanel.IsGestureAvailable)
            {
                now = zeroGesture;
                return;
            }

            now = TouchPanel.ReadGesture();

            if (debug) Game1.debug.Add(String.Format("{0} x:{1}, y:{2}, dY: {3}", now.GestureType, now.Position.X, now.Position.Y, now.Delta.Y));
        }

        public bool IsTapAtWorld(Matrix viewMatrix, out Vector2 worldPosition)
        {
            bool done = now.GestureType == GestureType.Tap;
            worldPosition = Vector2.Transform(now.Position, Matrix.Invert(viewMatrix));
            if (done && debug) Game1.debug.Add(String.Format("x:{0} y:{1}", worldPosition.X, worldPosition.Y));
            return done;
        }

        public bool IsTap(out float tapX, out float tapY)
        {
            tapX = now.Position.X;
            tapY = now.Position.Y;
            return now.GestureType == GestureType.Tap;
        }

        public bool IsTapAt(Rectangle rect)
        {
            return rect.Intersects(new Rectangle(
                (int)now.Position.X,
                (int)now.Position.Y,
                1, 1))
                && now.GestureType == GestureType.Tap;
        }

        public bool IsVerticalDrag(out float vertX, out float deltaY)
        {
            vertX = now.Position.X;
            deltaY = now.Delta.Y;
            return now.GestureType == GestureType.VerticalDrag;
        }

        public bool IsVerticalDragAt(Rectangle rect, out float x, out float y, out float deltaY)
        {
            bool done = rect.Intersects(new Rectangle(
                (int)now.Position.X,
                (int)now.Position.Y,
                1, 1))
                && now.GestureType == GestureType.VerticalDrag;

            if (done)
            {
                x = now.Position.X;
                y = now.Position.Y;
                deltaY = now.Delta.Y;
            }
            else
            {
                x = 0;
                y = 0;
                deltaY = 0;
            }
            /*
            direction = done && now.Delta.Y > 0 ? Direction.Down : Direction.Up; 
            deltaY = done ? now.Delta.Y : 0;
            */

            return done;
        }

        public bool IsFlickAt(Rectangle rect, out Direction direction)
        {
            bool done =
                now.GestureType == GestureType.Flick &&
                prev.GestureType == GestureType.VerticalDrag &&
                rect.Intersects(Rect(prev));

            direction = done && now.Delta.Y > 0 ? Direction.Down : Direction.Up;
            return done;
        }

        public Rectangle Rect(GestureSample gesture)
        {
            return new Rectangle((int)gesture.Position.X, (int)gesture.Position.X, fingerSize, fingerSize);
        }
    }
}
