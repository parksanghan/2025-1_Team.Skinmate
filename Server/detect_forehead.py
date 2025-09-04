import cv2
import mediapipe as mp
import numpy as np

mp_face_mesh = mp.solutions.face_mesh

# 실제 이마 중심부에 해당하는 landmark index (정확도 중심)
FOREHEAD_IDX = [70, 63, 105, 66, 107, 336, 296, 334, 293, 334, 10, 151]

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

                # landmark를 이미지 좌표로 변환
                pts = [
                    (
                        int(face_landmarks.landmark[i].x * w),
                        int(face_landmarks.landmark[i].y * h),
                    )
                    for i in FOREHEAD_IDX
                ]

                # 이마 바운딩 박스 계산
                x_min = min(p[0] for p in pts)
                y_min = min(p[1] for p in pts)
                x_max = max(p[0] for p in pts)
                y_max = max(p[1] for p in pts)

                # 사각형으로 이마 영역 표시
                cv2.rectangle(image, (x_min, y_min), (x_max, y_max), (0, 255, 0), 2)

                # 선택적으로 원으로 포인트도 찍기 (디버깅용)
                for p in pts:
                    cv2.circle(image, p, 2, (0, 0, 255), -1)

        cv2.imshow("Forehead Bounding Box", image)
        if cv2.waitKey(5) & 0xFF == 27:
            break

cap.release()
cv2.destroyAllWindows()
