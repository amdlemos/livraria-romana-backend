using System;
using System.Collections.Generic;
using System.Text;

namespace LivrariaRomana.Domain.DTO
{
    public class UserDTO
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string token { get; set; }
    }
}
