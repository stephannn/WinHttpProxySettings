using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using hanisch.WinHttpProxySettings;
using static hanisch.WinHttpProxySettings.IWinHttpProxySettingsInterface;

namespace hanisch.WinHttpProxySettings
{
    public partial class WinHttpProxySettings : IWinHttpProxySettingsInterface
    {

        // maybe to change: https://docs.microsoft.com/en-us/dotnet/api/system.collections.hashtable?view=net-5.0
        private Dictionary<string, Dictionary<string, string>> _regLocations = new Dictionary<string, Dictionary<string, string>>() {
            { "ProxySettingsPerUser", new Dictionary<String, String>() {
                { "Path", @"HKEY_LOCAL_MACHINE\Software\Policies\Microsoft\Windows\CurrentVersion\Internet Settings" },
                { "Name", "ProxySettingsPerUser" } }
            },
            { "CurrentUser", new Dictionary<String, String>() {
                { "Path", @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Connections" },
                { "Name", "DefaultConnectionSettings"} }
            },
            { "LocalMachine", new Dictionary<String, String>() {
                { "Path", @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Connections" },
                { "Name", "WinHttpSettings"} }
            },
            { "LocalMachineWoW64", new Dictionary<String, String>() {
                { "Path", @"HKEY_LOCAL_MACHINE\Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Internet Settings\Connections" },
                { "Name", "WinHttpSettings"} }
            }
        };

        public class WinHttpProxyConfig
        {
            public int Version { get; set; }
            public int Counter { get; set; }
            public string ConfigFlags { get; set; }
            public string Proxy { get; set; }
            public string Bypass { get; set; }
            public string AutoConfig { get; set; }
            public bool ProxySettingsPerUser { get; set; }
            public ContextObject Context { get; set; }
        }


        [SupportedOSPlatform("windows")]
        public List<IWinHttpProxyConfig> GetWinHttpProxySettings(ContextObject[] contextObject)
        {
            List<IWinHttpProxyConfig> whpcList = new List<IWinHttpProxyConfig>();

            foreach (ContextObject ctx in contextObject)
            {

                idx = -4;
                byte[] RawConfig;
                bool ProxySettingsPerUser = false;
                Dictionary<string, string> SettingsLocation;

                try
                {
                    SettingsLocation = _regLocations["ProxySettingsPerUser"];
                    if (Registry.GetValue(SettingsLocation["Path"], SettingsLocation["Name"], null) == null)
                    {
                        ProxySettingsPerUser = true;
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error ProxySettingsPerUser: " + ex);
                }

                try
                {
                    SettingsLocation = _regLocations[ctx.ToString()];
                    RawConfig = (byte[])Registry.GetValue(SettingsLocation["Path"], SettingsLocation["Name"], "");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error SettingsLocation: " + ex);
                    RawConfig = Encoding.ASCII.GetBytes("18, 00, 00, 00, 00, 00, 00, 00, 01, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00");
                }

                IWinHttpProxyConfig whpc = new IWinHttpProxyConfig();

                whpc.Version = RawConfig[_idx(ref idx)];
                whpc.Counter = RawConfig[_idx(ref idx)];
                whpc.ConfigFlags = ((WinHttpFlags)(RawConfig[_idx(ref idx)] - 1)); // we remove 1 because we don't want to display the "stuck bit"
                whpc.Proxy = _decodeString(_idx(ref idx), RawConfig);
                whpc.Bypass = _decodeString(_idx(ref idx), RawConfig);
                whpc.AutoConfig = _decodeString(_idx(ref idx), RawConfig);
                whpc.ProxySettingsPerUser = ProxySettingsPerUser;
                whpc.Context = ctx;

                whpcList.Add(whpc);

            }

            return whpcList;

        }

        // INTERNAL: decodes a byte array
        private string _decodeString(int start, byte[] byteArray, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.ASCII;
            }

            try
            {
                var strLen = byteArray[start];
                var str_ba = byteArray[(start + 4)..(start + 4 + strLen)];
                _idx(ref idx, strLen);
                return encoding.GetString(str_ba);
            }
            catch (IndexOutOfRangeException ex)
            {
                System.Diagnostics.Debug.WriteLine("_decodeString Error: " + ex.ToString());
                _idx(ref idx);
            }

            return "";
        }


        // INTERNAL: outputs current index value and increases it
        private int idx;
        private int _idx(ref int idx, int inc = 4)
        {
            int ind = idx;
            return idx += inc;

        }

        [SupportedOSPlatform("windows")]
        public void SetWinHttpProxySettings(ContextObject? contextObject, WinHttpFlags configFlags, string proxy, string bypass, string autoConfig, int version = 0x46)
        {
            bool ProxySettingsPerUser = false;
            Dictionary<string, string> SettingsLocation;
            try
            {
                SettingsLocation = _regLocations["ProxySettingsPerUser"];
                if (Registry.GetValue(SettingsLocation["Path"], SettingsLocation["Name"], null) == null)
                {
                    ProxySettingsPerUser = true;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error ProxySettingsPerUser: " + ex);
            }

            if (contextObject == null) {
                if (ProxySettingsPerUser) {
                    contextObject = ContextObject.CurrentUser;
                }
                else {
                    contextObject = ContextObject.LocalMachine;
                }
            }

            SettingsLocation = _regLocations[contextObject.ToString()];

            int counter = GetWinHttpProxySettings(new ContextObject[] { contextObject.Value }).Count;

            ASCIIEncoding ascii = new ASCIIEncoding();
            List<byte> Settings = new List<byte>();
            Settings.AddRange(new byte[] { (byte)(version), 00, 00, 00 });
            Settings.AddRange(new byte[] { (byte)(counter), 00, 00, 00 });
            Settings.AddRange(new byte[] { (byte)((WinHttpFlags)configFlags+1), 00, 00, 00 });
            Settings.AddRange(new byte[] { (byte)ascii.GetByteCount(proxy), 00, 00, 00 });
            Settings.AddRange(ascii.GetBytes(proxy));
            Settings.AddRange(new byte[] { (byte)ascii.GetByteCount(bypass), 00, 00, 00 });
            Settings.AddRange(ascii.GetBytes(bypass));
            Settings.AddRange(new byte[] { (byte)ascii.GetByteCount(autoConfig), 00, 00, 00 });
            Settings.AddRange(ascii.GetBytes(autoConfig));

            switch (version) {
                case 0x3c:
                    Settings.AddRange(Enumerable.Repeat<byte>(element: (byte)00, 28).ToArray());
                    break;
                case 0x46:
                    Settings.AddRange(Enumerable.Repeat<byte>(element: (byte)00, 32).ToArray());
                    break;
            }

            //SettingsLocation["Path"] = @"HKEY_CURRENT_USER\Software";
            try { 
                Registry.SetValue(SettingsLocation["Path"], SettingsLocation["Name"], Settings.ToArray(), Microsoft.Win32.RegistryValueKind.Binary);
            } catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Registry Error: " + ex.ToString());
            }
           
        }
    }
}
