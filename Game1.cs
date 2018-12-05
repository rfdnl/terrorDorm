using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TerrorWindows.Helper;
using TerrorWindows.States;

namespace TerrorWindows
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static DebugHelper debug;
        public static ContentHelper content;
        public static MouseHelper mouse;
        public static TouchHelper touch;
        public static KeyboardHelper keyboard;
        public static int screenWidth;
        public static int screenHeight;
        public static bool isAndroid = false;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public State nowState;
        public State nexState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

#if ANDROID
            isAndroid = true;
#else
            isAndroid = false;
#endif
            if (isAndroid)
            {
                screenWidth = 720;
                screenHeight = 1280;
            }
            else
            {
                screenWidth = (int)(720 * 0.6);
                screenHeight = (int)(1280 * 0.6);
            }

            IsFixedTimeStep = false;

            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.IsFullScreen = isAndroid;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
            content = new ContentHelper(Content);
            keyboard = new KeyboardHelper();
            mouse = new MouseHelper();
            touch = new TouchHelper();
            debug = new DebugHelper();

            nowState = new MenuState(this, graphics.GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            touch.Update();
            mouse.Update();
            keyboard.Update();

            debug.Update();

            if (nexState != null)
            {
                nowState = nexState;
                nexState = null;
            }

            nowState.Update(gameTime);
            nowState.PostUpdate(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            nowState.Draw(gameTime, spriteBatch);

            spriteBatch.Begin();
            debug.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
