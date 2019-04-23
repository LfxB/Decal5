// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------
// Original code from SlimMath project. http://code.google.com/p/slimmath/
// Greetings to SlimDX Group. Original code published with the following license:
// -----------------------------------------------------------------------------
/*
* Copyright (c) 2007-2011 SlimDX Group
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
* 
* https://tohjo.eu/dapkcuf/citizenmp/blob/c8710f0a3cf076c7f2a8fcbb22ed2902116f4f4c/client/clrcore/Math/MathUtil.cs
*/
using System;
using GTA; // This is a reference that is needed! do not edit this
using GTA.Native; // This is a reference that is needed! do not edit this
using GTA.Math;

namespace GTAMath
{
	public static class MathUtil
	{
		/// <summary>
		/// The value for which all absolute numbers smaller than are considered equal to zero.
		/// </summary>
		public const float ZeroTolerance = 1e-6f; // Value a 8x higher than 1.19209290E-07F

		/// <summary>
		/// A value specifying the approximation of π which is 180 degrees.
		/// </summary>
		public const float Pi = (float)Math.PI;

		/// <summary>
		/// A value specifying the approximation of 2π which is 360 degrees.
		/// </summary>
		public const float TwoPi = (float)(2 * Math.PI);

		/// <summary>
		/// A value specifying the approximation of π/2 which is 90 degrees.
		/// </summary>
		public const float PiOverTwo = (float)(Math.PI / 2);

		/// <summary>
		/// A value specifying the approximation of π/4 which is 45 degrees.
		/// </summary>
		public const float PiOverFour = (float)(Math.PI / 4);

		/// <summary>
		/// Checks if a and b are almost equals, taking into account the magnitude of floating point numbers (unlike <see cref="WithinEpsilon"/> method). See Remarks.
		/// See remarks.
		/// </summary>
		/// <param name="a">The left value to compare.</param>
		/// <param name="b">The right value to compare.</param>
		/// <returns><c>true</c> if a almost equal to b, <c>false</c> otherwise</returns>
		/// <remarks>
		/// The code is using the technique described by Bruce Dawson in 
		/// <a href="http://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/">Comparing Floating point numbers 2012 edition</a>. 
		/// </remarks>
		[System.Security.SecuritySafeCritical]
		public static bool NearEqual(float a, float b)
		{
			return NearEqualInternal(a, b);
		}

		[System.Security.SecurityCritical]
		private unsafe static bool NearEqualInternal(float a, float b)
		{
			// Check if the numbers are really close -- needed
			// when comparing numbers near zero.
			if (IsZero(a - b))
				return true;

			// Original from Bruce Dawson: http://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/
			int aInt = *(int*)&a;
			int bInt = *(int*)&b;

			// Different signs means they do not match.
			if ((aInt < 0) != (bInt < 0))
				return false;

			// Find the difference in ULPs.
			int ulp = Math.Abs(aInt - bInt);

			// Choose of maxUlp = 4
			// according to http://code.google.com/p/googletest/source/browse/trunk/include/gtest/internal/gtest-internal.h
			const int maxUlp = 4;
			return (ulp <= maxUlp);
		}

		/// <summary>
		/// Determines whether the specified value is close to zero (0.0f).
		/// </summary>
		/// <param name="a">The floating value.</param>
		/// <returns><c>true</c> if the specified value is close to zero (0.0f); otherwise, <c>false</c>.</returns>
		public static bool IsZero(float a)
		{
			return Math.Abs(a) < ZeroTolerance;
		}

		/// <summary>
		/// Determines whether the specified value is close to one (1.0f).
		/// </summary>
		/// <param name="a">The floating value.</param>
		/// <returns><c>true</c> if the specified value is close to one (1.0f); otherwise, <c>false</c>.</returns>
		public static bool IsOne(float a)
		{
			return IsZero(a - 1.0f);
		}

