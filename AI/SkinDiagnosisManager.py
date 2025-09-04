# skin_diagnosis_manager.py
import torch
import cv2
import numpy as np
import os
from torchvision import models, transforms


class SkinDiagnosisManager:
    def __init__(self, class_model_dir, regression_model_dir):
        self.device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
        self.class_model_dir = class_model_dir
        self.regression_model_dir = regression_model_dir
        self.classification_classes =
         {1: 15, 2: 7, 3: 7, 5: 12, 7: 5, 8: 7}
        # 애는 idx 의  등급 
        self.regression_classes =
         {0: 1, 1: 2, 3: 1, 5: 3, 8: 2}
        # 애는 idx 의 출력의 개수 
        self.class_result_keys = {
            1: "forehead_wrinkle", 
            2: "frown_wrinkle",
            3: "eyes_wrinkle",
            8: "cheek_pore",
            5: "lips_dryness",
            7: "jaw_sagging",
        }
        self.region_to_classification_idx={
            
            "forehead": 1 ,
            "left_cheek": 8 ,
            "right_cheek": 8 ,
            "left_eyewrinkles":3  ,
            "right_eyewrinkles":3  ,
            "middle_eyebrows":2 ,
            "chin": 7,
            "lips":5, 
        }
        self.region_to_regression_idx = {
            "face_box": 0,
            "forehead": 1 ,
            "left_cheek": 5 ,
            "right_cheek": 5  ,
            "left_eyewrinkles": 3 ,
            "right_eyewrinkles": 3  ,
            "chin": 8,
            
        }
        self.regression_result_keys = {
            0: ["face"],
            1: ["forehead_moisture", "forehead_elasticity"],
            3: ["eyes_wrinkle"],
            5: ["cheek_moisture", "cheek_elasticity", "cheek_pore"],
            8: ["jaw_moisture", "jaw_elasticity"],
        }
    def preprocess_image(self, image_path):
        img = cv2.imread(image_path)
        img = cv2.resize(img, (256, 256))
        transform = transforms.Compose(
            [
                transforms.ToTensor(),
            ]
        )
        img = transform(img).unsqueeze(0).to(self.device)
        return img
    def preprocess_array(self, image_bgr_np):
        img = cv2.resize(image_bgr_np, (256, 256))
        transform = transforms.Compose([transforms.ToTensor()])
        img = transform(img).unsqueeze(0).to(self.device)
        return img
    def load_model(self, model_dir, area_idx, num_classes):
        checkpoint_path = os.path.join(model_dir, str(area_idx), "state_dict.bin")
        if not os.path.isfile(checkpoint_path):
            print(f"[오류] 모델 파일이 존재하지 않습니다: {checkpoint_path}")
            return None
        try:
            model = models.resnet50()
            model.fc = torch.nn.Linear(model.fc.in_features, num_classes)
            checkpoint = torch.load(checkpoint_path, map_location=self.device)
            output_dim = checkpoint["model_state"]["fc.weight"].shape[0]
            if output_dim != num_classes:
                print(
                    f"[오류] 모델 구조 불일치: 저장된 출력 수({output_dim}) != 기대 출력 수({num_classes})"
                )
                return None
            model.load_state_dict(checkpoint["model_state"])
            model = model.to(self.device)
            model.eval()
            return model
        except Exception as e:
            print(f"[오류] 모델 로딩 실패: {e}")
            return None

    def infer(self, model, img, is_classification):
        with torch.no_grad():
            output = model(img)
            if is_classification:
                probabilities = torch.softmax(output, dim=1)
                predicted_class = torch.argmax(probabilities, dim=1).item()
                return predicted_class
            else:
                output = torch.clamp(output, 0.02054755955934525, 0.98874594569206238)
                return output.squeeze().cpu().numpy()

    def diagnose(self, image_path):
        img = self.preprocess_image(image_path)
        class_results = {}
        regression_results = {}
