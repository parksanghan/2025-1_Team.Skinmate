import cv2
import mediapipe as mp

# MediaPipe 초기화
mp_face_detection = mp.solutions.face_detection

# 카메라 열기
cap = cv2.VideoCapture(0)

# 바운딩 박스에 여유를 줄 비율
MARGIN_TOP_RATIO = 0.15
MARGIN_BOTTOM_RATIO = 0.1
MARGIN_LEFT_RATIO = 0.05
MARGIN_RIGHT_RATIO = 0.05

with mp_face_detection.FaceDetection(
    model_selection=0, min_detection_confidence=0.5
) as face_detection:
    while cap.isOpened():
        ret, frame = cap.read()
        if not ret:
            break

        h, w, _ = frame.shape
        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = face_detection.process(rgb_frame)

        if results.detections:
            for det in results.detections:
                bbox = det.location_data.relative_bounding_box

                # 원래 상대 좌표
                x = int((bbox.xmin - MARGIN_LEFT_RATIO) * w)
                y = int((bbox.ymin - MARGIN_TOP_RATIO) * h)
                box_w = int((bbox.width + MARGIN_LEFT_RATIO + MARGIN_RIGHT_RATIO) * w)
                box_h = int((bbox.height + MARGIN_TOP_RATIO + MARGIN_BOTTOM_RATIO) * h)

                # 이미지 범위를 벗어나지 않게 보정
                x = max(x, 0)
                y = max(y, 0)
                x2 = min(x + box_w, w)
                y2 = min(y + box_h, h)

                # 널널한 바운딩 박스 그리기
                cv2.rectangle(frame, (x, y), (x2, y2), (0, 255, 255), 2)

        cv2.imshow("Face Detection - Bounding Box", frame)
        if cv2.waitKey(5) & 0xFF == 27:
            break

cap.release()
cv2.destroyAllWindows()
