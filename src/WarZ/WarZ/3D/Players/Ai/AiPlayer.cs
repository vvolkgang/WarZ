using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WarZ
{
    class AiPlayer : Player
    {
        private const int MAX_BEHAVIORS = 5;

        private List<AiPlayerBehavior> _behaviors;

        private bool fullSpeed;
        private int id;


        /// <summary>
        /// Don't use this
        /// </summary>
        private bool enable = true;

        private WarZGame WZGame;

        public AiPlayer(WarZGame game, Camera camera, Tank tank, Terrain terrain, int id)
            : base(game, camera, tank, terrain)
        {
            WZGame = game;
            Behaviors = new List<AiPlayerBehavior>(MAX_BEHAVIORS);
            MaxSpeed = 5f;

            this.id = id;
        }

        #region Methods
        public override void Draw(GameTime gameTime)
        {
            UpdateTank(gameTime);
            Tank.IsAi = true;

            Tank.DrawModel(WZGame.ActiveCamera);
        }

        public override void Update(GameTime gameTime)
        {
            if (enable)
            {
                foreach (AiPlayerBehavior behavior in Behaviors)
                {
                    behavior.Update(gameTime);
                }
            

            base.Update(gameTime);
            }
            //  if (id == 0)
            //    WZGame.Window.Title = " | AI Id=0 FacingDirection = " + FacingDirection;
        }

        public void AddBehavior(AiPlayerBehavior behavior)
        {
            Behaviors.Add(behavior);
        }

        protected override void HandleInput(GameTime gameTime)
        {
            if (fullSpeed)
                _movement.Z = -1;
            else
                _movement.Z = 0;
        }

        protected override void UpdateTankTurretOrientation(Vector3 normal)
        {
            UpdateOrientation(normal, Matrix.CreateRotationY(_rotation.Y), out _turretOrientation);
        }

        protected override void UpdateTankCanonOrientation(Vector3 normal)
        {
            UpdateOrientation(normal, Matrix.CreateRotationX(_rotation.X) * Matrix.CreateRotationY(_rotation.Y), out _canonOrientation);
        }
        #endregion

        #region Properties
        public bool FullSpeed
        {
            get { return fullSpeed; }
            set { fullSpeed = value; }
        }

        public List<AiPlayerBehavior> Behaviors
        {
            get { return _behaviors; }
            set { _behaviors = value; }
        }

        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        public int Id
        {
            get { return id; }
        }

        #endregion
    }
}