		/// <summary>
		/// Checks if a - b are almost equals within a float epsilon.
		/// </summary>
		/// <param name="a">The left value to compare.</param>
		/// <param name="b">The right value to compare.</param>
		/// <param name="epsilon">Epsilon value</param>
		/// <returns><c>true</c> if a almost equal to b within a float epsilon, <c>false</c> otherwise</returns>
		public static bool WithinEpsilon(float a, float b, float epsilon)
		{
			float num = a - b;
			return ((-epsilon <= num) && (num <= epsilon));
		}

		/// <summary>
		/// Converts revolutions to degrees.
		/// </summary>
		/// <param name="revolution">The value to convert.</param>
		/// <returns>The converted value.</returns>
		public static float RevolutionsToDegrees(float revolution)
		{
			return revolution * 360.0f;
		}

		/// <summary>
		/// Converts revolutions to radians.
		/// </summary>
		/// <param name="revolution">The value to convert.</param>
		/// <returns>The converted value.</returns>
		public static float RevolutionsToRadians(float revolution)
		{
			return revolution * TwoPi;
		}

		/// <summary>
		/// Converts revolutions to gradians.
		/// </summary>
		/// <param name="revolution">The value to convert.</param>
		/// <returns>The converted value.</returns>
		public static float RevolutionsToGradians(float revolution)
		{
			return revolution * 400.0f;
		}

		/// <summary>
		/// Converts degrees to revolutions.
		/// </summary>
		/// <param name="degree">The value to convert.</param>
		/// <returns>The converted value.</returns>
		public static float DegreesToRevolutions(float degree)
		{
			return degree / 360.0f;
		}

		/// <summary>
		/// Converts degrees to radians.
		/// </summary>
		/// <param name="degree">The value to convert.</param>
		/// <returns>The converted value.</returns>
		public static float DegreesToRadians(float degree)
		{
			return degree * (Pi / 180.0f);
		}

		/// <summary>
		/// Converts radians to revolutions.
		/// </summary>
		/// <param name="radian">The value to convert.</param>
		/// <returns>The converted value.</returns>
		public static float RadiansToRevolutions(float radian)
		{
			return radian / TwoPi;
		}

		/// <summary>
		/// Converts radians to gradians.
		/// </summary>
		/// <param name="radian">The value to convert.</param>
		/// <returns>The converted value.</returns>
		public static float RadiansToGradians(float radian)
		{
			return radian * (200.0f / Pi);
		}

		/// <summary>
		/// Converts gradians to revolutions.
		/// </summary>
		/// <param name="gradian">The value to convert.</param>
		/// <returns>The converted value.</returns>
		public static float GradiansToRevolutions(float gradian)
		{
			return gradian / 400.0f;
		}

		/// <summary>
		/// Converts gradians to degrees.
		/// </summary>
		/// <param name="gradian">The value to convert.</param>
		/// <returns>The converted value.</returns>
		public static float GradiansToDegrees(float gradian)
		{
			return gradian * (9.0f / 10.0f);
		}

		/// <summary>
		/// Converts gradians to radians.
		/// </summary>
		/// <param name="gradian">The value to convert.</param>
		/// <returns>The converted value.</returns>
		public static float GradiansToRadians(float gradian)
		{
			return gradian * (Pi / 200.0f);
		}

		/// <summary>
		/// Converts radians to degrees.
		/// </summary>
		/// <param name="radian">The value to convert.</param>
		/// <returns>The converted value.</returns>
		public static float RadiansToDegrees(float radian)
		{
			return radian * (180.0f / Pi);
		}

		/// <summary>
		/// Clamps the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min.</param>
		/// <param name="max">The max.</param>
		/// <returns>The result of clamping a value between min and max</returns>
		public static float Clamp(float value, float min, float max)
		{
			return value < min ? min : value > max ? max : value;
		}

		/// <summary>
		/// Clamps the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min.</param>
		/// <param name="max">The max.</param>
		/// <returns>The result of clamping a value between min and max</returns>
		public static int Clamp(int value, int min, int max)
		{
			return value < min ? min : value > max ? max : value;
		}

