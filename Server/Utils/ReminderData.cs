using System.Data;
using Microsoft.Data.SqlClient;
using Server.Models;

namespace Server.Utils;
public class ReminderData
{
    public readonly string _connectionString;
    public List<Tasks> Value { get; set; } = new();
    public ReminderData(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("REMINDER JOB: connection string is empty");
        this._connectionString = connectionString;
    }

    public ReminderData BuildReminderData()
    {
        using SqlConnection connection = new (this._connectionString);
        using SqlCommand command = connection.CreateCommand();

        command.CommandText = "SELECT * FROM Tasks t JOIN Reminders r ON r.TaskId = t.Id WHERE r.ReminderTime = @ReminderTime;";
        command.Parameters.Add("@ReminderTime", SqlDbType.DateTime).Value = DateTime.Now;
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            this.Value.Add
            (
                new Tasks ()
                {
                    TaskName = (string) reader.GetValue(reader.GetOrdinal("TaskName")),
                    TaskDetails = (string) reader.GetValue(reader.GetString("TaskDetails"))                    
                }
            );
        }

        return this;
    }

    public string? ToHtmlBody()
    {        
        if (this.Value.Count > 0)
        {
            string htmlBody = "";
            foreach(Tasks task in this.Value)
            {
                htmlBody += 
                @$"
                <div>
                    <h1>{ task.TaskName }</h1>
                    <p>{ task.TaskDetails }</p>
                </div
                ";
            }

            return htmlBody;
        }
        
        return null;
    }
}