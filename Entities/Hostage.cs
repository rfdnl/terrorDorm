using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerrorWindows.Helper;
using TerrorWindows.Mapping;
using TerrorWindows.States;

namespace TerrorWindows.Entities
{
    public class Hostage : Entity
    {
        public static bool debug = true;
        public Hero Hero;
        public MapGen Map;
        public float GoalRotation;
        readonly List<GameObject> Objects;
        public List<Bullet> Bullets;
        TimeSpan timer;
        TimeSpan reset;

        public Hostage(Vector2 position, Hero target, List<GameObject> objects, MapGen map) : base()
        {
            reset = new TimeSpan(0, 0, 0, 1);
            timer = reset;
            Map = map;
            Hero = target;
            SetTexture(ContentHelper.HOSTAGE);
            Position = position;
            objects.Add(this);
            Objects = objects;
            Bullets = new List<Bullet>();
            UpdateTransform();
            UpdateRectangle();
            if (debug) Game1.debug.Add(String.Format("Hostage, x:{0} y:{1}", Position.X, Position.Y));
            Visible = false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Draw(gameTime, spriteBatch);
            }
            if (!Visible) return;
            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Origin, 1.0f, SpriteEffects.None, 1f);
        }

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Visible)
            {
                if (Routes == null)
                {
                    VertexNode start = Map.FindNearestNodeTo(Position);
                    VertexNode goal = Map.FindNearestNodeTo(Hero.Position);
                    Routes = VertexNode.AStar(start, goal);
                    while (Routes.Count > 5)
                    {
                        Routes.RemoveAt(Routes.Count - 1);
                    }
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
            }
            else
            {
                if (timer > TimeSpan.Zero)
                {
                    timer -= gameTime.ElapsedGameTime;
                    if (timer <= TimeSpan.Zero)
                    {
                        Bullets.Add(new Bullet(this, Objects, Bullets));
                        timer = reset;
                    }
                }
            }

            Vector3 heading = DirectionTo(Hero.Position);
            Rotation = VectorToRadian(new Vector2(heading.X, heading.Y));
            UpdateTransform();
            UpdateRectangle();

            if (!debug) return;
            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Update(gameTime);
            }
        }
        public override void OnCollide(Entity entity)
        {
            if (Visible == false) return;

            if (entity is Bullet bullet)
            {
                if (bullet.Owner is Enemy enemy)
                {
                    Vector2 toEnemy = enemy.Position - Hero.Position;
                    toEnemy.Normalize();
                    Hero.Rotation = VectorToRadian(toEnemy);
                    Hero.Camera.Update();
                    Hero.UpdateTransform();

                    Vector2 toHero = Hero.Position - enemy.Position;
                    toHero.Normalize();
                    enemy.Rotation = VectorToRadian(toHero);
                    enemy.UpdateTransform();

                    Game1.debug.Add("Lose: Hostage busted!");
                    GameState.LOSE = true;
                }
            }
        }

    }
}