		/// <summary>
		/// Interpolates between two values using a linear function by a given amount.
		/// </summary>
		/// <remarks>
		/// See http://www.encyclopediaofmath.org/index.php/Linear_interpolation and
		/// http://fgiesen.wordpress.com/2012/08/15/linear-interpolation-past-present-and-future/
		/// </remarks>
		/// <param name="from">Value to interpolate from.</param>
		/// <param name="to">Value to interpolate to.</param>
		/// <param name="amount">Interpolation amount.</param>
		/// <returns>The result of linear interpolation of values based on the amount.</returns>
		public static double Lerp(double from, double to, double amount)
		{
			return (1 - amount) * from + amount * to;
		}

		/// <summary>
		/// Interpolates between two values using a linear function by a given amount.
		/// </summary>
		/// <remarks>
		/// See http://www.encyclopediaofmath.org/index.php/Linear_interpolation and
		/// http://fgiesen.wordpress.com/2012/08/15/linear-interpolation-past-present-and-future/
		/// </remarks>
		/// <param name="from">Value to interpolate from.</param>
		/// <param name="to">Value to interpolate to.</param>
		/// <param name="amount">Interpolation amount.</param>
		/// <returns>The result of linear interpolation of values based on the amount.</returns>
		public static float Lerp(float from, float to, float amount)
		{
			return (1 - amount) * from + amount * to;
		}

		/// <summary>
		/// Interpolates between two values using a linear function by a given amount.
		/// </summary>
		/// <remarks>
		/// See http://www.encyclopediaofmath.org/index.php/Linear_interpolation and
		/// http://fgiesen.wordpress.com/2012/08/15/linear-interpolation-past-present-and-future/
		/// </remarks>
		/// <param name="from">Value to interpolate from.</param>
		/// <param name="to">Value to interpolate to.</param>
		/// <param name="amount">Interpolation amount.</param>
		/// <returns>The result of linear interpolation of values based on the amount.</returns>
		public static byte Lerp(byte from, byte to, float amount)
		{
			return (byte)Lerp((float)from, (float)to, amount);
		}

		/// <summary>
		/// Performs smooth (cubic Hermite) interpolation between 0 and 1.
		/// </summary>
		/// <remarks>
		/// See https://en.wikipedia.org/wiki/Smoothstep
		/// </remarks>
		/// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
		public static float SmoothStep(float amount)
		{
			return (amount <= 0) ? 0
				: (amount >= 1) ? 1
				: amount * amount * (3 - (2 * amount));
		}

		/// <summary>
		/// Performs a smooth(er) interpolation between 0 and 1 with 1st and 2nd order derivatives of zero at endpoints.
		/// </summary>
		/// <remarks>
		/// See https://en.wikipedia.org/wiki/Smoothstep
		/// </remarks>
		/// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
		public static float SmootherStep(float amount)
		{
			return (amount <= 0) ? 0
				: (amount >= 1) ? 1
				: amount * amount * amount * (amount * ((amount * 6) - 15) + 10);
		}

		/// <summary>
		/// Calculates the modulo of the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="modulo">The modulo.</param>
		/// <returns>The result of the modulo applied to value</returns>
		public static float Mod(float value, float modulo)
		{
			if (modulo == 0.0f)
			{
				return value;
			}

			return value % modulo;
		}

		/// <summary>
		/// Calculates the modulo 2*PI of the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The result of the modulo applied to value</returns>
		public static float Mod2PI(float value)
		{
			return Mod(value, TwoPi);
		}

		/// <summary>
		/// Wraps the specified value into a range [min, max]
		/// </summary>
		/// <param name="value">The value to wrap.</param>
		/// <param name="min">The min.</param>
		/// <param name="max">The max.</param>
		/// <returns>Result of the wrapping.</returns>
		/// <exception cref="ArgumentException">Is thrown when <paramref name="min"/> is greater than <paramref name="max"/>.</exception>
		public static int Wrap(int value, int min, int max)
		{
			if (min > max)
				throw new ArgumentException(string.Format("min {0} should be less than or equal to max {1}", min, max), "min");

			// Code from http://stackoverflow.com/a/707426/1356325
			int range_size = max - min + 1;

			if (value < min)
				value += range_size * ((min - value) / range_size + 1);

			return min + (value - min) % range_size;
		}

