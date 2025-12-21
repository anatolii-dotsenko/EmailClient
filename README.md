# Email Client Console Application

A .NET 8 console email client built with MailKit, following SOLID principles and clean architecture.

## Features
- Connect to Gmail using IMAP/SMTP
- List and view recent emails
- Filter emails by subject, sender, or attachments
- Send emails
- Save emails to JSON format
- Load saved emails from JSON

## Prerequisites
- .NET 8 SDK
- Gmail account with app password enabled

## Setup
1. Enable 2-factor authentication in your Google account
2. Generate an app password: Google Account → Security → App passwords
3. Build and run the application

## Building and Running
```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the application
dotnet run
```

## Usage
1. Run the application
2. Choose option 1 to connect to Gmail
3. Enter your email and app password
4. Use the menu to:
5. List recent emails
6. Filter emails
7. Send emails
8. Save emails to JSON
9. View saved emails

## Important Notes
- Use app password, not your regular Gmail password
- First run will create emails.json for storage
- Attachments are saved to attachments/ directory

## Dependencies
- MailKit/MimeKit 4.8.0+ for email protocols
- Microsoft.Extensions for DI, Logging, and Configuration
- .NET 8 runtime

## Architecture
- SOLID Principles: Each class has a single responsibility
- Dependency Injection: Services registered via DI container
- Clean Architecture: Separation of concerns (Core, Application, Infrastructure, Console)
- Repository Pattern: For email storage abstraction

## License
- MIT
