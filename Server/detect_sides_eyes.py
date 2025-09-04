import cv2
import mediapipe as mp

mp_face_mesh = mp.solutions.face_mesh

# 눈꼬리 Landmark
LEFT_EYE_TAIL_IDX = 130  # 왼쪽 눈꼬리
RIGHT_EYE_TAIL_IDX = 263  # 오른쪽 눈꼬리

# 바운딩 박스 크기
BOX_WIDTH = 10
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
        h, w, _ = image.shape

        if results.multi_face_landmarks:
            for face_landmarks in results.multi_face_landmarks:
                # 왼쪽 눈꼬리 좌표
                lx = int(face_landmarks.landmark[LEFT_EYE_TAIL_IDX].x * w)
                ly = int(face_landmarks.landmark[LEFT_EYE_TAIL_IDX].y * h)

                # 오른쪽 눈꼬리 좌표
                rx = int(face_landmarks.landmark[RIGHT_EYE_TAIL_IDX].x * w)
                ry = int(face_landmarks.landmark[RIGHT_EYE_TAIL_IDX].y * h)

                # 왼쪽 눈가 박스 (왼쪽 방향으로)
                cv2.rectangle(
                    image,
                    (lx - BOX_WIDTH, ly - BOX_HEIGHT // 2),
                    (lx, ly + BOX_HEIGHT // 2),
                    (0, 255, 0),
                    2,
                )

                # 오른쪽 눈가 박스 (오른쪽 방향으로)
                cv2.rectangle(
                    image,
                    (rx, ry - BOX_HEIGHT // 2),
                    (rx + BOX_WIDTH, ry + BOX_HEIGHT // 2),
                    (255, 0, 0),
                    2,
                )

                # 디버깅용 점 표시
                cv2.circle(image, (lx, ly), 2, (0, 0, 255), -1)
                cv2.circle(image, (rx, ry), 2, (0, 0, 255), -1)

        cv2.imshow("Eye Wrinkle Region (Crow's Feet)", image)
        if cv2.waitKey(5) & 0xFF == 27:
            break

cap.release()
cv2.destroyAllWindows()
