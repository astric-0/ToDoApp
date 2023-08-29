namespace Server.Utils;

public class StatusException : Exception
{
    public int StatusCode { get; set; }

    public StatusException(int statusCode, string message) : base (message)
    {
        this.StatusCode = statusCode;
    }
}