using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CryptoGuard.Core.Models;
using CryptoGuard.Core.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace CryptoGuard.MAUI.ViewModels
{
    public partial class TransactionHistoryViewModel : ObservableObject
    {
        private readonly IPortfolioService _portfolioService;
        private readonly ICoinLoreService _coinLoreService;
        private readonly ILogger<TransactionHistoryViewModel> _logger;
        public ObservableCollection<TransactionHistoryRow> Transactions { get; set; } = new();
        public ObservableCollection<TransactionHistoryRow> FilteredTransactions { get; set; } = new();

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private TimeSpan selectedTime = DateTime.Now.TimeOfDay;

        [ObservableProperty]
        private DateTime startDate = DateTime.Today.AddDays(-30);

        [ObservableProperty]
        private DateTime endDate = DateTime.Today;

        [ObservableProperty]
        private bool isDateFilterEnabled = false;

        public TransactionHistoryViewModel(IPortfolioService portfolioService, ICoinLoreService coinLoreService, ILogger<TransactionHistoryViewModel> logger)
        {
            _portfolioService = portfolioService;
            _coinLoreService = coinLoreService;
            _logger = logger;
            LoadTransactions();
        }

        public TransactionHistoryViewModel() { }

        [RelayCommand]
        private async Task LoadTransactions()
        {
            try
            {
                Transactions.Clear();
                FilteredTransactions.Clear();
                int userId = UserContext.CurrentUserId ?? 0;
                string username = UserContext.CurrentUsername ?? "Unknown";
                
                _logger?.LogInformation($"TransactionHistoryViewModel: Loading transactions for user ID: {userId}, Username: {username}");
                
                var history = await _portfolioService.GetTransactionHistoryByUserIdAsync(userId);
                
                _logger?.LogInformation($"TransactionHistoryViewModel: Found {history.Count()} transactions for user {userId}");
                
                if (!history.Any())
                {
                    _logger?.LogWarning($"TransactionHistoryViewModel: No transactions found for user {userId}");
                    return;
                }

                // İşlemleri önce yükle, coin bilgilerini sonra ekle
                foreach (var item in history)
                {
                    string islemText = item.TransactionType switch
                    {
                        TransactionType.Buy => "Alış",
                        TransactionType.Sell => "Satış",
                        TransactionType.Edit => "Düzenleme",
                        _ => item.TransactionType.ToString()
                    };
                    decimal realizedProfit = 0;
                    decimal realizedProfitPercent = 0;
                    if (item.TransactionType == TransactionType.Sell || item.TransactionType == TransactionType.Edit)
                    {
                        var buys = history.Where(h => h.CoinId == item.CoinId && h.TransactionType == TransactionType.Buy && h.TransactionDate <= item.TransactionDate).ToList();
                        decimal totalBuy = buys.Sum(b => b.TotalAmount);
                        if (totalBuy > 0)
                        {
                            realizedProfit = item.TotalAmount - totalBuy;
                            realizedProfitPercent = (realizedProfit / totalBuy) * 100;
                        }
                    }
                    var transactionRow = new TransactionHistoryRow
                    {
                        TransactionDate = item.TransactionDate,
                        CoinId = item.CoinId,
                        CoinSymbol = item.CoinId.ToUpper(), // Geçici olarak coin ID'sini kullan
                        TransactionType = item.TransactionType,
                        IslemText = islemText,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        RealizedProfit = realizedProfit,
                        RealizedProfitPercent = realizedProfitPercent
                    };
                    Transactions.Add(transactionRow);
                    FilteredTransactions.Add(transactionRow);
                }
                
                _logger?.LogInformation($"TransactionHistoryViewModel: Successfully loaded {Transactions.Count} transactions");
                
                // Coin bilgilerini arka planda güncelle
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var coinIds = history.Select(h => h.CoinId).Distinct().ToList();
                        var coinDict = new Dictionary<string, string>();
                        
                        foreach (var coinId in coinIds)
                        {
                            try
                            {
                                var coin = await _coinLoreService.GetCoinPriceAsync(coinId);
                                if (coin == null)
                                {
                                    coinDict[coinId] = coinId.ToUpper();
                                    _logger?.LogWarning($"TransactionHistoryViewModel: Could not get coin info for {coinId}");
                                }
                                else
                                {
                                    coinDict[coinId] = coin.Symbol;
                                }
                            }
                            catch (Exception ex)
                            { 
                                coinDict[coinId] = coinId.ToUpper();
                                _logger?.LogError(ex, $"TransactionHistoryViewModel: Error getting coin info for {coinId}");
                            }
                        }
                        
                        // UI thread'de coin sembollerini güncelle
                        await Application.Current?.Dispatcher.DispatchAsync(() =>
                        {
                            foreach (var transaction in Transactions)
                            {
                                if (coinDict.ContainsKey(transaction.CoinId))
                                {
                                    transaction.CoinSymbol = coinDict[transaction.CoinId];
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "TransactionHistoryViewModel: Error updating coin symbols");
                    }
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "TransactionHistoryViewModel: Error loading transactions");
            }
        }

        [RelayCommand]
        private void ApplyDateFilter()
        {
            try
            {
                if (!IsDateFilterEnabled)
                {
                    // Filtre kapalıysa tüm işlemleri göster
                    FilteredTransactions.Clear();
                    foreach (var transaction in Transactions)
                    {
                        FilteredTransactions.Add(transaction);
                    }
                    _logger?.LogInformation($"TransactionHistoryViewModel: Showing all {FilteredTransactions.Count} transactions (filter disabled)");
                    return;
                }

                FilteredTransactions.Clear();
                var startDateTime = StartDate.Date;
                var endDateTime = EndDate.Date.AddDays(1).AddSeconds(-1); // Günün sonuna kadar

                _logger?.LogInformation($"TransactionHistoryViewModel: Filtering transactions from {startDateTime:yyyy-MM-dd} to {endDateTime:yyyy-MM-dd}");

                foreach (var transaction in Transactions)
                {
                    if (transaction.TransactionDate >= startDateTime && transaction.TransactionDate <= endDateTime)
                    {
                        FilteredTransactions.Add(transaction);
                    }
                }
                
                _logger?.LogInformation($"TransactionHistoryViewModel: Filtered to {FilteredTransactions.Count} transactions out of {Transactions.Count} total");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "TransactionHistoryViewModel: Error applying date filter");
            }
        }

        [RelayCommand]
        private void ClearFilters()
        {
            try
            {
                IsDateFilterEnabled = false;
                StartDate = DateTime.Today.AddDays(-30);
                EndDate = DateTime.Today;
                // Tüm işlemleri göster
                FilteredTransactions.Clear();
                foreach (var transaction in Transactions)
                {
                    FilteredTransactions.Add(transaction);
                }
                _logger?.LogInformation($"TransactionHistoryViewModel: Cleared filters, showing all {FilteredTransactions.Count} transactions");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "TransactionHistoryViewModel: Error clearing filters");
            }
        }

        partial void OnIsDateFilterEnabledChanged(bool value)
        {
            // Filtre açılıp kapanırken otomatik uygulama yapma
            if (!value)
            {
                // Filtre kapatıldığında tüm işlemleri göster
                FilteredTransactions.Clear();
                foreach (var transaction in Transactions)
                {
                    FilteredTransactions.Add(transaction);
                }
                _logger?.LogInformation($"TransactionHistoryViewModel: Filter disabled, showing all {FilteredTransactions.Count} transactions");
            }
        }
    }

    public class TransactionHistoryRow : INotifyPropertyChanged
    {
        private string _coinSymbol;
        
        public DateTime TransactionDate { get; set; }
        public string CoinId { get; set; }
        public string CoinSymbol 
        { 
            get => _coinSymbol;
            set
            {
                if (_coinSymbol != value)
                {
                    _coinSymbol = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CoinSymbol)));
                }
            }
        }
        public TransactionType TransactionType { get; set; }
        public string IslemText { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal RealizedProfit { get; set; }
        public decimal RealizedProfitPercent { get; set; }
        public string RealizedProfitDisplay => (TransactionType == TransactionType.Buy) ? "-" : $"{RealizedProfit:N2} ({RealizedProfitPercent:N2}%)";
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
} 