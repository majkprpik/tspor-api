using Quartz;
using Quartz.Spi;
using WebApi.Models;

namespace WebApi.HostedServices
{
    public class SingletonJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SingletonJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            Common.Logs($"NewJob at " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), "NewJob" + DateTime.Now.ToString("hhmmss"));

            return _serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {

        }
    }
}