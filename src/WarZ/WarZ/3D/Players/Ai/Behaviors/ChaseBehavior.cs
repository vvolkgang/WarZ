using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WarZ
{
    class ChaseBehavior : AiPlayerBehavior
    {
        private Player _player;

        public ChaseBehavior(WarZGame game, AiPlayer aiPlayer, Player player)
            : base(game,aiPlayer)
        {
            _aiPlayer.FullSpeed = true;
            _player = player;
        }

        public override void Update(GameTime gameTime)
        {
            if (Enabled)
            {

                float orientation = _aiPlayer.FacingDirection;

                Vector3 aiPosition = _aiPlayer.Position;
               // aiPosition.Z *= 1;

                Vector3 p1Position = _player.Position;
               // p1Position.Z *= 1;

                orientation = MyMathHelper.TurnToFace(aiPosition, orientation, p1Position, _player.RotationSpeed);
                _aiPlayer.FacingDirection = orientation;

                
            }
        }
    }
}
