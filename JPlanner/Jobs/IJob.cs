using System.Threading.Tasks;

namespace DA.JPlanner.Jobs
{
    /// <summary>
    /// Базовый интерфейс для задач планировщика
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Метод выполняемый по плану
        /// </summary>
        public Task Process();
    }
}