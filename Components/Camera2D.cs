using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using TerrorWindows.Entities;

namespace TerrorWindows.Components
{
    public class Camera2D : GameObject
    {
        private float zoom;
        private Matrix transform;
        public float Rotation;
        public Vector2 Position;
        public Entity Object;
        public GraphicsDevice Device;
        readonly float rot90 = (float)Math.PI / 2;

        public bool debug = true;

        public Camera2D(Entity Object, GraphicsDevice Device, List<GameObject> Objects)
        {
            zoom = 1;
            this.Device = Device;
            this.Object = Object;
            Rotation = -Object.Rotation;
            Position = Object.Position;
            Object.Camera = this;
            Objects.Add(this);
        }

        public float Zoom
        {
            get { return zoom; }
            set { zoom = value; if (zoom < 0.1f) zoom = 0.1f; }
        }

        public Matrix GetTransform()
        {
            // author: David Amador
            // src: http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-Rotation/
            transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) // centre of rotation
                * Matrix.CreateRotationZ(Rotation)
                * Matrix.CreateScale(new Vector3(Zoom, Zoom, 1))
                * Matrix.CreateTranslation(new Vector3(Device.Viewport.Width * 0.5f, Device.Viewport.Height * 0.9f, 0)); // translation
            return transform;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //
        }

        public override void Update(GameTime gameTime)
        {
            Update();
            //Game1.debug.Add("CamRotation: " + Rotation);
        }

        public void Update()
        {
            Position = Object.Position;
            Rotation = -Object.Rotation + 3 * rot90;
        }
    }
}
