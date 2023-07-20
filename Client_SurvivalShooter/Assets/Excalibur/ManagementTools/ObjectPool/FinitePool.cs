using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excalibur
{
    public sealed class FinitePool<T> : IObjectPool<T> where T : new()
    {
        public const int COMMON_POOL_MAXIMUM = 200;

        private readonly ObjectPool<T> r_pool;
        private readonly int r_maximum;
        private readonly Action<T> r_actionOnOutOfBound;

        public int count => r_pool.countInactive;

        public FinitePool (int maximum, Action<T> actionOnOutOfBound, Action<T> actionOnGet, Action <T> actionOnRelease)
        {
            r_maximum = maximum > 0 ? maximum : COMMON_POOL_MAXIMUM;
            r_actionOnOutOfBound = actionOnOutOfBound;
            r_pool = new ObjectPool<T>(actionOnGet, actionOnRelease);
        }

        public T Get ()
        {
            return r_pool.Get();
        }

        public void Release (T instance)
        {
            if (r_pool.countInactive < r_maximum)
            {
                r_pool.Release(instance);
            }
            else
            {
                r_actionOnOutOfBound?.Invoke(instance);
            }
        }

        public void Clear()
        {
            r_pool.Clear();
        }
    }
}
