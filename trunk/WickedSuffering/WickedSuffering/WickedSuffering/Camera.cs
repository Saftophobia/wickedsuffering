using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WickedSuffering
{
    class Camera
    {
        public Matrix View { get; set; }

        public Matrix Projection { get; set; }

        public Vector3 Position { get;  set; }

        public Vector3 target { get;  set; }

        public Camera(Vector3 Position, Vector3 target, GraphicsDevice graphicsDevice)
        {
                Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
                graphicsDevice.Viewport.AspectRatio, 0.1f, 1000000.0f);
                this.Position = Position;
                this.target = target;
                
        }

        public void Update()
        {
            View = Matrix.CreateLookAt(Position, target, Vector3.Up);
        }
    }
}
