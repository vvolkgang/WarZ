using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WarZ
{
    public class ParticlesManager : DrawableGameComponent
    {
        private WarZGame WZGame;

        //PARTICLES

        // This sample uses five different particle systems.
        ParticleSystem explosionParticles;
        ParticleSystem explosionSmokeParticles;
        ParticleSystem projectileTrailParticles;
        ParticleSystem smokePlumeParticles;
        ParticleSystem fireParticles;

        const int fireParticlesPerFrame = 20;
        Random random = new Random();

        List<Vector3> _smokeEmitterPosition = new List<Vector3>(20);
        List<Explosion> explosions = new List<Explosion>(20);
        List<Vector3> _fireEmitterPosition = new List<Vector3>(20);

        public ParticlesManager(WarZGame game)
            : base(game)
        {
            WZGame = game;

            explosionParticles = new ExplosionParticleSystem(WZGame);
            explosionSmokeParticles = new ExplosionSmokeParticleSystem(WZGame);
            projectileTrailParticles = new ProjectileTrailParticleSystem(WZGame);
            smokePlumeParticles = new SmokePlumeParticleSystem(WZGame);
            fireParticles = new FireParticleSystem(WZGame);

            // Set the draw order so the explosions and fire
            // will appear over the top of the smoke.
            smokePlumeParticles.DrawOrder = 150;
            explosionSmokeParticles.DrawOrder = 200;
            projectileTrailParticles.DrawOrder = 300;
            explosionParticles.DrawOrder = 400;
            fireParticles.DrawOrder = 500;

            // Register the particle system WZGame.Components.
            WZGame.Components.Add(explosionParticles);
            WZGame.Components.Add(explosionSmokeParticles);
            WZGame.Components.Add(projectileTrailParticles);
            WZGame.Components.Add(smokePlumeParticles);
            WZGame.Components.Add(fireParticles);

        }

        public override void Update(GameTime gameTime)
        {
            UpdateSmokePlume();
            UpdateExplosions(gameTime);
            UpdateParticles(gameTime);
            UpdateFire();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawParticles();

            base.Draw(gameTime);
        }

        public void AddSmokePlumeEmitter(Vector3 position)
        {
            _smokeEmitterPosition.Add(position);
        }

        public void AddFireEmitter(Vector3 position)
        {
            _fireEmitterPosition.Add(position);
        }

        public void UpdateFire()
        {
            foreach (Vector3 vector in _fireEmitterPosition)
            {
                // Create a number of fire particles, randomly positioned around a circle.
                for (int i = 0; i < fireParticlesPerFrame; i++)
                {
                    fireParticles.AddParticle(RandomPointOnCircle(vector), Vector3.Zero);
                }

                // Create one smoke particle per frmae, too.
                smokePlumeParticles.AddParticle(RandomPointOnCircle(vector), Vector3.Zero);
            }
            
        }

        Vector3 RandomPointOnCircle(Vector3 position)
        {
            const float radius = 3;

            double angle = random.NextDouble() * Math.PI * 2;

            float x = (float)Math.Cos(angle);
            float z = (float)Math.Sin(angle);

            return new Vector3(x *radius + position.X, position.Y, -z * radius + position.Z);
        }

        public void ExplodeAt(Vector3 position)
        {
            explosions.Add(new Explosion(explosionParticles, explosionSmokeParticles)
            {
                Position = position
            });
        }

        void UpdateExplosions(GameTime gameTime)
        {
            int i = 0;

            while (i < explosions.Count)
            {
                if (!explosions[i].Update(gameTime))
                {
                    // Remove projectiles at the end of their life.
                    explosions.RemoveAt(i);
                }
                else
                {
                    // Advance to the next projectile.
                    i++;
                }
            }
        }

        public void UpdateSmokePlume()
        {
            foreach (Vector3 vector3 in _smokeEmitterPosition)
            {
                // This is trivial: we just create one new smoke particle per frame.
                smokePlumeParticles.AddParticle(vector3, Vector3.Zero);
            }

        }

        public void UpdateParticles(GameTime gameTime)
        {
            //PARTICLES
            explosionParticles.Update(gameTime);
            explosionSmokeParticles.Update(gameTime);
            smokePlumeParticles.Update(gameTime);
            fireParticles.Update(gameTime);
        }
        public void DrawParticles()
        {
            //particles
            explosionParticles.SetCamera(WZGame.ActiveCamera.View, WZGame.ActiveCamera.Projection);
            explosionSmokeParticles.SetCamera(WZGame.ActiveCamera.View, WZGame.ActiveCamera.Projection);
            smokePlumeParticles.SetCamera(WZGame.ActiveCamera.View, WZGame.ActiveCamera.Projection);
            fireParticles.SetCamera(WZGame.ActiveCamera.View, WZGame.ActiveCamera.Projection);
        }
    }
}
