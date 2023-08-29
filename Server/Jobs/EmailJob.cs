using Quartz;
using Microsoft.Data.SqlClient;
using Server.Services;
using Server.Utils;

namespace Server.Jobs;
public class EmailJob : IJob
{
    public object? GetConfigData (string connectionString) 
    {
        using SqlConnection connection = new (connectionString);
        using SqlCommand command = connection.CreateCommand();
        connection.Open();
        command.CommandText = "SELECT * FROM ToDoSettings";
        using SqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            string email = (string) reader.GetValue(reader.GetOrdinal("@Email"));

            return new 
            {
                email
            };
        }

        return null;
    }

    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("EMAIL JOB IS RUNNING");
        //return Task.CompletedTask;
        try
        {
            string? connectionString = context.JobDetail.JobDataMap.GetString("ToDoConnection");
            if (string.IsNullOrEmpty(connectionString))            
                throw new Exception("Email Job: Connection String is null or empty");            

            string htmlBody =  new DeadlineData(connectionString)
                                .GetDeadlineData()
                                .ToHtmlBody();

            if(EmailService.SendEmail("ecomsite094@gmail.com", "zmtfikjgcetwooam", "nittin1f4@gmail.com", htmlBody))
                Console.WriteLine("EMAIL SENT");

            return Task.CompletedTask;
        }

        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }            
    }

    public IJobDetail GetJobDetail() 
    {
        return JobBuilder
                    .Create<EmailJob>()
                    .WithIdentity("job1", "Group1")
                    .Build();
    }

    public ITrigger GetTrigger()
    {
        return TriggerBuilder
                    .Create()
                    .WithIdentity("trigger1", "Group1")
                    .WithCronSchedule("0 0 8 * * ?")
                    // "0 0 8 * * ? *"
                    .Build();
    }
}