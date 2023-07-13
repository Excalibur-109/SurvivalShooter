using UnityEngine;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace Excalibur
{
    /// <summary> 计时完成的委托 /// </summary>
    public delegate void TickCallback (int elapsed);

    public sealed partial class TimingSchedule : Singleton<TimingSchedule>, IExecutableBehaviour, IPersistant
    {
        public const int MILLI_SECONDS = 1000;

        private readonly ExecutableBehaviourAssistant _timerExecuteAssistant = new ExecutableBehaviourAssistant();
        private ObjectPool<Timer> _pool;

        private const int FRAME_COUNT_INTERVAL = 1; /// 帧数计算时间间隔(秒)

        private int
            _gameTime,          /// 游戏时间(单位：秒)
            _persistentTime,    /// 持续增长的时间，不受其他影响(单位：秒)
            _frameCounter,
            _circularPersistentTime,
            _circularGameTime;
        private float
            _timing;

        public int averageFrameRate { get; private set; }
        public bool Executable { get; set; }
        public bool UpdateGameTime { get; set; }

        protected override void OnConstructed()
        {
            _pool = new ObjectPool<Timer>(_OnGet, _OnRelease);
            averageFrameRate = (int)(1f / deltaTime);
            UpdateGameTime = false;
            Executable = true;
            GameManager.Instance.AttachExecutableUnit(this);
        }

        public void Execute()
        {
            _timing += deltaTime;
            _frameCounter += 1;
            while (_timing >= 1f)
            {
                _timing -= 1f;
                _persistentTime += 1;
                if (_persistentTime % FRAME_COUNT_INTERVAL == 0)
                {
                    averageFrameRate = _frameCounter / FRAME_COUNT_INTERVAL;
                    _frameCounter = 0;
                }
                if (_persistentTime < 0)
                {
                    _circularPersistentTime += 1;
                    _persistentTime = 0;
                }
                if (UpdateGameTime)
                {
                    _gameTime += 1;
                    if (_gameTime < 0)
                    {
                        _circularGameTime += 1;
                        _gameTime = 0;
                    }
                }
            }

            if (Executable)
            {
                _timerExecuteAssistant.Execute();
            }
        }

        public Timer GetTimer (bool async = false)
        {
            return _pool.Get();
        }

        public void ReleaseTimer (Timer timer)
        {
            _pool.Release(timer);
        }

        private void _OnGet (Timer timer)
        {
            _timerExecuteAssistant.Attach(timer);
        }

        private void _OnRelease (Timer timer)
        {
            timer.Reset();
            _timerExecuteAssistant.Detach(timer);
        }

        public void Save (IDataWriter writer)
        {
            writer.Write(_persistentTime);
            writer.Write(_gameTime);
            writer.Write(_circularPersistentTime);
            writer.Write(_circularGameTime);
        }

        public void Load (IDataReader reader)
        {
            _persistentTime = reader.ReadInt();
            _gameTime = reader.ReadInt();
            _circularPersistentTime = reader.ReadInt();
            _circularGameTime = reader.ReadInt();
        }
    }

    /// <summary> Timer 精确度为毫秒：milliseconds /// </summary>
    public partial class TimingSchedule
    {
        public class TimerToken
        {
            private Timer _timer;
            private bool isStarted;

            public TimerToken(Timer timer)
            {
                _timer = timer;
                isStarted = false;
            }

            public void Start() { if (!isStarted) { _timer.Start(); } isStarted = true; }
            public void Continue() => _timer.Continue();
            public void Stop() => _timer.Stop();
            public void Restart() => _timer.Restart();
            public void Release() { _timer.Release(); Dispose(); }
            internal void Dispose() => _timer = null;
        }

        public class Timer : IExecutableBehaviour
        {
            private float _timing;

            private int
                _timer, _elasped, _interval, _delay, _repeat, _time, _repeatRecord;

            private TickCallback _onTick;
            private Action _onComplete;
            private TimerToken _token;

            public bool Executable { get; set; }

            public Timer () { }

            public void Execute()
            {
                if (!Executable) return;

                _timing += deltaTimeMilliseconds;
                while (_timing >= _interval && Executable)
                {
                    _timing -= _interval;
                    _timer += _interval;
                    if (_timer > _delay)
                    {
                        _elasped += _interval;
                        if (_elasped > _time) { _elasped = _time; }
                        _onTick?.Invoke(_elasped);
                        if (_elasped == _time)
                        {
                            _onComplete?.Invoke();
                            _timing = 0f;
                            _timer = 0;
                            _elasped = 0;
                            if (_repeat > 0)
                            {
                                if (--_repeat == 0)
                                {
                                    Release();
                                    if (_token != null) { _token.Dispose(); }
                                }
                            }
                        }
                    }
                }
            }

            internal void Start()
            {
                if (_CheckValidate())
                {
                    Executable = true;
                }
            }

            public void Restart()
            {
                _timing = 0f;
                _timer = 0;
                _elasped = 0;
                _repeat = _repeatRecord;
                Continue();
            }

            public void Continue() => Executable = true;

            public void Stop() => Executable = false;

            public void Reset()
            {
                Stop();
                _timing = 0f;
                _timer = 0;
                _elasped = 0;
                _interval = 0;
                _delay = 0;
                _repeat = 1;
                _repeatRecord = 1;
                _token = null;
                _onTick = null;
                _onComplete = null;
            }

            public Timer SetTime (int time)
            {
                _time = time;
                return this;
            }

            public Timer SetInterval (int interval)
            {
                _interval = interval;
                return this;
            }

            public Timer SetDelay (int delay)
            {
                _delay = delay;
                return this;
            }

            public Timer SetRepeat (int repeat)
            {
                _repeat = repeat;
                _repeatRecord = repeat;
                return this;
            }

            public Timer OnTick (TickCallback onTick)
            {
                _onTick += onTick;
                return this;
            }

            public Timer OnCompelte (Action onComplete)
            {
                _onComplete += onComplete;
                return this;
            }

            public void Release ()
            {
                Stop();
                Instance.ReleaseTimer(this);
            }

            public void SetToken (TimerToken token)
            {
                _token = token;
            }

            private bool _CheckValidate()
            {
                if (_repeat == 0)
                {
                    Instance.ReleaseTimer(this);
                    return false;
                }
                if (_delay == 0)
                {
                    _onTick?.Invoke(0);
                }
                return true;
            }

            [Obsolete("未完成")]
            public async Task StartAsync()
            {
                Debug.Log($"Timer异步线程ID:{Thread.CurrentThread.ManagedThreadId}");
                if (!_CheckValidate()) { return; }

                await Task.Delay(_delay);

                if (_repeat > 0)
                {
                    int slice = _time / _interval;
                    while (_repeat > 0)
                    {
                        for (int i = 0; i < slice; ++i)
                        {
                            await Task.Delay(_interval);
                            _elasped += _interval;
                            _onTick(_elasped);
                        }
                        int remain = _time - _interval * slice;
                        await Task.Delay(remain);
                        _elasped += remain;
                        _onTick(_elasped);
                        --_repeat;
                    }
                    _onComplete?.Invoke();
                }
                else if (_repeat < 0)
                {

                }
            }
        }
    }

    /// <summary> Methods, repeat < 0 为无限 /// </summary>
    public partial class TimingSchedule
    {
        /// <summary> 没有Milliseconds的都是秒 /// </summary>
        public void ScheduleMilliseconds (int time, Action onComplete, TickCallback onTick = default, 
            int interval = 1, int delay = 0, int repeat = 1)
        {
            Timer timer = GetTimer();
            timer.SetTime(time).OnCompelte(onComplete).OnTick(onTick).SetInterval(interval).SetRepeat(repeat).SetDelay(delay).Start();
        }

        public void Schedule (int time, Action onComplete, TickCallback onTick = default, 
            int interval = 1, int delay = 0, int repeat = 1)
        {
            ScheduleMilliseconds(time * MILLI_SECONDS, onComplete, onTick, interval * MILLI_SECONDS, delay * MILLI_SECONDS, repeat);
        }

        public void Schedule (float time, Action onComplete, TickCallback onTick = default,
            float interval = 1f, float delay = 0f, int repeat = 1)
        {
            int t = (int)(time * MILLI_SECONDS);
            int i = (int)(interval * MILLI_SECONDS);
            int d = (int)(delay * MILLI_SECONDS);
            ScheduleMilliseconds(t, onComplete, onTick, i, d, repeat);
        }

        public void ScheduleOnce (int time, Action onComplete, TickCallback onTick = default,
            int interval = 1, int delay = 0)
        {
            Schedule(time, onComplete, onTick, interval, delay);
        }

        public void ScheduleOnce (float time, Action onComplete, TickCallback onTick = default,
            float interval = 1, float delay = 0)
        {
            Schedule(time, onComplete, onTick, interval, delay);
        }

        public void Tick (int time, TickCallback onTick, Action onComplete = default)
        {
            ScheduleMilliseconds(time * MILLI_SECONDS, onComplete, onTick);
        }

        public void Tick (float time, TickCallback onTick, Action onComplete = default)
        {
            ScheduleMilliseconds((int)(time * MILLI_SECONDS), onComplete, onTick);
        }

        [Obsolete("未完成", true)]
        public void ScheduleOnNextFrame() { }

        public TimerToken ScheduleTokenMilliseconds (int time, Action onComplete, TickCallback onTick = default,
            int interval = 1, int delay = 0, int repeat = 1)
        {
            Timer timer = GetTimer();
            timer.SetTime(time).OnCompelte(onComplete).OnTick(onTick).SetInterval(interval).SetRepeat(repeat).SetDelay(delay);
            TimerToken token = new TimerToken(timer);
            timer.SetToken(token);
            return token;
        }

        public TimerToken ScheduleToken (int time, Action onComplete, TickCallback onTick = default,
            int interval = 1, int delay = 0, int repeat = 1)
        {
            return ScheduleTokenMilliseconds(time * MILLI_SECONDS, onComplete, onTick, interval * MILLI_SECONDS, delay * MILLI_SECONDS, repeat);
        }

        public TimerToken ScheduleToken (float time, Action onComplete, TickCallback onTick = default,
            float interval = 1f, float delay = 0f, int repeat = 1)
        {
            int t = (int)(time * MILLI_SECONDS);
            int i = (int)(interval * MILLI_SECONDS);
            int d = (int)(delay * MILLI_SECONDS);
            return ScheduleTokenMilliseconds(t, onComplete, onTick, i, d, repeat);
        }

        public TimerToken ScheduleInfinite (int time, Action onComplete, int delay = 0)
        {
            return ScheduleToken(time, onComplete, null, 1, delay, -1);
        }

        public TimerToken ScheduleInfinite (float time, Action onComplete, float delay = 0f)
        {
            return ScheduleToken(time, onComplete, null, 1f, delay, -1);
        }

        [Obsolete("未完成", true)]
        public async void ScheduleMillisecondsAsync(int time, Action onComplete, TickCallback onTick = default,
            int interval = 1, int delay = 0, int repeat = 1)
        {
            Timer timer = GetTimer();
            await timer.SetTime(time).OnCompelte(onComplete).OnTick(onTick).SetInterval(interval).SetRepeat(repeat).SetDelay(delay).StartAsync();
            ReleaseTimer(timer);
        }

        [Obsolete("未完成", true)]
        public void ScheduleAsync(int time, Action onComplete, TickCallback onTick = default,
            int interval = 1, int delay = 0, int repeat = 1)
        {
            Debug.Log($"主线程ID:{Thread.CurrentThread.ManagedThreadId}");
            ScheduleMillisecondsAsync(time * MILLI_SECONDS, onComplete, onTick, interval * MILLI_SECONDS, delay * MILLI_SECONDS, repeat);
        }

        [Obsolete("未完成", true)]
        public void ScheduleAsync (float time, Action onComplete, TickCallback onTick = default,
            float interval = 1f, float delay = 0f, int repeat = 1)
        {
            int t = (int)(time * MILLI_SECONDS);
            int i = (int)(interval * MILLI_SECONDS);
            int d = (int)(delay * MILLI_SECONDS);
            ScheduleMillisecondsAsync(t, onComplete, onTick, i, d, repeat);
        }

        [Obsolete("未完成", true)]
        public void TickAsync (int time, TickCallback onTick, Action onComplete = default)
        {
            ScheduleMillisecondsAsync(time * MILLI_SECONDS, onComplete, onTick);
        }

        [Obsolete("未完成", true)]
        public void TickAsync (float time, TickCallback onTick, Action onComplete = default)
        {
            ScheduleMillisecondsAsync((int)(time * MILLI_SECONDS), onComplete, onTick);
        }
    }

    /// <summary> UnityEngine.Time /// </summary>
    public partial class TimingSchedule
    {
        public static float time => Time.time;
        public static double timeAsDouble => Time.timeAsDouble;
        public static float timeSinceLevelLoad => Time.timeSinceLevelLoad;
        public static double timeSinceLevelLoadAsDouble => Time.timeSinceLevelLoadAsDouble;
        public static float deltaTime => Time.deltaTime;
        public static float fixedTime => Time.fixedTime;
        public static double fixedTimeAsDouble => Time.fixedTimeAsDouble;
        public static double unscaledTime => Time.unscaledTime;
        public static double unscaledTimeAsDouble => Time.unscaledTimeAsDouble;
        public static double fixedUnscaledTime => Time.fixedUnscaledTime;
        public static double fixedUnscaledTimeAsDouble => Time.fixedUnscaledTimeAsDouble;
        public static double unscaledDeltaTime => Time.unscaledDeltaTime;
        public static double fixedUnscaledDeltaTime => Time.fixedUnscaledDeltaTime;
        public static float timeScale { get => Time.timeScale; set => Time.timeScale = value; }
        public static int frameCount => Time.frameCount;
        public static int renderedFrameCount => Time.renderedFrameCount;
        public static float realtimeSinceStartup => Time.realtimeSinceStartup;
        public static double realtimeSinceStartupAsDouble => Time.realtimeSinceStartupAsDouble;
        public static float captureDeltaTime { get => Time.captureDeltaTime; set => Time.captureDeltaTime = value; }
        public static int captureFramerate { get => Time.captureFramerate; set => Time.captureFramerate = value; }
        public static bool inFixedTimeStep => Time.inFixedTimeStep;
        public static float fixedDeltaTime
        {
            get => Time.fixedDeltaTime;
            set => Time.fixedDeltaTime = value;
        }
        public static float maximumDeltaTime
        {
            get => Time.maximumDeltaTime;
            set => Time.maximumDeltaTime = value;
        }
        public static float smoothDeltaTime => Time.smoothDeltaTime;
        public static float maximumParticleDeltaTime
        {
            get => Time.maximumParticleDeltaTime;
            set => Time.maximumParticleDeltaTime = value;
        }

        public static float deltaTimeMilliseconds => deltaTime * 1000f;
    }
}
