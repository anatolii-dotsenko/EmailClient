from core.email_sender import EmailSender
from core.email_receiver import EmailReceiver
from core.email_storage import EmailStorage
from cli.menu import Menu

def main():
    sender = EmailSender()
    receiver = EmailReceiver()
    storage = EmailStorage()
    menu = Menu()

    while True:
        menu.show()
        choice = input("choose option: ")

        if choice == "1":
            to_email = input("to: ")
            subject = input("subject: ")
            body = input("message: ")
            sender.send_email(to_email, subject, body)

        elif choice == "2":
            emails = receiver.fetch_emails()
            storage.save_to_json(emails)
            print("emails saved to json")

        elif choice == "3":
            break

if __name__ == "__main__":
    main()
