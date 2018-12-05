using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TerrorWindows.Helper;
using TerrorWindows.Mapping;
using TerrorWindows.States;

namespace TerrorWindows.Entities
{
    public class Hero : Entity
    {
        public bool debug = true;
        public const float RotationSpeed = 10f; //pixel per second
        public const float MovingSpeed = 100f;
        public MapGen Map;
        public List<GameObject> Objects;
        public List<Bullet> Bullets;
        public int visionDegree = 50;

        public Hero(MapGen map, List<GameObject> objects) : base()
        {
            Map = map;
            Init();
            objects.Add(this);
            Objects = objects;
            Bullets = new List<Bullet>();
        }

        public override void Init()
        {
            SetTexture(ContentHelper.PLAYER);
            var floorTexture = Game1.content.GetTexture(ContentHelper.FLOOR);
            int X = 9 * floorTexture.Width + floorTexture.Width / 2;
            int Y = 3 * floorTexture.Height + floorTexture.Height / 2;
            Position = new Vector2(X, Y);
            UpdateTransform();
        }

        public override void Update(GameTime gtime)
        {
            float time = (float)gtime.ElapsedGameTime.TotalSeconds;

#if ANDROID
            // ANDROID WORKING
            if (Game1.touch.IsTapAtWorld(Camera.GetTransform(), out Vector2 worldPosition)){
                //if (Rect.Intersects(new Rectangle((int)worldPosition.X, (int)worldPosition.Y, 10, 10))){
                    //Shoot();
                //} else {
                    ChooseGoal(worldPosition);
                //}
            }

            if (Game1.touch.IsVerticalDrag(out float vertX, out float deltaY))
            {
                if (vertX > Game1.screenWidth / 2) // right side of screen
                    Rotation -= (deltaY / RotationSpeed) * time;
                else
                    Rotation += (deltaY / RotationSpeed) * time;
            }
#else
            // PC WORKING
            if (Game1.mouse.IsLeftClickAtWorld(Camera.GetTransform(), out Vector2 worldPosition))
            {
                ChooseGoal(worldPosition);
            }

            if (Game1.keyboard.IsHold(Keys.A))
            {
                Rotation -= RotationSpeed * time;
            }

            if (Game1.keyboard.IsHold(Keys.D))
            {
                Rotation += RotationSpeed * time;
            }

            /*
            if (Game1.keyboard.IsPress(Keys.Space)){
                Shoot();
            }
            */
            //if (Game1.keyboard.IsPress(Keys.W)) move = !move;
#endif
            if (Routes != null && Routes.Count > 0) // there are routes to follow
            {
                if (GoalVect == Vector2.Zero) // select next node
                {
                    GoalVect = Routes[0].Position;
                    GoalRect = Routes[0].Rect;
                    Heading = GoalVect - Position;
                    Heading.Normalize();
                }
                else // move towards next node
                {
                    if (GoalRect.Intersects(Rect)) // arrived at node
                    {
                        Routes.RemoveAt(0);
                        if (Routes.Count < 1) Routes = null;
                        GoalVect = Vector2.Zero;
                        GoalRect = Rectangle.Empty;
                    }
                    else // continue moving to node
                    {
                        Position += Heading * MovingSpeed * time;
                    }
                }
            }

            //if (move) MoveForward();

            /*
            if (GoalVect != Vector2.Zero)
            {
                if (GoalRect.Intersects(Rect) == false)
                    Position += Heading * Speed;
            }
            */

            UpdateTransform();
            UpdateRectangle();

            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Update(gtime);
            }

            //See();
            //Vector2 Forward = new Vector2(Transform.Right.X, Transform.Right.Y);
            //float fov = 0.9f;
            //for(int i = 0; i < Objects.Count; i++)
            //{
            // if (Objects[i] is Entity ntt) {
            //  Rectangle rect = ntt.Rect;
            /*
            ntt.Visible = IsFacing(new Vector2(rect.Left, rect.Top), fov) 
                || IsFacing(new Vector2(rect.Right, rect.Top), fov) 
                || IsFacing(new Vector2(rect.Left, rect.Bottom), fov) 
                || IsFacing(new Vector2(rect.Right, rect.Bottom), fov);
                */
            //ntt.Visible = Vector2.Dot(ntt.Position - this.Position, Forward) > 0.99;
            //}
            //}
        }

