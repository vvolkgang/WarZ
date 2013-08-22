using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace WarZ
{
    class GodViewCamera : Camera, INotAttachableCamera
    {
        private WarZGame WZGame;
        private Vector3 _standartPosition = Vector3.Up * 30;
        public GodViewCamera(WarZGame game, Vector3 position, Vector3 target, Vector3 up) 
            : base(game, position, target, up)
        {
            WZGame = game;

        }


    }
}
