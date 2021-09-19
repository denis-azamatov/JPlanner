using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DA.JPlanner.Jobs;
using DA.JPlanner.Schedules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DA.JPlanner
{
    /// <summary>
    /// Сервис планировщик
    /// </summary>
    public class ScheduleService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(IServiceProvider services, ILogger<ScheduleService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Log(LogLevel.Information, "ScheduleService starting schedules");

            foreach (var schedule in GetSchedules<IIntervalJob>(typeof(IntervalSchedule<>)))
            {
                schedule?.GetType().GetMethod("Init")?.Invoke(schedule, null);
                schedule?.GetType().GetMethod("StartAsync")?.Invoke(schedule, new object?[] { stoppingToken });
            }

            foreach (var schedule in GetSchedules<ITimeJob>(typeof(TimeSchedule<>)))
            {
                schedule?.GetType().GetMethod("Init")?.Invoke(schedule, null);
                schedule?.GetType().GetMethod("StartAsync")?.Invoke(schedule, new object?[] { stoppingToken });
            }

            _logger.Log(LogLevel.Information, "ScheduleService started schedules");

            return Task.CompletedTask;
        }

        private IEnumerable<object?> GetSchedules<TJob>(Type schedule) where TJob : IJob
        {
            using var scope = _services.CreateScope();
            var jobs = scope.ServiceProvider.GetServices<TJob>().Select(x => x.GetType());
            var scopeFabric = scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
            return jobs
                .Select(x => schedule.MakeGenericType(x))
                .Select(x => Activator.CreateInstance(x, scopeFabric));
        }
    }
}