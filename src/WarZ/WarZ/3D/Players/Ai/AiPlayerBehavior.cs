using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WarZ
{
    abstract class AiPlayerBehavior
    {


        protected AiPlayer _aiPlayer;
        protected WarZGame WZGame;
        private bool _enabled;


        public AiPlayerBehavior(WarZGame game, AiPlayer player)
        {
            _aiPlayer = player;
            WZGame = game;
            _enabled = true;
        }



        public abstract void Update(GameTime gameTime);


        public AiPlayer AiPlayer
        {
            get { return _aiPlayer; }
            set { _aiPlayer = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }
    }
}
