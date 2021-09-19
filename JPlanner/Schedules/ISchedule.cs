using System.Threading;
using System.Threading.Tasks;

namespace DA.JPlanner.Schedules
{
    /// <summary>
    /// Базовый интерфейс плана
    /// </summary>
    internal interface ISchedule
    {
        /// <summary>
        /// Запуск плана
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken);
        
        /// <summary>
        /// Инициализация настроек плана
        /// </summary>
        public void Init();
    }
}