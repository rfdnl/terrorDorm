using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using TerrorWindows.Helper;
using TerrorWindows.Mapping;

namespace TerrorWindows.Entities
{
    public class Enemy : Entity
    {
        public static bool debug = false;
        public Entity Target;
        public Hostage Hostage;
        public int Index;
        public List<GameObject> Collides;
        public List<Bullet> Bullets;
        public float GoalRotation;
        public static float ConeView = 0.5f;
        public MapGen Map;
        TimeSpan timer;
        TimeSpan reset;

        public Enemy(int index, Vector2 Position, Entity Target, List<GameObject> collides, MapGen map, Hostage hostage) : base()
        {
            reset = new TimeSpan(0, 0, 0, 1);
            timer = reset;
            this.Map = map;
            this.Index = index;
            this.Target = Target;
            this.Hostage = hostage;
            SetTexture(ContentHelper.ENEMY);
            this.Position = Position;
            collides.Add(this);
            Collides = collides;
            Bullets = new List<Bullet>();
            if (debug) Game1.debug.Add(String.Format("Enemy({0}), x:{1} y:{2}", Index, Position.X, Position.Y));
        }

        public override void Update(GameTime gameTime)
        {
            if (!Alive) return;

            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Routes == null)
            {
                VertexNode start = Map.FindNearestNodeTo(Position);
                VertexNode goal = Map.GetLegalNodes(1)[0];
                Routes = VertexNode.AStar(start, goal);
            }

            if (Routes != null && Routes.Count > 0)
            {
                if (GoalVect == Vector2.Zero)
                {
                    GoalVect = Routes[0].Position;
                    GoalRect = Routes[0].Rect;
                    Heading = GoalVect - Position;
                    Heading.Normalize();
                }
                else
                {
                    if (GoalRect.Intersects(Rect))
                    {
                        GoalRotation = VectorToRadian(Heading);
                        Rotation = GoalRotation;
                        Routes.RemoveAt(0);
                        if (Routes.Count < 1) Routes = null;
                        GoalVect = Vector2.Zero;
                        GoalRect = Rectangle.Empty;
                    }
                    else
                    {
                        Position += Heading * Hero.MovingSpeed * time;
                    }
                }
            }

            /*
            if (GoalRotation > Rotation)
            {
                Rotation += Hero.RotationSpeed * time;
            }
            else if (GoalRotation < Rotation)
            {
                Rotation -= Hero.RotationSpeed * time;
            }
            */
            UpdateTransform();
            UpdateRectangle();

            if (timer > TimeSpan.Zero)
            {
                timer -= gameTime.ElapsedGameTime;
                if (timer <= TimeSpan.Zero)
                {
                    if (this.IsFacing(Target.Position, ConeView))
                    {
                        Bullet targetRay = new Bullet(this, Collides, Bullets);
                        Vector2 toTarget = Target.Position - targetRay.Position;
                        toTarget.Normalize();
                        targetRay.Rotation = VectorToRadian(toTarget);
                        targetRay.UpdateTransform();
                        Bullets.Add(targetRay);
                    }

                    if (Hostage.Visible && this.IsFacing(Hostage.Position, ConeView))
                    {
                        Bullet hostageRay = new Bullet(this, Collides, Bullets);
                        Vector2 toHostage = Hostage.Position - hostageRay.Position;
                        toHostage.Normalize();
                        hostageRay.Rotation = VectorToRadian(toHostage);
                        hostageRay.UpdateTransform();
                        Bullets.Add(hostageRay);
                    }
                    timer = reset;
                }
            }

            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!Alive) return;
            if (!Visible) return;
            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Origin, 1.0f, SpriteEffects.None, 1f);

            if (!debug) return;
            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Draw(gameTime, spriteBatch);
            }
        }

        public override void OnCollide(Entity entity)
        {
            /*
            if (entity is Bullet)
            {
                Bullet bullet = (Bullet)entity;
                if (bullet.Owner == Target)
                {
                    Alive = false;
                }
            }
            */
        }

        public override void Init()
        {
            // nothing
        }

    }
}
