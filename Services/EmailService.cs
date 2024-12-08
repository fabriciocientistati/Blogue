using System.Net;
using System.Net.Mail;

namespace Blogue.Services;

public class EmailService
{
    public bool Send(
        string toName,
        string toEmail,
        string subject,
        string body,
        string fromName = "Primo Móveis",
        string fromEmail = "fabricio.cientistati@gmail.com")
    {
        var smtClient = new SmtpClient(Configuration.Smtp.Host, Configuration.Smtp.Port);
        smtClient.Credentials = new NetworkCredential(Configuration.Smtp.UserName, Configuration.Smtp.Password);
        smtClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtClient.EnableSsl = true;

        var mail = new MailMessage();

        mail.From = new MailAddress(fromEmail, fromName);
        mail.To.Add(new MailAddress(toEmail, toName));
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = true;

        try
        {
            smtClient.Send(mail);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}