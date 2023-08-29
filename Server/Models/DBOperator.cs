using System.Data;
using Microsoft.Data.SqlClient;

namespace Server.Models;
public static class DBOperator 
{
    private static T? GetValueOrDefault <T> (SqlDataReader reader, string key)
    {
        var checkObj = reader.GetValue(reader.GetOrdinal(key));
        return (checkObj != DBNull.Value) ? (T) checkObj : default;
    }    

    private static T GetValue <T> (SqlDataReader reader, string key)
    {
        return (T) reader.GetValue(reader.GetOrdinal(key));
    }

    private static DateTime? GetDateOrNull (SqlDataReader reader, string key)
    {
        var checkObj = reader.GetValue(reader.GetOrdinal(key));
        return (checkObj != DBNull.Value) ? (DateTime) checkObj : null;
    }

    public static List<Tasks> GetAllTasks (string connectionString) 
    {
        using SqlConnection connection = new (connectionString);
        string commandString = "SELECT * FROM Tasks ORDER BY Importance";
        
        connection.Open();
        using SqlCommand command = new (commandString, connection);
        using SqlDataReader reader = command.ExecuteReader();

        List<Tasks> AllTasks = new();
        while(reader.Read())
        {            
            AllTasks.Add
            (
                new Tasks()
                {
                    Id = GetValue <int> (reader, "Id"),
                    TaskName = GetValue <string> (reader, "TaskName"),
                    TaskDetails = GetValueOrDefault <string> (reader, "TaskDetails"),
                    CreatedOn = GetValue <DateTime> (reader, "CreatedOn" ),
                    ModifiedOn = GetDateOrNull (reader, "ModifiedOn"),
                    CompletedOn = GetDateOrNull (reader, "CompletedOn"),
                    Deadline = GetDateOrNull (reader, "Deadline"),
                    Importance = GetValue <int> (reader, "Importance"),
                    Category = GetValue <string> (reader, "Category"),
                    IsCompleted = GetValue <bool> (reader, "IsCompleted")
                }
            );            
        }

        return AllTasks;
    }

    public static int DeleteTask (string connectionString, int id)
    {
        using SqlConnection connection = new (connectionString);
        string commandString = $"DELETE FROM Tasks WHERE Id = {id}";
        connection.Open();
        using SqlCommand command = new (commandString, connection);
        return command.ExecuteNonQuery();
    }   

    public static int? AddTask (string connectionString, Tasks taskData)
    {
        string commandString = @"
            INSERT INTO 
            TASKS (TaskName, TaskDetails, Importance, Category, Deadline)
            OUTPUT Inserted.Id 
            VALUES (@TaskName, @TaskDetails, @Importance, @Category, @Deadline)
        ";

        return DBOperator.Executor(connectionString, commandString, taskData, false);        
    }

    private static int? Executor(string connectionString, string commandString, Tasks taskData, Boolean isUpdate)
    {
        using SqlConnection connection = new (connectionString);
        connection.Open();
        using SqlCommand command = connection.CreateCommand();
        command.CommandText = commandString;
        command.Parameters.Add("@TaskName", SqlDbType.VarChar).Value = taskData.TaskName;
        command.Parameters.Add("@TaskDetails", SqlDbType.VarChar).Value = taskData.TaskDetails;
        command.Parameters.Add("@Importance", SqlDbType.Int).Value = taskData.Importance;
        command.Parameters.Add("@Category", SqlDbType.VarChar).Value = taskData.Category; 

        command.Parameters.Add("@Deadline", SqlDbType.DateTime).Value = !string.IsNullOrEmpty(taskData.DeadlineDateString) ? taskData.Deadline : DBNull.Value;

        if (!isUpdate) 
        {
            object returnObj = command.ExecuteScalar();         
            return returnObj != null ? (int) returnObj : null;
        }
        else        
            return command.ExecuteNonQuery();        
    }

    public static int? UpdateTask(string connectionString, Tasks taskData)
    {
        string commandString = 
        @$"
            UPDATE Tasks
            SET TaskName = @TaskName, TaskDetails = @TaskDetails, Importance = @Importance, Category = @Category, Deadline = @Deadline
            WHERE Id = {taskData.Id}
        ";

        return Executor(connectionString, commandString, taskData, true);
    }

    public static int ToogleTaskComplete(string connectionString, int id)
    {
        using SqlConnection connection = new (connectionString);
        connection.Open();
        using SqlCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE Tasks SET IsCompleted = ~IsCompleted WHERE Id=@Id";
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;       
        return command.ExecuteNonQuery();
    }
}