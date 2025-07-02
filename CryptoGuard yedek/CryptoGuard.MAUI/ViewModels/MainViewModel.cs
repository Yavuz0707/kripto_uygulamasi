using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoGuard.Core.Interfaces;
using CryptoGuard.Core.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using CryptoGuard.MAUI.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Media;
using CryptoGuard.MAUI.ViewModels;
using System.Collections.Generic;

namespace CryptoGuard.MAUI.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly ICoinLoreService _coinLoreService;
        private readonly IPortfolioService _portfolioService;
        private readonly IPriceAlertService _priceAlertService;
        private readonly IVoiceRecognitionService _voiceRecognitionService;

        [ObservableProperty]
        private ObservableCollection<Coin> topCoins;

        [ObservableProperty]
        private ObservableCollection<Portfolio> userPortfolios;

        [ObservableProperty]
        private ObservableCollection<PriceAlert> userAlerts;

        [ObservableProperty]
        private ObservableCollection<Coin> favoriteCoins = new();

        [ObservableProperty]
        private ObservableCollection<Coin> allCoins = new();

        private string? recognizedText;
        public string? RecognizedText
        {
            get => recognizedText;
            set => SetProperty(ref recognizedText, value);
        }

        [ObservableProperty]
        private bool isListening;

        private string? errorMessage;
        public string? ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        [ObservableProperty]
        private string profilePhoto = Preferences.Get("ProfilePhoto", string.Empty);

        public string Username => UserContext.CurrentUsername ?? "Kullanıcı";

        [ObservableProperty]
        private decimal portfolioTotalValue;

        [ObservableProperty]
        private double portfolioChangePercent;

        public Color PortfolioChangeColor => PortfolioChangePercent < 0 ? Colors.Red : Colors.Green;

        partial void OnPortfolioChangePercentChanged(double value)
        {
            OnPropertyChanged(nameof(PortfolioChangeColor));
        }

        public void RefreshUsername()
        {
            OnPropertyChanged(nameof(Username));
        }

        public PortfolioViewModel PortfolioVM { get; }
        public ObservableCollection<PortfolioItemViewModel> PortfolioItems => PortfolioVM.PortfolioItems;

        public IAsyncRelayCommand LoadDataAsyncCommand { get; }

        public VoiceAssistantViewModel VoiceAssistant { get; }

        public string PortfolioTotalValueDisplay => $"{PortfolioTotalValue:N2} ₺";
        public string PortfolioChangePercentDisplay => $"%{PortfolioChangePercent:F2}";

        public IRelayCommand<Coin> ShowCoinDetailCommand { get; }

        public MainViewModel(
            ICoinLoreService coinLoreService,
            IPortfolioService portfolioService,
            IPriceAlertService priceAlertService,
            IVoiceRecognitionService voiceRecognitionService)
        {
            _coinLoreService = coinLoreService;
            _portfolioService = portfolioService;
            _priceAlertService = priceAlertService;
            _voiceRecognitionService = voiceRecognitionService;

            Title = "CryptoGuard";
            TopCoins = new ObservableCollection<Coin>();
            UserPortfolios = new ObservableCollection<Portfolio>();
            UserAlerts = new ObservableCollection<PriceAlert>();

            PortfolioVM = new PortfolioViewModel(_portfolioService, _coinLoreService);

            ErrorMessage = "Test hata mesajı: MainViewModel çalışıyor!";
            RefreshUsername();
            LoadDataAsyncCommand = new AsyncRelayCommand(LoadDataAsync);

            // Profil fotoğrafı güncellemesini dinle
            Microsoft.Maui.Controls.MessagingCenter.Subscribe<object, string>(this, "ProfilePhotoChanged", (sender, path) =>
            {
                ProfilePhoto = path;
            });
            // Favori coinler değiştiğinde güncelle
            Microsoft.Maui.Controls.MessagingCenter.Subscribe<object>(this, "FavoritesChanged", async (sender) =>
            {
                await LoadFavoriteCoinsAsync();
            });

            // Portföyde değişiklik olursa ana sayfa verilerini güncelle
            Microsoft.Maui.Controls.MessagingCenter.Subscribe<object>(this, "PortfolioChanged", async (sender) =>
            {
                await LoadDataAsync();
            });

            VoiceAssistant = new VoiceAssistantViewModel(_coinLoreService, _portfolioService);
            VoiceAssistant.StartWakeWordListening();

            ShowCoinDetailCommand = new RelayCommand<Coin>(async (coin) => await ShowCoinDetailAsync(coin));
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            RefreshUsername();
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                var coins = (await _coinLoreService.GetTopCoinsAsync(500)).ToList();
                var favCoinIds = new List<string>();
                if (UserContext.CurrentUserId != null)
                    favCoinIds = await _portfolioService.GetFavoriteCoinsAsync(UserContext.CurrentUserId.Value);
                AllCoins.Clear();
                TopCoins.Clear();
                if (coins == null || !coins.Any())
                {
                    ErrorMessage = "API'dan veri alınamadı veya veri yok.";
                    return;
                }
                int i = 1;
                foreach (var coin in coins)
                {
                    coin.IsFavorite = favCoinIds.Contains(coin.Id);
                    coin.ToggleFavoriteCommand = new RelayCommand<Coin>(async (c) => await ToggleFavoriteAsync(c));
                    coin.Index = i++;
                    AllCoins.Add(coin);
                }
                // TopCoins ilk 10 coin
                foreach (var coin in AllCoins.Take(10))
                {
                    TopCoins.Add(coin);
                }
                // Portföy verilerini yükle ve ana sayfa için güncel değerleri al
                await PortfolioVM.LoadPortfolioData();
                PortfolioTotalValue = PortfolioVM.TotalPortfolioValue;
                PortfolioChangePercent = PortfolioVM.TotalPortfolioChangePercent;
                UpdateFavoriteCoins();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Veri yüklenemedi: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task StartListeningAsync()
        {
            if (IsListening) return;

            try
            {
                System.Diagnostics.Debug.WriteLine("Sesli komut başlatıldı!");
                IsListening = true;
                await _voiceRecognitionService.StartListeningAsync();
                var text = await _voiceRecognitionService.RecognizeSpeechAsync();
                RecognizedText = text;

                if (string.IsNullOrWhiteSpace(text))
                {
                    ErrorMessage = "Ses algılanamadı veya Deepgram'dan yanıt alınamadı.";
                    return;
                }

                // Process voice command
                await ProcessVoiceCommandAsync(text);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Sesli komut hatası: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Sesli komut hatası: {ex.Message}");
            }
            finally
            {
                IsListening = false;
                try { await _voiceRecognitionService.StopListeningAsync(); } catch { }
            }
        }

        private async Task ProcessVoiceCommandAsync(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return;

            command = command.ToLower();

            // 1. Portföyü aç
            if (command.Contains("portföy") || command.Contains("portfolio"))
            {
                await Shell.Current.GoToAsync("//PortfolioPage");
                ErrorMessage = "Portföy sayfası açıldı.";
                return;
            }

            // 2. Coin fiyatı göster (ör: "bitcoin fiyatı", "ethereum fiyatı")
            if (command.Contains("fiyat") && (command.Contains("bitcoin") || command.Contains("ethereum") || command.Contains("cosmos") || command.Contains("dogecoin") || command.Contains("solana") || command.Contains("cardano")))
            {
                string coinId = "bitcoin";
                if (command.Contains("ethereum")) coinId = "ethereum";
                else if (command.Contains("cosmos")) coinId = "cosmos";
                else if (command.Contains("dogecoin")) coinId = "dogecoin";
                else if (command.Contains("solana")) coinId = "solana";
                else if (command.Contains("cardano")) coinId = "cardano";
                var coin = await _coinLoreService.GetCoinPriceAsync(coinId);
                if (coin == null)
                {
                    ErrorMessage = $"{coinId} fiyatı alınamadı.";
                    return;
                }
                ErrorMessage = $"{coin.Name} fiyatı: {coin.Price} $";
            }

            // 3. Coin grafiği aç (ör: "bitcoin grafiği", "cosmos grafiği")
            if (command.Contains("grafik") && (command.Contains("bitcoin") || command.Contains("ethereum") || command.Contains("cosmos") || command.Contains("dogecoin") || command.Contains("solana") || command.Contains("cardano")))
            {
                string coinId = "bitcoin";
                string coinName = "Bitcoin";
                if (command.Contains("ethereum")) { coinId = "ethereum"; coinName = "Ethereum"; }
                else if (command.Contains("cosmos")) { coinId = "cosmos"; coinName = "Cosmos"; }
                else if (command.Contains("dogecoin")) { coinId = "dogecoin"; coinName = "Dogecoin"; }
                else if (command.Contains("solana")) { coinId = "solana"; coinName = "Solana"; }
                else if (command.Contains("cardano")) { coinId = "cardano"; coinName = "Cardano"; }
                var popup = new Views.CoinDetailPopup { BindingContext = new CoinDetailViewModel(coinId, coinName) };
                await Shell.Current.Navigation.PushModalAsync(popup);
                ErrorMessage = $"{coinName} grafiği açıldı.";
                return;
            }

            // 4. Coin ekle (ör: "bitcoin ekle", "cosmos ekle")
            if (command.Contains("ekle") && (command.Contains("bitcoin") || command.Contains("ethereum") || command.Contains("cosmos") || command.Contains("dogecoin") || command.Contains("solana") || command.Contains("cardano")))
            {
                string coinId = "bitcoin";
                string coinName = "Bitcoin";
                if (command.Contains("ethereum")) { coinId = "ethereum"; coinName = "Ethereum"; }
                else if (command.Contains("cosmos")) { coinId = "cosmos"; coinName = "Cosmos"; }
                else if (command.Contains("dogecoin")) { coinId = "dogecoin"; coinName = "Dogecoin"; }
                else if (command.Contains("solana")) { coinId = "solana"; coinName = "Solana"; }
                else if (command.Contains("cardano")) { coinId = "cardano"; coinName = "Cardano"; }
                // Basitçe coin ekleme popup'ı aç
                var popup = new Views.AddCoinPopup(new AddCoinPopupViewModel(_portfolioService, _coinLoreService));
                await Shell.Current.Navigation.PushModalAsync(popup);
                ErrorMessage = $"{coinName} ekleme popup'ı açıldı.";
                return;
            }

            // 5. Alarm kur (ör: "bitcoin için alarm kur", "cosmos alarmı")
            if (command.Contains("alarm") && (command.Contains("bitcoin") || command.Contains("ethereum") || command.Contains("cosmos") || command.Contains("dogecoin") || command.Contains("solana") || command.Contains("cardano")))
            {
                // Alarm kurma popup'ı veya sayfası açılabilir (örnek)
                ErrorMessage = "Alarm kurma özelliği yakında!";
                return;
            }

            // 6. Portföyü oku (tüm coinleri ve miktarları sesli oku)
            if (command.Contains("oku") && command.Contains("portföy"))
            {
                // Portföydeki coinleri ve miktarları birleştirip sesli olarak oku (Text-to-Speech ile)
                if (Preferences.Get("VoiceReadEnabled", false))
                {
                    var text = "Portföyünüzde şunlar var: ";
                    foreach (var p in UserPortfolios)
                    {
                        foreach (var item in p.Items)
                        {
                            text += $"{item.CoinId} {item.Quantity} adet, ";
                        }
                    }
                    await TextToSpeech.Default.SpeakAsync(text);
                }
                ErrorMessage = "Portföy sesli okundu.";
                return;
            }

            // 7. Yardım komutu
            if (command.Contains("yardım") || command.Contains("help"))
            {
                ErrorMessage = "Sesli komut örnekleri: 'Portföyümü aç', 'Bitcoin fiyatı', 'Ethereum grafiği', 'Cosmos ekle', 'Bitcoin için alarm kur', 'Portföyümü oku'";
                return;
            }

            // 8. Bilinmeyen komut
            ErrorMessage = "Komut anlaşılamadı. 'Yardım' diyerek örnek komutları görebilirsin.";
        }

        [RelayCommand]
        private async Task StopListeningAsync()
        {
            if (!IsListening) return;

            try
            {
                await _voiceRecognitionService.StopListeningAsync();
            }
            catch (Exception)
            {
                // Hata yönetimi
            }
            finally
            {
                IsListening = false;
            }
        }

        [RelayCommand]
        private async Task GoToAllCoinsAsync()
        {
            await Shell.Current.GoToAsync("//AllCoinsPage");
        }

        [RelayCommand]
        private async Task GoToTransactionHistoryAsync()
        {
            await Shell.Current.GoToAsync("//TransactionHistoryPage");
        }

        public async Task LoadFavoriteCoinsAsync()
        {
            // Sadece AllCoins üzerinden filtrele
            if (AllCoins == null || AllCoins.Count == 0)
            {
                await LoadDataAsync();
            }
            UpdateFavoriteCoins();
        }

        public void UpdateFavoriteCoins()
        {
            FavoriteCoins = new ObservableCollection<Coin>(AllCoins.Where(c => c.IsFavorite));
        }

        public async Task ToggleFavoriteAsync(Coin coin)
        {
            if (UserContext.CurrentUserId == null) return;
            int userId = UserContext.CurrentUserId.Value;
            if (coin.IsFavorite)
            {
                await _portfolioService.RemoveFavoriteCoinAsync(userId, coin.Id);
                coin.IsFavorite = false;
            }
            else
            {
                await _portfolioService.AddFavoriteCoinAsync(userId, coin.Id);
                coin.IsFavorite = true;
            }
            UpdateFavoriteCoins();
            Microsoft.Maui.Controls.MessagingCenter.Send(this, "FavoritesChanged");
        }

        private string MapToCoinGeckoId(string symbol)
        {
            return symbol.ToLower() switch
            {
                "btc" => "bitcoin",
                "eth" => "ethereum",
                "bnb" => "binancecoin",
                "ada" => "cardano",
                "xrp" => "ripple",
                "doge" => "dogecoin",
                "sol" => "solana",
                "dot" => "polkadot",
                "matic" => "matic-network",
                "atom" => "cosmos",
                "usdt" => "tether",
                _ => symbol.ToLower()
            };
        }

        private async Task ShowCoinDetailAsync(Coin coin)
        {
            if (coin == null) return;
            var geckoId = MapToCoinGeckoId(coin.Symbol);
            var popup = new CryptoGuard.MAUI.Views.CoinDetailPopup { BindingContext = new CryptoGuard.MAUI.ViewModels.CoinDetailViewModel(geckoId, coin.Name) };
            await Shell.Current.Navigation.PushModalAsync(popup);
        }
    }
} 