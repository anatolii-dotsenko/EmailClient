import poplib
import socket
from email.parser import BytesParser
from email.policy import default
from config.mailtrap_config import POP3_HOST, POP3_PORT, POP3_USER, POP3_PASSWORD

class EmailReceiver:
    def fetch_emails(self):
        # set timeout to avoid infinite blocking
        socket.setdefaulttimeout(5)

        try:
            # connect to pop3 server without ssl
            server = poplib.POP3(POP3_HOST, POP3_PORT)

            # upgrade connection to tls
            server.stls()

            # authenticate user
            server.user(POP3_USER)
            server.pass_(POP3_PASSWORD)

            emails = []
            message_count = len(server.list()[1])

            for i in range(1, message_count + 1):
                response, lines, _ = server.retr(i)
                msg_content = b"\n".join(lines)
                msg = BytesParser(policy=default).parsebytes(msg_content)

                emails.append({
                    "from": msg["from"],
                    "subject": msg["subject"],
                    "has_attachments": msg.is_multipart()
                })

            server.quit()
            return emails

        except socket.timeout:
            print("pop3 connection timed out, sandbox limitation detected")
            return []

        except poplib.error_proto as error:
            print(f"pop3 protocol error: {error}")
            return []
