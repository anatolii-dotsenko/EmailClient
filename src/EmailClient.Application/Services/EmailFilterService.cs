using EmailClient.Core.Models;

namespace EmailClient.Application.Services;

/// <summary>
/// Service for filtering email messages.
/// </summary>
public class EmailFilterService
{
    /// <summary>
    /// Filters emails by subject containing the specified keyword.
    /// </summary>
    public List<EmailMessage> FilterBySubject(List<EmailMessage> emails, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return emails;

        return emails
            .Where(e => e.Subject.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Filters emails by sender email address.
    /// </summary>
    public List<EmailMessage> FilterBySender(List<EmailMessage> emails, string sender)
    {
        if (string.IsNullOrWhiteSpace(sender))
            return emails;

        return emails
            .Where(e => e.From.Contains(sender, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Filters emails by date range.
    /// </summary>
    public List<EmailMessage> FilterByDate(List<EmailMessage> emails, DateTime fromDate, DateTime toDate)
    {
        return emails
            .Where(e => e.Date >= fromDate && e.Date <= toDate)
            .ToList();
    }

    /// <summary>
    /// Filters emails that have attachments.
    /// </summary>
    public List<EmailMessage> FilterByAttachments(List<EmailMessage> emails, bool hasAttachments = true)
    {
        return emails
            .Where(e => e.HasAttachments == hasAttachments)
            .ToList();
    }
}
