using System;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CryptoGuard.Core.Models
{
    public partial class Coin : ObservableObject
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Symbol { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal Price { get; set; }
        public decimal MarketCap { get; set; }
        public decimal PriceChangePercentage24h { get; set; }
        [NotMapped]
        public decimal PriceChangePercentage1h { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime LastUpdated { get; set; }
        private IRelayCommand<Coin> toggleFavoriteCommand;
        private bool isFavorite;
        private int index;
        [NotMapped]
        public IRelayCommand<Coin> ToggleFavoriteCommand
        {
            get => toggleFavoriteCommand;
            set => SetProperty(ref toggleFavoriteCommand, value);
        }
        [NotMapped]
        public bool IsFavorite
        {
            get => isFavorite;
            set => SetProperty(ref isFavorite, value);
        }
        [NotMapped]
        public int Index
        {
            get => index;
            set => SetProperty(ref index, value);
        }
    }
} 