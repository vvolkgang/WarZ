using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WarZ
{
    sealed class AiPlayerManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private const int MAX_AI = 15;
        private List<AiPlayer> _aiPlayers;
        private int idCount = 0;


        private WarZGame WZGame;
        private Tank _defaultTank;
        private bool enableAll = true;

        public AiPlayerManager(WarZGame game, Tank tank)
            : base(game)
        {
            _aiPlayers = new List<AiPlayer>(MAX_AI);
            this.WZGame = game;
            _defaultTank = tank;
        }

        public List<AiPlayer> AiPlayers
        {
            get { return _aiPlayers; }
        }

        #region Inherited Methods
        public override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);
            if (enableAll)
            {
                foreach (AiPlayer player in AiPlayers)
                {
                    player.Update(gameTime);
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            foreach (AiPlayer player in AiPlayers)
            {
                player.Draw(gameTime);
            }
            base.Draw(gameTime);
        }
        #endregion

        #region Class methods
        public void HandleInput(GameTime gameTime)
        {
            if (KeyboardHelper.IsKeyPressed(Keys.E))
                enableAll = !enableAll;
        }



        public void AddBot(Camera camera)
        {
            AddBot(camera, Vector3.Forward * Vector3.Right);
        }

        public void AddBot(Camera camera, Vector3 Position)
        {
            AiPlayer newAi = new AiPlayer(WZGame, camera, _defaultTank, WZGame.Terrain, idCount++)
            {
                Position = Position
            };

            //newAi.AddBehavior(new ChaseBehavior(WZGame, newAi, WZGame.Player1));
            newAi.AddBehavior(new GoingForwardBehavior(WZGame, newAi));
            //newAi.AddBehavior(new WanderBehavior(WZGame, newAi));
            AiPlayers.Add(newAi);
        }

        public void AddBots(Camera camera, int number)
        {
            for (int i = 0; i < number; i++)
            {
                AddBot(camera);
            }
        }
        #endregion
    }
}
