using EmailClient.Core.Models;

namespace EmailClient.Core.Interfaces;

/// <summary>
/// Defines the contract for email service operations.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Connects to the email server.
    /// </summary>
    Task ConnectAsync(string email, string password);
    
    /// <summary>
    /// Gets emails from the server.
    /// </summary>
    Task<List<EmailMessage>> GetEmailsAsync(int count);
    
    /// <summary>
    /// Sends an email.
    /// </summary>
    Task SendEmailAsync(string to, string subject, string body);
    
    /// <summary>
    /// Downloads an attachment from an email.
    /// </summary>
    Task<byte[]> DownloadAttachmentAsync(string emailId, string attachmentName);
    
    /// <summary>
    /// Disconnects from the email server.
    /// </summary>
    void Disconnect();
}
