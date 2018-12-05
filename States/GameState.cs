using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerrorWindows.Components;
using TerrorWindows.Entities;
using TerrorWindows.Mapping;

namespace TerrorWindows.States
{
    public class GameState : State
    {
        private List<GameObject> Objects;
        private List<GameObject> UI;
        public bool debug = true;
        public Camera2D Camera;
        public MapGen Map;
        public Hero Hero;
        public Hostage Hostage;
        public Game1 Game;
        public Textblock levelBlock;
        public Button retryButton;
        public Button nextButton;

        public static bool WIN = false;
        public static bool LOSE = false;

        public int enemyCount;

        public GameState(Game1 game, GraphicsDevice graphicsDevice, int level) : base(game, graphicsDevice)
        {
            if (debug) Game1.debug.Add("Entered Game");

            WIN = false;
            LOSE = false;

            enemyCount = level + 2;
            Game = game;
            var screenWidth = graphicsDevice.Viewport.Width;
            var screenHeight = graphicsDevice.Viewport.Height;

            Objects = new List<GameObject>();
            Map = new MapGen(Objects);
            Hero = new Hero(Map, Objects);
            Camera = new Camera2D(Hero, graphicsDevice, Objects)
            {
#if ANDROID
                Zoom = 2.0f
#else
                Zoom = 1.0f
#endif
            };

            List<VertexNode> legals = Map.GetLegalNodes(enemyCount + 1);
            Hostage = new Hostage(legals[enemyCount].Position, Hero, Objects, Map);
            for (int i = 0; i < enemyCount; i++)
            {
                new Enemy(i, legals[i].Position, Hero, Objects, Map, Hostage);
            }

            levelBlock = new Textblock(Game1.content.GetFont("Pixelade_20"), "LEVEL " + level);
            levelBlock.Position = new Vector2(screenWidth / 2 - levelBlock.GetWidth(),
                (float)(0.05 * screenHeight));

            Texture2D buttonTexture = Game1.content.GetTexture("button_100x100");
            retryButton = new Button(buttonTexture, levelBlock.Font, "LOSE")
            {
                Position = new Vector2(screenWidth / 2 - buttonTexture.Width / 2,
                (float)(0.6 * screenHeight))
            };
            retryButton.Clicked += RetryButton_Clicked;

            nextButton = new Button(buttonTexture, levelBlock.Font, "WIN")
            {
                Position = new Vector2(screenWidth / 2 - buttonTexture.Width / 2,
                (float)(0.6 * screenHeight))
            };
            nextButton.Clicked += NextButton_Clicked;

        }

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            game.nexState = new GameState(game, graphicsDevice, enemyCount - 1);
        }

        private void RetryButton_Clicked(object sender, EventArgs e)
        {
            game.nexState = new GameState(game, graphicsDevice, enemyCount - 2);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            spriteBatch.Begin(transformMatrix: Camera.GetTransform());
            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();

            spriteBatch.Begin();
            levelBlock.Draw(gameTime, spriteBatch);
            if (WIN || LOSE)
            {
                if (LOSE)
                {
                    retryButton.Draw(gameTime, spriteBatch);
                }
                else if (WIN)
                {
                    nextButton.Draw(gameTime, spriteBatch);
                }

            }
            spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (WIN || LOSE)
            {
                Camera.Update();

                if (WIN)
                {
                    nextButton.Update(gameTime);
                }
                else if (LOSE)
                {
                    retryButton.Update(gameTime);
                }
                return;
            }

            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].Update(gameTime);
            }

            // check winning condition
            if (Hostage.Rect.Intersects(Map.Floors[2, 9].Rect) || Hostage.Rect.Intersects(Map.Floors[4, 9].Rect))
            {
                if (Hero.Rect.Intersects(Map.Floors[2, 9].Rect) || Hero.Rect.Intersects(Map.Floors[4, 9].Rect))
                {
                    WIN = true;
                    if (debug) Game1.debug.Add("Win : Hostage rescued");
                }
            }
        }

        public override void PostUpdate(GameTime gameTime)
        {
            /*
            for (int i = 0; i < Objects.Count; i++)
            {
                GameObject obj = Objects[i];
                if (obj == null) continue;
                if (obj == Hero) continue;
                if (obj is Entity)
                {
                    Entity ntt = (Entity)obj;
                    ntt.Visible = Hero.IsFacing(ntt, 0.5);
                }
                else
                {
                    obj.Visible = true;
                }
            }
            */

            /*
            foreach (var objA in collides)
            {
                if (objA is Wall) continue;
                foreach (var objB in collides)
                {
                    if (objA == objB) continue;
                    if (objB is Wall) continue;
                    if (objA.Intersects(objB)) objA.OnCollide(objB);
                }
            }
            */
        }

    }
}
