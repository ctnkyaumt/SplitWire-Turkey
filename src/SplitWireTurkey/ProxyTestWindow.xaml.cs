using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;
using SplitWireTurkey.ProxyTest;

namespace SplitWireTurkey
{
    /// <summary>
    /// ByeByeDPI (romanvht) proxy testinin masaüstü karşılığı:
    /// strateji listesini sırayla ciadpi üzerinde çalıştırıp site erişim oranlarını raporlar.
    /// </summary>
    public partial class ProxyTestWindow : Window
    {
        private readonly ProxyTestSettings _settings;
        private readonly List<StrategyResult> _strategies = new List<StrategyResult>();
        private readonly ObservableCollection<StrategyResult> _display = new ObservableCollection<StrategyResult>();
        private readonly List<CheckBox> _siteListBoxes = new List<CheckBox>();

        private CancellationTokenSource _cts;
        private bool _isTesting;

        /// <summary>Kullanıcı bir stratejiyi uyguladığında tetiklenir.</summary>
        public event Action<string> StrategyApplied;

        public ProxyTestWindow()
        {
            InitializeComponent();

            _settings = ProxyTestSettings.Load();
            itemsResults.ItemsSource = _display;

            ApplyLanguage();
            BuildSiteListCheckboxes();
            LoadSettingsToUi();
            ApplyThemeFromRegistry();
            LoadPreviousResults();
        }

        #region Dil ve tema

        private static string T(string key, string fallback)
        {
            var text = LanguageManager.GetText("proxytest", key);
            return string.IsNullOrEmpty(text) || text == $"proxytest.{key}" ? fallback : text;
        }

        private void ApplyLanguage()
        {
            Title = T("title", "ByeDPI Proxy Testi");
            txtTitle.Text = Title;
            txtDisclaimer.Text = T("disclaimer", txtDisclaimer.Text);
            expSettings.Header = T("settings", "Test Ayarları");
            lblDelay.Text = T("delay", lblDelay.Text);
            lblRequests.Text = T("requests", lblRequests.Text);
            lblConcurrency.Text = T("concurrency", lblConcurrency.Text);
            lblTimeout.Text = T("timeout", lblTimeout.Text);
            lblSni.Text = T("sni", lblSni.Text);
            lblPort.Text = T("port", lblPort.Text);
            lblSiteLists.Text = T("site_lists", lblSiteLists.Text);
            lblCustomStrategies.Text = T("custom_strategies", lblCustomStrategies.Text);
            btnStartStop.Content = T("start", "Testi Başlat");
            btnCopyLog.Content = T("copy_log", "Günlüğü Kopyala");
            btnClose.Content = T("close", "Kapat");
        }

