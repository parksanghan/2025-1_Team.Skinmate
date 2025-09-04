using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Model
{
    public class RegisterRequest
    {
        public required string UserId { get; set; }
        public required string Password { get; set; }
        public required string Name { get; set; }
    }
}
