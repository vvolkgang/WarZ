using System;
using System.Collections.Generic;
using System.Linq;
using Indiefreaks.AOP.Profiler;
using Indiefreaks.Xna.Profiler;
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
    /// This is the main type for your game
    /// </summary>
    public class WarZGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private GUIManager _GUI;

        private Camera _activeCamera; //use ActiveCamera prop
        private Camera[] _cameras;
        private FPSCamera _fpsCamera;
        private TopViewCamera _topViewCamera;
        private ChaseCamera _chaseCamera;
        private SimpleCamera _aiCamera;
        private int cameraNumber;

        private Terrain _terrain;
        private Axis _3DAxis;
        private Skybox _skybox;

        private Player _player1;
        private AiPlayerManager _aiPlayerManager;

        private CollisionSystem _collisionSystem;

        private ParticlesManager _particlesManager;

        public WarZGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            var profilerGameComponent = new ProfilerGameComponent(this, "Fonts/Segoi");
            ProfilingManager.Run = false;
            Components.Add(profilerGameComponent);

            graphics.PreferMultiSampling = true;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Window.Title = "WarZ v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Prepare3DRenderer.Initialize(this);

            Song mainTheme = Content.Load<Song>(@"Music/main1");

            MediaPlayer.Play(mainTheme);
            MediaPlayer.Volume *= 0.7f;

            _fpsCamera = new FPSCamera(this, new Vector3(0, 10, 3), Vector3.Zero, Vector3.Up);
            _topViewCamera = new TopViewCamera(this, new Vector3(0, 10, 0), Vector3.Zero, Vector3.Forward);
            //  _chaseCamera = new ChaseCamera(this, new Vector3(0, 10, 3), Vector3.Zero, Vector3.Up);
            SimpleCamera simpleCam = new SimpleCamera(this, Vector3.Zero, Vector3.Forward, Vector3.Up);
            _aiCamera = new SimpleCamera(this, Vector3.Backward, Vector3.Zero, Vector3.Up);
            GodViewCamera godView = new GodViewCamera(this, Vector3.Up * 50, Vector3.Forward * 62.5f + Vector3.Right * 62.5f, Vector3.Up);
            _cameras = new Camera[3];
            _cameras[0] = _fpsCamera;
            _cameras[1] = _topViewCamera;
            _cameras[2] = godView;

            cameraNumber = 0;

            _activeCamera = _fpsCamera;

            _3DAxis = new Axis(this, _activeCamera);
            _GUI = new GUIManager(this);
            _GUI.CrossairVisible = true;

            Terrain = new Terrain(this, _activeCamera);
            _skybox = new Skybox(this, _activeCamera);

            Tank tank = new Tank(this, Content.Load<Model>(@"Models\Tank\DXTank"));

            Player1 = new Player(this, _activeCamera, tank, Terrain);
            _aiPlayerManager = new AiPlayerManager(this, tank);
            _collisionSystem = new CollisionSystem(this, _player1, _aiPlayerManager);
            _particlesManager = new ParticlesManager(this);





            Components.Add(_skybox);
            //Components.Add(_activeCamera);
            Components.Add(Player1);
            Components.Add(_3DAxis);
            Components.Add(Terrain);

            Components.Add(_aiPlayerManager);
            Components.Add(_collisionSystem);
            Components.Add(_particlesManager);
            Components.Add(_GUI);

            _aiPlayerManager.AddBot(_aiCamera, Vector3.Backward * 10 + Vector3.Right * 20);
            _aiPlayerManager.AddBot(_aiCamera, Vector3.Backward * 10 + Vector3.Right * 10);
            _aiPlayerManager.AddBot(_aiCamera, Vector3.Backward * 10 + Vector3.Right * 10);
            _aiPlayerManager.AddBot(_aiCamera, Vector3.Backward * 10 + Vector3.Right * 10);
            _aiPlayerManager.AddBot(_aiCamera, Vector3.Backward * 10 + Vector3.Right * 10);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
                this.Exit();

            KeyboardHelper.Update(); //Handles the keyboard input
            MouseHelper.Update();

            _terrain.LimitsCollisionCheckWithReposition(_player1);

            //_activeCamera.ApplySurfaceFollow(_terrain);
            //checkLimits();

            if (KeyboardHelper.IsKeyPressed(Keys.V))
                ChangeCamera();






            base.Update(gameTime);


        }


        private void ChangeCamera()
        {
            cameraNumber = ++cameraNumber % _cameras.Length;
            ActiveCamera = _cameras[cameraNumber];
        }

        /// <summary>
        /// Checks Terrain limits
        /// </summary>
        private void checkLimits()
        {
            Vector3 position = _fpsCamera.Position;



            if (position.X < 0.1f)
                position.X = 0.5f;

            if (position.Z > -0.1f)
                position.Z = -0.5f;

            if (position.Z < -Terrain.Width + 2)
                position.Z = -Terrain.Width + 4;

            if (position.X > Terrain.Height - 2)
                position.X = Terrain.Height - 4;

            _fpsCamera.Position = position;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);



            base.Draw(gameTime);
        }

        public Camera ActiveCamera
        {
            get { return _activeCamera; }
            set
            {
                _activeCamera = value;

                if (_activeCamera is TopViewCamera || _activeCamera is GodViewCamera)
                    _GUI.CrossairVisible = false;
                else
                    _GUI.CrossairVisible = true;

                foreach (var component in Components)
                {
                    IGotCamera gotCameraComponent = component as IGotCamera;
                    if (gotCameraComponent != null)
                        gotCameraComponent.Camera = _activeCamera;
                }

            }
        }

        public Terrain Terrain
        {
            get { return _terrain; }
            set { _terrain = value; }
        }

        public Player Player1
        {
            get { return _player1; }
            set { _player1 = value; }
        }

        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        public ParticlesManager ParticlesManager
        {
            get { return _particlesManager; }
            set { _particlesManager = value; }
        }
    }
}
