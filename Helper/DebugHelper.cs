using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TerrorWindows.Helper
{
    public class DebugHelper
    {
        private List<string> strings;
        private int x = 50;
        private int y = 30;
        private int limit = 10;
        public SpriteFont font;
        public Color color;

        private bool debug = false;

        public DebugHelper()
        {
            strings = new List<string>();
            //font = Game1.content.GetFont("Arial_12");
            font = Game1.content.GetFont("Consolas_18");
            color = Color.White;
        }

        public void Add(string s)
        {
            if (s == null) return;
            strings.Add(s);
        }

        public void Update()
        {
            if (strings.Count > limit) strings.RemoveAt(0);
        }

        public void Draw(SpriteBatch sprites)
        {
            if (font == null)
            {
                Console.WriteLine("Debug font is null");
                return;
            }

            if (!debug) return;

            for (int i = 0; i < strings.Count; i++)
            {
                sprites.DrawString(font, strings[i], new Vector2(x, y * i), color);
            }
        }
    }
}
