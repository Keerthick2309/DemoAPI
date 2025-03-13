using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
    public interface IAuthentication :IDisposable
    {
       Task<Authentication> Login(AuthDetails authentication);
        Task<(bool, string)> Register(Register register);
    }
}
