from supabase import create_client, Client
from typing import Optional
import json
import os
import bcrypt


class DbManager:
    def __init__(self, url, key):
        try:
            self.supabase: Client = create_client(url, key)
            print("Supabase 연결 성공")
        except Exception as e:
            print(f"Supabase 연결 실패: {e}")

    def register_user(self, username, password):
        try:

            response = (
                self.supabase.table("users")
                .insert({"username": username, "password": password})
                .execute()
            )
            print(f"회원가입 성공: {username}")
        except Exception as e:
            print(f"회원가입 실패: {e}")

    def registeer_prototype(self, username, password):
        try:
            password = bcrypt.hashpw(password.encode("utf-8"), bcrypt.gensalt()).decode(
                "utf-8"
            )

            response = (
                self.supabase.table("users")
                .insert({"username": username, "password": password})
                .execute()
            )
            print(f"회원가입 성공: {username}")
        except Exception as e:
            print(f"회원가입 실패: {e}")

    def login_user(self, username, password):
        try:
            response = (
                self.supabase.table("users")
                .select("*")
                .eq("username", username)
                .eq("password", password)
                .execute()
            )
            if response.data:
                print(f"로그인 성공: {username}")
                return True
            else:
                print("로그인 실패: 잘못된 사용자명 또는 비밀번호")
                return False
        except Exception as e:
            print(f"로그인 오류: {e}")
            return False

    def login_prototype(self, username, password):
        try:
            response = (
                self.supabase.table("users")
                .select("password")
                .eq("username", username)
                .execute()
            )
            if response.data:
                hashed_pw = response.data[0]["password"]
                if bcrypt.checkpw(password.encode("utf-8"), hashed_pw.encode("utf-8")):
                    print(f"로그인 성공: {username}")
                    return True
                else:
                    print("로그인 실패: 잘못된 비밀번호")
                    return False
            else:
                print("로그인 실패: 잘못된 사용자명 또는 비밀번호")
                return False
        except Exception as e:
            print(f"로그인 오류: {e}")
            return False

    def get_user_id(self, username) -> Optional[int]:
        try:
            response = (
                self.supabase.table("users")
                .select("user_id")
                .eq("username", username)
                .execute()
            )
            if response.data:
                return response.data[0]["user_id"]
            else:
                print(f"사용자 {username}을(를) 찾을 수 없습니다.")
                return None
        except Exception as e:
            print(f"사용자 ID 조회 실패: {e}")
            return None

    def add_chat_log(self, username, question, response):
        user_id = self.get_user_id(username)
        if user_id is None:
            print("유효하지 않은 사용자입니다.")
            return
        try:
            self.supabase.table("chat_logs").insert(
                {
                    "user_id": user_id,
                    "log_type": "질의응답",
                    "message": question,
                    "response": response,
                }
            ).execute()
            print("질의응답 데이터 추가 성공")
        except Exception as e:
            print(f"질의응답 데이터 추가 실패: {e}")

    def add_diagnosis_log(self, username, image_path, diagnosis_result, response):
        user_id = self.get_user_id(username)
        if user_id is None:
            print("유효하지 않은 사용자입니다.")
            return
        try:
            self.supabase.table("chat_logs").insert(
                {
                    "user_id": user_id,
                    "log_type": "진단분석",
                    "image_path": image_path,
                    "diagnosis_result": diagnosis_result,  # json dump 해야할수도
                    "response": response,
                }
            ).execute()
            print("진단분석 데이터 추가 성공")
        except Exception as e:
            print(f"진단분석 데이터 추가 실패: {e}")

    def add_setting_log(self, username, data):
        user_id = self.get_user_id(username)
        if user_id is None:
            print("유효하지 않은 사용자입니다.")
            return
        try:
            payload_dict = data.dict()

            # 1. 기존 사용자 설정 조회
            response = (
                self.supabase.table("chat_logs")
                .select("*")
                .eq("user_id", user_id)
                .eq("log_type", "사용자설정")
                .execute()
            )

            if response.data:  # ✅ 기존에 존재하면
                existing_id = response.data[0]["chat_id"]  # chat_id 기준
                print(
                    f"기존 사용자 설정이 있어 업데이트합니다. (chat_id={existing_id})"
                )

                self.supabase.table("chat_logs").update(
                    {
                        "message": json.dumps(payload_dict, ensure_ascii=False),
                        "response": "사용자 설정 업데이트 완료",
                    }
                ).eq("chat_id", existing_id).execute()
                print("유저 설정 업데이트 성공")

            else:  # ✅ 없으면 새로 추가
                print("사용자 설정이 없어 새로 추가합니다.")
                self.supabase.table("chat_logs").insert(
                    {
                        "user_id": user_id,
                        "log_type": "사용자설정",
                        "message": json.dumps(payload_dict, ensure_ascii=False),
                        "response": "사용자 설정 저장 완료",
                    }
                ).execute()
                print("유저 설정 추가 성공")

        except Exception as e:
            print(f"유저 설정 추가/업데이트 실패: {e}")

    def get_user_logs(self, username):
        user_id = self.get_user_id(username)
        if user_id is None:
            print("유효하지 않은 사용자입니다.")
            return []
        try:
            response = (
                self.supabase.table("chat_logs")
                .select("*")
                .eq("user_id", user_id)
                .order("timestamp")
                .execute()
            )
            print(f"총 {len(response.data)}개의 로그를 조회했습니다.")
            return response.data
        except Exception as e:
            print(f"로그 조회 실패: {e}")
            return []

    async def upload_image_to_storage(self, username, filename, data: bytes, filetype):

        userId = self.get_user_id(username)
        filepath = f"{userId}/{filename}"
        self.supabase.storage.from_("chat-images").upload(
            filepath, data, {"content-type": filetype}
        )
        res = self.supabase.storage.from_("chat-images").get_public_url(filepath)
        public_url = res if isinstance(res, str) else res["data"]["publicUrl"]
        return public_url

    async def get_image_from_stroage(self, username, filename):
        userId = self.get_user_id(username)
        filepath = f"{userId}/{filename}"
        res = self.supabase.storage.from_("chat-images").get_public_url(filepath)
        public_url = res if isinstance(res, str) else res["data"]["publicUrl"]
        return public_url


# 예시 사용
if __name__ == "__main__":
    url = os.getenv("SUPABASE_URL")
    key = os.getenv("SUPABASE_KEY")
    db_manager = DbManager(url, key)

    db_manager.register_user("user1", "password1")
    db_manager.add_chat_log(
        "user1", "주름 개선 방법이 뭐야?", "주름 개선에는 수분크림이 효과적입니다."
    )
    db_manager.add_diagnosis_log(
        "user1",
        "/path/to/image.jpg",
        '{"wrinkle": 2, "jawline": 4}',
        "주름은 평균보다 많고 턱선은 준수합니다.",
    )

    logs = db_manager.get_user_logs("user1")
    for log in logs:
        print(log)
