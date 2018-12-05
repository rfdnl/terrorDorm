using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerrorWindows.Helper;

namespace TerrorWindows.Entities
{
    public class Bullet : Entity
    {
        public Entity Owner;
        public List<GameObject> Collides;
        public List<Bullet> Bullets;
        public float Speed = 500f;
        public bool IsBullet;

        public Bullet(Entity owner, List<GameObject> collides, List<Bullet> bullets, bool isBullet = false) : base()
        {
            this.Owner = owner;
            Collides = collides;
            Bullets = bullets;
            this.IsBullet = isBullet;
            SetTexture(ContentHelper.BULLET);
            this.Position = Owner.Position;
            this.Rotation = Owner.Rotation;
            UpdateTransform();
            UpdateRectangle();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //if (IsBullet && Alive)
            spriteBatch.Draw(Texture, Position, null, Color.Black, Rotation, Origin, 1.0f, SpriteEffects.None, 1f);
        }

        public override void Init()
        {
            // do nothing
        }

        public override void Update(GameTime gameTime)
        {
            if (Alive == false) return;

            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position.X += Transform.Right.X * Speed * time;
            Position.Y += Transform.Right.Y * Speed * time;
            UpdateTransform();
            UpdateRectangle();

            for (int i = 0; i < Collides.Count; i++)
            {
                GameObject obj = Collides[i];
                if (obj is Entity ntt)
                {
                    if (ntt == Owner) continue;
                    if (ntt is Floor) continue;
                    if (Owner is Hero && ntt is Hostage) continue;
                    if (this.Intersects(ntt))
                    {
                        Alive = false;
                        //Game1.debug.Add(String.Format("{0} shoot {1}", Owner.Name, ntt.Name));
                        ntt.OnCollide(this);
                        Bullets.Remove(this);
                    }
                }
            }
        }
    }
}
