using System;

namespace Excalibur
{
    public sealed class AutoReleasePool<T> : IObjectPool<T> where T : new()
    {
        private const float AUTO_RELEASE_DELAY = 3f;

        private readonly ObjectPool<T> r_pool;
        private readonly Timing.TimerToken r_timerToken;
        private readonly Action<T> r_actionOnAutoRelease;

        public int count => r_pool.countInactive;

        /// <summary> releaseInterval 秒 /// </summary>
        public AutoReleasePool (float releaseInterval, Action<T> actionOnAutoRelease, Action<T> actionOnGet, Action <T> actionOnRelease)
        {
            r_pool = new ObjectPool<T>(actionOnGet, actionOnRelease);
            r_actionOnAutoRelease = actionOnAutoRelease;
            r_timerToken = Timing.Instance.ScheduleInfinite(releaseInterval, AutoRelease, AUTO_RELEASE_DELAY);
        }

        public T Get ()
        {
            r_timerToken.Stop();
            T instance = r_pool.Get();
            if (r_pool.countInactive > 0)
            {
                r_timerToken.Restart();
            }
            return instance;
        }

        public void Release (T instance)
        {
            r_timerToken.Stop();
            r_pool.Release(instance);
            r_timerToken.Restart();
        }

        private void AutoRelease ()
        {
            if (r_pool.countInactive > 0)
            {
                T instance = r_pool.Get();
                r_actionOnAutoRelease(instance);
                if (r_pool.countInactive == 0)
                {
                    r_timerToken.Stop();
                }
            }
        }

        public void Clear()
        {
            r_pool.Clear();
        }
    }
}
