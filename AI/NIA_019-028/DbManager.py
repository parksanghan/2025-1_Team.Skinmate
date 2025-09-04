import pymysql


class DbManager:
    def __init__(self, host, user, password, db, port=3306):
        try:
            self.connection = pymysql.connect(
                host=host,
                user=user,
                password=str(password).strip(),
                db=db,
                port=port,
                charset="utf8mb4",
                cursorclass=pymysql.cursors.DictCursor,
            )
            print("MySQL 연결 성공")
        except Exception as e:
            print(f"MySQL 연결 실패: {e}")

    def close(self):
        if self.connection:
            self.connection.close()
            print("MySQL 연결 종료")

    def register_user(self, username, password):
        try:
            with self.connection.cursor() as cursor:
                sql = "INSERT INTO users (username, password) VALUES (%s, %s)"
                cursor.execute(sql, (username, password))
            self.connection.commit()
            print(f"회원가입 성공: {username}")
        except Exception as e:
            print(f"회원가입 실패: {e}")

    def login_user(self, username, password):
        try:
            with self.connection.cursor() as cursor:
                sql = "SELECT * FROM users WHERE username = %s AND password = %s"
                cursor.execute(sql, (username, password))
                result = cursor.fetchone()
                if result:
                    print(f"로그인 성공: {username}")
                    return True
                else:
                    print("로그인 실패: 잘못된 사용자명 또는 비밀번호")
                    return False
        except Exception as e:
            print(f"로그인 오류: {e}")
            return False

    def get_user_id(self, username):
        try:
            with self.connection.cursor() as cursor:
                sql = "SELECT user_id FROM users WHERE username = %s"
                cursor.execute(sql, (username,))
                result = cursor.fetchone()
                if result:
                    return result["user_id"]
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
            with self.connection.cursor() as cursor:
                sql = """
                    INSERT INTO chat_logs (user_id, log_type, message, response) 
                    VALUES (%s, '질의응답', %s, %s)
                """
                cursor.execute(sql, (user_id, question, response))
            self.connection.commit()
            print("질의응답 데이터 추가 성공")
        except Exception as e:
            print(f"질의응답 데이터 추가 실패: {e}")

    def add_diagnosis_log(self, username, image_path, diagnosis_result, response):
        user_id = self.get_user_id(username)
        if user_id is None:
            print("유효하지 않은 사용자입니다.")
            return
        try:
            with self.connection.cursor() as cursor:
                sql = """
                    INSERT INTO chat_logs (user_id, log_type, image_path, diagnosis_result, response) 
                    VALUES (%s, '진단분석', %s, %s, %s)
                """
                cursor.execute(sql, (user_id, image_path, diagnosis_result, response))
            self.connection.commit()
            print("진단분석 데이터 추가 성공")
        except Exception as e:
            print(f"진단분석 데이터 추가 실패: {e}")

    def get_user_logs(self, username):
        user_id = self.get_user_id(username)
        if user_id is None:
            print("유효하지 않은 사용자입니다.")
            return []
        try:
            with self.connection.cursor() as cursor:
                sql = """
                    SELECT * FROM chat_logs WHERE user_id = %s ORDER BY timestamp ASC
                """
                cursor.execute(sql, (user_id,))
                logs = cursor.fetchall()
                print(f"총 {len(logs)}개의 로그를 조회했습니다.")
                return logs
        except Exception as e:
            print(f"로그 조회 실패: {e}")
            return []


if __name__ == "__main__":
    db_manager = DbManager("127.0.0.1", "root", "", "skin_health", 3306)

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

    db_manager.close()
