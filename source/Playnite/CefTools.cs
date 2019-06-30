﻿using CefSharp;
using CefSharp.Wpf;
using Playnite.Common;
using Playnite.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playnite
{
    public class CefTools
    {
        public static bool IsInitialized { get; private set; }

        public static void ConfigureCef()
        {
            string proxyAddress = PlayniteApplication.Current.AppSettings.ProxyAddress;

            FileSystem.CreateDirectory(PlaynitePaths.BrowserCachePath);
            var settings = new CefSettings();
            settings.WindowlessRenderingEnabled = true;
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
            settings.CachePath = PlaynitePaths.BrowserCachePath;
            settings.PersistSessionCookies = true;
            if (!string.IsNullOrEmpty(proxyAddress))
            {
                settings.CefCommandLineArgs.Add("proxy-server", proxyAddress);
            }
            settings.LogFile = Path.Combine(PlaynitePaths.ConfigRootPath, "cef.log");
            IsInitialized = Cef.Initialize(settings);
        }

        public static void Shutdown()
        {
            Cef.Shutdown();
            IsInitialized = false;
        }
    }
}