		/// <summary>
		/// Wraps the specified value into a range [min, max[
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The min.</param>
		/// <param name="max">The max.</param>
		/// <returns>Result of the wrapping.</returns>
		/// <exception cref="ArgumentException">Is thrown when <paramref name="min"/> is greater than <paramref name="max"/>.</exception>
		public static float Wrap(float value, float min, float max)
		{
			if (NearEqual(min, max)) return min;

			double mind = min;
			double maxd = max;
			double valued = value;

			if (mind > maxd)
				throw new ArgumentException(string.Format("min {0} should be less than or equal to max {1}", min, max), "min");

			var range_size = maxd - mind;
			return (float)(mind + (valued - mind) - (range_size * Math.Floor((valued - mind) / range_size)));
		}

		/// <summary>
		/// Gauss function.
		/// </summary>
		/// <param name="amplitude">Curve amplitude.</param>
		/// <param name="x">Position X.</param>
		/// <param name="y">Position Y</param>
		/// <param name="radX">Radius X.</param>
		/// <param name="radY">Radius Y.</param>
		/// <param name="sigmaX">Curve sigma X.</param>
		/// <param name="sigmaY">Curve sigma Y.</param>
		/// <returns>The result of Gaussian function.</returns>
		public static float Gauss(float amplitude, float x, float y, float radX, float radY, float sigmaX, float sigmaY)
		{
			return (float)Gauss((double)amplitude, x, y, radX, radY, sigmaX, sigmaY);
		}

		/// <summary>
		/// Gauss function.
		/// </summary>
		/// <param name="amplitude">Curve amplitude.</param>
		/// <param name="x">Position X.</param>
		/// <param name="y">Position Y</param>
		/// <param name="radX">Radius X.</param>
		/// <param name="radY">Radius Y.</param>
		/// <param name="sigmaX">Curve sigma X.</param>
		/// <param name="sigmaY">Curve sigma Y.</param>
		/// <returns>The result of Gaussian function.</returns>
		public static double Gauss(double amplitude, double x, double y, double radX, double radY, double sigmaX, double sigmaY)
		{
			return (amplitude * Math.E) -
				(
					Math.Pow(x - (radX / 2), 2) / (2 * Math.Pow(sigmaX, 2)) +
					Math.Pow(y - (radY / 2), 2) / (2 * Math.Pow(sigmaY, 2))
				);
		}
	}

	public static class MathHelper
	{
		public static Vector3 RotationToDirection(Vector3 Rot)
		{
			try
			{
				float z = Rot.Z;
				float retz = z * 0.0174532924F;
				float x = Rot.X;
				float retx = x * 0.0174532924F;
				float absx = (float)System.Math.Abs(System.Math.Cos(retx));
				return new Vector3((float)-System.Math.Sin(retz) * absx, (float)System.Math.Cos(retz) * absx, (float)System.Math.Sin(retx));
			}
			catch
			{
				return new Vector3(0, 0, 0);
			}

		}

