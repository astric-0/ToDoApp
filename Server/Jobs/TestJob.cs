using Quartz;

namespace Server.Jobs;

public class TestJob : IJob
{      
    public Task Execute(IJobExecutionContext context)
    {
        // /_config = context.
        Console.WriteLine("JOB RUNNING");

        object? connectionString = context.JobDetail.JobDataMap.GetString("ToDoConnection");
    
        if (connectionString == null)
        {
            Console.WriteLine("config is null");
            return Task.CompletedTask;
        }        

        Console.WriteLine(connectionString);

        Console.WriteLine();
        return Task.CompletedTask;
    }

    public IJobDetail GetJobDetail()    
    {
        return JobBuilder.Create<TestJob>()
                .WithIdentity("TEST JOB", "GROUP1")
                .Build();
    }

    public ITrigger GetTrigger()
    {
        return TriggerBuilder.Create()
                .WithIdentity("TRIGGER1", "GROUP1")
                .WithCronSchedule("0/5 * * * * ?")
                .Build();
    }
}       