using Domain.Interface;
using Domain.Model;
using Entity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Repository.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class AuthenticationRepo : IAuthentication
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly nEdit_DEVContext _DEVContext;
        private readonly JWTToken _tokenService;
        private readonly Service _service;
        private bool _disposed = false;
        public AuthenticationRepo(nEdit_DEVContext DEVContext, Service service, JWTToken tokenService, IHttpContextAccessor httpContextAccessor)
        {
            _DEVContext = DEVContext;
            _service = service;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _DEVContext.Dispose();
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<Authentication> Login(AuthDetails authentication)
        {
            try
            {
                var loginDetails = await _DEVContext.TblLogins.Where(x => x.LoginName == authentication.UserName).FirstOrDefaultAsync();
                if (loginDetails != null)
                {
                    var decryptPass = _service.DecryptData(loginDetails.LoginPass);
                    if (authentication.Password == decryptPass)
                    {
                        var token = _tokenService.GenerateToken(authentication.UserName!);
                        _httpContextAccessor.HttpContext.Response.Cookies.Append($"AuthToken_{loginDetails.LoginId}", token, new CookieOptions
                        {
                            HttpOnly = true,  // Prevents JavaScript access
                            Secure = true,   // Requires HTTPS
                            SameSite = SameSiteMode.None, // Prevents CSRF
                            Expires = DateTime.UtcNow.AddMinutes(60)
                        });
                        return new Authentication
                        {
                            UserName = loginDetails.LoginName,
                            Valid = true,
                            UserId = loginDetails.LoginId,
                            JWTToken = token
                        };
                    }
                    return new Authentication
                    {
                        Valid = false,
                        Message = "Wrong Password"
                    };
                }
                return new Authentication
                {
                    Valid = false,
                    Message = "Register"
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool, string)> Register(Register register)
        {
            try
            {
                var registerCheck = await _DEVContext.TblLogins.Where(x => x.LoginName == register.UserName).FirstOrDefaultAsync();
                if (registerCheck != null)
                {
                    return (false, "User is already registered!");
                }
                else
                {
                    var registerUser = new TblLogin
                    {
                        LoginName = register.UserName,
                        LoginPass = _service.EncryptData(register.Password)
                    };
                    _DEVContext.TblLogins.Add(registerUser);
                    _DEVContext.SaveChanges();
                    return (true, "User is registered! Please login");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
