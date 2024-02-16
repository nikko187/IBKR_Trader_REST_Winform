using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBKR_Trader_REST
{

    public class Tickle
    {
        public string session { get; set; }
        public int ssoExpires { get; set; }
        public bool collission { get; set; }
        public int userId { get; set; }
        public Hmds hmds { get; set; }
        public Iserver iserver { get; set; }
    }

    public class Hmds
    {
        public string error { get; set; }
    }

    public class Iserver
    {
        public Authstatus authStatus { get; set; }
    }

    public class Authstatus
    {
        public bool authenticated { get; set; }
        public bool competing { get; set; }
        public bool connected { get; set; }
        public string message { get; set; }
        public string MAC { get; set; }
        public Serverinfo serverInfo { get; set; }
    }

    public class Serverinfo
    {
        public object serverName { get; set; }
        public object serverVersion { get; set; }
    }

}
