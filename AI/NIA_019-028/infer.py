import torch
import cv2
import numpy as np
from torchvision import models, transforms
import argparse
import os
from model import resume_checkpoint

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
# 모델과 클래스 매핑
classification_classes = {1: 15, 2: 7, 3: 7, 5: 12, 7: 5, 8: 7}  # 분류 모델 클래스 개수
regression_classes = {0: 1, 1: 2, 3: 1, 5: 3, 8: 2}               # 회귀 모델 클래스 개수

# 영역 명칭
area_naming = {
    0: "전체 얼굴",
    1: "이마",
    2: "미간",
    3: "양측 눈가",
    5: "입술",
    7: "턱선 처짐",
    8: "양측 볼"
}

def parse_args():
    parser = argparse.ArgumentParser()
    parser.add_argument("--image_path", type=str, required=True, help="Path to input image")
    parser.add_argument("--class_model_dir", type=str, required=True, help="Directory of classification model checkpoints")
    parser.add_argument("--regression_model_dir", type=str, required=True, help="Directory of regression model checkpoints")
    return parser.parse_args()


# 이미지 전처리
def preprocess_image(image_path):
    img = cv2.imread(image_path)
    img = cv2.resize(img, (256, 256))
    transform = transforms.Compose([
        transforms.ToTensor(),
    ])
    img = transform(img).unsqueeze(0).to(device)
    return img

def load_model(model_dir, area_idx, num_classes):
    checkpoint_path = os.path.join(model_dir, str(area_idx), "state_dict.bin")
    if not os.path.isfile(checkpoint_path):
        print(f"[오류] 모델 파일이 존재하지 않습니다: {checkpoint_path}")
        return None
    
    try:
        # 모델 구조 동적으로 설정
        model = models.resnet50()
        model.fc = torch.nn.Linear(model.fc.in_features, num_classes)
        checkpoint = torch.load(checkpoint_path, map_location=device)

        # fc 레이어의 크기를 동적으로 설정
        output_dim = checkpoint['model_state']['fc.weight'].shape[0]
        if output_dim != num_classes:
            print(f"[오류] 모델 구조 불일치: 저장된 모델 출력 수({output_dim})와 기대 출력 수({num_classes})가 다릅니다.")
            return None
        
        model.load_state_dict(checkpoint['model_state'])
        model = model.to(device)
        model.eval()
        return model
    except Exception as e:
        print(f"[오류] 모델 로딩 실패: {e}")
        return None


# 추론 함수
def infer(model, img, is_classification):
    with torch.no_grad():
        output = model(img)
        if is_classification:
            probabilities = torch.softmax(output, dim=1)
            predicted_class = torch.argmax(probabilities, dim=1).item()
            confidence = torch.max(probabilities).item()
            return predicted_class, confidence
        else:
            output = torch.clamp(output, 0.0, 1.0)  # 여기 추가
            value = output.squeeze().cpu().numpy()
            return torch.clamp( value,0.0,1.0), None

def print_results(area_name, result, confidence, is_classification):
    if is_classification:
        print(f"[{area_name}] 예측 등급: {result}, 신뢰도: {confidence:.4f}")
    else:
        print(f"[{area_name}] 예측 값: {result}")

# 메인 함수
def main():
    args = parse_args()
    img = preprocess_image(args.image_path)

    print("\n[추론 결과 - 분류 모델]")
    print("=" * 50)

    # 6개 분류 모델 실행
    for idx, num_classes in classification_classes.items():
        try:
            model = load_model(args.class_model_dir, idx, num_classes)
            if model:
                result, confidence = infer(model, img, is_classification=True)
                print_results(area_naming[idx], result, confidence, is_classification=True)
        except Exception as e:
            print(f"[{area_naming.get(idx, 'Unknown')}] 모델 불러오기 실패 또는 추론 오류: {e}")

    print("\n[추론 결과 - 회귀 모델]")
    print("=" * 50)

    # 5개 회귀 모델 실행
    for idx, num_classes in regression_classes.items():
        try:
            model = load_model(args.regression_model_dir, idx, num_classes)
            if model:
                result, _ = infer(model, img, is_classification=False)
                print_results(area_naming[idx], result, None, is_classification=False)
        except Exception as e:
            print(f"[{area_naming.get(idx, 'Unknown')}] 모델 불러오기 실패 또는 추론 오류: {e}")

if __name__ == "__main__":
    main()
