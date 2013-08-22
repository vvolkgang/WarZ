using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarZ
{
    public interface IMesh : IComponent3D
    {

        void DrawModel(Camera camera);
        void Update(GameTime gameTime);

        Model Model { get; set; }
    }
}
