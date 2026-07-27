using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SplitWireTurkey.ProxyTest
{
    /// <summary>
    /// Siteleri yerel SOCKS5 proxy üzerinden test eder.
    /// ByeByeDPI (romanvht) SiteCheckUtils sınıfının .NET karşılığı.
    /// </summary>
    public class SiteChecker : IDisposable
    {
        private readonly HttpClient _client;

        public SiteChecker(string proxyIp, int proxyPort, int timeoutSeconds)
        {
            var handler = new SocketsHttpHandler
            {
                Proxy = new WebProxy($"socks5://{proxyIp}:{proxyPort}"),
                UseProxy = true,
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 5,
                // Her istek yeni bağlantı açsın; aksi halde tekrarlı istekler
                // TLS el sıkışmasını yeniden test etmez.
                PooledConnectionLifetime = TimeSpan.Zero,
                ConnectTimeout = TimeSpan.FromSeconds(timeoutSeconds)
            };

            _client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(timeoutSeconds)
            };
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        }

        /// <summary>
        /// Tüm siteleri eş zamanlı olarak (limitli) test eder.
        /// onSiteChecked: (site, başarılı istek sayısı, toplam istek sayısı)
        /// </summary>
        public async Task CheckSitesAsync(
            IEnumerable<string> sites,
            int requestsCount,
            int concurrentRequests,
            Action<string, int, int> onSiteChecked,
            CancellationToken ct)
        {
            using var semaphore = new SemaphoreSlim(Math.Max(1, concurrentRequests));

            var tasks = sites.Select(async site =>
            {
                await semaphore.WaitAsync(ct).ConfigureAwait(false);
                try
                {
                    var successCount = await CheckSiteAsync(site, requestsCount, ct).ConfigureAwait(false);
                    onSiteChecked?.Invoke(site, successCount, requestsCount);
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToList();

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        /// <summary>
        /// Tek bir siteye requestsCount kadar istek atar, başarılı olanların sayısını döner.
        /// Yanıt alınmış olsa bile gövde Content-Length'ten kısa geldiyse engelleme sayılır.
        /// </summary>
        private async Task<int> CheckSiteAsync(string site, int requestsCount, CancellationToken ct)
        {
            var url = (site.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                       site.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                ? site
                : "https://" + site;

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return 0;
            }

            var success = 0;

            for (var attempt = 0; attempt < requestsCount; attempt++)
            {
                ct.ThrowIfCancellationRequested();

                try
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, uri);
                    request.Headers.ConnectionClose = true;

                    using var response = await _client
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct)
                        .ConfigureAwait(false);

                    var declaredLength = response.Content.Headers.ContentLength ?? -1;
                    var limit = declaredLength > 0 ? declaredLength : 1024L * 1024;

                    long actualLength = 0;
                    try
                    {
                        using var stream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
                        var buffer = new byte[8192];
                        while (actualLength < limit)
                        {
                            var remaining = limit - actualLength;
                            var toRead = (int)Math.Min(buffer.Length, remaining);
                            var read = await stream.ReadAsync(buffer.AsMemory(0, toRead), ct).ConfigureAwait(false);
                            if (read == 0) break;
                            actualLength += read;
                        }
                    }
                    catch (OperationCanceledException) when (ct.IsCancellationRequested)
                    {
                        throw;
                    }
                    catch
                    {
                        // Gövde okunamadı; aşağıdaki uzunluk kontrolü engellemeyi yakalar.
                    }

                    if (declaredLength <= 0 || actualLength >= declaredLength)
                    {
                        success++;
                    }
                    else
                    {
                        Debug.WriteLine($"[ProxyTest] Engel: {site} beklenen {declaredLength}, gelen {actualLength}");
                    }
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ProxyTest] {site} hata: {ex.Message}");
                }
            }

            return success;
        }

        public void Dispose() => _client?.Dispose();
    }
}
