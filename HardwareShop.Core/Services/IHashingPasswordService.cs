using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Services
{
    public interface IHashingPasswordService
    {
        String Hash(String text, Int32 iterations);
        string Hash(String text);
        Boolean IsHashedPasswordSupported(String text);
        Boolean Verify(String password, String hashedPassword);
    }
}
