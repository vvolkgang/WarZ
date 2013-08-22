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
    public class GUIManager : Microsoft.Xna.Framework.DrawableGameComponent
    {

        private SpriteBatch spriteBatch;
        private SpriteFont _font1;
        private Texture2D _crossair;
        private Vector2 _crossairMiddle;

        private FPSCounter _fpsCounter;
        private bool _crossairVisible;


        public GUIManager(Game game)
            : base(game)
        {

        }



        public override void Initialize()
        {

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);



#if DEBUG
            _fpsCounter = new FPSCounter(Game);

            Game.Components.Add(_fpsCounter);
#endif
            _crossair = Game.Content.Load<Texture2D>(@"Textures\crossair");

            _crossairMiddle = new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - _crossair.Width / 2,
                                          Game.GraphicsDevice.Viewport.Height / 2 - _crossair.Height / 2);

            _font1 = Game.Content.Load<SpriteFont>(@"Fonts\Segoi");
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
#if DEBUG
            spriteBatch.DrawString(_font1,
                "FPS> " + _fpsCounter.FPS +
                "\nCamera Position:\n X: " + ((WarZGame)Game).ActiveCamera.Position.X +
                "\n Y: " + ((WarZGame)Game).ActiveCamera.Position.Y +
                "\n Z: " + ((WarZGame)Game).ActiveCamera.Position.Z +
                "\nPress F>Toggle FillMode." +
                "\nPress N>Toggle ShowNormals" +
                "\nPress CTRL+F12> Toggle Indiefreaks Profiler" +
                "\nPress V> Change Camera" +
                "\nPress E> Enable / Disable AI Behavior", Vector2.Zero, new Color(0, 255, 0, 255));

#else
            spriteBatch.DrawString(_font1,
                "Controls " +
                "\n Move> WASD or Arrows" +
                "\n Press Ctrl> Toggle FillMode" 
                , Vector2.Zero, new Color(0, 255, 0, 255));
            
#endif
            if (_crossairVisible)
                spriteBatch.Draw(_crossair, _crossairMiddle, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public bool CrossairVisible
        {
            get { return _crossairVisible; }
            set { _crossairVisible = value; }
        }
    }
}
