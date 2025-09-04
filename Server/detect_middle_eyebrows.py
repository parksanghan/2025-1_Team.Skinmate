import cv2
import mediapipe as mp

mp_face_mesh = mp.solutions.face_mesh

# 눈썹 안쪽 끝 좌표
GLABELLA_IDX = [107, 336]

# 고정된 박스 세로 길이 (픽셀 단위)
BOX_HEIGHT = 20

cap = cv2.VideoCapture(0)

with mp_face_mesh.FaceMesh(
    static_image_mode=False,
    max_num_faces=1,
    refine_landmarks=True,
    min_detection_confidence=0.5,
) as face_mesh:
    while cap.isOpened():
        success, frame = cap.read()
        if not success:
            break

        image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = face_mesh.process(image)
        image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

        if results.multi_face_landmarks:
            for face_landmarks in results.multi_face_landmarks:
                h, w, _ = image.shape

                # 좌표 추출 (픽셀 좌표화)
                pts = [
                    (
                        int(face_landmarks.landmark[i].x * w),
                        int(face_landmarks.landmark[i].y * h),
                    )
                    for i in GLABELLA_IDX
                ]

                # x 범위는 좌우 눈썹 끝
                x_min = min(p[0] for p in pts)
                x_max = max(p[0] for p in pts)

                # y 범위는 좌표 중 높은 쪽 기준으로 고정 세로 추가
                y_base = min(p[1] for p in pts)
                y_min = y_base
                y_max = y_base + BOX_HEIGHT

                # 파란색 사각형 표시
                cv2.rectangle(image, (x_min, y_min), (x_max, y_max), (255, 0, 0), 2)

                # 디버깅용 포인트 출력
                for p in pts:
                    cv2.circle(image, p, 2, (0, 0, 255), -1)

        cv2.imshow("Glabella Bounding Box", image)
        if cv2.waitKey(5) & 0xFF == 27:  # ESC 종료
            break

cap.release()
cv2.destroyAllWindows()
