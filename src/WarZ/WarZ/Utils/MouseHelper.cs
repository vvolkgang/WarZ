using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WarZ
{
    static class MouseHelper
    {
        private static int _prevValue;
        private static float _elapsedTime;
        private static MouseState _prevState;
        private static MouseState _currentState;

        static MouseHelper()
        {
            _prevValue = Mouse.GetState().ScrollWheelValue;

        }

        public static bool IsLeftButtonPressed()
        {
            return (_currentState.LeftButton == ButtonState.Pressed && _prevState.LeftButton == ButtonState.Released);
        }

        public static bool IsRightButtonPressed()
        {
            return (_currentState.RightButton == ButtonState.Pressed && _prevState.RightButton == ButtonState.Released);
        }

        public static bool IsMiddleButtonPressed()
        {
            return (_currentState.MiddleButton == ButtonState.Pressed && _prevState.MiddleButton == ButtonState.Released);
        }


        public static bool hasMoved()
        {
            return Mouse.GetState().ScrollWheelValue != _prevValue;
        }

        public static int ScrollWheelValue(GameTime gameTime)
        {
            int scroll = Mouse.GetState().ScrollWheelValue;
            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            int result = _prevValue - scroll;

            if (_elapsedTime > 0.01f)
            {
                _prevValue = Mouse.GetState().ScrollWheelValue;
                _elapsedTime = 0;
            }

            return result;
        }

        public static void Update()
        {
            // store last state
            _prevState = _currentState;

            // get newest state
            _currentState = Mouse.GetState();
        }
    }
}
