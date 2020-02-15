using ProtoBuf;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Clio.Utilities
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	[ProtoContract]
	public struct Vector3
	{
		[ProtoMember(1)]
		public float X;

		[ProtoMember(2)]
		public float Y;

		[ProtoMember(3)]
		public float Z;

		public static readonly Vector3 Zero = new Vector3(0f, 0f, 0f);

		public float MagnitudeSqr => X * X + Y * Y + Z * Z;

		public float Magnitude => (float)Math.Sqrt(MagnitudeSqr);

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3(string commaseperated)
		{
			string[] array = commaseperated.Split(new char[1]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			X = float.Parse(array[0], CultureInfo.InvariantCulture);
			Y = float.Parse(array[1], CultureInfo.InvariantCulture);
			Z = float.Parse(array[2], CultureInfo.InvariantCulture);
		}

		public Vector3(float value)
		{
			this = new Vector3(value, value, value);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "<{0}, {1}, {2}>", X, Y, Z);
		}

		public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
		{
			Vector3 left = value1 * (1f - amount);
			Vector3 right = value2 * amount;
			return left + right;
		}

		public static Vector3 Max(Vector3 value1, Vector3 value2)
		{
			return new Vector3((value1.X > value2.X) ? value1.X : value2.X, (value1.Y > value2.Y) ? value1.Y : value2.Y, (value1.Z > value2.Z) ? value1.Z : value2.Z);
		}

		public static Vector3 Min(Vector3 value1, Vector3 value2)
		{
			return new Vector3((value1.X < value2.X) ? value1.X : value2.X, (value1.Y < value2.Y) ? value1.Y : value2.Y, (value1.Z < value2.Z) ? value1.Z : value2.Z);
		}

		public void Normalize()
		{
			float magnitude = Magnitude;
			X /= magnitude;
			Y /= magnitude;
			Z /= magnitude;
		}

		public Vector2 ToVector2()
		{
			return new Vector2(X, Z);
		}

		public unsafe static float Distance(ref Vector3 v1, ref Vector3 v2)
		{
			void* ptr = stackalloc byte[8];
			float num = v1.X - v2.X;
			*(float*)ptr = v1.Y - v2.Y;
			*(float*)((byte*)ptr + 4) = v1.Z - v2.Z;
			return (float)Math.Sqrt(num * num + *(float*)ptr * *(float*)ptr + *(float*)((byte*)ptr + 4) * *(float*)((byte*)ptr + 4));
		}

		public float Distance(Vector3 to)
		{
			return Distance(ref this, ref to);
		}

		public float Distance3D(Vector3 to)
		{
			return Distance(ref this, ref to);
		}

		public float DistanceSqr(Vector3 to)
		{
			return DistanceSqr(ref this, ref to);
		}

		public static float Distance(Vector3 v1, Vector3 v2)
		{
			return Distance(ref v1, ref v2);
		}

		public unsafe static float DistanceSqr(ref Vector3 v1, ref Vector3 v2)
		{
			void* ptr = stackalloc byte[8];
			float num = v1.X - v2.X;
			*(float*)ptr = v1.Y - v2.Y;
			*(float*)((byte*)ptr + 4) = v1.Z - v2.Z;
			return num * num + *(float*)ptr * *(float*)ptr + *(float*)((byte*)ptr + 4) * *(float*)((byte*)ptr + 4);
		}

		public static float DistanceSqr(Vector3 v1, Vector3 v2)
		{
			return DistanceSqr(ref v1, ref v2);
		}

		public static float Distance2D(ref Vector3 v1, ref Vector3 v2)
		{
			float num = v1.X - v2.X;
			float num2 = v1.Z - v2.Z;
			return (float)Math.Sqrt(num * num + num2 * num2);
		}

		public static float Distance2DSqr(ref Vector3 v1, ref Vector3 v2)
		{
			float num = v1.X - v2.X;
			float num2 = v1.Z - v2.Z;
			return num * num + num2 * num2;
		}

		public static float AngleBetween(Vector3 u, Vector3 v)
		{
			u.Normalize();
			v.Normalize();
			if (Dot(u, v) < 0f)
			{
				return (float)(Math.PI - 2.0 * Math.Asin((-u - v).Magnitude / 2f));
			}
			return (float)(2.0 * Math.Asin((u - v).Magnitude / 2f));
		}

		public float Distance2D(Vector3 to)
		{
			return Distance2D(ref this, ref to);
		}

		public static double DotProduct(Vector3 pointA, Vector3 pointB, Vector3 pointC)
		{
			Vector2 vector = new Vector2(pointB.X - pointA.X, pointB.Z - pointA.Z);
			Vector2 vector2 = new Vector2(pointC.X - pointB.X, pointC.Z - pointC.Z);
			return vector.X * vector2.X + vector.Y * vector2.Y;
		}

		public static double CrossProduct(Vector3 pointA, Vector3 pointB, Vector3 pointC)
		{
			Vector2 vector = new Vector2(pointB.X - pointA.X, pointB.Z - pointA.Z);
			Vector2 vector2 = new Vector2(pointC.X - pointA.X, pointC.Z - pointA.Z);
			return vector.X * vector2.Y - vector.Y * vector2.X;
		}

		public float Distance2DSqr(Vector3 to)
		{
			return Distance2DSqr(ref this, ref to);
		}

		public static float Dot(ref Vector3 v1, ref Vector3 v2)
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
		}

		public static float Dot(Vector3 v1, Vector3 v2)
		{
			return Dot(ref v1, ref v2);
		}

		public float LengthSquared()
		{
			return X * X + Y * Y + Z * Z;
		}

		public static void Cross(ref Vector3 v1, ref Vector3 v2, out Vector3 result)
		{
			result.X = v1.Y * v2.Z - v1.Z * v2.Y;
			result.Y = v1.Z * v2.X - v1.X * v2.Z;
			result.Z = v1.X * v2.Y - v1.Y * v2.X;
		}

		public static Vector3 Cross(Vector3 v1, Vector3 v2)
		{
			Cross(ref v1, ref v2, out Vector3 result);
			return result;
		}

		public static Vector3 NormalizedDirection(Vector3 start, Vector3 end)
		{
			Vector3 result = end - start;
			result.Normalize();
			return result;
		}

		public static void CatmullRom(ref Vector3 beforeStart, ref Vector3 start, ref Vector3 end, ref Vector3 afterEnd, float amount, out Vector3 result)
		{
			Cardinal(ref beforeStart, ref start, ref end, ref afterEnd, 0.5f, 0.5f, amount, out result);
		}

		public static Vector3 CatmullRom(Vector3 beforeStart, Vector3 start, Vector3 end, Vector3 afterEnd, float amount)
		{
			Cardinal(ref beforeStart, ref start, ref end, ref afterEnd, 0.5f, 0.5f, amount, out Vector3 result);
			return result;
		}

		public static void Cardinal(ref Vector3 beforeStart, ref Vector3 start, ref Vector3 end, ref Vector3 afterEnd, float aStart, float aEnd, float amount, out Vector3 result)
		{
			Vector3 tangent = aStart * (end - beforeStart);
			Vector3 tangent2 = aEnd * (afterEnd - start);
			Hermite(ref start, ref tangent, ref end, ref tangent2, amount, out result);
		}

		public static Vector3 Cardinal(Vector3 beforeStart, Vector3 start, Vector3 end, Vector3 afterEnd, float aStart, float aEnd, float amount)
		{
			Cardinal(ref beforeStart, ref start, ref end, ref afterEnd, aStart, aEnd, amount, out Vector3 result);
			return result;
		}

		public unsafe static void Hermite(ref Vector3 value1, ref Vector3 tangent1, ref Vector3 value2, ref Vector3 tangent2, float amount, out Vector3 result)
		{
			void* ptr = stackalloc byte[24];
			*(float*)ptr = amount * amount;
			*(float*)((byte*)ptr + 4) = *(float*)ptr * amount;
			*(float*)((byte*)ptr + 8) = 2f * *(float*)((byte*)ptr + 4) - 3f * *(float*)ptr + 1f;
			*(float*)((byte*)ptr + 12) = -2f * *(float*)((byte*)ptr + 4) + 3f * *(float*)ptr;
			*(float*)((byte*)ptr + 16) = *(float*)((byte*)ptr + 4) - 2f * *(float*)ptr + amount;
			*(float*)((byte*)ptr + 20) = *(float*)((byte*)ptr + 4) - *(float*)ptr;
			result = value1 * *(float*)((byte*)ptr + 8) + value2 * *(float*)((byte*)ptr + 12) + tangent1 * *(float*)((byte*)ptr + 16) + tangent2 * *(float*)((byte*)ptr + 20);
		}

		public unsafe static Vector3 CubiceHermiteCurve(Vector3 start, Vector3 end, Vector3 u, Vector3 v, float t)
		{
			void* ptr = stackalloc byte[12];
			if (t > 1f || t < 0f)
			{
				throw new ArgumentOutOfRangeException("t", "t needs to be between 0.0f and 1.0f.");
			}
			*(float*)ptr = 1f - t;
			*(float*)((byte*)ptr + 4) = t * t;
			*(float*)((byte*)ptr + 8) = *(float*)ptr * *(float*)ptr;
			return *(float*)((byte*)ptr + 8) * (1f + 2f * t) * start + *(float*)((byte*)ptr + 4) * (1f + 2f * *(float*)ptr) * end + *(float*)((byte*)ptr + 8) * t * u - *(float*)((byte*)ptr + 4) * *(float*)ptr * v;
		}

		public static Vector3 Blend(Vector3 firstPoint, Vector3 secondPoint, float t)
		{
			if (t > 1f || t < 0f)
			{
				throw new ArgumentOutOfRangeException("t", "t needs to be between 0.0f and 1.0f.");
			}
			float right = 1f - t;
			return firstPoint * right + secondPoint * t;
		}

		public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, float amount)
		{
			Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out Vector3 result);
			return result;
		}

		public Vector3 Add(float x, float y, float z)
		{
			return this + new Vector3(x, y, z);
		}

		public Vector3 RayCast(float headingRadians, float distance)
		{
			Vector3 left = new Vector3((float)Math.Cos(0f - headingRadians), (float)Math.Sin(headingRadians), 0f);
			return this + left * distance;
		}

		public bool Equals(Vector3 other)
		{
			if (X == other.X && Y == other.Y)
			{
				return Z == other.Z;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != typeof(Vector3))
			{
				return false;
			}
			return Equals((Vector3)obj);
		}

		public override int GetHashCode()
		{
			return (((X.GetHashCode() * 397) ^ Y.GetHashCode()) * 397) ^ Z.GetHashCode();
		}

		public static Vector3 operator +(Vector3 left, Vector3 right)
		{
			return new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
		}

		public static Vector3 operator -(Vector3 left, Vector3 right)
		{
			return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
		}

		public static Vector3 operator *(Vector3 left, Vector3 right)
		{
			return new Vector3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
		}

		public static Vector3 operator *(Vector3 left, float right)
		{
			return left * new Vector3(right);
		}

		public static Vector3 operator *(float left, Vector3 right)
		{
			return new Vector3(left) * right;
		}

		public static Vector3 operator /(Vector3 left, Vector3 right)
		{
			return new Vector3(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
		}

		public static Vector3 operator /(Vector3 value1, float value2)
		{
			float num = 1f / value2;
			return new Vector3(value1.X * num, value1.Y * num, value1.Z * num);
		}

		public static Vector3 operator -(Vector3 value)
		{
			return Zero - value;
		}

		public static bool operator ==(Vector3 left, Vector3 right)
		{
			if (left.X == right.X && left.Y == right.Y)
			{
				return left.Z == right.Z;
			}
			return false;
		}

		public static bool operator !=(Vector3 left, Vector3 right)
		{
			if (left.X == right.X && left.Y == right.Y)
			{
				return left.Z != right.Z;
			}
			return true;
		}
	}
}
