using System;
using System.Collections.Generic;
using hanisch.WinHttpProxySettings;

namespace ConsoleWinHttpProxySettings
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IWinHttpProxySettingsInterface wps = new WinHttpProxySettings();
            List<IWinHttpProxySettingsInterface.IWinHttpProxyConfig> whpcList = wps.GetWinHttpProxySettings(new ContextObject[] { ContextObject.LocalMachine });


            foreach (IWinHttpProxySettingsInterface.IWinHttpProxyConfig whpc in whpcList)
            {
                Console.WriteLine("Version: " + whpc.Version);
                Console.WriteLine("Counter: " + whpc.Counter);
                Console.WriteLine("ConfigFlags: " + whpc.ConfigFlags);
                Console.WriteLine("Proxy: " + whpc.Proxy);
                Console.WriteLine("Bypass: " + whpc.Bypass);
                Console.WriteLine("AutoConfig: " + whpc.AutoConfig);
                Console.WriteLine("ProxzSettingPerUser: " + whpc.ProxySettingsPerUser);
                Console.WriteLine("Context: " + whpc.Context);
            }

            wps.SetWinHttpProxySettings(ContextObject.LocalMachine, WinHttpFlags.auto, "10.0.0.8");

            #if !DEBUG
                Console.ReadKey();
            #endif
        }
    }
}
