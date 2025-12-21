This project is a console-based email client implemented in Python, developed as part of an educational programming practice.

The application allows users to:

send emails via SMTP

fetch emails via POP3

parse email headers and body

detect attachments

store received emails in JSON format

The project uses Mailtrap Sandbox as a safe testing environment, avoiding real email accounts.

Project Goals

practice working with email protocols (SMTP, POP3)

understand MIME email structure

apply modular architecture principles

implement error handling for network operations

build a cross-platform CLI application
Technologies Used

Python 3.9+

smtplib — sending emails (SMTP)

poplib — receiving emails (POP3)

email — parsing MIME messages

json — data storage

Mailtrap Sandbox — email testing environment

Configuration

Create and configure the file:
config/mailtrap_config.py
```code
SMTP_HOST = "sandbox.smtp.mailtrap.io"
SMTP_PORT = 587
SMTP_USER = "your_username"
SMTP_PASSWORD = "your_password"

POP3_HOST = "pop3.mailtrap.io"
POP3_PORT = 1100
POP3_USER = SMTP_USER
POP3_PASSWORD = SMTP_PASSWORD
```
Application Usage

After launching, the following menu appears:

1. send email
2. fetch emails
3. exit


1️⃣ Send Email

sends a test email via Mailtrap SMTP

message appears in Mailtrap Sandbox inbox

2️⃣ Fetch Emails

connects to Mailtrap POP3

parses emails

extracts sender, recipient, subject, body

detects attachments

saves results to data/emails.json

3️⃣ Exit

terminates the application