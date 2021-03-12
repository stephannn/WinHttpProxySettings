# WinHttpProxySettings
WinHTTP Proxy Configuration in C#: netsh winhttp * proxy

# Intellectual Property
The intellectual property comes from Xavier Plantefève:
https://gist.github.com/XPlantefeve/a53a6af53b458188ee0766acc8508776

# Example
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
