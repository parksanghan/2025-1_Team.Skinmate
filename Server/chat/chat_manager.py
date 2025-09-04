import openai
import os
from datetime import datetime
from db.db_manager import DbManager
from dotenv import load_dotenv
import json

load_dotenv()


class ChatManager:
    def __init__(self):
        self.api_key = os.getenv("OPENAI_API_KEY")
        openai.api_key = self.api_key
        self.client = openai.OpenAI()

    def diagnosis_to_question(self, diagnosis_result):
        # return f"다음과 같은 결과를 설명해주면 좋겠어. ```{diagnosis_result}```"
        return f"다음 피부 진단 결과를 사람에게 설명해줘:\n```json\n{json.dumps(diagnosis_result, ensure_ascii=False, indent=2)}\n```"

    def setting_to_question(self, setting):
        return f"이게 내 관심사와 나이 성별인데 앞으로 이를 참고해서 대답해줘 :\n```json\n{json.dumps(setting, ensure_ascii=False, indent=2)}\n```"

    # def logs_to_chatlist(self, logs):
    #     chat_list = []

    #     for log in logs:
    #         if log["log_type"] == "진단분석":
    #             message = self.diagnosis_to_question(log["diagnosis_result"])
    #         if log["log_type"] == "사용자설정":
    #             message = self
    #             # 나머지는 채팅이므로
    #         else:
    #             message = log["message"]
    #         if message:
    #             chat_list.append({"role": "user", "content": message})
    #         if log["response"]:
    #             chat_list.append({"role": "assistant", "content": log["response"]})

    #     return chat_list
    def logs_to_chatlist(self, logs):
        chat_list = []

        for log in logs:
            if log["log_type"] == "진단분석":
                diagnosis = log["diagnosis_result"]
                if isinstance(diagnosis, str):
                    diagnosis = json.loads(diagnosis)
                message = self.diagnosis_to_question(diagnosis)

            elif log["log_type"] == "사용자설정":
                setting = log["message"]
                if isinstance(setting, str):
                    setting = json.loads(setting)
                message = self.setting_to_question(setting)

            else:
                message = log["message"]

            if message:
                chat_list.append({"role": "user", "content": message})
            if log["response"]:
                chat_list.append({"role": "assistant", "content": log["response"]})
        for chat in chat_list:
            print(chat)
        return chat_list

    def request_chat_response(self, logs, user_message):
        try:
            chat_list = self.logs_to_chatlist(logs)

            chat_list.insert(
                0, {"role": "system", "content": "You are a helpful assistant."}
            )
            chat_list.append({"role": "user", "content": user_message})

            response = self.client.responses.create(
                model="gpt-4o-mini", input=chat_list
            )
            print(chat_list)

            return response.output_text

        except Exception as e:
            print(f"오류 발생: {e}")
            return "현재 질의응답 서비스를 이용할 수 없습니다."

    def request_chat_dignosis(self, logs, diagnosis_result):
        try:
            prompt = f"다음 피부 진단 결과를 사람에게 설명해줘:\n```json\n{json.dumps(diagnosis_result, ensure_ascii=False, indent=2)}\n```"

            chat_list = self.logs_to_chatlist(logs)
            chat_list.insert(
                0, {"role": "system", "content": "You are a helpful assistant."}
            )
            chat_list.append({"role": "user", "content": prompt})
            print(prompt)
            response = self.client.responses.create(
                model="gpt-4o-mini", input=chat_list
            )
            return response.output_text

        except Exception as e:
            print(f"오류 발생: {e}")
            return "현재 질의응답 서비스를 이용할 수 없습니다."


if __name__ == "__main__":
    dbManager = DbManager("127.0.0.1", "root", "GoodKamo72@", "skin_health", 3306)

    chat_manager = ChatManager()
    logs = [
        {
            "chat_id": 1,
            "user_id": 1,
            "log_type": "질의응답",
            "image_path": None,
            "diagnosis_result": None,
            "message": "주름 개선 방법이 뭐야?",
            "response": "주름 개선에는 수분크림이 효과적입니다.",
            "timestamp": datetime(2025, 3, 31, 21, 55, 11),
        },
        {
            "chat_id": 2,
            "user_id": 1,
            "log_type": "진단분석",
            "image_path": "/path/to/image.jpg",
            "diagnosis_result": '{"jawline": 4, "wrinkle": 2}',
            "message": None,
            "response": "주름은 평균보다 많고 턱선은 준수합니다.",
            "timestamp": datetime(2025, 3, 31, 21, 55, 11),
        },
    ]
    logs = dbManager.get_user_logs("user1")
    user_message = "피부 관리 방법 추천해줘."
    response = chat_manager.request_chat_response(logs, user_message)
    print(f"챗봇: {response}")
