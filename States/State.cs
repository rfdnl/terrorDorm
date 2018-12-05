using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TerrorWindows.States
{
    public abstract class State
    {
        protected GraphicsDevice graphicsDevice;
        protected Game1 game;

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public abstract void PostUpdate(GameTime gameTime);
        public abstract void Update(GameTime gameTime);

        public State(Game1 game, GraphicsDevice graphicsDevice)
        {
            this.game = game;
            this.graphicsDevice = graphicsDevice;
        }
    }
}
