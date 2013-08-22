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
    public class StaticMesh : IMesh
    {
        private Model _model;
        private Vector3 _scale;
        private Vector3 _rotation;
        private Vector3 _position;

        public StaticMesh(Game game, Model model)
        {
            _model = model;
        }

        #region IMesh
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {

        }

      

        //TODO:CHANGE the Matrix.World of the Draw, this should be scale * rotation * position
        public void DrawModel(Camera camera)
        {
            _model.Draw(Matrix.CreateTranslation(camera.Position), camera.View, camera.Projection);
        }


        #endregion

        #region Properties
        public Model Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public Vector3 Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        #endregion
    }
}
