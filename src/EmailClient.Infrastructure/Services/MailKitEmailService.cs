using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using EmailClient.Core.Interfaces;
using EmailClient.Core.Models;
using Microsoft.Extensions.Logging;

namespace EmailClient.Infrastructure.Services;

/// <summary>
/// Implementation of IEmailService using MailKit library.
/// </summary>
public class MailKitEmailService : IEmailService, IDisposable
{
    private readonly ILogger<MailKitEmailService> _logger;
    private ImapClient? _imapClient;
    private SmtpClient? _smtpClient;
    private bool _disposed;

    public MailKitEmailService(ILogger<MailKitEmailService> logger)
    {
        _logger = logger;
    }

    public async Task ConnectAsync(string email, string password)
    {
        try
        {
            _logger.LogInformation("Connecting to email server for {Email}", email);
            
            // Connect to IMAP server
            _imapClient = new ImapClient();
            await _imapClient.ConnectAsync("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
            await _imapClient.AuthenticateAsync(email, password);
            
            // Connect to SMTP server
            _smtpClient = new SmtpClient();
            await _smtpClient.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await _smtpClient.AuthenticateAsync(email, password);
            
            _logger.LogInformation("Successfully connected to email server");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to email server");
            throw new EmailConnectionException("Failed to connect to email server", ex);
        }
    }

    public async Task<List<EmailMessage>> GetEmailsAsync(int count)
    {
        if (_imapClient == null || !_imapClient.IsConnected)
            throw new InvalidOperationException("Not connected to email server");

        try
        {
            var inbox = _imapClient.Inbox;
            await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

            var emails = new List<EmailMessage>();
            var uids = await inbox.SearchAsync(SearchQuery.All);
            var recentUids = uids.TakeLast(count).Reverse(); // Get most recent emails

            foreach (var uid in recentUids)
            {
                var message = await inbox.GetMessageAsync(uid);
                
                var email = new EmailMessage
                {
                    Id = uid.ToString(),
                    Subject = message.Subject,
                    Body = message.TextBody ?? message.HtmlBody ?? string.Empty,
                    From = message.From.ToString(),
                    To = message.To.ToString(),
                    Date = message.Date.UtcDateTime,
                    HasAttachments = message.Attachments.Any()
                };

                foreach (var attachment in message.Attachments)
                {
                    if (attachment is MimePart mimePart)
                    {
                        email.Attachments.Add(new EmailAttachment
                        {
                            FileName = mimePart.FileName ?? "attachment",
                            Size = mimePart.ContentDisposition?.Size ?? 0,
                            ContentType = mimePart.ContentType.MimeType
                        });
                    }
                }

                emails.Add(email);
            }

            _logger.LogInformation("Retrieved {Count} emails", emails.Count);
            return emails;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve emails");
            throw;
        }
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        if (_smtpClient == null || !_smtpClient.IsConnected)
            throw new InvalidOperationException("Not connected to SMTP server");

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("", ""));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            await _smtpClient.SendAsync(message);
            _logger.LogInformation("Email sent to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task<byte[]> DownloadAttachmentAsync(string emailId, string attachmentName)
    {
        if (_imapClient == null || !_imapClient.IsConnected)
            throw new InvalidOperationException("Not connected to email server");

        try
        {
            var inbox = _imapClient.Inbox;
            await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

            if (!int.TryParse(emailId, out var uid))
                throw new ArgumentException("Invalid email ID");

            var message = await inbox.GetMessageAsync(uid);
            
            foreach (var attachment in message.Attachments)
            {
                if (attachment is MimePart mimePart && 
                    (mimePart.FileName == attachmentName || attachmentName == "attachment"))
                {
                    using var memoryStream = new MemoryStream();
                    await mimePart.Content.DecodeToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }

            throw new FileNotFoundException($"Attachment '{attachmentName}' not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download attachment");
            throw;
        }
    }

    public void Disconnect()
    {
        _smtpClient?.Disconnect(true);
        _imapClient?.Disconnect(true);
        _logger.LogInformation("Disconnected from email servers");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _smtpClient?.Dispose();
            _imapClient?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Custom exception for email connection errors.
/// </summary>
public class EmailConnectionException : Exception
{
    public EmailConnectionException(string message, Exception innerException) 
        : base(message, innerException) { }
}
