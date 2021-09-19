using System;

namespace DA.JPlanner.Jobs
{
    public interface ITimeJob : IJob
    {
        public TimeSpan TimeOfDay { get; set; }
    }
}