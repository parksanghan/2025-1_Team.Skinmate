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


class DetectManager:
    def __init__(self):
        self.mp_face_mesh = mp.solutions.face_mesh
        self.mp_face_detection = mp.solutions.face_detection

    def __call__(self, image_bgr):
        self.image_bgr = image_bgr
        self.h, self.w, _ = image_bgr.shape
        self.image_rgb = cv2.cvtColor(image_bgr, cv2.COLOR_BGR2RGB)
        with self.mp_face_mesh.FaceMesh(
            static_image_mode=True,  # 사진
            max_num_faces=1,
            refine_landmarks=True,
            min_detection_confidence=0.5,
        ) as face_mesh:  # 정밀 좌표영역임
            self.results = face_mesh.process(self.image_rgb)
        with self.mp_face_detection.FaceDetection(
            model_selection=0, min_detection_confidence=0.5
        ) as face_detection:  # 얼굴 위치 박스영역임
            self.face_box_results = face_detection.process(self.image_rgb)
        if self.results.multi_face_landmarks:
            self.face_landmarks = self.results.multi_face_landmarks[0]

    def get_cropped_all_img(self):
        try:
            regions = {
                "face_box": self.crop_detected_face_box(),
                "forehead": self.crop_detected_forehead(),
                "left_cheek": self.crop_detected_left_cheek(),
                "right_cheek": self.crop_detected_right_cheek(),
                "left_eyewrinkles": self.crop_detected_left_eyewrinkles(),
                "right_eyewrinkles": self.crop_detected_right_eyewrinkles(),
                "middle_eyebrows": self.crop_detected_middle_eyebrows(),
                "chin": self.crop_detected_chin(),
                "lips": self.crop_detected_lips(),
                # "left_eyewrinkles": cv2.flip(self.crop_detected_left_eyewrinkles(), 1),
            }
            return regions
        except Exception as e:
            raise RuntimeError("get all img box error")

    # face box 처리
    def crop_detected_face_box(self):
        try:
            if not self.face_box_results or not self.face_box_results.detections:
                raise ValueError("Detection 에러")
            det = self.face_box_results.detections[0]
            bbox = det.location_data.relative_bounding_box

            x = int((bbox.xmin - MARGIN_LEFT_RATIO) * self.w)
            y = int((bbox.ymin - MARGIN_TOP_RATIO) * self.h)
            box_w = int((bbox.width + MARGIN_LEFT_RATIO + MARGIN_RIGHT_RATIO) * self.w)
            box_h = int((bbox.height + MARGIN_TOP_RATIO + MARGIN_BOTTOM_RATIO) * self.h)

            x = max(x, 0)
            y = max(y, 0)
            x2 = min(x + box_w, self.w)
            y2 = min(y + box_h, self.h)

            return self.image_bgr[y:y2, x:x2]
        except Exception as e:
            raise RuntimeError("crop error")

    def crop_detected_left_eyewrinkles(self):
        try:
            if not self.face_landmarks:
                raise ValueError("Detection 에러")
            BOX_WIDTH = int(self.w * 0.02)
            BOX_HEIGHT = int(self.h * 0.035)
            LEFT_EYE_TAIL_IDX = 130
            lx = int(self.face_landmarks.landmark[LEFT_EYE_TAIL_IDX].x * self.w)
            ly = int(self.face_landmarks.landmark[LEFT_EYE_TAIL_IDX].y * self.h)
            x1 = max(lx - BOX_WIDTH, 0)
            y1 = max(ly - BOX_HEIGHT // 2, 0)
            x2 = min(lx, self.w)
            y2 = min(ly + BOX_HEIGHT // 2, self.h)

            return self.image_bgr[y1:y2, x1:x2]

        except Exception as e:
            raise RuntimeError("crop error")

    def crop_detected_right_eyewrinkles(self):
        try:
            if not self.face_landmarks:
                raise ValueError("Detection 에러")
            BOX_WIDTH = int(self.w * 0.02)
            BOX_HEIGHT = int(self.h * 0.035)
            RIGHT_EYE_TAIL_IDX = 263
            lx = int(self.face_landmarks.landmark[RIGHT_EYE_TAIL_IDX].x * self.w)
            ly = int(self.face_landmarks.landmark[RIGHT_EYE_TAIL_IDX].y * self.h)
            x1 = max(lx, 0)
            y1 = max(ly - BOX_HEIGHT // 2, 0)
            x2 = min(lx + BOX_WIDTH, self.w)
            y2 = min(ly + BOX_HEIGHT // 2, self.h)

            return self.image_bgr[y1:y2, x1:x2]

        except Exception as e:
            raise RuntimeError("crop error")

    def crop_detected_chin(self):
        try:
            if not self.face_landmarks:
                raise ValueError("Detection 에러")
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

            x1 = max(lx, 0)
            x2 = min(rx, self.w)
            y1 = max(lip_y, 0)
            y2 = min(chin_y, self.h)

            return self.image_bgr[y1:y2, x1:x2]

        except Exception as e:
            raise RuntimeError("crop error")

    def crop_detected_forehead(self):
        try:
            if not self.face_landmarks:
                raise ValueError("Detection 에러")

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
            x_min = max(min(p[0] for p in pts), 0)
            y_min = max(min(p[1] for p in pts), 0)
            x_max = min(max(p[0] for p in pts), self.w)
            y_max = min(max(p[1] for p in pts), self.h)

            return self.image_bgr[y_min:y_max, x_min:x_max]

        except Exception as e:
            raise RuntimeError("crop error (forehead)")

    def crop_detected_lips(self):
        try:
            if not self.face_landmarks:
                raise ValueError("Detection 에러")

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

            x_min = max(min(xs), 0)
            x_max = min(max(xs), self.w)
            y_min = max(min(ys), 0)
            y_max = min(max(ys), self.h)

            return self.image_bgr[y_min:y_max, x_min:x_max]

        except Exception as e:
            raise RuntimeError("crop error (lips)")

    def crop_detected_middle_eyebrows(self):
        try:
            if not self.face_landmarks:
                raise ValueError("Detection 에러")

            GLABELLA_IDX = [107, 336]
            BOX_HEIGHT = int(self.h * 0.035)  # 정적 20픽셀 대신 이미지 비율 기반

            pts = [
                (
                    int(self.face_landmarks.landmark[i].x * self.w),
                    int(self.face_landmarks.landmark[i].y * self.h),
                )
                for i in GLABELLA_IDX
            ]

            x_min = max(min(p[0] for p in pts), 0)
            x_max = min(max(p[0] for p in pts), self.w)

            y_base = min(p[1] for p in pts)
            y_min = max(y_base, 0)
            y_max = min(y_base + BOX_HEIGHT, self.h)

            return self.image_bgr[y_min:y_max, x_min:x_max]

        except Exception as e:
            raise RuntimeError("crop error (middle eyebrows)")

    def crop_detected_left_cheek(self):
        try:
            if not self.face_landmarks:
                raise ValueError("Detection 에러")

            LEFT_CHEEK_IDX = [120, 234, 203, 132]
            points = [
                (
                    int(self.face_landmarks.landmark[idx].x * self.w),
                    int(self.face_landmarks.landmark[idx].y * self.h),
                )
                for idx in LEFT_CHEEK_IDX
            ]

            x_coords = [p[0] for p in points]
            y_coords = [p[1] for p in points]

            x_min = max(min(x_coords), 0)
            x_max = min(max(x_coords), self.w)
            y_min = max(min(y_coords), 0)
            y_max = min(max(y_coords), self.h)

            return self.image_bgr[y_min:y_max, x_min:x_max]

        except Exception as e:
            raise RuntimeError("crop error (left cheek)")

    def crop_detected_right_cheek(self):
        try:
            if not self.face_landmarks:
                raise ValueError("Detection 에러")

            RIGHT_CHEEK_IDX = [348, 454, 423, 376]
            points = [
                (
                    int(self.face_landmarks.landmark[idx].x * self.w),
                    int(self.face_landmarks.landmark[idx].y * self.h),
                )
                for idx in RIGHT_CHEEK_IDX
            ]

            x_coords = [p[0] for p in points]
            y_coords = [p[1] for p in points]

            x_min = max(min(x_coords), 0)
            x_max = min(max(x_coords), self.w)
            y_min = max(min(y_coords), 0)
            y_max = min(max(y_coords), self.h)

            return self.image_bgr[y_min:y_max, x_min:x_max]

        except Exception as e:
            raise RuntimeError("crop error (right cheek)")


# import cv2
# import numpy as np
# from pathlib import Path

# image_path = Path(r"C:\Users\박상한\Pictures\Camera Roll\WIN_20250528_10_34_40_Pro.jpg")

# if not image_path.exists():
#     print(f"[경고] 파일이 존재하지 않습니다:\n{image_path}")
# else:
#     # 한글 경로 대응 이미지 로드
#     image = cv2.imdecode(np.fromfile(str(image_path), dtype=np.uint8), cv2.IMREAD_COLOR)

#     if image is None:
#         print(f"[오류] 이미지를 읽지 못했습니다. 파일 손상 가능성:\n{image_path}")
#     else:
#         dm = DetectManager()
#         dm(image.copy())  # __call__ 호출

#         regions = {
#             "face_box": dm.crop_detected_face_box(),
#             "forehead": dm.crop_detected_forehead(),
#             "left_cheek": dm.crop_detected_left_cheek(),
#             "right_cheek": dm.crop_detected_right_cheek(),
#             "left_eyewrinkles": dm.crop_detected_left_eyewrinkles(),
#             "right_eyewrinkles": dm.crop_detected_right_eyewrinkles(),
#             "middle_eyebrows": dm.crop_detected_middle_eyebrows(),
#             "chin": dm.crop_detected_chin(),
#             "lips": dm.crop_detected_lips(),
#         }

#         save_dir = Path("saved_regions")
#         save_dir.mkdir(exist_ok=True)

#         for name, img in regions.items():
#             save_path = save_dir / f"{name}.jpg"
#             cv2.imencode(".jpg", img)[1].tofile(str(save_path))  # 한글 경로 대응 저장

#         print("✅ 9개 영역 저장 완료:", save_dir.resolve())
