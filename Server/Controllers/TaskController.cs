using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Utils;

namespace Server.Controllers;
public class TaskController : Controller
{
    private readonly IConfiguration _configuration;
    public TaskController(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public string GetConnectionString ()
    {
        return this._configuration.GetConnectionString("ToDoConnection")
        ?? throw new StatusException(500, "Connection string is null");
    }

    [HttpGet]
    [Route("tasks/getalltasks")]
    public IActionResult GetAllTasks()
    {
        try
        {            
            return Json(DBOperator.GetAllTasks(this.GetConnectionString()));
        }
        catch (StatusException exception)
        {               
            return StatusCode(exception.StatusCode, exception.Message);
        }
        catch(Exception exception)
        {            
            Console.WriteLine(exception.Message);
            return StatusCode(500, exception.Message);
        }
    }

    [HttpDelete]
    [Route("tasks/deletetask/{id}")]
    public IActionResult DeleteTask(int id)
    {
        try
        {
            if (id < 0 || DBOperator.DeleteTask(this.GetConnectionString(), id) == 0)
                throw new StatusException(400, "Task Id is invalid");
            return Ok();
        }
        catch (StatusException exception)
        {
            return StatusCode(exception.StatusCode, exception.Message);
        }
        catch (Exception exception)
        {
            return StatusCode(500, exception.Message);
        }
    }

    [HttpPost]
    [Route("tasks/addtask")]
    public IActionResult AddTask([FromBody] Tasks taskData)
    {
        try
        {            
            _ = taskData.IsValidOrException();
            
            int id = DBOperator.AddTask(this.GetConnectionString(), taskData) 
            ?? throw new Exception("Couldn't Save Task");

            return Ok(id);
        }
        catch(StatusException exception)
        {
            return StatusCode(exception.StatusCode, exception.Message);
        }
        catch(Exception exception)
        {
            return StatusCode(500, exception.Message);
        }
    }

    [HttpPut]
    [Route("tasks/updatetask")]
    public IActionResult UpdateTask([FromBody] Tasks taskData)
    {
        try
        {
            _ = taskData.IsValidOrException();
            object? result = DBOperator.UpdateTask(this.GetConnectionString(), taskData);
            if (result == null || (int) result == 0)
                throw new StatusException(400, "Couldn't Update Data");
            return Ok(true);
        }
        catch(StatusException exception)
        {
            return StatusCode(exception.StatusCode, exception.Message);
        }
        catch(Exception exception)
        {
            return StatusCode(500, exception.Message);
        }
    }

    [HttpPut]
    [Route("tasks/tooglecomplete/{id}")]
    public IActionResult ToggleComplete(int id)
    {
        try
        {
            if(DBOperator.ToogleTaskComplete(this.GetConnectionString(), id) == 0)
                throw new StatusException(400, "Couldn't find Task");
            return Ok();
        }
        catch(StatusException exception)
        {
            return StatusCode(exception.StatusCode, exception.Message);
        }
        catch(Exception exception)
        {
            Console.WriteLine(exception.Message);
            return StatusCode(500, exception.Message);
        }
    }
}