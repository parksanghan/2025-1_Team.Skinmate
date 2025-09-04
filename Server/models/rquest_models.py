from pydantic import BaseModel

from typing import List


class LoginRequest(BaseModel):
    UserId: str
    Password: str


class RegisterRequest(BaseModel):
    UserId: str
    Password: str
    Name: str


class UserSetingPayload(BaseModel):
    interests: List[str]
    gender: str
    age: str
