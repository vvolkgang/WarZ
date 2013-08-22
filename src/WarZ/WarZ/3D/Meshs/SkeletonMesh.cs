using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace WarZ
{
    public abstract class SkeletonMesh : IMesh
    {
        protected Model _model;
        protected Vector3 _scale = Vector3.One;
        protected Vector3 _rotation = Vector3.Zero;
        protected Vector3 _position = Vector3.Zero;

        protected SkeletonMesh(Game game, Model model)
        {
            _model = model;
        }

        #region IMesh

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void Update(GameTime gameTime)
        {

        }


        public virtual void DrawModel(Camera camera)
        {
            
        }

        #endregion
        #region Properties
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

        public Model Model
        {
            get
            {
                return _model;
            }
            set { _model = value; }
        }
        #endregion



        
    }
}
