namespace EmailClient.Core.Models;

/// <summary>
/// Represents an email message with basic properties.
/// </summary>
public class EmailMessage
{
    public string Id { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool HasAttachments { get; set; }
    public List<EmailAttachment> Attachments { get; set; } = new();
}

/// <summary>
/// Represents an email attachment.
/// </summary>
public class EmailAttachment
{
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ContentType { get; set; } = string.Empty;
}