		public static Vector3 DirToRotTest(Vector3 Dir)
		{
			try
			{
				/*
				This is rotation to direction:
				rotx = 0.2
				retx = 0.2 *  0.0174532924 = 0.00349065848 //radians to degrees
				Cos(0.2 *  0.0174532924) = 0.9999939076578741
				absx = Abs(Cos(0.2 *  0.0174532924)) = 0.9999939076578741

				rotz = 0.3
				retz = 0.3 * 0.0174532924 = 0.00523598772 //radians to degrees

				dirx = -Sin(retz) * absx =  -0.005235963795437085 * 0.9999939076578741 = -0.0052359318961542843713968514009985 //-0.0052359318961542846 if rounded
				diry = Cos(retz) * absx = 0.9999862922476151 * 0.9999939076578741 = 0.999980199989
				dirz = Sin(retx) = 0.0052359637954370846088922220789420743196847079456760

				now this is direction to rotation:
				dirz = Sin(retx) =  0.0052359637954370846088922220789420743196847079456760
				num1 = retx = Asin(dirz)
				rotx = num1 / 0.0174532924

				num2 = absx = Abs(Cos(num1)
				num3 = diry / num2 = Cos(retz)
				num4 = retz = Acos(num3)
				rotz = num4 / 0.0174532924

				roty?


				*/

				float trueRotZ;

				float dirz = Dir.Z;
				float num1 = (float)Math.Asin(dirz);
				float rotx = num1 / 0.0174532924f;

				float dirx = Dir.X;
				float num2 = (float)Math.Cos(num1);
				float num3 = dirx / num2;
				float num4 = (float)Math.Asin(-num3);
				float rotz1 = num4 / 0.0174532924f;

				float diry = Dir.Y;
				float num5 = (float)Math.Cos(num1);
				float num6 = diry / num5;
				float num7 = (float)Math.Acos(num6);
				float rotz2 = num7 / 0.0174532924f;

				if (rotz1 > 0)
				{
					if (rotz2 < 90)
					{
						trueRotZ = rotz1;
					}
					else
					{
						trueRotZ = rotz2;
					}
				}
				else
				{
					if (rotz2 < 90)
					{
						trueRotZ = rotz1;
					}
					else
					{
						trueRotZ = -rotz2;
					}
				}

				return new Vector3(rotx, 0, trueRotZ);
			}
			catch
			{
				return Vector3.Zero;
			}
		}

		public static Vector3 DirectionToRotation(Vector3 dir, float roll)
		{
			dir = Vector3.Normalize(dir);
			Vector3 rotval;
			rotval.Z = -MathUtil.RadiansToDegrees((float)Math.Atan2(dir.X, dir.Y));
			Vector3 rotpos = Vector3.Normalize(new Vector3(dir.Z, new Vector3(dir.X, dir.Y, 0.0f).Length(), 0.0f));
			rotval.X = MathUtil.RadiansToDegrees((float)Math.Atan2(rotpos.X, rotpos.Y));
			rotval.Y = roll;
			return rotval;
		}

		public static float DirectionToHeading(Vector3 dir)
		{
			//https://tohjo.eu/dapkcuf/citizenmp/blob/c8710f0a3cf076c7f2a8fcbb22ed2902116f4f4c/client/clrcore/Math/GameMath.cs
			dir.Z = 0.0f;
			dir.Normalize();
			return RadianToDegree((float)-Math.Atan2(dir.X, dir.Y));
		}

		public static Vector3 HeadingToDirection(float Heading)
		{
			Heading = MathUtil.DegreesToRadians(Heading);
			return new Vector3((float)-Math.Sin(Heading), (float)Math.Cos(Heading), 0.0f);
		}

		public static Vector3 VectorBetween(Vector3 vec1, Vector3 vec2)
		{
			return (vec1 + vec2).Normalized;
		}

		public static float Deg2Rad(float _deg)
		{
			//https://forums.gta5-mods.com/post/32103 Thanks Lee!
			double Radian = (Math.PI / 180) * _deg;
			return (float)Radian;
		}

		public static float RadianToDegree(float angle)
		{
			//http://www.vcskicks.com/csharp_net_angles.php
			return (float)(angle * (180.0 / Math.PI));
		}
		
		public static Vector3 ToRotation(this Quaternion q)
		{
			float pitch = (float)Math.Atan2(2.0f * (q.Y * q.Z + q.W * q.X), q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z);
			float yaw = (float)Math.Atan2(2.0f * (q.X * q.Y + q.W * q.Z), q.W * q.W + q.X * q.X - q.Y * q.Y - q.Z * q.Z);
			float roll = (float)Math.Asin(-2.0f * (q.X * q.Z - q.W * q.Y));

			return new Vector3(MathUtil.RadiansToDegrees(pitch), MathUtil.RadiansToDegrees(roll), MathUtil.RadiansToDegrees(yaw));
		}

