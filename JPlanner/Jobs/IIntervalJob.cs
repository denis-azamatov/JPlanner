using System;

namespace DA.JPlanner.Jobs
{
    /// <summary>
    /// Интерфейс задач с интервальным запуском
    /// </summary>
    public interface IIntervalJob : IJob
    {
        /// <summary>
        /// Интервал через который вызывается задача
        /// </summary>
        public TimeSpan Interval { get; set; }
    }
}