using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EmailClient.Infrastructure.Extensions;
using EmailClient.Application.Services;
using EmailClient.Core.Interfaces;
using EmailClient.Core.Models;

namespace EmailClient;

/// <summary>
/// Main entry point for the email client console application.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        
        try
        {
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(configure => configure.AddConsole());
                services.AddInfrastructureServices();
                services.AddScoped<EmailFilterService>();
                services.AddScoped<AttachmentService>();
                services.AddHostedService<EmailClientHostedService>();
            });
    }
}

/// <summary>
/// Hosted service that runs the email client console interface.
/// </summary>
public class EmailClientHostedService : IHostedService
{
    private readonly IEmailService _emailService;
    private readonly IEmailStorage _emailStorage;
    private readonly EmailFilterService _filterService;
    private readonly ILogger<EmailClientHostedService> _logger;
    private bool _isRunning = true;

    public EmailClientHostedService(
        IEmailService emailService,
        IEmailStorage emailStorage,
        EmailFilterService filterService,
        ILogger<EmailClientHostedService> logger)
    {
        _emailService = emailService;
        _emailStorage = emailStorage;
        _filterService = filterService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Email Client Console Application Starting");
        
        while (_isRunning && !cancellationToken.IsCancellationRequested)
        {
            ShowMenu();
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    await ConnectToEmail();
                    break;
                case "2":
                    await ListEmails();
                    break;
                case "3":
                    await FilterEmails();
                    break;
                case "4":
                    await SendEmail();
                    break;
                case "5":
                    await SaveEmailsToJson();
                    break;
                case "6":
                    await ShowSavedEmails();
                    break;
                case "0":
                    _isRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
        
        _logger.LogInformation("Email Client Console Application Stopping");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _emailService.Disconnect();
        return Task.CompletedTask;
    }

    private void ShowMenu()
    {
        Console.WriteLine("\n=== Email Client Console ===");
        Console.WriteLine("1. Connect to Gmail");
        Console.WriteLine("2. List recent emails");
        Console.WriteLine("3. Filter emails");
        Console.WriteLine("4. Send email");
        Console.WriteLine("5. Save emails to JSON");
        Console.WriteLine("6. Show saved emails");
        Console.WriteLine("0. Exit");
        Console.Write("Enter your choice: ");
    }

    private async Task ConnectToEmail()
    {
        Console.Write("Enter your email: ");
        var email = Console.ReadLine();
        
        Console.Write("Enter your password/app password: ");
        var password = ReadPassword();
        
        try
        {
            await _emailService.ConnectAsync(email!, password);
            Console.WriteLine("Successfully connected to email server!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection failed: {ex.Message}");
        }
    }

    private async Task ListEmails()
    {
        Console.Write("How many emails to fetch? ");
        if (!int.TryParse(Console.ReadLine(), out var count) || count <= 0)
            count = 10;
        
        try
        {
            var emails = await _emailService.GetEmailsAsync(count);
            
            Console.WriteLine($"\nRecent Emails ({emails.Count}):");
            foreach (var email in emails)
            {
                Console.WriteLine($"- [{email.Date:yyyy-MM-dd HH:mm}] From: {email.From}");
                Console.WriteLine($"  Subject: {email.Subject}");
                Console.WriteLine($"  Has attachments: {email.HasAttachments}");
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to list emails: {ex.Message}");
        }
    }

    private async Task FilterEmails()
    {
        Console.WriteLine("\nFilter options:");
        Console.WriteLine("1. By subject keyword");
        Console.WriteLine("2. By sender");
        Console.WriteLine("3. With attachments");
        Console.Write("Enter filter choice: ");
        
        var filterChoice = Console.ReadLine();
        
        try
        {
            // First get emails to filter
            var emails = await _emailService.GetEmailsAsync(50);
            List<EmailMessage> filtered;
            
            switch (filterChoice)
            {
                case "1":
                    Console.Write("Enter keyword: ");
                    var keyword = Console.ReadLine();
                    filtered = _filterService.FilterBySubject(emails, keyword!);
                    break;
                case "2":
                    Console.Write("Enter sender email: ");
                    var sender = Console.ReadLine();
                    filtered = _filterService.FilterBySender(emails, sender!);
                    break;
                case "3":
                    filtered = _filterService.FilterByAttachments(emails, true);
                    break;
                default:
                    Console.WriteLine("Invalid filter choice");
                    return;
            }
            
            Console.WriteLine($"\nFiltered Emails ({filtered.Count}):");
            foreach (var email in filtered)
            {
                Console.WriteLine($"- From: {email.From}, Subject: {email.Subject}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Filtering failed: {ex.Message}");
        }
    }

    private async Task SendEmail()
    {
        Console.Write("Recipient email: ");
        var to = Console.ReadLine();
        
        Console.Write("Subject: ");
        var subject = Console.ReadLine();
        
        Console.WriteLine("Body (press Enter on empty line to finish):");
        var bodyLines = new List<string>();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;
            bodyLines.Add(line);
        }
        
        var body = string.Join(Environment.NewLine, bodyLines);
        
        try
        {
            await _emailService.SendEmailAsync(to!, subject!, body);
            Console.WriteLine("Email sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }
    }

    private async Task SaveEmailsToJson()
    {
        Console.Write("How many emails to save? ");
        if (!int.TryParse(Console.ReadLine(), out var count) || count <= 0)
            count = 10;
        
        try
        {
            var emails = await _emailService.GetEmailsAsync(count);
            
            foreach (var email in emails)
            {
                await _emailStorage.SaveAsync(email);
            }
            
            Console.WriteLine($"Saved {emails.Count} emails to JSON storage");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save emails: {ex.Message}");
        }
    }

    private async Task ShowSavedEmails()
    {
        try
        {
            var emails = await _emailStorage.LoadAllAsync();
            
            Console.WriteLine($"\nSaved Emails ({emails.Count}):");
            foreach (var email in emails)
            {
                Console.WriteLine($"- From: {email.From}");
                Console.WriteLine($"  Subject: {email.Subject}");
                Console.WriteLine($"  Date: {email.Date:yyyy-MM-dd HH:mm}");
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load saved emails: {ex.Message}");
        }
    }

    private static string ReadPassword()
    {
        var password = string.Empty;
        ConsoleKeyInfo key;
        
        do
        {
            key = Console.ReadKey(true);
            
            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Remove(password.Length - 1);
                Console.Write("\b \b");
            }
        }
        while (key.Key != ConsoleKey.Enter);
        
        Console.WriteLine();
        return password;
    }
}
