using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TerrorWindows.Components
{
    public class Textblock : GameObject
    {
        public SpriteFont Font;
        public Color FontColor;
        public Vector2 Position;
        public string Text;

        public Textblock(SpriteFont font, string text)
        {
            this.Font = font;
            this.FontColor = Color.White;
            this.Text = text;
        }

        public float GetWidth()
        {
            return Font.MeasureString(Text).X;
        }

        public Rectangle Rect()
        {
            Vector2 size = Font.MeasureString(Text);
            return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    (int)size.X,
                    (int)size.Y
                );
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                var rect = Rect();

                var x = rect.X + (rect.Width / 2);
                var y = rect.Y + (rect.Height / 2);

                spriteBatch.DrawString(Font, Text, new Vector2(x, y), FontColor);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // do nothing
        }
    }
}
