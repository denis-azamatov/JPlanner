using System;
using System.Threading;
using System.Threading.Tasks;
using DA.JPlanner.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace DA.JPlanner.Schedules
{
    /// <summary>
    /// План запускающий задачи интервально
    /// </summary>
    /// <typeparam name="T">Тип задачи</typeparam>
    public class IntervalSchedule<T> : Schedule<T> where T : IIntervalJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly AutoResetEvent _autoResetEvent;
        private DateTime _nextInvocationTime;
        private TimeSpan _interval;

        public IntervalSchedule(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _autoResetEvent = new AutoResetEvent(false);
        }

        /// <inheritdoc />
        public override void Init()
        {
            using var scope = _scopeFactory.CreateScope();
            var job = GetJob(scope);
            _nextInvocationTime = DateTime.Now;
            _interval = job.Interval;
        }

        /// <inheritdoc />
        protected override async Task Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var job = GetJob(scope);

                Wait();

                await job.Process();
            }
        }

        private void Wait()
        {
            var waitTime = _nextInvocationTime - DateTime.Now;
            if (waitTime > TimeSpan.FromSeconds(1))
                _autoResetEvent.WaitOne(waitTime);
            _nextInvocationTime = DateTime.Now.Add(_interval);
        }
    }
}