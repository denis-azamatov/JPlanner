using System.Linq;
using System.Reflection;
using DA.JPlanner.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace DA.JPlanner
{
    /// <summary>
    /// Расширение <see cref="IServiceCollection"/>
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Регистрирует все задачи(<see cref="IJob"/>) из сборки
        /// </summary>
        /// <param name="services">Провайдер сервисов</param>
        /// <param name="assembly">Сборка в которой содержатся задачи</param>
        public static void RegisterJobsFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            RegisterJobs<IIntervalJob>(services, assembly);
            RegisterJobs<ITimeJob>(services, assembly);
        }

        private static void RegisterJobs<T>(IServiceCollection services, Assembly assembly) where T : class, IJob
        {
            var jobs = assembly.GetTypes().Where(x => x.IsClass && x.GetInterfaces().Any(type => type == typeof(T)));
            foreach (var job in jobs)
            {
                services.AddScoped(job);
                services.AddScoped(provider => (T)provider.GetRequiredService(job));
            }
        }
    }
}