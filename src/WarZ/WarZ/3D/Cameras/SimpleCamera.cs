using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace WarZ
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SimpleCamera : Camera
    {
        protected float _keyboardMovOffset = 30f;
        protected float _mouseMovOffset = 10f;

        public SimpleCamera(Game game, Vector3 position, Vector3 target, Vector3 up)
            : base(game, position, target, up)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        private void handleInput(GameTime gameTime)
        {
            KeyboardState k = Keyboard.GetState();
            MouseState m = Mouse.GetState();

            //Keyboard
            if (k.IsKeyDown(Keys.A))
                _position.X += -_keyboardMovOffset * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (k.IsKeyDown(Keys.D))
                _position.X += _keyboardMovOffset * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (k.IsKeyDown(Keys.W))
                _position.Y += _keyboardMovOffset * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (k.IsKeyDown(Keys.S))
                _position.Y += -_keyboardMovOffset * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Mouse
            if (MouseHelper.hasMoved())
            {
                _position.Z += MouseHelper.ScrollWheelValue(gameTime) * _mouseMovOffset * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            
            handleInput(gameTime);
            UpdateViewMatrix();

            base.Update(gameTime);
        }
    }
}
