import cv2
import mediapipe as mp

mp_face_mesh = mp.solutions.face_mesh
face_mesh = mp_face_mesh.FaceMesh(static_image_mode=False, max_num_faces=1)

LIPS_INDEXES = [
    61,
    146,
    91,
    181,
    84,
    17,
    314,
    405,
    321,
    375,
    291,
    308,
    324,
    318,
    402,
    317,
    14,
    87,
    178,
    88,
    95,
    185,
    40,
    39,
    37,
    0,
    267,
    269,
    270,
    409,
    415,
    310,
    311,
    312,
    13,
    82,
    81,
    80,
    191,
]

cap = cv2.VideoCapture(0)

while cap.isOpened():
    ret, frame = cap.read()
    if not ret:
        break

    h, w, _ = frame.shape
    rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    results = face_mesh.process(rgb)

    if results.multi_face_landmarks:
        for face_landmarks in results.multi_face_landmarks:
            xs, ys = [], []
            for idx in LIPS_INDEXES:
                pt = face_landmarks.landmark[idx]
                xs.append(int(pt.x * w))
                ys.append(int(pt.y * h))

            # 바운딩 박스 계산
            x_min, x_max = min(xs), max(xs)
            y_min, y_max = min(ys), max(ys)
            cv2.rectangle(frame, (x_min, y_min), (x_max, y_max), (0, 255, 0), 2)

    cv2.imshow("Lips Bounding Box", frame)
    if cv2.waitKey(1) & 0xFF == 27:
        break

cap.release()
cv2.destroyAllWindows()
