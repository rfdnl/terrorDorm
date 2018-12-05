using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TerrorWindows.Components
{
    public class Button : GameObject
    {
        public SpriteFont Font;
        public Texture2D Texture;
        public Color Color;
        public Color FontColor;
        public Vector2 Position;
        public string Text;
        public event EventHandler Clicked;

        public Button(Texture2D texture, SpriteFont font, String text)
        {
            this.Texture = texture;
            this.Text = text;
            this.Font = font;
            this.FontColor = Color.Black;
            this.Color = Color.White;
        }

        public Rectangle Rect()
        {
            return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    Texture.Width,
                    Texture.Height
                    );
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rect(), Color);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rect().X + (Rect().Width / 2)) - (Font.MeasureString(Text).X / 2);
                var y = (Rect().Y + (Rect().Height / 2)) - (Font.MeasureString(Text).Y / 2);

                spriteBatch.DrawString(Font, Text, new Vector2(x, y), FontColor);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Game1.mouse.IsLeftClickAt(Rect()) || Game1.touch.IsTapAt(Rect()))
            {
                Clicked?.Invoke(this, null);
            }
        }
    }
}
