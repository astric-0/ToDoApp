using System.Net.Mail;
using System.Net;

namespace Server.Services;

//Sender
public static class EmailService
{    
    public static bool SendEmail (string sender, string appPassword, string receiver, string htmlBody)
    {
        try
        {
            MailMessage mailMessage = new() 
            {                
                From = new MailAddress(sender),
                Subject = "Todo App Dailies",
                IsBodyHtml = true,
                Body = htmlBody
            };

            mailMessage.To.Add(receiver);

            SmtpClient smtpClient = new()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(sender, appPassword),
                EnableSsl = true    
            };

            smtpClient.Send(mailMessage);
            return true;
        }
        catch(Exception exception)
        {
            Console.WriteLine(exception.Message);
            return false;
        }
    }
}