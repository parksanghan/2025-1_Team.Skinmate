import cv2
import mediapipe as mp

mp_face_mesh = mp.solutions.face_mesh

# 각 Landmark Index
LEFT_EYE_BOTTOM = 145
RIGHT_EYE_BOTTOM = 374
LEFT_LIP_CORNER = 61
RIGHT_LIP_CORNER = 291
LOWER_LIP_CENTER_IDX = 17
CHIN_CENTER_IDX = 152

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
                # 볼 중간점 좌표 계산
                lx1 = int(face_landmarks.landmark[LEFT_EYE_BOTTOM].x * w)
                ly1 = int(face_landmarks.landmark[LEFT_EYE_BOTTOM].y * h)
                lx2 = int(face_landmarks.landmark[LEFT_LIP_CORNER].x * w)
                ly2 = int(face_landmarks.landmark[LEFT_LIP_CORNER].y * h)
                lcx = (lx1 + lx2) // 2
                lcy = (ly1 + ly2) // 2

                rx1 = int(face_landmarks.landmark[RIGHT_EYE_BOTTOM].x * w)
                ry1 = int(face_landmarks.landmark[RIGHT_EYE_BOTTOM].y * h)
                rx2 = int(face_landmarks.landmark[RIGHT_LIP_CORNER].x * w)
                ry2 = int(face_landmarks.landmark[RIGHT_LIP_CORNER].y * h)
                rcx = (rx1 + rx2) // 2
                rcy = (ry1 + ry2) // 2

                # 입술 아래 ~ 턱 끝 y 범위
                lip_y = int(face_landmarks.landmark[LOWER_LIP_CENTER_IDX].y * h)
                chin_y = int(face_landmarks.landmark[CHIN_CENTER_IDX].y * h)

                # 턱 바운딩 박스: 볼 중심 좌우 빨간점 기준으로 width 사용
                x1 = lcx
                x2 = rcx

                cv2.rectangle(image, (x1, lip_y), (x2, chin_y), (0, 255, 255), 2)

                # 디버깅용 점
                cv2.circle(image, (lcx, lcy), 2, (0, 0, 255), -1)
                cv2.circle(image, (rcx, rcy), 2, (0, 0, 255), -1)
                cv2.circle(image, (x1, lip_y), 3, (0, 255, 0), -1)
                cv2.circle(image, (x2, chin_y), 3, (255, 0, 0), -1)

        cv2.imshow("Chin Box with Cheek-based Width", image)
        if cv2.waitKey(5) & 0xFF == 27:
            break

cap.release()
cv2.destroyAllWindows()
