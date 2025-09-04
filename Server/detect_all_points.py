import cv2
import mediapipe as mp

mp_face_mesh = mp.solutions.face_mesh
drawing_spec = mp.solutions.drawing_utils.DrawingSpec(
    color=(0, 255, 0), thickness=1, circle_radius=1
)

cap = cv2.VideoCapture(0)

with mp_face_mesh.FaceMesh(
    static_image_mode=False,
    max_num_faces=1,
    refine_landmarks=True,
    min_detection_confidence=0.5,
) as face_mesh:
    while cap.isOpened():
        ret, frame = cap.read()
        if not ret:
            break

        image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = face_mesh.process(image)
        image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
        h, w, _ = image.shape

        if results.multi_face_landmarks:
            for face_landmarks in results.multi_face_landmarks:
                for idx, lm in enumerate(face_landmarks.landmark):
                    x, y = int(lm.x * w), int(lm.y * h)
                    cv2.circle(image, (x, y), 1, (0, 255, 0), -1)
                    cv2.putText(
                        image,
                        str(idx),
                        (x, y),
                        cv2.FONT_HERSHEY_SIMPLEX,
                        0.3,
                        (0, 0, 255),
                        1,
                    )

        cv2.imshow("Face Mesh Index Viewer", image)
        if cv2.waitKey(5) & 0xFF == 27:  # ESC 종료
            break

cap.release()
cv2.destroyAllWindows()
