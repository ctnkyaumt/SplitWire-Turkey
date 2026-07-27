using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SplitWireTurkey.ProxyTest
{
    public class SiteResult
    {
        public string Site { get; set; }
        public int SuccessCount { get; set; }
        public int TotalCount { get; set; }

        public string Display => $"{Site} — {SuccessCount}/{TotalCount}";
    }

    /// <summary>
    /// Tek bir ByeDPI stratejisinin test sonucu.
    /// </summary>
    public class StrategyResult : INotifyPropertyChanged
    {
        private int _successCount;
        private int _totalRequests;
        private int _currentProgress;
        private bool _isCompleted;
        private bool _isRunning;
        private bool _failedToStart;

        public string Command { get; set; }

        public ObservableCollection<SiteResult> SiteResults { get; } = new ObservableCollection<SiteResult>();

        public int SuccessCount
        {
            get => _successCount;
            set { _successCount = value; Raise(); Raise(nameof(SuccessPercentage)); Raise(nameof(ProgressText)); }
        }

        public int TotalRequests
        {
            get => _totalRequests;
            set { _totalRequests = value; Raise(); Raise(nameof(SuccessPercentage)); Raise(nameof(ProgressText)); }
        }

        public int CurrentProgress
        {
            get => _currentProgress;
            set { _currentProgress = value; Raise(); Raise(nameof(ProgressText)); }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set { _isCompleted = value; Raise(); Raise(nameof(ProgressText)); }
        }

        public bool IsRunning
        {
            get => _isRunning;
            set { _isRunning = value; Raise(); Raise(nameof(ProgressText)); }
        }

        /// <summary>
        /// ciadpi bu parametrelerle hiç ayağa kalkmadı (ör. bu sürümün desteklemediği bir seçenek).
        /// Sonuç 0% değil, "geçersiz" olarak raporlanır.
        /// </summary>
        public bool FailedToStart
        {
            get => _failedToStart;
            set { _failedToStart = value; Raise(); Raise(nameof(ProgressText)); }
        }

        public int SuccessPercentage => _totalRequests > 0 ? (_successCount * 100) / _totalRequests : 0;

        public string ProgressText
        {
            get
            {
                if (_failedToStart) return "n/a";
                if (_isCompleted) return $"{SuccessPercentage}%  ({_successCount}/{_totalRequests})";
                if (_isRunning) return $"{_currentProgress}/{_totalRequests}";
                return "—";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Raise([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>Diske yazılan/okunan sade sonuç modeli.</summary>
    public class StoredStrategyResult
    {
        public string Command { get; set; }
        public int SuccessCount { get; set; }
        public int TotalRequests { get; set; }
        public List<SiteResult> SiteResults { get; set; } = new List<SiteResult>();
    }
}
