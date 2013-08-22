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
    public class FPSCamera : Camera
    {
        private const float _rotationSpeed = 0.005f;

        private MouseState _originalMouseState;
        private float _moveSpeed = 20f;//0.02f;

        private float _standingOffset = 4f;
        private float _crouchedOffset = 1.5f;

        private bool keyW_pressed, keyA_pressed, keyS_pressed, keyD_pressed, keyQ_pressed, keyZ_pressed;

        

        public FPSCamera(Game game, Vector3 position, Vector3 target, Vector3 up)
            : base(game, position, target, up)
        {
            _yaw = 0;
            _pitch = 0;

            //_verticalOffset = _standingOffset;

            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            _originalMouseState = Mouse.GetState();
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

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            MouseState currentMouseState = Mouse.GetState();
            //KeyboardState keyState = Keyboard.GetState();
            
            if (currentMouseState != _originalMouseState)
            {
                float xDifference = currentMouseState.X - _originalMouseState.X;
                float yDifference = currentMouseState.Y - _originalMouseState.Y;
                _yaw -= _rotationSpeed*xDifference;
                _pitch -= _rotationSpeed*yDifference;
                Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            }

            /*
            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))      //Forward
                AddToCameraPosition(new Vector3(0, 0, -1), gameTime);
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))    //Backward
                AddToCameraPosition(new Vector3(0, 0, 1), gameTime);
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))   //Right
                AddToCameraPosition(new Vector3(1, 0, 0), gameTime);
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))    //Left
                AddToCameraPosition(new Vector3(-1, 0, 0), gameTime);
            if (keyState.IsKeyDown(Keys.Q))                                     //Up
                AddToCameraPosition(new Vector3(0, 1, 0), gameTime);
            if (keyState.IsKeyDown(Keys.Z))                                     //Down
                AddToCameraPosition(new Vector3(0, -1, 0), gameTime);


            if (keyState.IsKeyDown(Keys.LeftControl))
                _verticalOffset = _crouchedOffset;
            else
                _verticalOffset = _standingOffset;
             * */
                UpdateViewMatrix();

                base.Update(gameTime);

        }

        private void AddToCameraPosition(Vector3 vectorToAdd, GameTime gameTime)
        {
            
            Matrix cameraRotation = Matrix.CreateRotationX(_pitch) * Matrix.CreateRotationY(_yaw);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            _position += rotatedVector * (float)gameTime.ElapsedGameTime.TotalSeconds * _moveSpeed;
            UpdateViewMatrix();
        }

        public override void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(_pitch) * Matrix.CreateRotationY(_yaw);

            Vector3 cameraOriginalTarget = Vector3.Forward;
            Vector3 cameraOriginalUpVector = Vector3.Up;

            Vector3 cameraRotatedTarget =  Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = _position + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);
            Vector3 cameraFinalUpVector = _position + cameraRotatedUpVector;

       
            _position += Vector3.Transform(_offset, cameraRotation); 

            _view = Matrix.CreateLookAt(_position, cameraFinalTarget + _offset.Y * Vector3.Up, cameraRotatedUpVector);
            
        }

        public void ToggleCrouch()
        {
            _offset.Y = _offset.Y.CompareTo(_standingOffset) == 0 ? _crouchedOffset : _standingOffset;
        }

        #region Properties
        public new Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                this.UpdateViewMatrix();
            }
        }

        public Vector3 TargetPosition
        {
            get
            {
                Matrix cameraRotation = Matrix.CreateRotationX(_pitch) * Matrix.CreateRotationY(_yaw);
                Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
                Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
                Vector3 cameraFinalTarget = _position + cameraRotatedTarget;
                return cameraFinalTarget;
            }
        }

        public Vector3 Forward
        {
            get
            {
                Matrix cameraRotation = Matrix.CreateRotationX(_pitch) * Matrix.CreateRotationY(_yaw);
                Vector3 cameraForward = new Vector3(0, 0, -1);
                Vector3 cameraRotatedForward = Vector3.Transform(cameraForward, cameraRotation);
                return cameraRotatedForward;
            }
        }
        public Vector3 SideVector
        {
            get
            {
                Matrix cameraRotation = Matrix.CreateRotationX(_pitch) * Matrix.CreateRotationY(_yaw);
                Vector3 cameraOriginalSide = new Vector3(1, 0, 0);
                Vector3 cameraRotatedSide = Vector3.Transform(cameraOriginalSide, cameraRotation);
                return cameraRotatedSide;
            }
        }
        public Vector3 UpVector
        {
            get
            {
                Matrix cameraRotation = Matrix.CreateRotationX(_pitch) * Matrix.CreateRotationY(_yaw);
                Vector3 cameraOriginalUp = new Vector3(0, 1, 0);
                Vector3 cameraRotatedUp = Vector3.Transform(cameraOriginalUp, cameraRotation);
                return cameraRotatedUp;
            }
        }

        
        #region inputProperties

        public bool KeyWPressed
        {
            get { return keyW_pressed; }
            set { keyW_pressed = value; }
        }

        public bool KeyAPressed
        {
            get { return keyA_pressed; }
            set { keyA_pressed = value; }
        }

        public bool KeySPressed
        {
            get { return keyS_pressed; }
            set { keyS_pressed = value; }
        }

        public bool KeyDPressed
        {
            get { return keyD_pressed; }
            set { keyD_pressed = value; }
        }

        public bool KeyQPressed
        {
            get { return keyQ_pressed; }
            set { keyQ_pressed = value; }
        }

        public bool KeyZPressed
        {
            get { return keyZ_pressed; }
            set { keyZ_pressed = value; }
        }
        #endregion
        #endregion

       
    }
}
