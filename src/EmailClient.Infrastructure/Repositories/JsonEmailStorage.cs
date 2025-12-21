using System.Text.Json;
using EmailClient.Core.Interfaces;
using EmailClient.Core.Models;

namespace EmailClient.Infrastructure.Repositories;

/// <summary>
/// JSON-based email storage implementation.
/// </summary>
public class JsonEmailStorage : IEmailStorage
{
    private readonly string _filePath;
    private readonly List<EmailMessage> _emails;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly object _lock = new();

    public JsonEmailStorage()
    {
        _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emails.json");
        _emails = new List<EmailMessage>();
        _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        
        LoadFromFile();
    }

    private void LoadFromFile()
    {
        if (!File.Exists(_filePath))
            return;

        try
        {
            lock (_lock)
            {
                var json = File.ReadAllText(_filePath);
                var loaded = JsonSerializer.Deserialize<List<EmailMessage>>(json, _jsonOptions);
                if (loaded != null)
                    _emails.AddRange(loaded);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load emails from JSON: {ex.Message}");
        }
    }

    private void SaveToFile()
    {
        try
        {
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(_emails, _jsonOptions);
                File.WriteAllText(_filePath, json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save emails to JSON: {ex.Message}");
        }
    }

    public Task SaveAsync(EmailMessage email)
    {
        _emails.RemoveAll(e => e.Id == email.Id);
        _emails.Add(email);
        SaveToFile();
        return Task.CompletedTask;
    }

    public Task<List<EmailMessage>> LoadAllAsync()
    {
        return Task.FromResult(new List<EmailMessage>(_emails));
    }

    public Task DeleteAsync(string emailId)
    {
        _emails.RemoveAll(e => e.Id == emailId);
        SaveToFile();
        return Task.CompletedTask;
    }
}
