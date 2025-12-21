import json

class EmailStorage:
    def save_to_json(self, emails, path="data/emails.json"):
        # save emails to json file
        with open(path, "w", encoding="utf-8") as file:
            json.dump(emails, file, indent=4, ensure_ascii=False)
