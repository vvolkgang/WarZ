using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WarZ
{
    class CollisionSystem : GameComponent
    {
        private Player _player;
        private AiPlayerManager _aiM;
        private WarZGame WZGame;

        public CollisionSystem(WarZGame game, Player player, AiPlayerManager aiManager)
            : base(game)
        {
            _player = player;
            _aiM = aiManager;
            WZGame = game;
        }

        public override void Update(GameTime gameTime)
        {
            bool coll = false;
            foreach (AiPlayer ai in _aiM.AiPlayers)
            {

                //check for collision between tanks
                if (FinerCheck(_player.Tank.Model, _player.TankWorld, ai.Tank.Model, ai.TankWorld))
                {
                    //colidio
                    _player.IsGhost = true;

                    //  WZgame.Window.Title = "Collision!!";
                    //device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Red, 1, 0);
                }
                else
                {
                    _player.IsGhost = false;
                    //WZgame.Window.Title = "No collision ...";
                    //device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1, 0);
                }


                foreach (CanonBall canonBall in _player.CanonManager.Balls)
                {
                    if (canonBall.IsFlying)
                    {
                        //check collision between CanonBalls and AiTanks
                        coll = RayCollision(ai.Tank.Model, ai.Tank.World, canonBall.LastPosition, canonBall.Position);
                        if (coll)
                        {
                            ai.Enable = false;
                            canonBall.Explode();
                            WZGame.ParticlesManager.ExplodeAt(canonBall.Position);
                            WZGame.ParticlesManager.AddSmokePlumeEmitter(ai.Position);
                        //    WZGame.ParticlesManager.AddFireEmitter(ai.Position);

                            continue;
                        }

                        //check collision between CanonBalls and Terrain
                        float exactHeight = WZGame.Terrain.GetExactHeightAt(canonBall.Position.X, canonBall.Position.Z);
                        float canonHeight = canonBall.Position.Y;
                        if (exactHeight > canonHeight || WZGame.Terrain.LimitsCollision(canonBall.Position))
                        {
                            canonBall.Explode();
                            
                            continue;
                        }
                    }
                }
                coll = false;

            }

            base.Update(gameTime);
        }


        private bool CoarseCheck(Model model1, Matrix world1, Model model2, Matrix world2)
        {
            BoundingSphere origSphere1 = (BoundingSphere)model1.Tag;
            BoundingSphere sphere1 = XNAUtils.TransformBoundingSphere(origSphere1, world1);

            BoundingSphere origSphere2 = (BoundingSphere)model2.Tag;
            BoundingSphere sphere2 = XNAUtils.TransformBoundingSphere(origSphere2, world2);

            bool collision = sphere1.Intersects(sphere2);
            return collision;
        }

        private bool FinerCheck(Model model1, Matrix world1, Model model2, Matrix world2)
        {
            if (CoarseCheck(model1, world1, model2, world2) == false)
                return false;

            bool collision = false;
            Matrix[] model1Transforms = new Matrix[model1.Bones.Count];
            Matrix[] model2Transforms = new Matrix[model2.Bones.Count];
            model1.CopyAbsoluteBoneTransformsTo(model1Transforms);
            model2.CopyAbsoluteBoneTransformsTo(model2Transforms);
            foreach (ModelMesh mesh1 in model1.Meshes)
            {
                BoundingSphere origSphere1 = mesh1.BoundingSphere;
                Matrix trans1 = model1Transforms[mesh1.ParentBone.Index] * world1;
                BoundingSphere transSphere1 = XNAUtils.TransformBoundingSphere(origSphere1, trans1);

                foreach (ModelMesh mesh2 in model2.Meshes)
                {
                    BoundingSphere origSphere2 = mesh2.BoundingSphere;
                    Matrix trans2 = model2Transforms[mesh2.ParentBone.Index] * world2;
                    BoundingSphere transSphere2 = XNAUtils.TransformBoundingSphere(origSphere2, trans2);

                    if (transSphere1.Intersects(transSphere2))
                        collision = true;
                }
            }
            return collision;
        }

        private bool RayCollision(Model model, Matrix world, Vector3 lastPosition, Vector3 currentPosition)
        {
            BoundingSphere modelSpere = (BoundingSphere)model.Tag;
            BoundingSphere transSphere = XNAUtils.TransformBoundingSphere(modelSpere, world);

            Vector3 direction = currentPosition - lastPosition;
            float distanceCovered = direction.Length();
            direction.Normalize();

            Ray ray = new Ray(lastPosition, direction);

            bool collision = false;
            float? intersection = ray.Intersects(transSphere);
            if (intersection != null)
                if (intersection <= distanceCovered)
                    collision = true;

            return collision;
        }
    }
}
