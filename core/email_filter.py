class EmailFilter:
    def filter_by_sender(self, emails, sender):
        # filter emails by sender
        return [email for email in emails if sender in email["from"]]

    def filter_by_subject(self, emails, keyword):
        # filter emails by subject
        return [email for email in emails if keyword in email["subject"]]