		/// <summary>
		/// Returns a Quaternion from a Rotation
		/// </summary>
		/// <param name="vect"></param>
		/// <returns></returns>
		public static Quaternion ToQuaternion(this Vector3 vect)
		{
			float rotX = MathUtil.DegreesToRadians(vect.X);
			float rotY = MathUtil.DegreesToRadians(vect.Y);
			float rotZ = MathUtil.DegreesToRadians(vect.Z);

			Quaternion qyaw = Quaternion.RotationAxis(Vector3.WorldUp, rotZ);
			qyaw.Normalize();

			Quaternion qpitch = Quaternion.RotationAxis(Vector3.WorldEast, rotX);
			qpitch.Normalize();

			Quaternion qroll = Quaternion.RotationAxis(Vector3.WorldNorth, rotY);
			qroll.Normalize();

			Quaternion yawpitch = qyaw * qpitch * qroll;
			yawpitch.Normalize();

			return yawpitch;
		   /* vect = new Vector3()
			{
				X = vect.X.Denormalize() * -1f,
				Y = vect.Y.Denormalize() - 180f,
				Z = vect.Z.Denormalize() - 180f,
			};

			vect = vect.TransformVector(Deg2Rad);

			float rollOver2 = vect.Z * 0.5f;
			float sinRollOver2 = (float)Math.Sin((double)rollOver2);
			float cosRollOver2 = (float)Math.Cos((double)rollOver2);
			float pitchOver2 = vect.Y * 0.5f;
			float sinPitchOver2 = (float)Math.Sin((double)pitchOver2);
			float cosPitchOver2 = (float)Math.Cos((double)pitchOver2);
			float yawOver2 = vect.X * 0.5f; // pitch
			float sinYawOver2 = (float)Math.Sin((double)yawOver2);
			float cosYawOver2 = (float)Math.Cos((double)yawOver2);
			GTA.Math.Quaternion result = new GTA.Math.Quaternion();
			result.X = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
			result.Y = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;
			result.Z = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
			result.W = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
			return result;*/
		}

		public static Vector3 ToEuler(this GTA.Math.Quaternion q)
		{
			var pitchYawRoll = new Vector3();

			double sqw = q.W * q.W;
			double sqx = q.X * q.X;
			double sqy = q.Y * q.Y;
			double sqz = q.Z * q.Z;

			pitchYawRoll.Y = (float)Math.Atan2(2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (sqz + sqw));     // Yaw 
			pitchYawRoll.X = (float)Math.Asin(2f * (q.X * q.Z - q.W * q.Y));                             // Pitch 
			pitchYawRoll.Z = (float)Math.Atan2(2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (sqy + sqz));

			pitchYawRoll = pitchYawRoll.TransformVector(RadianToDegree);

			pitchYawRoll = pitchYawRoll.Denormalize();

			pitchYawRoll = new Vector3()
			{
				Y = pitchYawRoll.Y * -1f + 180f,
				X = pitchYawRoll.X,
				Z = pitchYawRoll.Z,
			};

			return pitchYawRoll;
		}

		public static Vector3 AchieveRotation(this Vector3 rot, Vector3 currentRot, float speed)
		{
			Vector3 temp = currentRot;
			
			float xDiff = temp.X - rot.X;
			if ((xDiff > 0f && xDiff < 180f) || (xDiff < 0f && xDiff < -180f))
			{
				//temp.X -= 1f * speed;
				temp.X -= xDiff * speed;
			}
			else
			{
				//temp.X += 1f * speed;
				temp.X += xDiff * speed;
			}

			float yDiff = temp.Y - rot.Y;
			if ((yDiff > 0f && yDiff < 180f) || (yDiff < 0f && yDiff < -180f))
			{
				temp.Y -= yDiff * speed;
			}
			else
			{
				temp.Y += yDiff * speed;
			}

			float zDiff = temp.Z - rot.Z;
			if ((zDiff > 0f && zDiff < 180f) || (zDiff < 0f && zDiff < -180f))
			{
				temp.Z -= zDiff * speed;
			}
			else
			{
				temp.Z += zDiff * speed;
			}

			return temp;
		}
		
