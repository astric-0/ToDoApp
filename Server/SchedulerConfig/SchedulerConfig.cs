using Quartz;
using Quartz.Impl;
using Server.Jobs;

namespace Server.SchedulerConfig;

public class SchedulerConfig
{
    public static async Task Start(IConfiguration configuration)
    {
        _ = configuration ?? throw new Exception("CONFIGURATION IS NULL");
        string? connectionString = configuration.GetConnectionString("ToDoConnection");

        ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
        IScheduler scheduler = await schedulerFactory.GetScheduler();

        await scheduler.Start();
    
        EmailJob emailJob = new ();
        IJobDetail emailJobDetail = emailJob.GetJobDetail();
        emailJobDetail.JobDataMap.Put("ToDoConnection", connectionString);
        await scheduler.ScheduleJob(emailJobDetail,emailJob.GetTrigger());

        DBJob dbJob = new();
        IJobDetail dbJobDetail = dbJob.GetJobDetail();
        dbJobDetail.JobDataMap.Put("ToDoConnection", connectionString);
        await scheduler.ScheduleJob(dbJobDetail, dbJob.GetTrigger());
    }
}