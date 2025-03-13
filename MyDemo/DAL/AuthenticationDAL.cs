using Domain.Interface;
using Domain.Model;

namespace MyDemo.DAL
{
    public class AuthenticationDAL
    {
        public readonly IAuthentication _Iauthentication;
        public AuthenticationDAL(IAuthentication authentication)
        {
            _Iauthentication = authentication;
        }

        public async Task<Authentication> Login(AuthDetails authentication)
        {
           return await _Iauthentication.Login(authentication);
        }

        public async Task<(bool, string)> Register(Register register)
        {
            return await _Iauthentication.Register(register);
        }
    }
}
