using System;
using System.Reflection;

namespace Excalibur
{
    public static class Comparer
    {
        public static bool IsTheSameType (Type t1, Type t2)
        {
            return t1 == t2;
        }
    }
}