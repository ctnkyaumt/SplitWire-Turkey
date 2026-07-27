using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SplitWireTurkey.ProxyTest
{
    /// <summary>
    /// Strateji listesini sırayla ciadpi üzerinde çalıştırıp site erişimini ölçer.
    /// ByeByeDPI (romanvht) TestActivity akışının .NET karşılığı.
    /// </summary>
    public class ProxyTestRunner
    {
        private readonly ProxyTestSettings _settings;

        public ProxyTestRunner(ProxyTestSettings settings)
        {
            _settings = settings;
        }

        public static string ProxyTestResourcePath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "byedpi", "proxytest");

        public static string CiadpiPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "res", "byedpi", "ciadpi.exe");

        public static string ResultsPath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SplitWire-Turkey", "proxy_test_results.json");

        /// <summary>Kullanılabilir site listelerinin adları (dosya adı, uzantısız).</summary>
        public static List<string> GetAvailableSiteLists()
        {
            try
            {
                var dir = Path.Combine(ProxyTestResourcePath, "sites");
                if (!Directory.Exists(dir)) return new List<string>();
                return Directory.GetFiles(dir, "*.sites")
                    .Select(Path.GetFileNameWithoutExtension)
                    .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ProxyTest] Site listeleri okunamadı: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>Aktif listelerdeki alan adları (tekrarsız).</summary>
        public List<string> LoadSites()
        {
            var sites = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var listName in _settings.ActiveSiteLists ?? new List<string>())
            {
                var path = Path.Combine(ProxyTestResourcePath, "sites", listName + ".sites");
                if (!File.Exists(path)) continue;

                foreach (var raw in File.ReadAllLines(path))
                {
                    var line = raw.Trim();
                    if (line.Length == 0 || line.StartsWith("#")) continue;
                    if (seen.Add(line)) sites.Add(line);
                }
            }

            return sites;
        }

        /// <summary>Test edilecek strateji komutları. {sni} yer tutucusu ayarlardaki değerle değiştirilir.</summary>
        public List<string> LoadStrategies()
        {
            string content;

            if (_settings.UseCustomStrategies)
            {
                content = _settings.CustomStrategies ?? string.Empty;
            }
            else
            {
                var path = Path.Combine(ProxyTestResourcePath, "strategies.list");
                content = File.Exists(path) ? File.ReadAllText(path) : string.Empty;
            }

            var sni = string.IsNullOrWhiteSpace(_settings.Sni) ? "google.com" : _settings.Sni.Trim();

            return content
                .Replace("{sni}", "\"" + sni + "\"")
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0 && !x.StartsWith("#"))
                .ToList();
        }

        /// <summary>
        /// Stratejileri sırayla test eder. Her strateji için ciadpi ayrı bir portta başlatılır,
        /// siteler test edilir ve süreç kapatılır.
        /// </summary>
        public async Task RunAsync(
            IList<StrategyResult> strategies,
            IList<string> sites,
            Action<int, int> onStrategyStarting,
            Action<int, string, int, int> onSiteChecked,
            Action<int, bool> onStrategyCompleted,
            CancellationToken ct)
        {
            var delayMs = _settings.Delay * 500;

            for (var i = 0; i < strategies.Count; i++)
            {
                ct.ThrowIfCancellationRequested();

                var strategy = strategies[i];
                // Model güncellemeleri UI iş parçacığında yapılsın diye geri çağrıya bırakılır.
                onStrategyStarting?.Invoke(i, sites.Count * _settings.Requests);

                Process process = null;
                var started = false;
                try
                {
                    process = StartCiadpi(strategy.Command);
                    started = process != null && await WaitForProxyAsync(process, ct).ConfigureAwait(false);

                    // Proxy ayağa kalkmadıysa (bu ciadpi sürümünün desteklemediği bir seçenek vb.)
                    // strateji test edilmez; 0% yerine "n/a" olarak raporlanır.
                    if (started)
                    {
                        if (delayMs > 0) await Task.Delay(delayMs, ct).ConfigureAwait(false);

                        using var checker = new SiteChecker("127.0.0.1", _settings.ProxyPort, _settings.Timeout);
                        var index = i;
                        await checker.CheckSitesAsync(
                            sites,
                            _settings.Requests,
                            _settings.Concurrency,
                            (site, success, total) => onSiteChecked?.Invoke(index, site, success, total),
                            ct).ConfigureAwait(false);
                    }
                }
                finally
                {
                    KillProcess(process);
                }

                onStrategyCompleted?.Invoke(i, started);

                if (delayMs > 0) await Task.Delay(delayMs, ct).ConfigureAwait(false);
            }
        }

        private Process StartCiadpi(string arguments)
        {
            try
            {
                if (!File.Exists(CiadpiPath))
                {
                    Debug.WriteLine($"[ProxyTest] ciadpi.exe bulunamadı: {CiadpiPath}");
                    return null;
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = CiadpiPath,
                    Arguments = $"-i 127.0.0.1 -p {_settings.ProxyPort} {arguments}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.GetDirectoryName(CiadpiPath)
                };

                var process = Process.Start(startInfo);
                // Çıktıyı tüket, aksi halde boru dolunca ciadpi bloklanır.
                process?.BeginOutputReadLine();
                process?.BeginErrorReadLine();
                return process;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ProxyTest] ciadpi başlatılamadı: {ex.Message}");
                return null;
            }
        }

        /// <summary>Portun dinlemeye başlamasını en fazla 5 saniye bekler.</summary>
        private async Task<bool> WaitForProxyAsync(Process process, CancellationToken ct)
        {
            var deadline = DateTime.UtcNow.AddSeconds(5);

            while (DateTime.UtcNow < deadline)
            {
                ct.ThrowIfCancellationRequested();

                if (process.HasExited) return false;

                try
                {
                    using var client = new TcpClient();
                    await client.ConnectAsync("127.0.0.1", _settings.ProxyPort, ct).ConfigureAwait(false);
                    return true;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    throw;
                }
                catch
                {
                    await Task.Delay(150, ct).ConfigureAwait(false);
                }
            }

            return false;
        }

        private static void KillProcess(Process process)
        {
            if (process == null) return;
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                    process.WaitForExit(3000);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ProxyTest] ciadpi kapatılamadı: {ex.Message}");
            }
            finally
            {
                process.Dispose();
            }
        }
    }
}
