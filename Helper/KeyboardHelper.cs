using Microsoft.Xna.Framework.Input;
using System;

namespace TerrorWindows.Helper
{
    public class KeyboardHelper
    {
        private KeyboardState prev;
        private KeyboardState now;

        private bool debug = false;

        public KeyboardHelper() { }

        public void Update()
        {
            prev = now;
            now = Keyboard.GetState();
        }

        public bool IsDown(Keys key)
        {
            bool done = now.IsKeyDown(key);
            if (done && debug) Game1.debug.Add(String.Format("KeyDown: {0}", key));
            return done;
        }

        public bool IsPress(Keys key)
        {
            bool done = now.IsKeyUp(key) && prev.IsKeyDown(key);
            if (done && debug) Game1.debug.Add(String.Format("KeyPress: {0}", key));
            return done;
        }

        public bool IsHold(Keys key)
        {
            bool done = now.IsKeyDown(key) && prev.IsKeyDown(key);
            if (done && debug) Game1.debug.Add(String.Format("KeyHold: {0}", key));
            return done;
        }
    }
}
