using System.Text.Json;
using EmailClient.Core.Interfaces;
using EmailClient.Core.Models;

namespace EmailClient.Infrastructure.Repositories;

public class JsonEmailStorage : IEmailStorage
{
    private readonly string _filePath;
    private readonly List<EmailMessage> _emails;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly object _lock = new();

    // constructor with parameter for tests
    public JsonEmailStorage(string filePath = "emails.json")
    {
        _filePath = filePath;
        _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        _emails = LoadFromFile();
    }

    public Task SaveAsync(EmailMessage email)
    {
        lock (_lock)
        {
            // remove the old version of the email if it exists to update it
            _emails.RemoveAll(e => e.Id == email.Id);
            _emails.Add(email);
            SaveToFile();
        }
        return Task.CompletedTask;
    }

    public Task<List<EmailMessage>> LoadAllAsync()
    {
        lock (_lock)
        {
            // return a copy of the list
            return Task.FromResult(new List<EmailMessage>(_emails));
        }
    }

    // implementation of the delete method required by the interface
    public Task DeleteAsync(string emailId)
    {
        lock (_lock)
        {
            _emails.RemoveAll(e => e.Id == emailId);
            SaveToFile();
        }
        return Task.CompletedTask;
    }

    private void SaveToFile()
    {
        try 
        {
            var json = JsonSerializer.Serialize(_emails, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving emails: {ex.Message}");
        }
    }

    private List<EmailMessage> LoadFromFile()
    {
        if (!File.Exists(_filePath))
            return new List<EmailMessage>();

        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<EmailMessage>>(json, _jsonOptions) ?? new List<EmailMessage>();
        }
        catch
        {
            return new List<EmailMessage>();
        }
    }
}