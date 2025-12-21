namespace EmailClient.Application.Interfaces;

/// <summary>
/// Command pattern interface for console commands.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    Task ExecuteAsync();
    
    /// <summary>
    /// Gets the command name.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the command description.
    /// </summary>
    string Description { get; }
}
