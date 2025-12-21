import smtplib
from email.message import EmailMessage
from config.mailtrap_config import SMTP_HOST, SMTP_PORT, SMTP_USER, SMTP_PASSWORD

class EmailSender:
    def send_email(self, to_email, subject, body):
        # create email message
        message = EmailMessage()
        message["From"] = SMTP_USER
        message["To"] = to_email
        message["Subject"] = subject
        message.set_content(body)

        # send email via smtp
        with smtplib.SMTP(SMTP_HOST, SMTP_PORT) as server:
            server.starttls()
            server.login(SMTP_USER, SMTP_PASSWORD)
            server.send_message(message)
