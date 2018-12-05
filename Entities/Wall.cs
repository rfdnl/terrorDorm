using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TerrorWindows.Helper;

namespace TerrorWindows.Entities
{
    public class Wall : Entity
    {
        /*
    0) top
    1) right
    2) bottom
    3) left

    . = no wall
    0 = wall
    1 = door
        */

        public float Scale = 1.0f;
        public string Type;
        public int Row;
        public int Col;
        public int Dir;

        public bool debug = false;

        private static float rot90 = (float)(Math.PI / 2);

        public Wall(List<GameObject> collides, int row, int column, int direction = 0, string type = ".") : base()
        {
            Type = type;
            switch (type)
            {
                case "0": // wall
                    SetTexture(ContentHelper.WALL);
                    break;
                case "1": // door
                    SetTexture(ContentHelper.DOOR);
                    break;
                default:
                    break;
            }

            if (Texture == null) return;

            Rotation = rot90 * direction;
            //Origin = Vector2.Zero;
            int X = (column * Texture.Width) + Texture.Width / 2;
            int Y = (row * Texture.Height) + Texture.Height / 2;

            Position = new Vector2(X, Y);
            collides.Add(this);
            if (debug) Game1.debug.Add(String.Format("wall, r:{0} c:{1} rota:{2}", row, column, (int)(Rotation * (180 / Math.PI))));
            UpdateTransform();
            UpdateRectangle();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Texture == null) return;
            if (!Visible) return;
            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 1);
        }

        public bool IsDoor()
        {
            return Type == "1";
        }

        public bool IsWall()
        {
            return Type == "0";
        }

        public bool IsClear()
        {
            return Type == ".";
        }

        public override void Update(GameTime gameTime)
        {
            //Position -= new Vector2(1f, 0);

        }

        public override void Init()
        {
            throw new NotImplementedException();
        }
    }
}
