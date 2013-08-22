using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WarZ
{
    class TopViewCamera : Camera
    {
        public TopViewCamera(Game game, Vector3 position, Vector3 target, Vector3 up) 
            : base(game, position, target, up)
        {
            _up = Vector3.Forward;
        }

        public override void Update(GameTime gameTime)
        {
            lockMouse();

            base.UpdateViewMatrix();

            base.Update(gameTime);
        }

        private void lockMouse()
        {
            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
        }
    }
}
