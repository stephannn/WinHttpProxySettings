using System;

namespace hanisch.WinHttpProxySettings
{

        [Flags]
        public enum WinHttpFlags : short
        {
            None = 0,
            alwayson = 1,   // this flag is always on. it will be removed for display
            manual = 2,     // uses the 'proxy' field
            auto = 4,       // uses the 'autoconfig' field
            detect = 8      // uses proxy auto-discovering protocol
        }

}
