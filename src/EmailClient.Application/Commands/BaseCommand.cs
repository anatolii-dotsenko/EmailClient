using EmailClient.Application.Interfaces;

namespace EmailClient.Application.Commands;

/// <summary>
/// Base class for all commands with common functionality.
/// </summary>
public abstract class BaseCommand : ICommand
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    
    /// <summary>
    /// Validates the command parameters.
    /// </summary>
    protected virtual bool Validate()
    {
        return true;
    }
    
    /// <summary>
    /// Shows command usage instructions.
    /// </summary>
    protected virtual void ShowUsage()
    {
        Console.WriteLine($"Usage: {Name}");
        Console.WriteLine($"Description: {Description}");
    }
    
    public abstract Task ExecuteAsync();
    
    /// <summary>
    /// Logs an error message.
    /// </summary>
    protected void LogError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {message}");
        Console.ResetColor();
    }
    
    /// <summary>
    /// Logs a success message.
    /// </summary>
    protected void LogSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Success: {message}");
        Console.ResetColor();
    }
}
