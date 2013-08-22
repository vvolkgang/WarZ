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
    public abstract class
Camera : Microsoft.Xna.Framework.GameComponent
    {
        protected Matrix _view;
        protected Matrix _projection;
        protected Vector3 _position;
        protected Vector3 _target;
        protected Vector3 _up;
        protected float _viewAngle;
        protected float _aspectRatio;
        protected float _nearPlane;
        protected float _farPlane;
        protected Vector3 _offset;
        protected float _pitch;
        protected float _yaw;
        protected float _roll;



        public Camera(Game game, Vector3 position, Vector3 target, Vector3 up)
            : base(game)
        {

            //Camera default values
            _viewAngle = MathHelper.PiOver4;
            _aspectRatio = Game.GraphicsDevice.Viewport.AspectRatio;
            _nearPlane = 1f;
            _farPlane = 1000.0f;
            //**************************

            _position = position;
            _target = target;
            _up = up;

            this.UpdateViewMatrix();
            this.UpdateProjectionMatrix();

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {

            base.Initialize();
        }


        /// <summary>
        /// Updates the ViewMatrix
        /// </summary>
        public virtual void UpdateViewMatrix()
        {
            _view = Matrix.CreateLookAt(_position + _offset, _target, _up);
        }

        public virtual void UpdateProjectionMatrix()
        {
            _projection = Matrix.CreatePerspectiveFieldOfView(
                                _viewAngle,
                                _aspectRatio,
                                _nearPlane,
                                _farPlane);
        }

        public void ApplySurfaceFollow(Terrain terrain)
        {
            _position.Y = _offset.Y + terrain.GetPositionHeight(_position.X, _position.Z);
        }

        public Matrix View { get { return _view; } protected set { _view = value; } }
        public Matrix Projection { get { return _projection; } protected set { _projection = value; } }

        public Vector3 Position { get { return _position; } set { _position = value; } }
        public Vector3 Target { get { return _target; } set { _target = value; } }
        public Vector3 Up { get { return _up; } set { _up = value; } }
        public float ViewAngle { get { return _viewAngle; } set { _viewAngle = value; } }
        public float AspectRation { get { return _aspectRatio; } set { _aspectRatio = value; } }
        public float NearPlane { get { return _nearPlane; } set { _nearPlane = value; } }
        public float FarPlane { get { return _farPlane; } set { _farPlane = value; } }
        public Vector3 Offset { get { return _offset; } set { _offset = value; } }

        public float Roll { get { return _roll; } set { _roll = value; } }
        public float Yaw { get { return _yaw; } set { _yaw = value; } }
        public float Pitch { get { return _pitch; } set { _pitch = value; } }
    }
}