		/// <summary>
		/// Get's the rotation needed to face the specified direction.
		/// </summary>
		/// <param name="direction">The direction to face.</param>
		/// <param name="planeNormal">Projects the rotation on this normal vector.</param>
		/// <returns></returns>
		public static Quaternion LookRotation(Vector3 direction, Vector3 planeNormal)
		{
			// Calculate the up rotation.
			var upRotation = Quaternion.FromToRotation(Vector3.WorldUp, planeNormal);

			// Get the signed angle from (0, 1, 0) to the direction delta.
			var sin = Vector3.SignedAngle(Vector3.RelativeFront, direction, planeNormal);

			// Calculate the facing rotation.
			var forwardRotation = Quaternion.Euler(0, 0, sin);

			// Get the look rotation by multiplying both upQ and facingQ.
			var lookRotation = upRotation * forwardRotation;

			// Return the new rotation.
			return lookRotation;
		}

		/// <summary>
		/// Get's the rotation needed to face the specified direction. 
		/// Default plane normal is our up vector.
		/// </summary>
		/// <param name="direction">The direction to face.</param>
		/// <returns></returns>
		public static Quaternion LookRotation(Vector3 direction)
		{
			return LookRotation(direction, Vector3.WorldUp);
		}

		public static Vector3 TransformVector(this Vector3 i, Func<float, float> method)
		{
			return new Vector3()
			{
				X = method(i.X),
				Y = method(i.Y),
				Z = method(i.Z),
			};
		}

		public static float Denormalize(this float h)
		{
			return h < 0f ? h + 360f : h;
		}

		public static Vector3 Denormalize(this Vector3 v)
		{
			return new Vector3(v.X.Denormalize(), v.Y.Denormalize(), v.Z.Denormalize());
		}

