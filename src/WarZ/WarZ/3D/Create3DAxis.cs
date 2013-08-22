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
    public class Axis : Microsoft.Xna.Framework.DrawableGameComponent, IGotCamera
    {
        private VertexPositionColor[] vertices;
        private BasicEffect basicEffect;
        private Camera _camera;

        public Axis(Game game, Camera camera)
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
            vertices = new VertexPositionColor[6];
            vertices[0] = new VertexPositionColor(Vector3.Down, Color.White);
            vertices[1] = new VertexPositionColor(Vector3.Up, Color.Green);
            vertices[2] = new VertexPositionColor(Vector3.Left, Color.White);
            vertices[3] = new VertexPositionColor(Vector3.Right, Color.Red);
            vertices[4] = new VertexPositionColor(Vector3.Backward, Color.Blue);
            vertices[5] = new VertexPositionColor(Vector3.Forward, Color.White);
            
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.LightingEnabled = false;
            basicEffect.VertexColorEnabled = true;

            base.LoadContent();
        }


        public override void Draw(GameTime gameTime)
        {
            Prepare3DRenderer.PrepareGraphicsDevice();

            basicEffect.CurrentTechnique.Passes[0].Apply();
            basicEffect.World = Matrix.Identity;
            basicEffect.View = _camera.View;
            basicEffect.Projection = _camera.Projection;

            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 3);

            base.Draw(gameTime);
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

        public Camera Camera
        {
            get { return _camera; }
            set { _camera = value; }
        }
    }
}
