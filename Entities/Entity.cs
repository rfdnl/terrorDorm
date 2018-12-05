using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TerrorWindows.Components;
using TerrorWindows.Mapping;

namespace TerrorWindows.Entities
{
    public abstract class Entity : GameObject
    {
        public bool Alive;
        public string Name;
        public float Rotation;
        public bool Visible = true;
        public Vector2 Position;
        public Vector2 Origin;
        public Vector2 Heading;
        public Vector2 GoalVect;
        public Matrix Transform;
        public Rectangle Rect;
        public Rectangle GoalRect;
        public Texture2D Texture;
        public List<VertexNode> Routes;
        public Color[] TextureData;
        public Camera2D Camera;

        public Entity()
        {
            Name = string.Empty;
            Alive = true;
            Rotation = 0;
            Origin = Position = Vector2.Zero;
            Heading = new Vector2(1, 0);
        }

        public abstract void Init();

        public void SetTexture(string textureName)
        {
            Alive = true;
            Name = textureName;
            this.Texture = Game1.content.GetTexture(textureName);
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            TextureData = new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureData);
        }

        // https://www.youtube.com/watch?v=crYMJRTFmBs
        // always call this after updating any of the rotation, position, origin, scale
        public void UpdateTransform()
        {
            Transform = Matrix.CreateTranslation(new Vector3(-Origin, 0))
                * Matrix.CreateScale(new Vector3(1))
                * Matrix.CreateRotationZ(Rotation)
                * Matrix.CreateTranslation(new Vector3(Position, 0));
        }

        // always call this after updateTransform()
        public void UpdateRectangle()
        {
            if (Texture == null) return;
            /*
            Vector2 topLeft = Vector2.Transform(Vector2.Zero, Transform);
            Vector2 topRight = Vector2.Transform(new Vector2(Texture.Width, 0), Transform);
            Vector2 bottomLeft = Vector2.Transform(new Vector2(0, Texture.Height), Transform);
            Vector2 bottomRight = Vector2.Transform(new Vector2(Texture.Width, Texture.Height), Transform);

            Vector2 min = new Vector2(Min(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X),
                Min(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y));
            Vector2 max = new Vector2(Max(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X),
                Max(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y));

            Rect = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
            */
            Rect = new Rectangle((int)Position.X - (int)Origin.X, (int)Position.Y - (int)Origin.Y, Texture.Width, Texture.Height);
        }

        // https://github.com/Oyyou/MonoGame_Tutorials/blob/master/MonoGame_Tutorials/Tutorial019/Sprites/Sprite.cs
        public bool Intersects(Entity other)
        {
            if (other.Texture == null) return false;

            var transformAtoB = this.Transform * Matrix.Invert(other.Transform);

            var stepX = Vector2.TransformNormal(Vector2.UnitX, transformAtoB);
            var stepY = Vector2.TransformNormal(Vector2.UnitY, transformAtoB);

            var yPosInB = Vector2.Transform(Vector2.Zero, transformAtoB);

            for (int yA = 0; yA < Rect.Height; yA++)
            {
                var posInB = yPosInB;

                for (int xA = 0; xA < this.Rect.Height; xA++)
                {
                    var xB = (int)Math.Round(posInB.X);
                    var yB = (int)Math.Round(posInB.Y);

                    if (0 <= xB && xB < other.Rect.Width &&
                        0 <= yB && yB < other.Rect.Height)
                    {
                        var colorA = this.TextureData[xA + yA * this.Rect.Width];
                        var colorB = other.TextureData[xB + yB * other.Rect.Width];

                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            return true;
                        }
                    }
                    posInB += stepX;
                }
                yPosInB += stepY;
            }
            return false;
        }

        public virtual void OnCollide(Entity entity)
        {
            // do  nothing
        }

        public Vector3 DirectionTo(Vector2 entity)
        {
            Vector3 heading = new Vector3(entity, 1) - new Vector3(this.Position, 1);
            heading.Normalize();
            return heading;
        }

        public bool IsFacing(Vector2 entity, double fov = 0)
        {
            return Vector3.Dot(DirectionTo(entity), this.Transform.Right) > fov;
        }

        protected float VectorToRadian(Vector2 heading)
        {
            // Calculate rotation relative to Vector(1, 0)
            return (float)Math.Atan2(heading.Y, heading.X);
        }

        public static float Min(params float[] values)
        {
            float min = float.MaxValue;
            for (int i = 0; i < values.Length; i++)
            {
                min = values[i] < min ? values[i] : min;
            }
            return min;
        }

        public static float Max(params float[] values)
        {
            float max = float.MinValue;
            for (int i = 0; i < values.Length; i++)
            {
                max = values[i] > max ? values[i] : max;
            }
            return max;
        }
    }
}
