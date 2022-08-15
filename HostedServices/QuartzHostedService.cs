using Quartz;
using Quartz.Spi;
using WebApi.Models;
using WebApi.Models.Tasks;

namespace WebApi.HostedServices
{
    public class QuartzHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IEnumerable<MyJobs> _myJobs;
        public QuartzHostedService(ISchedulerFactory schedulerFactory,
                                IJobFactory jobFactory,
                                IEnumerable<MyJobs> myJobs)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _myJobs = myJobs;
        }

        public IScheduler Scheduler { get; set; }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Common.Logs($"StartAsync at " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), "StartAsync" + DateTime.Now.ToString("hhmmss"));

            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;
            foreach (var myJob in _myJobs)
            {
                var job = CreateJob(myJob);
                var trigger = CreateTrigger(myJob);
                await Scheduler.ScheduleJob(job, trigger);
            }
            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Common.Logs($"StopAsync at " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), "StopAsync" + DateTime.Now.ToString("hhmmss"));

            await Scheduler?.Shutdown(cancellationToken);
        }

        private static IJobDetail CreateJob(MyJobs job)
        {
            var type = job.Type;
            return JobBuilder.Create(type).WithIdentity(type.FullName).WithDescription(type.Name).Build();
        }

        private static ITrigger CreateTrigger(MyJobs job)
        {
            return TriggerBuilder.Create().WithIdentity($"{job.Type.FullName}.trigger").WithCronSchedule(job.Expression).WithDescription(job.Expression).Build();
        }

    }
}