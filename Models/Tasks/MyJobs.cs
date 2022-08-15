namespace WebApi.Models.Tasks
{
    public class MyJobs
    {
        public MyJobs(Type type, string expression)
        {
            Common.Logs($"MyJob at " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), "MyJob" + DateTime.Now.ToString("hhmmss"));

            Type = type;
            Expression = expression;
        }
        public Type Type { get; }
        public string Expression { get; set; }
    }
}