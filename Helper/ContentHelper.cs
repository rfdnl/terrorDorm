using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TerrorWindows.Helper
{
    public class ContentHelper
    {
        public const string FLOOR = "Floor150x150";
        public const string WALL = "Wall150x150";
        public const string DOOR = "Door150x150";
        public const string PLAYER = "Player";
        public const string ENEMY = "Enemy";
        public const string HOSTAGE = "Hostage";
        public const string BULLET = "Bullet";
        public const string BUTTON = "button_100x100";

        private bool debug = true;

        public ContentManager manager;
        public Dictionary<string, SpriteFont> fonts;
        public Dictionary<string, Texture2D> textures;

        public ContentHelper(ContentManager content)
        {
            this.manager = content;
            fonts = new Dictionary<string, SpriteFont>();
            fonts.Add("Arial_12", content.Load<SpriteFont>("Arial_12"));
            fonts.Add("Arial_20", content.Load<SpriteFont>("Arial_20"));
            fonts.Add("Consolas_18", content.Load<SpriteFont>("Consolas_18"));
            fonts.Add("Pixelade_12", content.Load<SpriteFont>("Pixelade_12"));
            fonts.Add("Pixelade_20", content.Load<SpriteFont>("Pixelade_20"));
            fonts.Add("Pixelade_30", content.Load<SpriteFont>("Pixelade_30"));

            textures = new Dictionary<string, Texture2D>();
            textures.Add(PLAYER, content.Load<Texture2D>(PLAYER));
            textures.Add(ENEMY, content.Load<Texture2D>(ENEMY));
            textures.Add(HOSTAGE, content.Load<Texture2D>(HOSTAGE));
            textures.Add(BUTTON, content.Load<Texture2D>(BUTTON));
            textures.Add(FLOOR, content.Load<Texture2D>(FLOOR));
            textures.Add("Stairs150x150", content.Load<Texture2D>("Stairs150x150"));
            textures.Add(WALL, content.Load<Texture2D>(WALL));
            textures.Add(DOOR, content.Load<Texture2D>(DOOR));
            textures.Add(BULLET, content.Load<Texture2D>(BULLET));
            textures.Add("AreaNode", content.Load<Texture2D>("AreaNode"));
            textures.Add("VertexNode", content.Load<Texture2D>("VertexNode"));
            textures.Add("EdgeStraight", content.Load<Texture2D>("EdgeStraight"));
        }

        public SpriteFont GetFont(string key)
        {
            if (fonts.TryGetValue(key, out SpriteFont font)) return font;
            if (debug) Game1.debug.Add(String.Format("font ({0}) not exist", key));
            return null;
        }

        public Texture2D GetTexture(string key)
        {
            if (textures.TryGetValue(key, out Texture2D texture)) return texture;
            if (debug) Game1.debug.Add(String.Format("texture ({0}) not exist", key));
            return null;
        }
    }
}
