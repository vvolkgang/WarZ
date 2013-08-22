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
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent, IComponent3D, IGotCamera
    {
        private WarZGame WZGame;
        protected Vector3 _scale;
        protected Vector3 _rotation = new Vector3(0, 0, 0);
        protected Vector3 _position = Vector3.Forward + Vector3.Right;


        protected Camera _camera;
        readonly Vector3 _fpsCameraOffset = Vector3.Backward * 6f + Vector3.Up * 2f;
        readonly Vector3 _topViewCameraOffset = Vector3.Up * 20f;
        readonly Vector3 _chaseCameraOffset = Vector3.Backward * 20f + Vector3.Up * 2f;

        private Tank _mesh;
        private BoundingSphere _boundingSphere;

        protected Terrain _terrain;

        protected float _rotationSpeed = 2f;
        private float _maxSpeed = 15f;
        protected float _turnAmount = 0;

        protected Vector3 _rotateRight = Vector3.Down; //Because the rotation is going to be in the Y axis so, the Y in here is 1
        protected Vector3 _rotateLeft = Vector3.Up; //and in here is -1

        private float _facingDirection = 0;
        protected Vector3 _facingDirectionVector = Vector3.Forward;
        protected Matrix _facingDirectionMatrix = Matrix.Identity;

        protected Vector3 _rotationDirection = Vector3.Zero;
        protected Vector3 _movementDirection = Vector3.Zero;
        protected Vector3 _newMovement;

        protected Matrix wheelRollMatrix = Matrix.Identity;
        protected Matrix _orientation = Matrix.Identity;
        protected Matrix _turretOrientation = Matrix.Identity;
        protected Matrix _canonOrientation = Matrix.Identity;

        // The radius of the tank's wheels. This is used when we calculate how fast they
        // should be rotating as the tank moves.
        private const float _tankWheelRadius = 1.5f;
        protected Vector3 _movement;
        private bool recenter;

        private CanonManager _canonManager;

        private SoundEffect _soundFire;
        private SoundEffect _soundTankGo;
        private SoundEffectInstance _soundTankGoInstance;

        private bool soundIsPlaying;
        private bool isGhost;

        private const float reloadingTime = 2.2f;
        private float timeSpent = 2.2f;

        public Player(WarZGame game, Camera camera, Tank tank, Terrain terrain)
            : base(game)
        {
            WZGame = game;
            _mesh = tank;

            _terrain = terrain;

            ChangeCamera(camera);

            _canonManager = new CanonManager(game);

            _soundFire = WZGame.Content.Load<SoundEffect>("Sounds/tank - shoot");
            _soundTankGo = WZGame.Content.Load<SoundEffect>("Sounds/tank - go");
            _soundTankGoInstance = _soundTankGo.CreateInstance();
            _soundTankGoInstance.IsLooped = true;
            // _camera.HorizontalOffset = 4f;
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



        public override void Draw(GameTime gameTime)
        {
            UpdateTank(gameTime);
            _mesh.IsAi = false;
            _mesh.DrawModel(_camera);

            CanonManager.Draw(gameTime);

            base.Draw(gameTime);
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);

            if (recenter)
            {
                Recenter(gameTime);
                recenter = false;
            }
            else
            {
                // tratar da rotacao e posicao do tank
                _turnAmount = MathHelper.Clamp(_turnAmount, -1, 1);
                _facingDirection += _turnAmount * _rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //_facingDirection = MathHelper.WrapAngle(FacingDirection);
            }

            _facingDirectionMatrix = Matrix.CreateRotationY(_facingDirection);

            if (_camera is ChaseCamera)
                UpdateChaseCameraDirection((ChaseCamera)_camera);

            Vector3 velocity = Vector3.Transform(_movement, _orientation);
            velocity *= _maxSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 newPosition = Position + velocity;

            // calculate tank orientation
            Vector3 normal;
            normal = _terrain.GetInterpolatedNormalAt(newPosition.X, newPosition.Z);

            UpdateTankOrientation(normal);
            UpdateTankTurretOrientation(normal);
            UpdateTankCanonOrientation(normal);

            //calculate wheel rotation
            float distanceMoved = Vector3.Distance(_position, newPosition);
            float theta = distanceMoved / _tankWheelRadius;
            int rollDirection = _movement.Z < 0 ? 1 : -1;

            wheelRollMatrix *= Matrix.CreateRotationX(theta * rollDirection);

            _position = newPosition;

            _position.Y = _terrain.GetInterpolatedHeightAt(_position.X, _position.Z);

            if (distanceMoved > 0 && !soundIsPlaying)
            {
                _soundTankGoInstance.Play();
                soundIsPlaying = true;
            }
            else
                _soundTankGoInstance.Stop();

            if (_camera != null)
                UpdateCamera(gameTime);

            UpdateCanonManager(gameTime);
            UpdateTank(gameTime);

            base.Update(gameTime);
        }

        private void ChangeCamera(Camera camera)
        {
            _camera = camera;

            if (_camera != null)
            {
                if (_camera is TopViewCamera)
                    _camera.Offset = _topViewCameraOffset;
                else
                    if (_camera is ChaseCamera)
                        _camera.Offset = _chaseCameraOffset;
                    else
                        _camera.Offset = _fpsCameraOffset;
            }
        }

        private void UpdateChaseCameraDirection(ChaseCamera camera)
        {
            _facingDirectionVector = Vector3.TransformNormal(_facingDirectionVector, _facingDirectionMatrix);
            _facingDirectionVector.Normalize();

            camera.ChaseDirection = _facingDirectionVector;

        }

        void UpdateCanonManager(GameTime gameTime)
        {
            CanonManager.Update(gameTime);
        }

        protected virtual void HandleInput(GameTime gameTime)
        {

            //movement
            _movement = Vector3.Zero;
            if (KeyboardHelper.IsKeyDown(Keys.W))
                _movement.Z = -1;
            else if (KeyboardHelper.IsKeyDown(Keys.S))
                _movement.Z = 1;

            //rotation
            _turnAmount = 0;
            if (KeyboardHelper.IsKeyDown(Keys.A))
                _turnAmount += 1;
            else if (KeyboardHelper.IsKeyDown(Keys.D))
                _turnAmount += -1;

            timeSpent += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSpent > reloadingTime)
            {
                if (MouseHelper.IsLeftButtonPressed())
                {
                    CanonManager.Fire(_position + Vector3.Up * 1.7f, Vector3.Zero, _camera.Yaw, _camera.Pitch, gameTime.TotalGameTime);
                    _soundFire.Play();

                    timeSpent = 0f;
                }


            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                recenter = true;

            if (Keyboard.GetState().IsKeyDown(Keys.T))
                _soundTankGoInstance.Play();
        }

        public void Recenter(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float angle = 0;
            MyMathHelper.VectorToRadiansY(
                Vector3.Lerp(_facingDirectionVector, Vector3.Up * (_camera.Yaw + MathHelper.Pi), _rotationSpeed * elapsed),
                out angle);


            FacingDirection = angle;


        }


        void UpdateTankOrientation(Vector3 normal)
        {
            UpdateOrientation(normal, _facingDirectionMatrix, out _orientation);
        }

        protected virtual void UpdateTankTurretOrientation(Vector3 normal)
        {
            UpdateOrientation(normal, Matrix.CreateRotationY(_camera.Yaw), out _turretOrientation);

        }

        protected virtual void UpdateTankCanonOrientation(Vector3 normal)
        {
            UpdateOrientation(normal, Matrix.CreateRotationX(_camera.Pitch) * Matrix.CreateRotationY(_camera.Yaw), out _canonOrientation);
        }

        void UpdateCamera(GameTime gameTime)
        {
            if (!(_camera is INotAttachableCamera))
                _camera.Position = _position;

            if (_camera is TopViewCamera)
                _camera.Target = _position;

            if (_camera is ChaseCamera)
            {
                _camera.Target = _position;
                ((ChaseCamera)_camera).Reset();
            }

            _camera.Update(gameTime);

        }

        protected virtual void UpdateTank(GameTime gameTime)
        {
            UpdateTankData();
            _mesh.Update(gameTime);
        }


        void UpdateTankData()
        {
            _mesh.Position = _position;
            _mesh.Rotation = _rotation;
            _mesh.Orientation = _orientation;
            _mesh.TurretOrientation = _turretOrientation;
            _mesh.WheelRollMatrix = wheelRollMatrix;
            _mesh.CanonOrientation = _canonOrientation;
            _mesh.IsGhost = isGhost;
        }


        protected void UpdateOrientation(Vector3 normal, Matrix initialization, out Matrix orientation)
        {
            orientation = initialization;
            orientation.Up = normal;

            orientation.Right = Vector3.Cross(orientation.Forward, orientation.Up);
            orientation.Right = Vector3.Normalize(orientation.Right);

            orientation.Forward = Vector3.Cross(orientation.Up, orientation.Right);
            orientation.Forward = Vector3.Normalize(orientation.Forward);
        }

        #region Properties

        public Matrix TankWorld
        {
            get
            {
                UpdateTankData();
                return _mesh.World;
            }
        }

        public float RotationSpeed { get { return _rotationSpeed; } }

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

        public Camera Camera
        {
            get { return _camera; }
            set { ChangeCamera(value); }
        }

        public float FacingDirection
        {
            get { return _facingDirection; }

            set { _facingDirection = value; }
        }

        public float MaxSpeed
        {
            get { return _maxSpeed; }
            set { _maxSpeed = value; }
        }

        public Tank Tank
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        public CanonManager CanonManager
        {
            get { return _canonManager; }
        }

        public bool IsGhost
        {
            get { return isGhost; }
            set { isGhost = value; }
        }

        #endregion


    }
}
