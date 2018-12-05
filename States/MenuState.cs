using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TerrorWindows.Components;

namespace TerrorWindows.States
{
    public class MenuState : State
    {
        private List<GameObject> objects;
        public bool debug = true;
        public MenuState(Game1 game, GraphicsDevice graphicsDevice) : base(game, graphicsDevice)
        {
            if (debug) Game1.debug.Add("Entered Menu");

            var screenWidth = graphicsDevice.Viewport.Width;
            var screenHeight = graphicsDevice.Viewport.Height;

            Texture2D button_100x100 = Game1.content.GetTexture("button_100x100");
            SpriteFont pixelade_20 = Game1.content.GetFont("Pixelade_20");
            SpriteFont pixelade_30 = Game1.content.GetFont("Pixelade_30");

            Textblock titleBlock = new Textblock(pixelade_20, "TERROR DORM");
            titleBlock.Position = new Vector2(
                    screenWidth / 2 - titleBlock.GetWidth(),
                    (float)(0.1 * screenHeight)
                );

            Button playButton = new Button(button_100x100, pixelade_20, "PLAY")
            {
                Position = new Vector2(
                        screenWidth / 2 - button_100x100.Width / 2,
                        (float)(0.6 * screenHeight))
            };
            playButton.Clicked += PlayButton_Clicked;

            Button quitButton = new Button(button_100x100, pixelade_20, "QUIT")
            {
                Position = new Vector2(
                        screenWidth / 2 - button_100x100.Width / 2,
                        (float)(0.8 * screenHeight)
                        )
            };
            quitButton.Clicked += QuitButton_Clicked;

            objects = new List<GameObject>()
            {
                titleBlock,
                playButton,
                quitButton
            };
        }

        private void QuitButton_Clicked(object sender, EventArgs e)
        {
            game.Exit();
        }

        private void PlayButton_Clicked(object sender, EventArgs e)
        {
            Play();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            foreach (GameObject c in objects)
            {
                c.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // do nothing
        }

        public override void Update(GameTime gameTime)
        {
            if (Game1.keyboard.IsPress(Keys.Space))
            {
                Play();
            }

            foreach (GameObject c in objects)
            {
                c.Update(gameTime);
            }
        }

        private void Play()
        {
            game.nexState = new GameState(game, graphicsDevice, 1);
        }
    }
}
