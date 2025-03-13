using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Services
{
    public class Service
    {
        private readonly IDataProtector _protector;
        public Service(IDataProtectionProvider provider) { 
            _protector = provider.CreateProtector("MySecretKey");
        }

        public string EncryptData(string plainText)
        {
            return _protector.Protect(plainText);
        }

        public string DecryptData(string encryptedData)
        {
            return _protector.Unprotect(encryptedData);
        }
    }
}
