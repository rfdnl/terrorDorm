using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using TerrorWindows.Entities;
using TerrorWindows.Helper;

namespace TerrorWindows.Mapping
{
    public class MapGen : GameObject
    {
        int MidRow;
        int MidCol;

        public static int MaxRow;
        public static int MaxCol;

        public static int MapWidth = 0;
        public static int MapHeight = 0;

        public bool debug = false;

        public Floor[,] Floors;
        Room[,] Rooms;
        VertexNode[,] Nodes;

        public MapGen(List<GameObject> collides, string floorfile = "floors.txt", string wallfile = "walls.txt")
        {
            string[] floors = new string[4];
            string[] walls = new string[4];

#if ANDROID
            AssetManager asset = ANDROID.Activity1.asset;
            Stream floorStream = asset.Open("floors.txt");
            using (StreamReader reader = new StreamReader(floorStream))
            {
                for(int i = 0; i < 4; i++)
                {
                    string line = reader.ReadLine();
                    floors[i] = line;
                    Game1.debug.Add(floors[i]);
                }
            }

            Stream wallStream = asset.Open("walls.txt");
            using (StreamReader reader = new StreamReader(wallStream))
            {
                for (int i = 0; i < 4; i++)
                {
                    string line = reader.ReadLine();
                    walls[i] = line;
                    Game1.debug.Add(walls[i]);
                }
            }
#else
            floorfile = Path.Combine("Mapping", floorfile);
            floors = File.ReadAllLines(floorfile);
            wallfile = Path.Combine("Mapping", wallfile);
            walls = File.ReadAllLines(wallfile);
#endif
            GetFloors(floors, collides);
            GetRooms(walls, collides);

            var floorTexture = Game1.content.GetTexture(ContentHelper.FLOOR);
            MapHeight = MaxRow * floorTexture.Height;
            MapWidth = MaxCol * floorTexture.Width;

            PlaceNodes(collides);
            LinkNodes();
        }

        private void GetFloors(string[] floors, List<GameObject> collides)
        {
            if (floors.Length < 1) return;

            MidRow = floors.Length;
            MidCol = floors[0].Length;
            MaxRow = 2 * MidRow - 1;
            MaxCol = 2 * MidCol - 1;

            Floors = new Floor[MaxRow, MaxCol];

            for (int i = 0; i < MaxRow; i++)
            {
                int row = i < MidRow ? i : MaxRow - 1 - i;
                string str = floors[row] + Reverse(floors[row].Substring(0, MidCol - 1));
                for (int k = 0; k < MaxCol; k++)
                {
                    Floors[i, k] = new Floor(collides, i, k, str.Substring(k, 1));
                }
            }
        }

        /*
       0..0001.....0.100.0.001.....0.1000......
       .1001..10.1.1.0.0...1.0.0.1.11...001....
       .....0.01..000...0.00..010...0.0........
       0.00.0.....0.0...0.0...0.0.....00.0.0.0.
       */

        private void GetRooms(string[] walls, List<GameObject> collides)
        {
            if (walls.Length < 1) return;

            MidRow = walls.Length;
            MidCol = walls[0].Length / 4;
            MaxRow = 2 * MidRow - 1;
            MaxCol = 2 * MidCol - 1;

            Rooms = new Room[MaxRow, MaxCol];

            for (int i = 0; i < MaxRow; i++)
            {
                int row = i < MidRow ? i : MaxRow - 1 - i;
                string halfWall = walls[row]; // 0..0 001. .... 0.10 0.0. 001. .... 0.10 00.. .... ..00 01.0 .... .100 .0.0 01.0 .... .100 0..0
                string fullWall = halfWall + Reverse(halfWall.Substring(0, halfWall.Length - 4));//                         001. .... 0.10 00.. k > midCol: rightshift
                for (int k = 0; k < MaxCol; k++)                                                 //                         
                {                                                                                //                         100. .... 1.00 .00. both: rightshift then vertswap
                    string wallCode = fullWall.Substring(k * 4, 4);
                    if (k > MidCol - 1) wallCode = RightShift(wallCode);
                    if (i > MidRow - 1) wallCode = SwapVertical(wallCode);
                    Rooms[i, k] = new Room(collides, i, k, wallCode);
                }
            }
        }

        private string RightShift(string str = "....")
        {
            return str.Substring(str.Length - 1, 1) + str.Substring(0, str.Length - 1); ;
        }

