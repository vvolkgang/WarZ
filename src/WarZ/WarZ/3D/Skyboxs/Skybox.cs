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
    public class Skybox : Microsoft.Xna.Framework.DrawableGameComponent, IGotCamera
    {
        private Model skyboxModel;
        private Matrix[] skyboxTransforms;
        private Camera _camera;
        BasicEffect basicEffect;

        public Skybox(Game game, Camera camera)
            : base(game)
        {
            _camera = camera;
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }
        protected override void LoadContent()
        {
            skyboxModel = Game.Content.Load<Model>(@"Skybox\skybox");
            skyboxTransforms = new Matrix[skyboxModel.Bones.Count];

            base.LoadContent();
        }
        
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Prepare3DRenderer.PrepareGraphicsDevice();
            Game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            //draw the skybox
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            
            skyboxModel.CopyAbsoluteBoneTransformsTo(skyboxTransforms);
            
            foreach (ModelMesh mesh in skyboxModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = skyboxTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(_camera.Position);
                    effect.View = _camera.View;
                    effect.Projection = _camera.Projection;
                }
                mesh.Draw();
            }

            
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            base.Draw(gameTime);
        }

        public Camera Camera
        {
            get { return _camera; }
            set { _camera = value; }
        }
    }
}
