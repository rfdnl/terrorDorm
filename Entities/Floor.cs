using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
namespace TerrorWindows.Entities
{
    public class Floor : Entity
    {
        /*
         . = floor
         0 = no floor
         1 = stairs
             */

        public float Scale = 1.0f;
        public string Type;
        public Floor(List<GameObject> collides, int row, int column, string type = "0")
        {
            switch (type)
            {
                case ".": // normal
                    SetTexture("Floor150x150");
                    break;
                case "1": // stairs
                    SetTexture("Stairs150x150");
                    break;
                default: // none
                    break;
            }

            if (Texture == null) return;

            int X = column * Texture.Width;
            int Y = row * Texture.Height;
            Type = type;
            Position = new Vector2(X, Y);
            Origin = Vector2.Zero;
            UpdateTransform();
            UpdateRectangle();
            collides.Add(this);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Texture == null) return;
            if (!Visible) return;
            spriteBatch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, Scale, SpriteEffects.None, 1);
        }

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            //Position -= new Vector2(1, 0);
        }
    }
}
