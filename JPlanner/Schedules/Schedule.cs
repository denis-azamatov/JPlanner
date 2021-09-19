using System.Threading;
using System.Threading.Tasks;
using DA.JPlanner.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace DA.JPlanner.Schedules
{
    /// <summary>
    /// План - определяет логику вызова задачи
    /// </summary>
    /// <typeparam name="T">Тип задачи</typeparam>
    public abstract class Schedule<T> : ISchedule where T : IJob
    {
        /// <inheritdoc />
        public virtual async Task StartAsync(CancellationToken cancellationToken) => await Task.Run(async () => await Run(cancellationToken), cancellationToken);

        /// <inheritdoc />
        public abstract void Init();

        /// <summary>
        /// Получение задачи которую выполняет план
        /// </summary>
        protected virtual T GetJob(IServiceScope scope) => scope.ServiceProvider.GetRequiredService<T>();

        protected abstract Task Run(CancellationToken cancellationToken);
    }
}