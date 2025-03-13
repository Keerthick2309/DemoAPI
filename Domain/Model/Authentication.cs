using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Authentication
    {
        public string? UserName { get; set; }
        public string? Message { get; set; }
        public int? UserId { get; set; }
        public bool? Valid { get; set; }
        public string? JWTToken { get; set; }
    }
    public class AuthDetails
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
    public class Register 
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
