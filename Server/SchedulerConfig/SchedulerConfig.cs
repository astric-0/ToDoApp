using Quartz;
using Quartz.Impl;
using Server.Jobs;
using Server.Models;

namespace Server.SchedulerConfig;

public class SchedulerConfig
{
    public static async Task Start(IConfiguration configuration)
    {
        _ = configuration ?? throw new Exception("CONFIGURATION IS NULL");
        string? connectionString = configuration.GetConnectionString("ToDoConnection");

        Secret emailInfo = new ();
        configuration.GetSection("EmailInfo").Bind(emailInfo);
        
        Console.WriteLine(emailInfo.Sender);

        ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
        IScheduler scheduler = await schedulerFactory.GetScheduler();

        await scheduler.Start();
    
        EmailJob emailJob = new ();
        IJobDetail emailJobDetail = emailJob.GetJobDetail();
        emailJobDetail.JobDataMap.Put("ToDoConnection", connectionString);
        emailJobDetail.JobDataMap.Put("Sender", emailInfo.Sender);
        emailJobDetail.JobDataMap.Put("AppPassword", emailInfo.AppPassword);
        emailJobDetail.JobDataMap.Put("Receiver", emailInfo.Receiver);

        await scheduler.ScheduleJob(emailJobDetail,emailJob.GetTrigger());

        DBJob dbJob = new();
        IJobDetail dbJobDetail = dbJob.GetJobDetail();
        dbJobDetail.JobDataMap.Put("ToDoConnection", connectionString);
        await scheduler.ScheduleJob(dbJobDetail, dbJob.GetTrigger());
    }
}