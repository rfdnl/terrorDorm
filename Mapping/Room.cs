using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerrorWindows.Entities;

namespace TerrorWindows.Mapping
{
    public class Room : GameObject
    {
        public List<Wall> Walls;
        public bool debug = false;

        public Room(List<GameObject> collides, int row, int column, string wallStr = "....")
        {
            Walls = new List<Wall>(wallStr.Length);

            for (int direction = 0; direction < wallStr.Length; direction++)
            {
                Walls.Add(new Wall(collides, row, column, direction, wallStr.Substring(direction, 1)));
            }

            if (wallStr.Length > 4) return;
            if (debug) Game1.debug.Add(String.Format("room, {0},{1}, code:{2}", row, column, wallStr));
        }

        public bool CheckWall(int arow, int acol, int brow, int bcol)
        {
            // a is current
            // b is target
            // return false 
            int roomRowA = (int)Math.Floor((decimal)arow / 3);
            int roomColA = (int)Math.Floor((decimal)acol / 3);
            int roomRowB = (int)Math.Floor((decimal)brow / 3);
            int roomColB = (int)Math.Floor((decimal)bcol / 3);

            if (roomRowA == roomRowB && roomColA == roomColB) return false;

            bool aMidRow = arow % 3 == 1;
            bool aMidCol = acol % 3 == 1;
            bool bMidRow = brow % 3 == 1;
            bool bMidCol = bcol % 3 == 1;

            Wall topA = Walls[0];
            Wall rightA = Walls[1];
            Wall bottomA = Walls[2];
            Wall leftA = Walls[3];

            if (roomRowA == roomRowB)
            {
                if (roomColA > roomColB)
                {
                    // A right of B
                    if (leftA.IsClear()) return false;
                    if (leftA.IsDoor() && aMidRow && bMidRow) return false;
                }
                else if (roomColA < roomColB)
                {
                    // A left of B
                    if (rightA.IsClear()) return false;
                    if (rightA.IsDoor() && aMidRow && bMidRow) return false;
                }
            }

            if (roomColA == roomColB)
            {
                if (roomRowA > roomRowB)
                {
                    // A below of B
                    if (topA.IsClear()) return false;
                    if (topA.IsDoor() && aMidCol && bMidCol) return false;
                }
                else if (roomRowA < roomRowB)
                {
                    // A top of B
                    if (bottomA.IsClear()) return false;
                    if (bottomA.IsDoor() && aMidCol && bMidCol) return false;
                }
            }

            return true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            /*
            foreach (var wall in Walls)
            {
                wall.Draw(gameTime, spriteBatch);
            }
            */
        }

        public override void Update(GameTime gameTime)
        {
            /*
            foreach (var wall in Walls)
            {
                wall.Update(gameTime);
            }
            */
        }
    }
}
