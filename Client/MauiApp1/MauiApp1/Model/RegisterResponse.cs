using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Model
{
    public class RegisterResponse
    {
        public required string Message { get; set; }    // 서버 메시지 ("로그인 성공", "비밀번호 틀림" 등)
        public required bool Success { get; set; }      // 로그인 성공 여부
    }
}
