from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.responses import JSONResponse
from SkinDiagnosisManager import SkinDiagnosisManager
import shutil
import os
import uuid
import uvicorn
from PIL import Image
import numpy as np
import cv2
app = FastAPI()

# 모델 관리자 인스턴스 초기화
diagnosis_manager = SkinDiagnosisManager(
    class_model_dir=r"S:\[Education]\Capstone1\NIA_019-028\checkpoint\class\testing\250414_2014",
    regression_model_dir=r"S:\[Education]\Capstone1\NIA_019-028\checkpoint\regression\testing\250414_2014"
)
@app.post("/diagnose")
async def diognose2(files: List[UploadFile]=  file(...)):
    if image.content_type not in ["image/jpeg", "image/png"]:
        raise HTTPException(status_code=400, detail="지원되지 않는 이미지 형식입니다 (jpeg 또는 png)")

    region_images   = {}
    for file in files:
        contents =  await file.read()
        img_array = np.frombuffer(contents, np.uint8)
        img  = cv2.imdecode(img_array, cv2.IMREAD_COLOR)
        key = file.filename.split(".").[0]
        region_images[key]= img
 
    results =diagnosis_manager.diagnose3(region_images)

@app.post("/diagnose")
async def diagnose_skin(image: UploadFile = File(...)):
    # 유효한 이미지 MIME 타입 검사
    if image.content_type not in ["image/jpeg", "image/png"]:
        raise HTTPException(status_code=400, detail="지원되지 않는 이미지 형식입니다 (jpeg 또는 png)")

    # 고유한 임시 파일명 생성
    temp_filename = f"temp_{uuid.uuid4().hex}.{image.filename.split('.')[-1]}"
    
    try:
        # 이미지 파일 저장
        with open(temp_filename, "wb") as buffer:
            shutil.copyfileobj(image.file, buffer)

        # 추론 수행
        result = diagnosis_manager.diagnose(temp_filename)

        # 결과 반환
        return JSONResponse(content=result)

    except Exception as e:
        print(e + "\n" + e.__traceback__)
        raise HTTPException(status_code=500, detail=f"서버 내부 오류: {str(e)}")

    finally:
        # 파일 삭제 (예외 여부와 무관하게 실행됨)
        if os.path.exists(temp_filename):
            os.remove(temp_filename)

@app.get("/")
def read_root():
    return "SkinMate Server on live"

if __name__ == "__main__":
    uvicorn.run("main:app", host="10.101.196.155", port=5000)
    # uvicorn.run("main:app", host="10.101.140.1", port=5000)