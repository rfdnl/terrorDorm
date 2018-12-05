using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerrorWindows.Mapping
{
    public class VertexNode : GameObject
    {
        public int Row;
        public int Col;
        public Vector2 Position;
        public Rectangle Rect;
        public List<VertexNode> Neighbours;
        public List<float> Directions;
        public Texture2D Texture;
        public static Texture2D EdgeTexture;
        public static bool debug = false;

        public VertexNode(int row, int col, List<GameObject> collides)
        {
            collides.Add(this);
            int textureWidth = MapGen.MapWidth / MapGen.MaxCol;
            int textureHeight = MapGen.MapHeight / MapGen.MaxRow;

            int sizeWidth = textureWidth / 3;
            int sizeHeight = textureHeight / 3;

            var x = col * sizeWidth + sizeWidth / 2;
            var y = row * sizeHeight + sizeHeight / 2;

            Row = row;
            Col = col;
            Position = new Vector2(x, y);
            Neighbours = new List<VertexNode>();
            Directions = new List<float>();
            Texture = Game1.content.GetTexture("VertexNode");
            if (EdgeTexture == null) EdgeTexture = Game1.content.GetTexture("EdgeStraight");
            Rect = new Rectangle(x, y, Texture.Width, Texture.Height);
        }


        public void Link(VertexNode neighbour)
        {
            Neighbours.Add(neighbour);
            Vector2 direction = neighbour.Position - Position;
            direction.Normalize();
            float rotation = (float)Math.Atan2(direction.Y, direction.X);
            Directions.Add(rotation);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!debug) return;
            spriteBatch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

            foreach (float rotation in Directions)
            {
                spriteBatch.Draw(EdgeTexture, Position, null, Color.White, rotation, Vector2.Zero, 1, SpriteEffects.None, 1);
            }
        }

        public static List<VertexNode> AStar(VertexNode start, VertexNode end)
        {
            var closedSet = new List<VertexNode>();
            var openSet = new List<VertexNode>() { start };
            var cameFrom = new Dictionary<VertexNode, VertexNode>();
            var currentDistance = new Dictionary<VertexNode, int>();
            var predictedDistance = new Dictionary<VertexNode, float>();

            currentDistance.Add(start, 0);
            predictedDistance.Add(start, start.Distance(end.Position));

            while (openSet.Count > 0)
            {
                // get the node with the lowest estimated cost to finish
                var current = (from p in openSet orderby predictedDistance[p] ascending select p).First();

                if (current.Row == end.Row && current.Col == end.Col)
                {
                    return ProcessPath(cameFrom, end);
                }

                openSet.Remove(current);
                closedSet.Add(current);

                for (int i = 0; i < current.Neighbours.Count; i++)
                {
                    var tempCurrentDistance = currentDistance[current] + 1;

                    VertexNode neighbour = current.Neighbours[i];
                    if (closedSet.Contains(neighbour) && tempCurrentDistance >= currentDistance[neighbour])
                    {
                        continue;
                    }

                    if (!closedSet.Contains(neighbour) || tempCurrentDistance < currentDistance[neighbour])
                    {
                        if (cameFrom.Keys.Contains(neighbour))
                        {
                            cameFrom[neighbour] = current;
                        }
                        else
                        {
                            cameFrom.Add(neighbour, current);
                        }

                        currentDistance[neighbour] = tempCurrentDistance;
                        predictedDistance[neighbour] = currentDistance[neighbour] + neighbour.Distance(end.Position);

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }

            throw new Exception(
                    string.Format(
                            "Unable to find path, start: {0},{1}, end: {2},{3}",
                            start.Row, start.Col,
                            end.Row, end.Col
                        )
                );
        }

        private static List<VertexNode> ProcessPath(Dictionary<VertexNode, VertexNode> cameFrom, VertexNode current)
        {
            if (!cameFrom.Keys.Contains(current))
            {
                return new List<VertexNode> { current };
            }

            var path = ProcessPath(cameFrom, cameFrom[current]);
            path.Add(current);
            return path;
        }

        public float Distance(Vector2 Position)
        {
            var direction = Position - this.Position;
            return direction.LengthSquared();
        }

        public override void Update(GameTime gameTime)
        {
            // do nothing
        }
    }
}