        private void ApplyThemeFromRegistry()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\SplitWire-Turkey\Theme");
                var isDark = key?.GetValue("IsDarkMode") is int v && v == 1;
                if (!isDark) return;

                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1c1c1d"));
                Foreground = Brushes.White;
                TextElement.SetForeground(this, Brushes.White);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ProxyTest] Tema uygulanamadı: {ex.Message}");
            }
        }

        #endregion

        #region Ayarlar

        private void BuildSiteListCheckboxes()
        {
            var lists = ProxyTestRunner.GetAvailableSiteLists();
            var active = new HashSet<string>(_settings.ActiveSiteLists ?? new List<string>(), StringComparer.OrdinalIgnoreCase);

            foreach (var name in lists)
            {
                var box = new CheckBox
                {
                    Content = name,
                    Tag = name,
                    IsChecked = active.Contains(name),
                    Margin = new Thickness(0, 0, 16, 6),
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                _siteListBoxes.Add(box);
                itemsSiteLists.Items.Add(box);
            }
        }

        private void LoadSettingsToUi()
        {
            txtDelay.Text = _settings.Delay.ToString();
            txtRequests.Text = _settings.Requests.ToString();
            txtConcurrency.Text = _settings.Concurrency.ToString();
            txtTimeout.Text = _settings.Timeout.ToString();
            txtSni.Text = _settings.Sni;
            txtPort.Text = _settings.ProxyPort.ToString();
            chkCustomStrategies.IsChecked = _settings.UseCustomStrategies;
            txtCustomStrategies.Text = _settings.CustomStrategies;
            txtCustomStrategies.IsEnabled = _settings.UseCustomStrategies;
        }

        /// <summary>UI değerlerini ayarlara aktarır ve sınırlara sıkıştırır.</summary>
        private void CommitSettingsFromUi()
        {
            _settings.Delay = ParseClamped(txtDelay.Text, _settings.Delay, 0, 10);
            _settings.Requests = ParseClamped(txtRequests.Text, _settings.Requests, 1, 20);
            _settings.Concurrency = ParseClamped(txtConcurrency.Text, _settings.Concurrency, 1, 50);
            _settings.Timeout = ParseClamped(txtTimeout.Text, _settings.Timeout, 1, 15);
            _settings.ProxyPort = ParseClamped(txtPort.Text, _settings.ProxyPort, 1024, 65535);
            _settings.Sni = string.IsNullOrWhiteSpace(txtSni.Text) ? "google.com" : txtSni.Text.Trim();
            _settings.UseCustomStrategies = chkCustomStrategies.IsChecked == true;
            _settings.CustomStrategies = txtCustomStrategies.Text;
            _settings.ActiveSiteLists = _siteListBoxes
                .Where(b => b.IsChecked == true)
                .Select(b => (string)b.Tag)
                .ToList();

            _settings.Save();
            LoadSettingsToUi();
        }

        private static int ParseClamped(string text, int fallback, int min, int max)
            => int.TryParse(text?.Trim(), out var value) ? Math.Clamp(value, min, max) : fallback;

        private void ChkCustomStrategies_Changed(object sender, RoutedEventArgs e)
        {
            if (txtCustomStrategies != null)
            {
                txtCustomStrategies.IsEnabled = chkCustomStrategies.IsChecked == true;
            }
        }

        #endregion

        #region Test akışı

        private void BtnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (_isTesting)
            {
                StopTesting();
            }
            else
            {
                _ = StartTestingAsync();
            }
        }

        private async Task StartTestingAsync()
        {
            CommitSettingsFromUi();

            var runner = new ProxyTestRunner(_settings);

            if (!File.Exists(ProxyTestRunner.CiadpiPath))
            {
                MessageBox.Show(this, T("ciadpi_missing", "ciadpi.exe bulunamadı."),
                    T("title", "ByeDPI Proxy Testi"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var sites = runner.LoadSites();
            if (sites.Count == 0)
            {
                MessageBox.Show(this, T("no_sites", "En az bir site listesi seçmelisiniz."),
                    T("title", "ByeDPI Proxy Testi"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var commands = runner.LoadStrategies();
            if (commands.Count == 0)
            {
                MessageBox.Show(this, T("no_strategies", "Test edilecek strateji bulunamadı."),
                    T("title", "ByeDPI Proxy Testi"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _strategies.Clear();
            _strategies.AddRange(commands.Select(c => new StrategyResult { Command = c }));
            RefreshDisplay(sortByPercentage: false);

            _isTesting = true;
            _cts = new CancellationTokenSource();
            btnStartStop.Content = T("stop", "Testi Durdur");
            expSettings.IsEnabled = false;

            try
            {
                await runner.RunAsync(
                    _strategies,
                    sites,
                    onStrategyStarting: (i, totalRequests) => Dispatcher.Invoke(() =>
                    {
                        _strategies[i].TotalRequests = totalRequests;
                        _strategies[i].IsRunning = true;
                        txtProgress.Text = string.Format(T("progress", "Test ediliyor: {0}/{1}"), i + 1, _strategies.Count);
                    }),
                    onSiteChecked: (i, site, success, total) => Dispatcher.InvokeAsync(() =>
                    {
                        var strategy = _strategies[i];
                        strategy.CurrentProgress += total;
                        strategy.SuccessCount += success;
                        strategy.SiteResults.Add(new SiteResult { Site = site, SuccessCount = success, TotalCount = total });
                    }),
                    onStrategyCompleted: (i, started) => Dispatcher.Invoke(() =>
                    {
                        _strategies[i].IsRunning = false;
                        _strategies[i].FailedToStart = !started;
                        _strategies[i].IsCompleted = started;
                        RefreshDisplay(sortByPercentage: true);
                        SaveResults();
                    }),
                    _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Kullanıcı durdurdu.
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, T("title", "ByeDPI Proxy Testi"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                FinishTesting();
            }
        }

        private void StopTesting()
        {
            try { _cts?.Cancel(); } catch { }
        }

        private void FinishTesting()
        {
            _isTesting = false;
            _cts?.Dispose();
            _cts = null;

            foreach (var strategy in _strategies.Where(s => s.IsRunning))
            {
                strategy.IsRunning = false;
            }

            btnStartStop.Content = T("start", "Testi Başlat");
            expSettings.IsEnabled = true;
            txtProgress.Text = T("complete", "Test tamamlandı.");

            RefreshDisplay(sortByPercentage: true);
            SaveResults();
        }

        /// <summary>Tamamlanan stratejileri başarı oranına göre öne alır.</summary>
        private void RefreshDisplay(bool sortByPercentage)
        {
            IEnumerable<StrategyResult> ordered = _strategies;

            if (sortByPercentage)
            {
                ordered = _strategies
                    .Select((s, index) => (s, index))
                    .OrderByDescending(x => x.s.IsCompleted)
                    .ThenByDescending(x => x.s.IsCompleted ? x.s.SuccessPercentage : 0)
                    .ThenBy(x => x.index)
                    .Select(x => x.s);
            }

            _display.Clear();
            foreach (var strategy in ordered)
            {
                _display.Add(strategy);
            }
        }

        #endregion

        #region Sonuç saklama ve dışa aktarma

        private void SaveResults()
        {
            try
            {
                var stored = _strategies
                    .Where(s => s.IsCompleted)
                    .Select(s => new StoredStrategyResult
                    {
                        Command = s.Command,
                        SuccessCount = s.SuccessCount,
                        TotalRequests = s.TotalRequests,
                        SiteResults = s.SiteResults.ToList()
                    })
                    .ToList();

                var path = ProxyTestRunner.ResultsPath;
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, JsonSerializer.Serialize(stored));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ProxyTest] Sonuçlar kaydedilemedi: {ex.Message}");
            }
        }

        private void LoadPreviousResults()
        {
            try
            {
                var path = ProxyTestRunner.ResultsPath;
                if (!File.Exists(path)) return;

                var stored = JsonSerializer.Deserialize<List<StoredStrategyResult>>(File.ReadAllText(path));
                if (stored == null || stored.Count == 0) return;

                foreach (var item in stored)
                {
                    var strategy = new StrategyResult
                    {
                        Command = item.Command,
                        TotalRequests = item.TotalRequests,
                        SuccessCount = item.SuccessCount,
                        CurrentProgress = item.TotalRequests,
                        IsCompleted = true
                    };

                    foreach (var site in item.SiteResults ?? new List<SiteResult>())
                    {
                        strategy.SiteResults.Add(site);
                    }

                    _strategies.Add(strategy);
                }

                RefreshDisplay(sortByPercentage: true);
                txtProgress.Text = T("previous_results", "Önceki test sonuçları gösteriliyor.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ProxyTest] Önceki sonuçlar okunamadı: {ex.Message}");
            }
        }

        private void BtnCopyLog_Click(object sender, RoutedEventArgs e)
        {
            var completed = _strategies.Where(s => s.IsCompleted).ToList();
            if (completed.Count == 0) return;

            var sb = new StringBuilder();
            foreach (var strategy in completed.OrderByDescending(s => s.SuccessPercentage))
            {
                sb.AppendLine(strategy.Command);
                sb.AppendLine();
                foreach (var site in strategy.SiteResults)
                {
                    sb.AppendLine($"{site.Site} - {site.SuccessCount}/{site.TotalCount}");
                }
                sb.AppendLine();
                sb.AppendLine($"{strategy.SuccessCount}/{strategy.TotalRequests} ({strategy.SuccessPercentage}%)");
                sb.AppendLine("-------------");
            }

            try
            {
                Clipboard.SetText(sb.ToString());
                txtProgress.Text = T("copied", "Günlük panoya kopyalandı.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ProxyTest] Panoya kopyalanamadı: {ex.Message}");
            }
        }

        #endregion

        private void BtnApplyStrategy_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not string command) return;

            if (_isTesting)
            {
                MessageBox.Show(this, T("stop_first", "Önce testi durdurun."),
                    T("title", "ByeDPI Proxy Testi"), MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            StrategyApplied?.Invoke(command);
            txtProgress.Text = string.Format(T("applied", "Uygulandı: {0}"), command);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_isTesting)
            {
                StopTesting();
            }
            base.OnClosing(e);
        }
    }
}
