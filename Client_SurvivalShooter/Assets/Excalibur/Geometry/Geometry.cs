using System;

namespace Excalibur.Geometry
{
    public struct Point : IComparable, IFormattable, IComparable<Point>, IEquatable<Point>
    {
        public float x, y, z;

        public Point(float x, float y, float z = 0f) { this.x = x; this.y = y; this.z = z; }

        public static bool operator <(Point left, Point right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Point left, Point right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Point left, Point right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Point left, Point right)
        {
            return left.CompareTo(right) >= 0;
        }

        public bool Equals(Point other)
        {
            return default;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(Point other)
        {
            throw new NotImplementedException();
        }
    }
}