#   self.classification_classes = {1: 15, 2: 7, 3: 7, 5: 12, 7: 5, 8: 7}
#         self.regression_classes = {0: 1, 1: 2, 3: 1, 5: 3, 8: 2}
        for idx, num_classes in self.classification_classes.items():
            model = self.load_model(self.class_model_dir, idx, num_classes)
            if model:
                result = self.infer(model, img, is_classification=True)
                key = self.class_result_keys.get(idx)
                if key:
                    class_results[key] = result

        for idx, num_classes in self.regression_classes.items():
            model = self.load_model(self.regression_model_dir, idx, num_classes)
            if model:
                result = self.infer(model, img, is_classification=False)
                keys = self.regression_result_keys.get(idx, [])

                if isinstance(result, np.ndarray):
                    if result.ndim == 0:
                        # 0차원 ndarray → 단일 값
                        if keys:
                            regression_results[keys[0]] = float(result.item())
                    else:
                        for i, key in enumerate(keys):
                            if i < result.size:
                                regression_results[key] = float(result[i])
                elif isinstance(result, (list, tuple)):
                    for i, key in enumerate(keys):
                        if i < len(result):
                            regression_results[key] = float(result[i])
                elif isinstance(result, (int, float, np.float32, np.float64)):
                    if keys:
                        regression_results[keys[0]] = float(result)
                else:
                    print(
                        f"[경고] 처리 불가능한 result 타입: {type(result)} / 값: {result}"
                    )

        return {"class": class_results, "regression": regression_results}
        #   self.classification_classes = 
        # {1: 15, 2: 7, 3: 7, 5: 12, 7: 5, 8: 7}
   
    def diagnose3(self, region_images:dict):
        class_results= {}
        regression_results = {}
        # 영역 , 이미지 class_result_keys
        # 분류 모델 순회회
        for region_name , img in region_images.items():
            cls_idx = self.region_to_classification_idx.get(region_name) # 영역별 idx 
            if cls_idx is None or img is None:
                continue
            model = self.load_model(self.class_model_dir, cls_idx, 
            self.classification_classes[cls_idx])
            if model:
                input_tensor=self.preprocess_array(img)
                result =  self.infer(model,input_tensor,is_classification =True )
                result_key  = self.class_result_keys.get(cls_idx)
                if result_key:
                    if result_key in class_results:
                        class_results[result_key]= (class_results[result_key] + result)/2 
                    else:
                        class_results[result_key] = result
    
        # 회귀 모델 순회
        for region_name , img  in region_images.items():
            reg_idx =  self.region_to_regression_idx.get(region_name)
            if reg_idx is None or img is None:
                raise RuntimeError("No img in region")
            model = self.load_model(self.regression_model_dir, reg_idx 
            , self.regression_classes[reg_idx])
            if model :
                input_tensor =  self.preprocess_array(img)
                result =  self.infer(model,input_tensor,is_classification=False)
                result_keys  =  self.regression_result_keys.get(reg_idx, [])
                if isinstance(result,np.ndarray):
                    if result.ndim == 0  and result_keys:
                        if result_keys[0] in regression_results:
                            regression_results[result_keys[0]] = (regression_results[result_keys[0]] + float(result.item()))/2
                        else:
                            regression_results[result_keys[0]]  = float(result.item())
                    else:
                        for i,key in enumerate(result_keys):
                            if i < result.size:
                                if key in regression_results:
                                    regression_results[key] = (regression_results[key] + float(result[i]))/2
                                else:
                                    regression_results[key] = float(result[i])
                elif isinstance(result ,(list , tuple)):
                    for i , key in enumerate(result_keys):
                        if i  < len(result):
                            if key in regression_results:
                                regression_results[key]=(regression_results[key] + float(result[i]))/2
                            else:
                                regression_results[key] =  float(result[i])
                elif isinstance(result , (int, float , np.float32, np.float64)):
                    if result_keys:
                        if result_keys[0] in regression_results:
                            regression_results[result_keys[0]] =  (regression_results[result_keys[0]] + float(result))/2
                        else:                        
                            regression_results[result_keys[0]]= float(result)
                else:
                    print(
                        f"[경고] 처리 불가능한 result 타입: {type(result)} / 값: {result}"
                    )
        return {"class": class_results, "regression": regression_results}

