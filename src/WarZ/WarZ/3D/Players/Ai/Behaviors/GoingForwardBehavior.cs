using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WarZ
{
    class GoingForwardBehavior : AiPlayerBehavior
    {
        Random random = new Random();

        private float[] _directions = {
                                          MathHelper.PiOver2,
                                          MathHelper.PiOver4,
                                          MathHelper.PiOver4/2,
                                          MathHelper.PiOver4/3
                                      };

        public GoingForwardBehavior(WarZGame game, AiPlayer aiPlayer)
            : base(game, aiPlayer)
        {
            aiPlayer.FullSpeed = true;
        }

        public override void Update(GameTime gameTime)
        {

            if (WZGame.Terrain.LimitsCollisionCheckWithReposition(_aiPlayer))
            {
                _aiPlayer.FacingDirection += _directions[random.Next(0, _directions.Length - 1)];
                random.Next(); //sometimes random returns the same values, this will try to prevent that.
            }
                
        }
    }
}
