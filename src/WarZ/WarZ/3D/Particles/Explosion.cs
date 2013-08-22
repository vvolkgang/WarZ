using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WarZ
{
    class Explosion
    {
        #region Constants

        const float trailParticlesPerSecond = 100;
        const int numExplosionParticles = 30;
        const int numExplosionSmokeParticles = 50;
        const float projectileLifespan = 0.5f;
        const float sidewaysVelocityRange = 60;
        const float verticalVelocityRange = 20;
        const float gravity = 15;

        #endregion

        #region Fields

        ParticleSystem explosionParticles;
        ParticleSystem explosionSmokeParticles;

        private Vector3 position;
        Vector3 velocity;
        float age;

        static Random random = new Random();
        #endregion

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Explosion(ParticleSystem explosionParticles,
                          ParticleSystem explosionSmokeParticles)
        {
            this.explosionParticles = explosionParticles;
            this.explosionSmokeParticles = explosionSmokeParticles;

            // Start at the origin, firing in a random (but roughly upward) direction.
            //Position = Vector3.Zero;

            velocity.X = (float)(random.NextDouble() - 0.5) * sidewaysVelocityRange;
            velocity.Y = (float)(random.NextDouble() + 0.5) * verticalVelocityRange;
            velocity.Z = (float)(random.NextDouble() - 0.5) * sidewaysVelocityRange;

        }

        public bool Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Simple projectile physics.
            Position += velocity * elapsedTime;
            velocity.Y -= elapsedTime * gravity;
            age += elapsedTime;


            for (int i = 0; i < numExplosionParticles; i++)
                explosionParticles.AddParticle(Position, velocity);

            for (int i = 0; i < numExplosionSmokeParticles; i++)
                explosionSmokeParticles.AddParticle(Position, velocity);

            return false;

        }
    }
}