		public static float CalculateRelativeValue(float input, float inputMin, float inputMax, float outputMin, float outputMax)
		{
			//http://stackoverflow.com/questions/22083199/method-for-calculating-a-value-relative-to-min-max-values
			//Making sure bounderies arent broken...
			if (input > inputMax)
			{
				input = inputMax;
			}
			if (input < inputMin)
			{
				input = inputMin;
			}
			//Return value in relation to min og max

			double position = (double)(input - inputMin) / (inputMax - inputMin);

			float relativeValue = (float)(position * (outputMax - outputMin)) + outputMin;

			return relativeValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="increment"></param>
		/// <param name="inputMax"></param>
		/// <param name="loopAround"></param>
		/// <param name="inputMin"></param>
		/// <param name="rounding">If less than 0 = no rounding. Else, round to this number of digits.</param>
		/// <returns></returns>
		public static float IncreaseNumber(this float input, float increment, float inputMax, bool loopAround = false, float inputMin = 0f, int rounding = -1)
		{
			float temp = (input + increment) < inputMax ? input + increment : (loopAround ? input + increment - inputMax + inputMin : inputMax);
			return rounding < 0 ? temp : (float)Math.Round(temp, rounding);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="decrement"></param>
		/// <param name="inputMin"></param>
		/// <param name="loopAround"></param>
		/// <param name="inputMax"></param>
		/// <param name="rounding">If less than 0 = no rounding. Else, round to this number of digits.</param>
		/// <returns></returns>
		public static float DecreaseNumber(this float input, float decrement, float inputMin, bool loopAround = false, float inputMax = 360f, int rounding = -1)
		{
			float temp = (input - decrement) > inputMin ? input - decrement : (loopAround ? input - decrement + inputMax - inputMin : inputMin);
			return rounding < 0 ? temp : (float)Math.Round(temp, rounding);
		}

		public static float Average(params int[] values)
		{
			int totalValue = 0;
			for (int i = 0; i < values.Length; i++)
			{
				totalValue += values[i];
			}

			return totalValue / values.Length;
		}

		public static float Average(params float[] values)
		{
			float totalValue = 0;
			for (int i = 0; i < values.Length; i++)
			{
				totalValue += values[i];
			}

			return totalValue / values.Length;
		}

		public static float ShowRounded(this float input, int digits)
		{
			return (float)Math.Round(input, digits);
		}

		/// <summary>
		/// Taken from https://stackoverflow.com/a/839904
		/// </summary>
		/// <param name="radius"></param>
		/// <param name="angleInDegrees"></param>
		/// <param name="origin"></param>
		/// <returns></returns>
		public static Vector2 PointOnCircle(float radius, float angleInDegrees, Vector2 origin)
		{
			// Convert from degrees to radians via multiplication by PI/180   
			double radians = angleInDegrees * Math.PI / 180F;
			float x = (float)(radius * Math.Cos(radians)) + origin.X;
			float y = (float)(radius * Math.Sin(radians)) + origin.Y;

			return new Vector2(x, y);
		}

		/// <summary>
		/// Performs a coordinate transformation using the given <see cref="Matrix"/>.
		/// </summary>
		/// <param name="coordinate">The coordinate vector to transform.</param>
		/// <param name="transform">The transformation <see cref="Matrix"/>.</param>
		/// <param name="result">When the method completes, contains the transformed coordinates.</param>
		/// <remarks>
		/// A coordinate transform performs the transformation with the assumption that the w component
		/// is one. The four dimensional vector obtained from the transformation operation has each
		/// component in the vector divided by the w component. This forces the w component to be one and
		/// therefore makes the vector homogeneous. The homogeneous vector is often preferred when working
		/// with coordinates as the w component can safely be ignored.
		/// </remarks>
		public static void TransformCoordinate(ref Vector3 coordinate, ref Matrix transform, out Vector3 result)
		{
			Vector4 vector = new Vector4();
			vector.X = (coordinate.X * transform.M11) + (coordinate.Y * transform.M21) + (coordinate.Z * transform.M31) + transform.M41;
			vector.Y = (coordinate.X * transform.M12) + (coordinate.Y * transform.M22) + (coordinate.Z * transform.M32) + transform.M42;
			vector.Z = (coordinate.X * transform.M13) + (coordinate.Y * transform.M23) + (coordinate.Z * transform.M33) + transform.M43;
			vector.W = 1f / ((coordinate.X * transform.M14) + (coordinate.Y * transform.M24) + (coordinate.Z * transform.M34) + transform.M44);

			result = new Vector3(vector.X * vector.W, vector.Y * vector.W, vector.Z * vector.W);
		}

		/// <summary>
		/// Performs a coordinate transformation using the given <see cref="Matrix"/>.
		/// </summary>
		/// <param name="coordinate">The coordinate vector to transform.</param>
		/// <param name="transform">The transformation <see cref="Matrix"/>.</param>
		/// <returns>The transformed coordinates.</returns>
		/// <remarks>
		/// A coordinate transform performs the transformation with the assumption that the w component
		/// is one. The four dimensional vector obtained from the transformation operation has each
		/// component in the vector divided by the w component. This forces the w component to be one and
		/// therefore makes the vector homogeneous. The homogeneous vector is often preferred when working
		/// with coordinates as the w component can safely be ignored.
		/// </remarks>
		public static Vector3 TransformCoordinate(Vector3 coordinate, Matrix transform)
		{
			Vector3 result;
			TransformCoordinate(ref coordinate, ref transform, out result);
			return result;
		}
	}

	public class Vector4
	{
		public float X;
		public float Y;
		public float Z;
		public float W;

		public Vector4()
		{

		}
	}
}