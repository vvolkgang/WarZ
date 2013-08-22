using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WarZ
{
    public interface IComponent3D
    {
         Vector3 Scale { get; set; }
         Vector3 Rotation { get; set; }
         Vector3 Position { get; set; }
    }
}
