using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBKR_Trader_REST
{
    public class AuthStatus
    {
        public bool authenticated { get; set; }
        public bool competing { get; set; }
        public bool connected { get; set; }
        public string MAC { get; set; }
        
    }

}
