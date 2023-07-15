using System;
using UnityEditor;

namespace Excalibur
{
    public class Singleton<T> where T : Singleton<T>, new ()
    {
        private static readonly Lazy<T> _lazy = new Lazy<T> ( () => new T ());
        protected Singleton () { OnConstructed (); }
        public static T Instance => _lazy.Value;
        protected virtual void OnConstructed () { }
    }

    internal class SingletonExample<T> where T : SingletonExample<T>, new ()
    {
        private static readonly Lazy<T> _lazy = new Lazy<T> ( () => new T ());
        protected SingletonExample () { OnConstructed (); }
        public static T Instance => _lazy.Value;
        protected virtual void OnConstructed () { }
    }

    internal class SingletonExample1
    {
        private static readonly Lazy<SingletonExample1> _lazy = new Lazy<SingletonExample1> ( () => new SingletonExample1 ());
        //static SingletonExample () { }
        public static SingletonExample1 Instance => _lazy.Value;
    }

    internal class SingletonExample2
    {
        public static SingletonExample2 Instance => Nested.instance;
        class Nested
        {
            static Nested () { }
            internal static readonly SingletonExample2 instance = new SingletonExample2 ();
        }
    }

    internal class SingletonExample3<T> where T : SingletonExample3<T>, new ()
    {
        public static T Instance => Nested<T>.instance;
        protected SingletonExample3 () { }
        private class Nested<U> where U : new ()
        {
            static Nested () { }
            internal static readonly U instance = new U ();
        }
    }

    internal class SingletonExample4<T> where T : SingletonExample4<T>, new ()
    {
        private static T _instance;

        private static object _locker = new object ();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new T ();
                        }
                    }
                }
                return _instance;
            }
        }

        protected SingletonExample4 ()
        {
            OnConcrete ();
        }

        protected virtual void OnConcrete ()
        {

        }
    }
}
