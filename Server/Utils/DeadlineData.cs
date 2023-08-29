using Server.Models;
using Microsoft.Data.SqlClient;

namespace Server.Utils;
public class DeadlineData
{
    private readonly string _connectionString;
    public string Body { get; private set; } = "";
    public List<Tasks> TaskList { get; private set; } = new();
    
    public DeadlineData (string connectionString)
    {
        this._connectionString = connectionString;
    }

    private string GetDataString (SqlDataReader reader, string fieldName)
    {
        return (string) reader.GetValue(reader.GetOrdinal(fieldName));                
    }

    private T? GetDataOrDefault <T> (SqlDataReader reader, string fieldName)
    {
        object fieldObj = reader.GetValue(reader.GetOrdinal(fieldName));
        if (fieldObj == DBNull.Value || fieldObj == null)
            return default;
        return (T) fieldObj;
    }
    public DeadlineData GetDeadlineData()
    {
        using SqlConnection connection = new (_connectionString);
        using SqlCommand command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Tasks WHERE deadline = GETDATE() AND IsCompleted = 0";
        connection.Open();
        SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            this.TaskList.Add
            (
                new ()
                {
                    TaskName = GetDataString (reader, "TaskName"),
                    TaskDetails = GetDataOrDefault <string> (reader, "TaskDetails")
                }
            );
        }

        return this;
    }
    public string ToHtmlBody()
    {
        string body = "";
        if (TaskList.Count == 0) 
        {
            body = @"
            <h3>
                There are not tasks for today! Add deadlines
            </h3>
            ";
        }

        else {
            foreach(Tasks task in TaskList)
            {
                body += @$"
                    <div>
                        <h3>{ task.TaskName }</h3>
                        <p>{ task.TaskDetails }</p>
                    </div>
                ";
            }

            body += "<h3>MAKE SURE TO COMPLETE THESE TASKS TODAY</h3>";
        }

        return body;
    }
}