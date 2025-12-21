using EmailClient.Core.Models;

namespace EmailClient.Application.Services;

/// <summary>
/// Service for handling email attachments.
/// </summary>
public class AttachmentService
{
    /// <summary>
    /// Saves attachment bytes to a file.
    /// </summary>
    public async Task SaveAttachmentToFile(byte[] data, string fileName, string outputPath = "attachments")
    {
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        var fullPath = Path.Combine(outputPath, fileName);
        await File.WriteAllBytesAsync(fullPath, data);
    }

    /// <summary>
    /// Gets total size of all attachments in emails.
    /// </summary>
    public long GetTotalAttachmentSize(List<EmailMessage> emails)
    {
        return emails
            .SelectMany(e => e.Attachments)
            .Sum(a => a.Size);
    }

    /// <summary>
    /// Extracts all attachment file names from emails.
    /// </summary>
    public List<string> ExtractAttachmentNames(List<EmailMessage> emails)
    {
        return emails
            .SelectMany(e => e.Attachments)
            .Select(a => a.FileName)
            .ToList();
    }
}
