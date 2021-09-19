using System;
using System.Threading;
using System.Threading.Tasks;
using DA.JPlanner.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace DA.JPlanner.Schedules
{
    
    public class TimeSchedule<T> : Schedule<T> where T : ITimeJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly AutoResetEvent _autoResetEvent;
        private DateTime _nextInvocationTime;
        private TimeSpan _timeOfDay;

        public TimeSchedule(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _autoResetEvent = new AutoResetEvent(false);
        }

        public override void Init()
        {
            using var scope = _scopeFactory.CreateScope();
            var job = GetJob(scope);
            _timeOfDay = job.TimeOfDay;
            _nextInvocationTime = _timeOfDay < DateTime.Now.TimeOfDay
                ? DateTime.Today.AddDays(1).Add(_timeOfDay)
                : DateTime.Today.Add(_timeOfDay);
        }

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
            if (waitTime > TimeSpan.FromMinutes(1))
                _autoResetEvent.WaitOne(waitTime);
            _nextInvocationTime = DateTime.Today.AddDays(1).Add(_timeOfDay);
        }
    }
}