        private string SwapVertical(string str = "....")
        {
            return str.Substring(2, 1) + str.Substring(1, 1) + str.Substring(0, 1) + str.Substring(3, 1); ;
        }

        // for floor
        private string Reverse(string str)
        {
            string res = "";
            for (int i = 0; i < str.Length; i++)
                res += str[str.Length - 1 - i];
            return res;
        }

        public void PlaceNodes(List<GameObject> collides)
        {
            int textureWidth = MapWidth / MaxCol;
            int textureHeight = MapHeight / MaxRow;
            int sizeWidth = textureWidth / 3;
            int sizeHeight = textureHeight / 3;

            Nodes = new VertexNode[MaxRow * 3, MaxCol * 3];

            for (int row = 0; row < MaxRow * 3; row++)
            {
                int floorRow = (int)(Math.Floor((decimal)(row / 3)));
                for (int col = 0; col < MaxCol * 3; col++)
                {
                    int floorCol = (int)(Math.Floor((decimal)(col / 3)));

                    if (Floors[floorRow, floorCol].Type == null)
                    {
                        // not a floor
                        continue;
                    }

                    // if Node at center of floor, it is areaNode
                    Nodes[row, col] = (row % 3 == 1 && col % 3 == 1) ?
                        new AreaNode(row, col, collides) : new VertexNode(row, col, collides);
                }
            }
        }

        /*
         \|/
         -+-
         /|\
             */

        public void LinkNodes()
        {
            for (int row = 0; row < MaxRow * 3; row++)
            {
                int roomRow = (int)Math.Floor((decimal)row / 3);

                for (int col = 0; col < MaxCol * 3; col++)
                {
                    if (Nodes[row, col] == null) continue;

                    int roomCol = (int)Math.Floor((decimal)col / 3);
                    Room room = Rooms[roomRow, roomCol];

                    for (int irow = row - 1; irow <= row + 1; irow++)
                    {
                        for (int icol = col - 1; icol <= col + 1; icol++)
                        {
                            // make sure no out of bound vertexes
                            if ((0 <= irow) && (irow < MaxRow * 3) && (0 <= icol) && (icol < MaxCol * 3))
                            {
                                if (irow == row && icol == col) continue;
                                if (Nodes[irow, icol] == null) continue;
                                if (room.CheckWall(row, col, irow, icol)) continue;

                                Nodes[row, col].Link(Nodes[irow, icol]);
                            }
                        }
                    }
                }
            }
        }

        public VertexNode FindNearestNodeTo(Vector2 goal)
        {
            VertexNode nearest = null;
            float min = float.MaxValue;
            for (int row = 0; row < MaxRow * 3; row++)
            {
                for (int col = 0; col < MaxCol * 3; col++)
                {
                    if (Nodes[row, col] == null) continue;

                    float directionSqr = Nodes[row, col].Distance(goal);

                    if (directionSqr < min)
                    {
                        min = directionSqr;
                        nearest = Nodes[row, col];
                    }
                }
            }
            return nearest;
        }



        public List<VertexNode> GetLegalNodes(int count)
        {
            List<VertexNode> legals = new List<VertexNode>();
            Random random = new Random();

            while (legals.Count < count)
            {
                int row = random.Next(MaxRow - 1);
                int col = random.Next(MaxCol - 1);

                if (col >= 8 - 4 && col <= 10 + 4) continue;
                VertexNode node = Nodes[row * 3 + 1, col * 3 + 1];
                if (node == null) continue;
                if (legals.Contains(node)) continue;
                legals.Add(node);
            }

            return legals;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            /*
            for(int row = 0; row < MaxRow; row++)
            {
                for(int col = 0; col < MaxCol; col++)
                {
                    Floors[row, col].Draw(gameTime, spriteBatch);
                    Rooms[row, col].Draw(gameTime, spriteBatch);
                }
            }

            //if (!debug) return;
            for(int row = 0; row < MaxRow * 3; row++)
            {
                for(int col = 0; col < MaxCol * 3; col++)
                {
                    var node = Nodes[row, col];
                    if (node != null) node.Draw(gameTime, spriteBatch);
                }
            }
            */
        }

        public override void Update(GameTime gameTime)
        {
            /*
            for (int row = 0; row < MaxRow; row++)
            {
                for (int col = 0; col < MaxCol; col++)
                {
                    Floors[row, col].Update(gameTime);
                    Rooms[row, col].Update(gameTime);
                }
            }
            */
        }
    }
}