        public void ChooseGoal(Vector2 worldPosition)
        {
            /*
                GoalVect = worldPostion;
                GoalRect = new Rectangle((int)GoalVect.X, (int)GoalVect.Y, 5, 5);
                Heading= GoalVect - Position;
                Heading.Normalize();
                */
            VertexNode start = Map.FindNearestNodeTo(Position);
            VertexNode goal = Map.FindNearestNodeTo(worldPosition);
            Routes = VertexNode.AStar(start, goal);
        }

        public void See()
        {
            //Bullets.Add(new Bullet(this, Objects, Bullets, true));
            for (int i = 0; i < Objects.Count; i++)
            {
                GameObject go = Objects[i];
                if (go is Entity ntt)
                    ntt.Visible = false;
            }

            int right = visionDegree / 2;
            for (int i = -right; i <= right; i++)
            {
                RayCast(Position, Rotation + (float)(i * (Math.PI / 180.0)), ref Objects);
            }
        }

        public void RayCast(Vector2 position, float rotation, ref List<GameObject> gos)
        {
            List<Entity> hits = new List<Entity>();
            Ray ray = new Ray(
                new Vector3(position, 0),
                new Vector3((float)Math.Cos(rotation), (float)Math.Sin(rotation), 0));
            for (int i = 0; i < gos.Count; i++)
            {
                GameObject go = gos[i];
                if (go == this)
                {
                    Visible = true; continue;
                }

                if (go is Entity ntt)
                {
                    if (ntt is Bullet)
                    {
                        continue;
                    }

                    BoundingBox box = new BoundingBox(
                        new Vector3(ntt.Rect.X, ntt.Rect.Y, 0),
                        new Vector3(ntt.Rect.X + ntt.Rect.Width, ntt.Rect.Y + ntt.Rect.Height, 1));
                    if (ray.Intersects(box) != null)
                    {
                        hits.Add(ntt);
                    }
                }
            }

            // sort by distance ascending
            List<Entity> sorted = new List<Entity>();
            while(hits.Count > 0)
            {
                int minIndex = hits.Count;
                float minDistance = float.MaxValue;

                for (int i = 0; i < hits.Count; i++)
                {
                    float distance = (hits[i].Position - this.Position).LengthSquared();
                    if (distance <= minDistance)
                    {
                        minIndex = i;
                        minDistance = distance;
                    }
                }

                sorted.Add(hits[minIndex]);
                hits.RemoveAt(minIndex);
            }

            // find wall
            int wallIndex = -1;
            for (int i = 0; i < sorted.Count; i++)
            {
                if (sorted[i] is Wall wall)
                {
                    wallIndex = i;
                    break;
                }
            }

            for (int i = 0; i < wallIndex ; i++)
            {
                sorted[i].Visible = true;
            }
        }

        public override void OnCollide(Entity entity)
        {
            //Game1.debug.Add(String.Format("Hero collided with {0}", entity.Name));
            if (entity is Bullet bullet)
            {
                if (bullet.Owner is Hostage hostage)
                {
                    hostage.Visible = true;
                }
                if (bullet.Owner is Enemy enemy)
                {
                    Vector2 toEnemy = enemy.Position - Position;
                    toEnemy.Normalize();
                    Rotation = VectorToRadian(toEnemy);
                    UpdateTransform();

                    Vector2 toHero = Position - enemy.Position;
                    toHero.Normalize();
                    enemy.Rotation = VectorToRadian(toHero);
                    enemy.UpdateTransform();

                    Game1.debug.Add("Lose: You were found");
                    GameState.LOSE = true;
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Origin, 1.0f, SpriteEffects.None, 1f);
            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Draw(gameTime, spriteBatch);
            }
        }
    }
}
