using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using WebApi.Models;

namespace WebApi.HostedServices
{
    public class TestJob : IJob
    {
        public TestJob()
        {

        }

        public Task Execute(IJobExecutionContext context)
        {
            Common.Logs($"TestJob at " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), "TestJob" + DateTime.Now.ToString("hhmmss"));
            return Task.CompletedTask;
        }
    }
}