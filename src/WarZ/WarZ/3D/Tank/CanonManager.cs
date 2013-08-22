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
    public class CanonManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private WarZGame WZGame;

        private const int MAX_BALLS = 10;

        private List<CanonBall> _balls;

        private float timeOut = 10f;




        public CanonManager(WarZGame game)
            : base(game)
        {
            WZGame = game;
            _balls = new List<CanonBall>(MAX_BALLS);
            fillCanon();
            
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            
            foreach (CanonBall canonBall in _balls)
            {
                if (canonBall.IsFlying)
                {
                    canonBall.Update(gameTime);

                    float timeSinceFired = (float)gameTime.TotalGameTime.Subtract(canonBall.TimeFired).TotalSeconds;

                    if (timeSinceFired > timeOut)
                    {
                        canonBall.Explode();
                        WZGame.ParticlesManager.ExplodeAt(canonBall.Position);

                        //Particles Explode


                    }
                }
                else
                {
                    canonBall.IsFlying = false;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (CanonBall canonBall in _balls)
            {
                
                if (canonBall.IsFlying)
                {
                    canonBall.Draw(gameTime);
                }
            }
            base.Draw(gameTime);
        }




        public bool Fire(Vector3 position, Vector3 tankVelocity, float yaw, float pitch, TimeSpan totalGameTime)
        {
            int i = 0;
            while (i < _balls.Count)
            {
                CanonBall current = _balls[i];
                if (!current.IsFlying)
                {
                    current.Fire(position, tankVelocity, yaw, pitch, totalGameTime);
                    return true;
                }
                ++i;
            }

            return false;
        }

        private void fillCanon()
        {

            for (int i = 0; i < MAX_BALLS; i++)
            {
                _balls.Add(new CanonBall(WZGame));
            }
        }






        #region Properties
        public List<CanonBall> Balls
        {
            get { return _balls; }
        }
        #endregion
    }
}
