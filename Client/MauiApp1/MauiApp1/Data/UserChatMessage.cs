using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Data
{
    //유저간 채팅 메시지 
    public class UserChatMessage
    {
        public string? SenderId { get; set; }  // 보낸 유저
        public string? TargetId { get; set; } // 받는 유저 
        public string? Message { get; set; } // 메시지 
        public string? ImageUrl {  get; set; }   // 
    }
}
