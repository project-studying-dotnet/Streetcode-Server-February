namespace Streetcode.DAL.Entities.AdditionalContent.Email;

public class EmailConfiguration
{
    required public string From { get; set; }
    required public string SmtpServer { get; set; }
    public int Port { get; set; }
    required public string UserName { get; set; }
    required public string Password { get; set; }
}
