import cv2
import mediapipe as mp

LEFT_CHEEK_IDX = [120, 234, 203, 132]
RIGHT_CHEEK_IDX = [348, 454, 423, 376]

mp_face_mesh = mp.solutions.face_mesh

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

                def draw_cheek(points_idx, color):
                    points = []
                    for idx in points_idx:
                        x = int(face_landmarks.landmark[idx].x * w)
                        y = int(face_landmarks.landmark[idx].y * h)
                        points.append((x, y))
                        cv2.circle(image, (x, y), 2, (0, 0, 255), -1)
                    x_coords = [p[0] for p in points]
                    y_coords = [p[1] for p in points]
                    x_min, x_max = min(x_coords), max(x_coords)
                    y_min, y_max = min(y_coords), max(y_coords)
                    cv2.rectangle(image, (x_min, y_min), (x_max, y_max), color, 2)

                draw_cheek(LEFT_CHEEK_IDX, (0, 255, 0))  # 초록
                draw_cheek(RIGHT_CHEEK_IDX, (255, 0, 0))  # 파랑

        cv2.imshow("Cheek Detection", image)
        if cv2.waitKey(5) & 0xFF == 27:
            break

cap.release()
cv2.destroyAllWindows()
