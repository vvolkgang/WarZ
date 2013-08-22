using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WarZ
{
   public class CanonBall : DrawableGameComponent, IComponent3D
    {
        private readonly WarZGame WZGame;
        private readonly SpherePrimitive _ball;

        private Vector3 _scale;
        private Vector3 _rotation;
        private Vector3 _position = new Vector3(0.95f, 6f, -2.4f);
        private Vector3 _velocity;
        private Vector3 _direction;
       private Vector3 _lastPosition;

        private const float _mass = 1.0f;
        private const float _thrustForce = 15000f;//24000.0f;
        private const float _dragFactor = 0.67f;
        private float _thrustAmount = 1f; //is used as Input, remains 1f
        //private Vector3 _gravity = new Vector3(1f, 0.97f, 1f);
        private Vector3 _gravity = Vector3.Down / 500f;

        private const float Diameter = 0.2f;
        private const int Tesselation = 6;
        private bool isFlying;
        private TimeSpan _timeFired;

        private BoundingSphere _boundingSphere;

       private SoundEffect _soundExplode;

      


        public CanonBall(WarZGame game)
            : base(game)
        {
            WZGame = game;
            _ball = new SpherePrimitive(game.GraphicsDevice, Diameter, Tesselation);
            _boundingSphere = new BoundingSphere(_position, Diameter / 2f);

            _soundExplode = WZGame.Content.Load<SoundEffect>("Sounds/canon - explode2");

        }

        public override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);
            CalculatePhysics(gameTime);
            UpdateBoundingSphereLocation();

           

            base.Update(gameTime);
        }



        public override void Draw(GameTime gameTime)
        {

            Matrix world = Matrix.CreateTranslation(_position);
            _ball.Draw(world, WZGame.ActiveCamera.View, WZGame.ActiveCamera.Projection, Color.Black);

            

            base.Draw(gameTime);
        }


        public void Fire(Vector3 position, Vector3 tankVelocity, float yaw, float pitch, TimeSpan timeFired)
        {
            _position = position;
            _velocity += tankVelocity;

            _direction = Vector3.TransformNormal(Vector3.Forward, Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw));
            _direction.Normalize();
            _timeFired = timeFired;
            isFlying = true;
        }

        public void CalculatePhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Apply pseudo gravity
            if (_direction.Length() > 0)
            {
                _direction += _gravity;
                _direction.Normalize();
            }

            // Calculate force from thrust amount
            Vector3 force = _direction * _thrustAmount * _thrustForce;

            // Apply acceleration
            Vector3 acceleration = force / _mass;
            Velocity += acceleration * elapsed;

            // Apply psuedo drag
            _velocity *= _dragFactor;

            _lastPosition = _position;
            // Apply velocity
            _position += _velocity * elapsed;

        }

        public void Explode()
        {
            if (_soundExplode != null)
                _soundExplode.Play();

            WZGame.ParticlesManager.ExplodeAt(_position);

            isFlying = false;
        }

        [Conditional("DEBUG")]
        void HandleInput(GameTime gameTime)
        {
            float speed = 3f;
            if (KeyboardHelper.IsKeyDown(Keys.NumPad8))
                _position += Vector3.Up * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (KeyboardHelper.IsKeyDown(Keys.NumPad4))
                _position += Vector3.Left * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (KeyboardHelper.IsKeyDown(Keys.NumPad6))
                _position += Vector3.Right * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (KeyboardHelper.IsKeyDown(Keys.NumPad2))
                _position += Vector3.Down * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (KeyboardHelper.IsKeyDown(Keys.NumPad5))
                _position += Vector3.Forward * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (KeyboardHelper.IsKeyDown(Keys.NumPad0))
                _position += Vector3.Backward * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (KeyboardHelper.IsKeyPressed(Keys.OemMinus))
                WZGame.Window.Title = "CanonBall Position>"+_position.ToString() + " | Tank Position>" + WZGame.Player1.Position;

            if (KeyboardHelper.IsKeyPressed(Keys.F))
                Fire(_position, Vector3.Zero, WZGame.ActiveCamera.Yaw, WZGame.ActiveCamera.Pitch, gameTime.TotalGameTime);
        }

        void UpdateBoundingSphereLocation()
        {
            _boundingSphere.Center = _position;
        }
        #region Properties
        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public Vector3 Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public bool IsFlying
        {
            get { return isFlying; }
            set { isFlying = value; }
        }


        public Vector3 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public TimeSpan TimeFired
        {
            get { return _timeFired; }
        }

       public Vector3 LastPosition
       {
           get { return _lastPosition; }
       }

       public SoundEffect SoundExplode
       {
           get { return _soundExplode; }
           set { _soundExplode = value; }
       }

       #endregion
    }
}
