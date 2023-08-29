using Quartz;
using Server.Services;
using Server.Utils;

namespace Server.Jobs;
public class ReminderJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        try
        {            
            Console.WriteLine("REMINDER JOB RUNNING");
            string? htmlBody = new ReminderData(context.JobDetail.JobDataMap.GetString("ToDoConnection"))
            .BuildReminderData().ToHtmlBody();

            if (string.IsNullOrEmpty(htmlBody))
                return Task.CompletedTask;

            if(EmailService.SendEmail("ecomsite094@gmail.com", "zmtfikjgcetwooam", "nittin1f4@gmail.com", htmlBody))
                Console.WriteLine("REMINDER EMAIL SENT");

            return Task.CompletedTask;
        }
        catch(Exception exception)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }
    }

    public IJobDetail GetJobDetail()
    {
        return JobBuilder.Create<ReminderJob>()
                .WithIdentity("ReminderJob", "GROUP1")
                .Build();
    }

    public ITrigger GetTrigger()
    {
        return TriggerBuilder.Create()
                .WithIdentity("ReminderJobTrigger", "GROUP1")
                .WithCronSchedule("0 0/1 * * * ?")
                .Build();
    }
}