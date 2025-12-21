namespace EmailClient.Core.Models;

/// <summary>
/// Represents email account configuration.
/// </summary>
public class EmailAccount
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ImapServer { get; set; } = string.Empty;
    public string SmtpServer { get; set; } = string.Empty;
    public int ImapPort { get; set; } = 993;
    public int SmtpPort { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
}
