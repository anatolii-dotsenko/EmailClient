using EmailClient.Core.Models;

namespace EmailClient.Core.Interfaces;

/// <summary>
/// Defines the contract for email storage operations.
/// </summary>
public interface IEmailStorage
{
    /// <summary>
    /// Saves an email message to storage.
    /// </summary>
    Task SaveAsync(EmailMessage email);
    
    /// <summary>
    /// Loads all saved email messages.
    /// </summary>
    Task<List<EmailMessage>> LoadAllAsync();
    
    /// <summary>
    /// Deletes an email message from storage.
    /// </summary>
    Task DeleteAsync(string emailId);
}
