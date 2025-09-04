import cv2
import mediapipe as mp

LEFT_EYE_BOTTOM = 145
RIGHT_EYE_BOTTOM = 374
LEFT_LIP_CORNER = 61
RIGHT_LIP_CORNER = 291
LOWER_LIP_CENTER_IDX = 17
CHIN_CENTER_IDX = 152

MARGIN_TOP_RATIO = 0.15
MARGIN_BOTTOM_RATIO = 0.1
MARGIN_LEFT_RATIO = 0.05
MARGIN_RIGHT_RATIO = 0.05


# cv2.imread()후 Detetmanager 사용용
class DetectManager:
    def __init__(self, image_bgr):
        self.mp_face_mesh = mp.solutions.face_mesh
        self.image_bgr = image_bgr
        self.mp_face_detection = mp.solutions.face_detection  # ✅ 이 줄 추가
        self.h, self.w, _ = image_bgr.shape
        self.image_rgb = cv2.cvtColor(image_bgr, cv2.COLOR_BGR2RGB)
        with self.mp_face_mesh.FaceMesh(
            static_image_mode=True,  # 사진이므로 True
            max_num_faces=1,
            refine_landmarks=True,
            min_detection_confidence=0.5,
        ) as face_mesh:
            self.results = face_mesh.process(self.image_rgb)
        with self.mp_face_detection.FaceDetection(
            model_selection=0, min_detection_confidence=0.5
        ) as face_detection:
            self.face_box_results = face_detection.process(self.image_rgb)

    def __call__(self, *args, **kwds):
        if self.results.multi_face_landmarks:
            self.face_landmarks = self.results.multi_face_landmarks[0]
            self.draw_cheecks()
            self.draw_chin()
            self.draw_forehead()
            self.draw_lips()
            self.draw_middle_eyebrows()
            self.draw_eye_wrinkles()
            self.draw_loose_face_box()

    def draw_loose_face_box(self):
        for det in self.face_box_results.detections:
            bbox = det.location_data.relative_bounding_box

            x = int((bbox.xmin - MARGIN_LEFT_RATIO) * self.w)
            y = int((bbox.ymin - MARGIN_TOP_RATIO) * self.h)
            box_w = int((bbox.width + MARGIN_LEFT_RATIO + MARGIN_RIGHT_RATIO) * self.w)
            box_h = int((bbox.height + MARGIN_TOP_RATIO + MARGIN_BOTTOM_RATIO) * self.h)

            x = max(x, 0)
            y = max(y, 0)
            x2 = min(x + box_w, self.w)
            y2 = min(y + box_h, self.h)

            cv2.rectangle(self.image_bgr, (x, y), (x2, y2), (0, 255, 255), 2)

    def draw_eye_wrinkles(self):
        if not self.face_landmarks:
            return

        LEFT_EYE_TAIL_IDX = 130
        RIGHT_EYE_TAIL_IDX = 263
        BOX_WIDTH = int(self.w * 0.02)
        BOX_HEIGHT = int(self.h * 0.035)

        lx = int(self.face_landmarks.landmark[LEFT_EYE_TAIL_IDX].x * self.w)
        ly = int(self.face_landmarks.landmark[LEFT_EYE_TAIL_IDX].y * self.h)
        rx = int(self.face_landmarks.landmark[RIGHT_EYE_TAIL_IDX].x * self.w)
        ry = int(self.face_landmarks.landmark[RIGHT_EYE_TAIL_IDX].y * self.h)

        cv2.rectangle(
            self.image_bgr,
            (lx - BOX_WIDTH, ly - BOX_HEIGHT // 2),
            (lx, ly + BOX_HEIGHT // 2),
            (0, 255, 0),
            2,
        )
        cv2.rectangle(
            self.image_bgr,
            (rx, ry - BOX_HEIGHT // 2),
            (rx + BOX_WIDTH, ry + BOX_HEIGHT // 2),
            (255, 0, 0),
            2,
        )
        cv2.circle(self.image_bgr, (lx, ly), 2, (0, 0, 255), -1)
        cv2.circle(self.image_bgr, (rx, ry), 2, (0, 0, 255), -1)

    # 턱부분
    def draw_chin(self):
        if not self.face_landmarks:
            return
        lip_y = int(self.face_landmarks.landmark[17].y * self.h)
        chin_y = int(self.face_landmarks.landmark[152].y * self.h)

        lx = int(
            (
                (
                    self.face_landmarks.landmark[145].x
                    + self.face_landmarks.landmark[61].x
                )
                / 2
            )
            * self.w
        )
        rx = int(
            (
                (
                    self.face_landmarks.landmark[374].x
                    + self.face_landmarks.landmark[291].x
                )
                / 2
            )
            * self.w
        )

        cv2.rectangle(self.image_bgr, (lx, lip_y), (rx, chin_y), (0, 255, 255), 2)

    def draw_forehead(self):
        if not self.face_landmarks:
            return
        FOREHEAD_IDX = [70, 63, 105, 66, 107, 336, 296, 334, 293, 334, 10, 151]

        # landmark를 이미지 좌표로 변환
        pts = [
            (
                int(self.face_landmarks.landmark[i].x * self.w),
                int(self.face_landmarks.landmark[i].y * self.h),
            )
            for i in FOREHEAD_IDX
        ]

        # 이마 바운딩 박스 계산
        x_min = min(p[0] for p in pts)
        y_min = min(p[1] for p in pts)
        x_max = max(p[0] for p in pts)
        y_max = max(p[1] for p in pts)

        # 사각형으로 이마 영역 표시
        cv2.rectangle(self.image_bgr, (x_min, y_min), (x_max, y_max), (0, 255, 0), 2)

        # 디버깅용 포인트
        for p in pts:
            cv2.circle(self.image_bgr, p, 2, (0, 0, 255), -1)

    def draw_lips(self):
        if not self.face_landmarks:
            return
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

        xs, ys = [], []
        for idx in LIPS_INDEXES:
            pt = self.face_landmarks.landmark[idx]
            xs.append(int(pt.x * self.w))
            ys.append(int(pt.y * self.h))

        x_min, x_max = min(xs), max(xs)
        y_min, y_max = min(ys), max(ys)

        cv2.rectangle(self.image_bgr, (x_min, y_min), (x_max, y_max), (0, 255, 0), 2)

    def draw_middle_eyebrows(self):
        if not self.face_landmarks:
            return
        GLABELLA_IDX = [107, 336]
        BOX_HEIGHT = 20

        pts = [
            (
                int(self.face_landmarks.landmark[i].x * self.w),
                int(self.face_landmarks.landmark[i].y * self.h),
            )
            for i in GLABELLA_IDX
        ]

        x_min = min(p[0] for p in pts)
        x_max = max(p[0] for p in pts)

        y_base = min(p[1] for p in pts)
        y_min = y_base
        y_max = y_base + BOX_HEIGHT

        # 사각형 그리기
        cv2.rectangle(self.image_bgr, (x_min, y_min), (x_max, y_max), (255, 0, 0), 2)

        # 디버깅용 점
        for p in pts:
            cv2.circle(self.image_bgr, p, 2, (0, 0, 255), -1)

    def draw_cheecks(self):
        if not self.face_landmarks:
            return

        LEFT_CHEEK_IDX = [120, 234, 203, 132]
        RIGHT_CHEEK_IDX = [348, 454, 423, 376]

        def draw_cheek(points_idx, color):
            points = []
            for idx in points_idx:
                x = int(self.face_landmarks.landmark[idx].x * self.w)
                y = int(self.face_landmarks.landmark[idx].y * self.h)
                points.append((x, y))
                cv2.circle(self.image_bgr, (x, y), 2, (0, 0, 255), -1)
            x_coords = [p[0] for p in points]
            y_coords = [p[1] for p in points]
            x_min, x_max = min(x_coords), max(x_coords)
            y_min, y_max = min(y_coords), max(y_coords)
            cv2.rectangle(self.image_bgr, (x_min, y_min), (x_max, y_max), color, 2)

        draw_cheek(LEFT_CHEEK_IDX, (0, 255, 0))  # 왼쪽 볼 - 초록색
        draw_cheek(RIGHT_CHEEK_IDX, (255, 0, 0))  # 오른쪽 볼 - 파란색


import cv2
import numpy as np
from pathlib import Path

if __name__ == "__main__":
    image_path = Path(
        r"C:\Users\박상한\Pictures\Camera Roll\WIN_20250528_10_34_40_Pro.jpg"
    )

    if not image_path.exists():
        print(f"[경고] 파일이 존재하지 않습니다:\n{image_path}")
    else:
        # 한글 경로 문제 방지용 이미지 로드
        image = cv2.imdecode(
            np.fromfile(str(image_path), dtype=np.uint8), cv2.IMREAD_COLOR
        )

        if image is None:
            print(f"[오류] 이미지를 읽지 못했습니다. 파일 손상 가능성:\n{image_path}")
        else:
            dm = DetectManager(image.copy())
            dm()

            # 윈도우 창 크기 조절 가능하게 설정
            cv2.namedWindow("Face Analysis", cv2.WINDOW_NORMAL)
            # 필요 시 특정 크기로 설정
            # cv2.resizeWindow("Face Analysis", 1280, 720)

            # 이미지 출력
            cv2.imshow("Face Analysis", dm.image_bgr)
            cv2.waitKey(0)
            cv2.destroyAllWindows()
