using Microsoft.Data.SqlClient;
using Quartz;

namespace Server.Jobs;

public class DBJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        try 
        {
            Console.WriteLine("DBJob RUNNING");

            string? connectionString = context.JobDetail.JobDataMap.GetString("ToDoConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("CONNECTION STRING IS EMPTY");

            using SqlConnection connection = new (connectionString);
            using SqlCommand command = connection.CreateCommand();            
            command.CommandText = 
            @"
                DELETE FROM Tasks 
                WHERE IsCompleted = 1 AND DATEDIFF(DAY, CompletedOn, GETDATE()) > 1
            ";

            connection.Open();
            Console.WriteLine("TASKS REMOVED : " + command.ExecuteNonQuery());

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
        return JobBuilder.Create<DBJob>()
                .WithIdentity("DBJOB", "GROUP2")
                .Build();
    }

    public ITrigger GetTrigger()
    {
        return TriggerBuilder.Create()
                .WithIdentity("DBJobTrigger", "GROUP2")
                .WithCronSchedule("0 0 22 * * ?")
                .Build();
    }
}