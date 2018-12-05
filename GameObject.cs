using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TerrorWindows
{
    public abstract class GameObject
    {
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public abstract void Update(GameTime gameTime);
    }
}
