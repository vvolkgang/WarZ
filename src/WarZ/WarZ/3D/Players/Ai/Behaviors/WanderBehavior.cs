using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WarZ
{
    class WanderBehavior : AiPlayerBehavior
    {
        private Vector3 _wanderDirection = Vector3.One;
        private Random random = new Random();

        public WanderBehavior(WarZGame game, AiPlayer player)
            : base(game, player)
        {
            _wanderDirection.X = (float)Math.Cos(player.FacingDirection);
            _wanderDirection.Z = (float)Math.Sin(player.FacingDirection);
            player.FullSpeed = true;
        }

        public override void Update(GameTime gameTime)
        {
            _wanderDirection.X +=
                MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());
            _wanderDirection.Y +=
                MathHelper.Lerp(-.25f, .25f, (float)random.NextDouble());

            if (_wanderDirection != Vector3.Zero)
            {
                _wanderDirection.Normalize();
            }

            MyMathHelper.TurnToFace(_aiPlayer.Position, _aiPlayer.FacingDirection, _aiPlayer.Position + _wanderDirection, .15f * _aiPlayer.RotationSpeed);

            Vector3 screenCenter = new Vector3(WZGame.Terrain.Width / 2, 0, -WZGame.Terrain.Height / 2);

            float distanceFromCenter = Vector3.Distance(screenCenter, _aiPlayer.Position);
            float MaxDistanceFromScreenCenter =
                Math.Min(screenCenter.Y, screenCenter.X);

            float normalizedDistance = distanceFromCenter / MaxDistanceFromScreenCenter;

            float turnToCenterSpeed = .3f * normalizedDistance * normalizedDistance *
                _aiPlayer.RotationSpeed;

            MyMathHelper.TurnToFace(_aiPlayer.Position, _aiPlayer.FacingDirection, screenCenter, turnToCenterSpeed);

            //_aiPlayer.CurrentSpeed = .25f * _aiPlayer.MaxSpeed;
        }
    }
}
