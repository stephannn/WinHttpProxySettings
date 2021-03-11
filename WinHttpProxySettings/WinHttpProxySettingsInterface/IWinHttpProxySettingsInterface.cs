using System;
using System.Collections.Generic;

namespace hanisch.WinHttpProxySettings
{

    public interface IWinHttpProxySettingsInterface
    {

        /// <summary>
        /// WinHttpProxyConfig objects that contains the current proxy settings depending on the target.
        /// </summary>
        /// <output>
        /// - Version: the version of the WinHttpSettings object
        /// - Counter: increased every time the object is modified
        /// - ConfigFlags: which settings to use
        /// - Proxy: the proxy to use, in the form 'name:port'
        /// - Bypass: a semicolon-separated list of host bypassing the proxy
        /// - AutoConfig: the address of a.Pac file.
        /// - ProxySettingsPerUser: true if the user settings are used.
        /// - Context: origin (LocalMachine, CurrentUser) of the record.
        /// </output>
        public class IWinHttpProxyConfig {
            public string AutoConfig { get; set; }
            public string Bypass { get; set; }
            public WinHttpFlags ConfigFlags { get; set; }
            public ContextObject Context { get; set; }
            public int Counter { get; set; }
            public string Proxy { get; set; }
            public bool ProxySettingsPerUser { get; set; }
            public int Version { get; set; }
        }

        /// <summary>
        /// The GetWinHttpProxySettings cmdlet outputs the configuration of the WinHTTP proxy.
        /// It detects whether the computer is setup for a per-machine or a per-user setup.
        /// </summary>
        /// <param name="contextObject">LocalMachine or CurrentUser. Without this parameter, the cmdlet checks the computer configuration to determine which one to output.</param>
        /// <returns></returns>
        public List<IWinHttpProxyConfig> GetWinHttpProxySettings(ContextObject[] contextObject);

        /// <summary>
        /// The SetWinHttpProxySettings cmdlet is a replacement for netsh winhttp set proxy, with the added
        /// option of setting the autoconfig filed(normally only set when copying the WinINet settings)
        /// </summary>
        /// <param name="contextObject">LocalMachine or CurrentUser. Without this parameter, the method checks the computer configuration to determine which one to modify.</param>
        /// <param name="configFlags">Can be one or a set from manual, autoconfig and detect.
        /// Manual: uses the Proxy field.
        /// Autoconfig: Uses the autoconfig field.
        /// AutoDetect: uses the proxy discovery protocol.</param>
        /// <param name="proxy">The proxy to set-up</param>
        /// <param name="bypass">A semicolon-separated list of hosts that will not use the proxy. You can use '<LOCAL>' for all local addresses.</param>
        /// <param name="autoConfig">The address of a PAC file.</param>
        /// <param name="version">The version of the WinHttpSettings record. Internal.</param>
        public void SetWinHttpProxySettings(ContextObject? contextObject, WinHttpFlags configFlags, string proxy = "", string bypass = "", string autoConfig = "", int version = 0x46);

    }
}
