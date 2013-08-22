using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarZ
{
    static class Prepare3DRenderer
    {
        private static Game _game;

        public static void Initialize(Game game)
        {
            _game = game;
        }

        public static void PrepareGraphicsDevice()
        {
            _game.GraphicsDevice.BlendState = BlendState.Opaque;
            _game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //_game.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
