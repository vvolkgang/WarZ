using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarZ
{
    public static class InputController
    {
        private static WarZGame _game;

        private static bool initialized = false;

        public static void Initialize(WarZGame game)
        {
            if (initialized)
                return;

            _game = game;
            initialized = true;
        }
        public static void Update()
        {
            if (!initialized)
                return;

            //if(KeyboardHelper.IsKeyPressed())
        }
    }
}
