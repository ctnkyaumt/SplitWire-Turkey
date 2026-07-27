using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;

namespace SplitWireTurkey.ProxyTest
{
    /// <summary>
    /// ByeByeDPI (romanvht) proxy testi ayarları. Registry'de saklanır.
    /// </summary>
    public class ProxyTestSettings
    {
        private const string RegPath = @"Software\SplitWire-Turkey\ProxyTest";

        /// <summary>Her strateji öncesi/sonrası bekleme (0-10, x500 ms).</summary>
        public int Delay { get; set; } = 1;

        /// <summary>Site başına istek sayısı (1-20).</summary>
        public int Requests { get; set; } = 1;

        /// <summary>Eş zamanlı istek limiti (1-50).</summary>
        public int Concurrency { get; set; } = 20;

        /// <summary>İstek zaman aşımı, saniye (1-15).</summary>
        public int Timeout { get; set; } = 5;

        /// <summary>Stratejilerdeki {sni} yer tutucusu bu değerle değiştirilir.</summary>
        public string Sni { get; set; } = "google.com";

        /// <summary>Test sırasında ciadpi'nin dinleyeceği yerel SOCKS5 portu.</summary>
        public int ProxyPort { get; set; } = 10800;

        /// <summary>Hazır strateji listesi yerine kullanıcının kendi listesi kullanılsın mı?</summary>
        public bool UseCustomStrategies { get; set; } = false;

        /// <summary>Kullanıcı stratejileri (satır başına bir komut).</summary>
        public string CustomStrategies { get; set; } = string.Empty;

        /// <summary>Aktif site listeleri (Resources/byedpi/proxytest/sites içindeki dosya adları).</summary>
        public List<string> ActiveSiteLists { get; set; } = new List<string> { "turkiye" };

        public static ProxyTestSettings Load()
        {
            var s = new ProxyTestSettings();
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegPath);
                if (key == null) return s;

                s.Delay = ReadInt(key, nameof(Delay), s.Delay, 0, 10);
                s.Requests = ReadInt(key, nameof(Requests), s.Requests, 1, 20);
                s.Concurrency = ReadInt(key, nameof(Concurrency), s.Concurrency, 1, 50);
                s.Timeout = ReadInt(key, nameof(Timeout), s.Timeout, 1, 15);
                s.ProxyPort = ReadInt(key, nameof(ProxyPort), s.ProxyPort, 1024, 65535);
                s.Sni = key.GetValue(nameof(Sni)) as string ?? s.Sni;
                s.UseCustomStrategies = ReadInt(key, nameof(UseCustomStrategies), 0, 0, 1) == 1;
                s.CustomStrategies = key.GetValue(nameof(CustomStrategies)) as string ?? string.Empty;

                var lists = key.GetValue(nameof(ActiveSiteLists)) as string;
                if (lists != null)
                {
                    // Boş string "hiçbir liste seçili değil" anlamına gelir, varsayılana dönme.
                    s.ActiveSiteLists = lists
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => x.Length > 0)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Proxy testi ayarları okunamadı: {ex.Message}");
            }
            return s;
        }

        public void Save()
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(RegPath);
                if (key == null) return;

                key.SetValue(nameof(Delay), Delay, RegistryValueKind.DWord);
                key.SetValue(nameof(Requests), Requests, RegistryValueKind.DWord);
                key.SetValue(nameof(Concurrency), Concurrency, RegistryValueKind.DWord);
                key.SetValue(nameof(Timeout), Timeout, RegistryValueKind.DWord);
                key.SetValue(nameof(ProxyPort), ProxyPort, RegistryValueKind.DWord);
                key.SetValue(nameof(Sni), Sni ?? string.Empty, RegistryValueKind.String);
                key.SetValue(nameof(UseCustomStrategies), UseCustomStrategies ? 1 : 0, RegistryValueKind.DWord);
                key.SetValue(nameof(CustomStrategies), CustomStrategies ?? string.Empty, RegistryValueKind.String);
                key.SetValue(nameof(ActiveSiteLists), string.Join(",", ActiveSiteLists ?? new List<string>()), RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Proxy testi ayarları kaydedilemedi: {ex.Message}");
            }
        }

        private static int ReadInt(RegistryKey key, string name, int fallback, int min, int max)
        {
            var value = key.GetValue(name);
            if (value is int i) return Math.Clamp(i, min, max);
            return fallback;
        }
    }
}
