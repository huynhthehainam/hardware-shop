﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareShop.Core.Services
{
    public interface ICurrentAccountService
    {
        bool IsSystemAdmin();
        int GetAccountId();
    }
}
