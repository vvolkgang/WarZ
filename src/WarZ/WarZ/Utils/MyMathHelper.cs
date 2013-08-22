using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WarZ
{
    static class MyMathHelper
    {

        /// <summary>
        /// Turns the entity towards a vector at the given speed. This is the same logic
        /// as the TurnToFace function that was introduced in the Chase and Evade
        /// sample.
        /// </summary>
        public static float TurnToFace(Vector3 Position, float orientation, Vector3 facePosition, float turnSpeed)
        {
            
            float x = facePosition.X - Position.X;
            float z = facePosition.Z - Position.Z;
            
            float desiredAngle = (float)Math.Atan2(z, x);

           // float difference = WrapAngle(desiredAngle - orientation);
            float difference = desiredAngle - orientation;
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

           // orientation = WrapAngle(orientation + difference);
            orientation = orientation + difference;
            return orientation;
        }


        /// <summary>
        /// Turns the entity towards a vector at the given speed. This is the same logic
        /// as the TurnToFace function that was introduced in the Chase and Evade
        /// sample.
        /// </summary>
        public static void TurnToFace(Vector3 Position , ref float orientation, Vector2 facePosition, float turnSpeed)
        {
            float x = facePosition.X - Position.X;
            float y = facePosition.Y - Position.Y;

            
            float desiredAngle = (float)Math.Atan2(y, x);

            float difference = WrapAngle(desiredAngle - orientation);

            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);
            orientation = WrapAngle(orientation + difference);
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// <param name="radians">the angle to wrap, in radians.</param>
        /// <returns>the input value expressed in radians from -Pi to Pi.</returns>
        /// </summary>
        public static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        public static Vector3 RadiansToVector(float upDownRadians, float leftRightRadians)
        {
            Vector3 tempVector3;

            tempVector3.X = (float)System.Math.Cos((double)leftRightRadians) * (float)System.Math.Sin((double)upDownRadians);
            tempVector3.Y = (float)System.Math.Sin((double)leftRightRadians) * (float)System.Math.Sin((double)upDownRadians);
            tempVector3.Z = (float)System.Math.Cos((double)upDownRadians);

            return tempVector3;
        }

        public static float VectorToRadiansY(Vector3 vector)
        {
            return (float)System.Math.Atan2(vector.Y, vector.X);
        }

        public static void VectorToRadiansY(Vector3 vector, out float leftRightRadians)
        {
            leftRightRadians = (float)System.Math.Atan2(vector.Y, vector.X);
        }

        public static void VectorToRadiansX(Vector3 vector, out float upDownRadians)
        {
            upDownRadians = (float)System.Math.Acos(vector.Z / vector.Length());
        }



        public static void VectorToRadians(Vector3 vector, out float upDownRadians, out float leftRightRadians)
        {
            upDownRadians = (float)System.Math.Acos(vector.Z / vector.Length());
            leftRightRadians = (float)System.Math.Atan2(vector.Y, vector.X);
        }




    }
}
