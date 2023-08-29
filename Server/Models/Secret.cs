namespace Server.Models;

public class Secret
{
    public string Sender { get; set; } = null!;
    public string AppPassword { get; set; } = null!;
    public string Receiver { get; set; } = null!;
}