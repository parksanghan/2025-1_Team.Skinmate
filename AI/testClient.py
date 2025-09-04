import requests

# 서버 주소
url = "http://192.168.123.161:5000/diagnose"

# 테스트할 이미지 경로
image_path = r"S:\[Education]\Capstone1\NIA_019-028\dataset\img\digitcamera\0002\0002_01_F.jpg"  # 반드시 이 경로에 테스트용 이미지가 있어야 함

# 파일 전송
with open(image_path, "rb") as img_file:
    files = {"image": ("filename.jpg", img_file, "image/jpeg")}  # 명시적으로 MIME 타입 지정 가능
    response = requests.post(url, files=files)

# 결과 출력
if response.status_code == 200:
    print("추론 결과:")
    print(response.json())
else:
    print(f"요청 실패! 상태 코드: {response.status_code}")
    print("에러 메시지:", response.text)
