using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace TerrorWindows.Helper
{
    public class MouseHelper
    {
        public MouseHelper() { }

        public MouseState prev;
        public MouseState now;
        public bool debug = true;

        public void Update()
        {
            prev = now;
            now = Mouse.GetState();
        }

        private Rectangle Rect()
        {
            return new Rectangle(now.X, now.Y, 1, 1);
        }

        // https://gamedev.stackexchange.com/questions/21681/how-to-get-mouse-position-relative-to-the-map
        public bool IsLeftClickAtWorld(Matrix viewMatrix, out Vector2 worldPosition)
        {
            bool done = prev.LeftButton == ButtonState.Pressed && now.LeftButton == ButtonState.Released;
            worldPosition = Vector2.Transform(now.Position.ToVector2(), Matrix.Invert(viewMatrix));
            if (done && debug) Game1.debug.Add(String.Format("x:{0} y:{1}", worldPosition.X, worldPosition.Y));
            return done;
        }

        public bool IsLeftClickAt(Rectangle rect)
        {
            bool done = prev.LeftButton == ButtonState.Pressed && now.LeftButton == ButtonState.Released;
            if (done && debug) Game1.debug.Add(String.Format("leftClick, x: {0}, y: {1}", now.X, now.Y));
            return done && Rect().Intersects(rect);
        }

        public bool IsLeftClick()
        {
            bool done = prev.LeftButton == ButtonState.Pressed && now.LeftButton == ButtonState.Released;
            if (done && debug) Game1.debug.Add(String.Format("leftClick, x:{0}, y:{1}", now.X, now.Y));
            return done;
        }
    }
}
