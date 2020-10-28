using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerMinesweeper.Core.StopWatch
{
    public class StopWatch
    {
        private Counter _hour, _minute, _second;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancelToken;
        private Task _task;

        public StopWatch()
        {
            _hour = new Counter("hour");
            _minute = new Counter("minute");
            _second = new Counter("second");
            _tokenSource = new CancellationTokenSource();
            _cancelToken = _tokenSource.Token;
            _task = null;
            TimeElapsed = 0;
            IsStarted = IsStopped = false;
        }

        public bool IsStarted { get; private set; }
        public bool IsStopped { get; private set; }
        public int TimeElapsed { get; private set; }

        public string GetTime()
        {
            string hour = _hour.Value < 10 ? $"0{_hour.Value}" : _hour.Value.ToString(),
                minute = _minute.Value < 10 ? $"0{_minute.Value}" : _minute.Value.ToString(),
                second = _second.Value < 10 ? $"0{_second.Value}" : _second.Value.ToString();

            return _hour.Value != 0 ? $"{hour}:{minute}:{second}" : $"{minute}:{second}";
        }

        public void Tick()
        {
            TimeElapsed++;
            _second.Increment();

            if(_second.Value == 60)
            {
                _second.Reset();
                _minute.Increment();

                if(_minute.Value == 60)
                {
                    _minute.Reset();
                    _hour.Increment();
                }
            }
        }

        /// <summary>
        /// Start the stopwatch on a separate task in the thread pool (must be null to start the operation)
        /// </summary>
        public void Start()
        {
            if(_task == null)
            {
                _task = Task.Run(async () =>
                {
                    while (!_cancelToken.IsCancellationRequested)
                    {
                        Tick();
                        await Task.Delay(950);
                    }
                }, _cancelToken);

                IsStarted = true;
                IsStopped = false;
            }
        }

        /// <summary>
        /// Stop the stopwatch from running on another separate task in the thread pool (if any)
        /// </summary>
        public void Stop()
        {
            if(_task != null)
            {
                _tokenSource.Cancel();
                _task = null;

                IsStarted = false;
                IsStopped = true;
            }
        }
    }
}
