using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBKR_Trader_REST
{
    public class Validate
    {
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public bool RESULT { get; set; }
        public long AUTH_TIME { get; set; }
        public bool SF_ENABLED { get; set; }
        public bool IS_FREE_TRIAL { get; set; }
        public string CREDENTIAL { get; set; }
        public string IP { get; set; }
        public int EXPIRES { get; set; }
        public string QUALIFIED_FOR_MOBILE_AUTH { get; set; }
        public string LANDING_APP { get; set; }
        public bool IS_MASTER { get; set; }
        public long lastAccessed { get; set; }
        public Features features { get; set; }
        public string region { get; set; }
    }

    public class Features
    {
        public string env { get; set; }
        public bool wlms { get; set; }
        public bool realtime { get; set; }
        public bool bond { get; set; }
        public bool optionChains { get; set; }
        public bool calendar { get; set; }
        public bool newMf { get; set; }
    }

}
