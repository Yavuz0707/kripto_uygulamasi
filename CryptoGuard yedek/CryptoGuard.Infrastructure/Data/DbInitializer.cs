using CryptoGuard.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CryptoGuard.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedDataAsync(CryptoGuardDbContext context)
        {
            // Kullanıcılar zaten varsa ekleme
            if (await context.Users.AnyAsync())
            {
                Console.WriteLine("⚠️ Veritabanında zaten veri var, seed işlemi atlanıyor.");
                Console.WriteLine($"📊 Mevcut kayıt sayıları:");
                Console.WriteLine($"   - Kullanıcılar: {await context.Users.CountAsync()}");
                Console.WriteLine($"   - İşlem Geçmişi: {await context.TransactionHistories.CountAsync()}");
                return;
            }

            Console.WriteLine("🌱 Veritabanı seed işlemi başlıyor...");

            // Örnek kullanıcılar
            var users = new List<User>
            {
                new User
                {
                    Username = "burak",
                    Email = "burak@email.com",
                    PasswordHash = HashPassword("12345678"),
                    CreatedAt = DateTime.UtcNow.AddDays(-60),
                    LastLogin = DateTime.UtcNow.AddHours(-1),
                    ProfilePhoto = "profile_placeholder.png",
                    Biography = "Kripto para yatırımcısı. Aktif trading yapıyorum ve portföyümü sürekli güncelliyorum.",
                    IsPortfolioPublic = true
                },
                new User
                {
                    Username = "ahmet_yilmaz",
                    Email = "ahmet.yilmaz@email.com",
                    PasswordHash = HashPassword("123456"),
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    LastLogin = DateTime.UtcNow.AddHours(-2),
                    ProfilePhoto = "profile_placeholder.png",
                    Biography = "Kripto para yatırımcısı ve blockchain teknolojisi meraklısı. 3 yıldır aktif olarak trading yapıyorum.",
                    IsPortfolioPublic = true
                },
                new User
                {
                    Username = "ayse_demir",
                    Email = "ayse.demir@email.com",
                    PasswordHash = HashPassword("123456"),
                    CreatedAt = DateTime.UtcNow.AddDays(-45),
                    LastLogin = DateTime.UtcNow.AddHours(-1),
                    ProfilePhoto = "profile_placeholder.png",
                    Biography = "DeFi projeleri ve NFT'lerle ilgileniyorum. Yeni başlayanlar için rehberlik yapmaya çalışıyorum.",
                    IsPortfolioPublic = true
                },
                new User
                {
                    Username = "mehmet_kaya",
                    Email = "mehmet.kaya@email.com",
                    PasswordHash = HashPassword("123456"),
                    CreatedAt = DateTime.UtcNow.AddDays(-60),
                    LastLogin = DateTime.UtcNow.AddMinutes(-30),
                    ProfilePhoto = "profile_placeholder.png",
                    Biography = "Uzun vadeli yatırım stratejileri benim tarzım. HODL! 🚀",
                    IsPortfolioPublic = false
                },
                new User
                {
                    Username = "fatma_ozturk",
                    Email = "fatma.ozturk@email.com",
                    PasswordHash = HashPassword("123456"),
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    LastLogin = DateTime.UtcNow.AddHours(-3),
                    ProfilePhoto = "profile_placeholder.png",
                    Biography = "Kripto para dünyasına yeni katıldım. Öğrenmeye açığım! 📚",
                    IsPortfolioPublic = true
                },
                new User
                {
                    Username = "ali_celik",
                    Email = "ali.celik@email.com",
                    PasswordHash = HashPassword("123456"),
                    CreatedAt = DateTime.UtcNow.AddDays(-90),
                    LastLogin = DateTime.UtcNow.AddDays(-1),
                    ProfilePhoto = "profile_placeholder.png",
                    Biography = "Teknik analiz ve day trading uzmanı. Risk yönetimi çok önemli! 📊",
                    IsPortfolioPublic = true
                },
                new User
                {
                    Username = "zeynep_arslan",
                    Email = "zeynep.arslan@email.com",
                    PasswordHash = HashPassword("123456"),
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    LastLogin = DateTime.UtcNow.AddHours(-4),
                    ProfilePhoto = "profile_placeholder.png",
                    Biography = "Mining ve staking ile ilgileniyorum. Pasif gelir kaynakları arıyorum.",
                    IsPortfolioPublic = true
                },
                new User
                {
                    Username = "can_ozdemir",
                    Email = "can.ozdemir@email.com",
                    PasswordHash = HashPassword("123456"),
                    CreatedAt = DateTime.UtcNow.AddDays(-75),
                    LastLogin = DateTime.UtcNow.AddHours(-6),
                    ProfilePhoto = "profile_placeholder.png",
                    Biography = "Blockchain geliştirici ve kripto para araştırmacısı. Yeni projeleri takip ediyorum.",
                    IsPortfolioPublic = false
                },
                new User
                {
                    Username = "elif_yildiz",
                    Email = "elif.yildiz@email.com",
                    PasswordHash = HashPassword("123456"),
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    LastLogin = DateTime.UtcNow.AddMinutes(-15),
                    ProfilePhoto = "profile_placeholder.png",
                    Biography = "Kripto para eğitimi veriyorum. Güvenli yatırım için bilgi paylaşımı yapıyorum.",
                    IsPortfolioPublic = true
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            // Örnek coinler ekle
            var coins = new List<Coin>
            {
                new Coin
                {
                    Id = "bitcoin",
                    Name = "Bitcoin",
                    Symbol = "BTC",
                    CurrentPrice = 43577.50m,
                    MarketCap = 850000000m,
                    PriceChangePercentage24h = 2.5m,
                    LastUpdated = DateTime.UtcNow
                },
                new Coin
                {
                    Id = "ethereum",
                    Name = "Ethereum",
                    Symbol = "ETH",
                    CurrentPrice = 2650.75m,
                    MarketCap = 320000000m,
                    PriceChangePercentage24h = -1.2m,
                    LastUpdated = DateTime.UtcNow
                },
                new Coin
                {
                    Id = "binancecoin",
                    Name = "BNB",
                    Symbol = "BNB",
                    CurrentPrice = 315.25m,
                    MarketCap = 48000000m,
                    PriceChangePercentage24h = 0.8m,
                    LastUpdated = DateTime.UtcNow
                },
                new Coin
                {
                    Id = "cardano",
                    Name = "Cardano",
                    Symbol = "ADA",
                    CurrentPrice = 0.485m,
                    MarketCap = 17000000m,
                    PriceChangePercentage24h = 3.1m,
                    LastUpdated = DateTime.UtcNow
                },
                new Coin
                {
                    Id = "solana",
                    Name = "Solana",
                    Symbol = "SOL",
                    CurrentPrice = 98.50m,
                    MarketCap = 42000000m,
                    PriceChangePercentage24h = -0.5m,
                    LastUpdated = DateTime.UtcNow
                }
            };

            await context.Coins.AddRangeAsync(coins);
            await context.SaveChangesAsync();

            // Örnek portföyler ekle
            var portfolios = new List<Portfolio>
            {
                new Portfolio
                {
                    Name = "Burak'ın Ana Portföyü",
                    UserId = 1, // burak
                    TotalValue = 25000.00m,
                    LastUpdated = DateTime.UtcNow
                },
                new Portfolio
                {
                    Name = "Ana Portföy",
                    UserId = 2, // ahmet_yilmaz
                    TotalValue = 12500.00m,
                    LastUpdated = DateTime.UtcNow
                },
                new Portfolio
                {
                    Name = "DeFi Portföyü",
                    UserId = 3, // ayse_demir
                    TotalValue = 8750.00m,
                    LastUpdated = DateTime.UtcNow
                },
                new Portfolio
                {
                    Name = "Uzun Vadeli",
                    UserId = 4, // mehmet_kaya
                    TotalValue = 25000.00m,
                    LastUpdated = DateTime.UtcNow
                },
                new Portfolio
                {
                    Name = "Öğrenme Portföyü",
                    UserId = 5, // fatma_ozturk
                    TotalValue = 1500.00m,
                    LastUpdated = DateTime.UtcNow
                },
                new Portfolio
                {
                    Name = "Trading Portföyü",
                    UserId = 6, // ali_celik
                    TotalValue = 18000.00m,
                    LastUpdated = DateTime.UtcNow
                }
            };

            await context.Portfolios.AddRangeAsync(portfolios);
            await context.SaveChangesAsync();

            // Örnek portföy kalemleri ekle
            var portfolioItems = new List<PortfolioItem>
            {
                // Burak'ın portföyü
                new PortfolioItem
                {
                    PortfolioId = 1,
                    CoinId = "bitcoin",
                    Quantity = 0.5m,
                    BuyPrice = 42000.00m,
                    CurrentValue = 21788.75m,
                    PurchaseDate = DateTime.UtcNow.AddDays(-45),
                    LastUpdated = DateTime.UtcNow
                },
                new PortfolioItem
                {
                    PortfolioId = 1,
                    CoinId = "ethereum",
                    Quantity = 3.0m,
                    BuyPrice = 2500.00m,
                    CurrentValue = 7952.25m,
                    PurchaseDate = DateTime.UtcNow.AddDays(-40),
                    LastUpdated = DateTime.UtcNow
                },
                new PortfolioItem
                {
                    PortfolioId = 1,
                    CoinId = "solana",
                    Quantity = 100m,
                    BuyPrice = 95.00m,
                    CurrentValue = 9850.00m,
                    PurchaseDate = DateTime.UtcNow.AddDays(-35),
                    LastUpdated = DateTime.UtcNow
                },
                // Ahmet'in portföyü
                new PortfolioItem
                {
                    PortfolioId = 2,
                    CoinId = "bitcoin",
                    Quantity = 0.25m,
                    BuyPrice = 42000.00m,
                    CurrentValue = 10894.38m,
                    PurchaseDate = DateTime.UtcNow.AddDays(-30),
                    LastUpdated = DateTime.UtcNow
                },
                new PortfolioItem
                {
                    PortfolioId = 2,
                    CoinId = "ethereum",
                    Quantity = 2.5m,
                    BuyPrice = 2500.00m,
                    CurrentValue = 6626.88m,
                    PurchaseDate = DateTime.UtcNow.AddDays(-25),
                    LastUpdated = DateTime.UtcNow
                },
                // Ayşe'nin portföyü
                new PortfolioItem
                {
                    PortfolioId = 3,
                    CoinId = "cardano",
                    Quantity = 5000m,
                    BuyPrice = 0.45m,
                    CurrentValue = 2425.00m,
                    PurchaseDate = DateTime.UtcNow.AddDays(-40),
                    LastUpdated = DateTime.UtcNow
                },
                new PortfolioItem
                {
                    PortfolioId = 3,
                    CoinId = "solana",
                    Quantity = 25m,
                    BuyPrice = 95.00m,
                    CurrentValue = 2462.50m,
                    PurchaseDate = DateTime.UtcNow.AddDays(-35),
                    LastUpdated = DateTime.UtcNow
                },
                // Mehmet'in portföyü
                new PortfolioItem
                {
                    PortfolioId = 4,
                    CoinId = "bitcoin",
                    Quantity = 0.5m,
                    BuyPrice = 40000.00m,
                    CurrentValue = 21788.75m,
                    PurchaseDate = DateTime.UtcNow.AddDays(-60),
                    LastUpdated = DateTime.UtcNow
                },
                new PortfolioItem
                {
                    PortfolioId = 4,
                    CoinId = "ethereum",
                    Quantity = 1.2m,
                    BuyPrice = 2400.00m,
                    CurrentValue = 3180.90m,
                    PurchaseDate = DateTime.UtcNow.AddDays(-55),
                    LastUpdated = DateTime.UtcNow
                },
                // Fatma'nın portföyü
                new PortfolioItem
                {
                    PortfolioId = 5,
                    CoinId = "binancecoin",
                    Quantity = 4m,
                    BuyPrice = 300.00m,
                    CurrentValue = 1261.00m,
                    PurchaseDate = DateTime.UtcNow.AddDays(-10),
                    LastUpdated = DateTime.UtcNow
                },
                // Ali'nin portföyü
                new PortfolioItem
                {
                    PortfolioId = 6,
                    CoinId = "bitcoin",
                    Quantity = 0.3m,
                    BuyPrice = 43000.00m,
                    CurrentValue = 13073.25m,
                    PurchaseDate = DateTime.UtcNow.AddDays(-20),
                    LastUpdated = DateTime.UtcNow
                },
                new PortfolioItem
                {
                    PortfolioId = 6,
                    CoinId = "solana",
                    Quantity = 50m,
                    BuyPrice = 100.00m,
                    CurrentValue = 4925.00m,
                    PurchaseDate = DateTime.UtcNow.AddDays(-15),
                    LastUpdated = DateTime.UtcNow
                }
            };

            await context.PortfolioItems.AddRangeAsync(portfolioItems);
            await context.SaveChangesAsync();

            // Örnek favori coinler ekle
            var favoriteCoins = new List<FavoriteCoin>
            {
                new FavoriteCoin { UserId = 1, CoinId = "bitcoin" },
                new FavoriteCoin { UserId = 1, CoinId = "ethereum" },
                new FavoriteCoin { UserId = 1, CoinId = "solana" },
                new FavoriteCoin { UserId = 2, CoinId = "bitcoin" },
                new FavoriteCoin { UserId = 2, CoinId = "ethereum" },
                new FavoriteCoin { UserId = 3, CoinId = "cardano" },
                new FavoriteCoin { UserId = 3, CoinId = "solana" },
                new FavoriteCoin { UserId = 4, CoinId = "bitcoin" },
                new FavoriteCoin { UserId = 5, CoinId = "binancecoin" },
                new FavoriteCoin { UserId = 6, CoinId = "bitcoin" },
                new FavoriteCoin { UserId = 6, CoinId = "solana" },
                new FavoriteCoin { UserId = 7, CoinId = "ethereum" },
                new FavoriteCoin { UserId = 8, CoinId = "cardano" },
                new FavoriteCoin { UserId = 9, CoinId = "bitcoin" }
            };

            await context.FavoriteCoins.AddRangeAsync(favoriteCoins);
            await context.SaveChangesAsync();

            // Örnek fiyat alarmları ekle
            var priceAlerts = new List<PriceAlert>
            {
                new PriceAlert
                {
                    UserId = 1,
                    CoinId = "bitcoin",
                    User = null!, // Navigation property, veritabanına kaydedilirken doldurulacak
                    Coin = null!, // Navigation property, veritabanına kaydedilirken doldurulacak
                    TargetPrice = 50000.00m,
                    IsTriggered = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new PriceAlert
                {
                    UserId = 1,
                    CoinId = "ethereum",
                    User = null!, // Navigation property, veritabanına kaydedilirken doldurulacak
                    Coin = null!, // Navigation property, veritabanına kaydedilirken doldurulacak
                    TargetPrice = 3000.00m,
                    IsTriggered = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new PriceAlert
                {
                    UserId = 3,
                    CoinId = "cardano",
                    User = null!, // Navigation property, veritabanına kaydedilirken doldurulacak
                    Coin = null!, // Navigation property, veritabanına kaydedilirken doldurulacak
                    TargetPrice = 0.60m,
                    IsTriggered = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new PriceAlert
                {
                    UserId = 4,
                    CoinId = "bitcoin",
                    User = null!, // Navigation property, veritabanına kaydedilirken doldurulacak
                    Coin = null!, // Navigation property, veritabanına kaydedilirken doldurulacak
                    TargetPrice = 45000.00m,
                    IsTriggered = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    TriggeredAt = DateTime.UtcNow.AddDays(-2)
                }
            };

            await context.PriceAlerts.AddRangeAsync(priceAlerts);
            await context.SaveChangesAsync();

            // Örnek feed postları ekle
            var feedPosts = new List<FeedPost>
            {
                new FeedPost
                {
                    UserId = 1,
                    Content = "Bitcoin'in son yükselişi gerçekten etkileyici! HODL stratejisi işe yarıyor. 🚀",
                    CreatedAt = DateTime.UtcNow.AddHours(-2),
                    LikeCount = 5,
                    CommentCount = 3,
                    ImagePath = "profile_placeholder.png",
                    CoinTag = "BTC"
                },
                new FeedPost
                {
                    UserId = 2,
                    Content = "Cardano'nun yeni güncellemesi hakkında ne düşünüyorsunuz? DeFi ekosistemi için umut verici görünüyor.",
                    CreatedAt = DateTime.UtcNow.AddHours(-4),
                    LikeCount = 8,
                    CommentCount = 6,
                    ImagePath = "profile_placeholder.png",
                    CoinTag = "ADA"
                },
                new FeedPost
                {
                    UserId = 5,
                    Content = "Günlük trading analizi: Solana'da güçlü destek seviyeleri var. Dikkatli olun! 📊",
                    CreatedAt = DateTime.UtcNow.AddHours(-6),
                    LikeCount = 12,
                    CommentCount = 4,
                    ImagePath = "profile_placeholder.png",
                    CoinTag = "SOL"
                },
                new FeedPost
                {
                    UserId = 8,
                    Content = "Yeni başlayanlar için kripto para eğitimi başlatıyorum. İlgilenenler DM atabilir! 📚",
                    CreatedAt = DateTime.UtcNow.AddHours(-8),
                    LikeCount = 15,
                    CommentCount = 8,
                    ImagePath = "profile_placeholder.png",
                    CoinTag = ""
                }
            };

            await context.FeedPosts.AddRangeAsync(feedPosts);
            await context.SaveChangesAsync();

            // Örnek yorumlar ekle
            var comments = new List<Comment>
            {
                new Comment
                {
                    UserId = 3,
                    PostId = 1,
                    Content = "Katılıyorum! Uzun vadeli düşünmek önemli.",
                    CreatedAt = DateTime.UtcNow.AddHours(-1)
                },
                new Comment
                {
                    UserId = 4,
                    PostId = 1,
                    Content = "Ben de Bitcoin'e yatırım yapmaya başladım. Umarım doğru zamandır!",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-30)
                },
                new Comment
                {
                    UserId = 6,
                    PostId = 2,
                    Content = "Cardano'nun bilimsel yaklaşımı beni etkiliyor. Güvenilir bir proje.",
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                },
                new Comment
                {
                    UserId = 7,
                    PostId = 3,
                    Content = "Analiz için teşekkürler! Hangi seviyelerde alım yapmayı öneriyorsun?",
                    CreatedAt = DateTime.UtcNow.AddHours(-3)
                }
            };

            await context.Comments.AddRangeAsync(comments);
            await context.SaveChangesAsync();

            // Örnek beğeniler ekle
            var likes = new List<Like>
            {
                new Like { UserId = 2, PostId = 1 },
                new Like { UserId = 3, PostId = 1 },
                new Like { UserId = 4, PostId = 1 },
                new Like { UserId = 5, PostId = 1 },
                new Like { UserId = 6, PostId = 1 },
                new Like { UserId = 1, PostId = 2 },
                new Like { UserId = 3, PostId = 2 },
                new Like { UserId = 4, PostId = 2 },
                new Like { UserId = 5, PostId = 2 },
                new Like { UserId = 6, PostId = 2 },
                new Like { UserId = 7, PostId = 2 },
                new Like { UserId = 8, PostId = 2 },
                new Like { UserId = 1, PostId = 3 },
                new Like { UserId = 2, PostId = 3 },
                new Like { UserId = 3, PostId = 3 },
                new Like { UserId = 4, PostId = 3 },
                new Like { UserId = 6, PostId = 3 },
                new Like { UserId = 7, PostId = 3 },
                new Like { UserId = 8, PostId = 3 },
                new Like { UserId = 1, PostId = 4 },
                new Like { UserId = 2, PostId = 4 },
                new Like { UserId = 3, PostId = 4 },
                new Like { UserId = 4, PostId = 4 },
                new Like { UserId = 5, PostId = 4 },
                new Like { UserId = 6, PostId = 4 },
                new Like { UserId = 7, PostId = 4 }
            };

            await context.Likes.AddRangeAsync(likes);
            await context.SaveChangesAsync();

            // Örnek takip ilişkileri ekle
            var follows = new List<Follow>
            {
                new Follow { FollowerId = 2, FollowingId = 1 }, // Ayşe -> Ahmet
                new Follow { FollowerId = 4, FollowingId = 1 }, // Fatma -> Ahmet
                new Follow { FollowerId = 6, FollowingId = 1 }, // Zeynep -> Ahmet
                new Follow { FollowerId = 1, FollowingId = 2 }, // Ahmet -> Ayşe
                new Follow { FollowerId = 3, FollowingId = 2 }, // Mehmet -> Ayşe
                new Follow { FollowerId = 5, FollowingId = 2 }, // Ali -> Ayşe
                new Follow { FollowerId = 1, FollowingId = 5 }, // Ahmet -> Ali
                new Follow { FollowerId = 2, FollowingId = 5 }, // Ayşe -> Ali
                new Follow { FollowerId = 4, FollowingId = 5 }, // Fatma -> Ali
                new Follow { FollowerId = 1, FollowingId = 8 }, // Ahmet -> Elif
                new Follow { FollowerId = 2, FollowingId = 8 }, // Ayşe -> Elif
                new Follow { FollowerId = 4, FollowingId = 8 }, // Fatma -> Elif
                new Follow { FollowerId = 6, FollowingId = 8 }  // Zeynep -> Elif
            };

            await context.Follows.AddRangeAsync(follows);
            await context.SaveChangesAsync();

            // Burak için örnek işlem geçmişi ekle
            var transactionHistory = new List<TransactionHistory>
            {
                // Bitcoin alış işlemleri
                new TransactionHistory
                {
                    PortfolioId = 1, // Burak'ın portföyü
                    CoinId = "bitcoin",
                    TransactionType = TransactionType.Buy,
                    Quantity = 0.2m,
                    Price = 41000.00m,
                    TotalAmount = 8200.00m,
                    TransactionDate = DateTime.UtcNow.AddDays(-45).AddHours(10).AddMinutes(30)
                },
                new TransactionHistory
                {
                    PortfolioId = 1,
                    CoinId = "bitcoin",
                    TransactionType = TransactionType.Buy,
                    Quantity = 0.3m,
                    Price = 43000.00m,
                    TotalAmount = 12900.00m,
                    TransactionDate = DateTime.UtcNow.AddDays(-40).AddHours(14).AddMinutes(15)
                },
                // Ethereum alış işlemleri
                new TransactionHistory
                {
                    PortfolioId = 1,
                    CoinId = "ethereum",
                    TransactionType = TransactionType.Buy,
                    Quantity = 1.5m,
                    Price = 2400.00m,
                    TotalAmount = 3600.00m,
                    TransactionDate = DateTime.UtcNow.AddDays(-42).AddHours(9).AddMinutes(45)
                },
                new TransactionHistory
                {
                    PortfolioId = 1,
                    CoinId = "ethereum",
                    TransactionType = TransactionType.Buy,
                    Quantity = 1.5m,
                    Price = 2600.00m,
                    TotalAmount = 3900.00m,
                    TransactionDate = DateTime.UtcNow.AddDays(-38).AddHours(16).AddMinutes(20)
                },
                // Solana alış işlemleri
                new TransactionHistory
                {
                    PortfolioId = 1,
                    CoinId = "solana",
                    TransactionType = TransactionType.Buy,
                    Quantity = 50m,
                    Price = 90.00m,
                    TotalAmount = 4500.00m,
                    TransactionDate = DateTime.UtcNow.AddDays(-35).AddHours(11).AddMinutes(10)
                },
                new TransactionHistory
                {
                    PortfolioId = 1,
                    CoinId = "solana",
                    TransactionType = TransactionType.Buy,
                    Quantity = 50m,
                    Price = 100.00m,
                    TotalAmount = 5000.00m,
                    TransactionDate = DateTime.UtcNow.AddDays(-32).AddHours(13).AddMinutes(25)
                },
                // Düzenleme işlemleri
                new TransactionHistory
                {
                    PortfolioId = 1,
                    CoinId = "bitcoin",
                    TransactionType = TransactionType.Edit,
                    Quantity = 0.5m,
                    Price = 42000.00m,
                    TotalAmount = 21000.00m,
                    TransactionDate = DateTime.UtcNow.AddDays(-30).AddHours(15).AddMinutes(45)
                },
                new TransactionHistory
                {
                    PortfolioId = 1,
                    CoinId = "ethereum",
                    TransactionType = TransactionType.Edit,
                    Quantity = 3.0m,
                    Price = 2500.00m,
                    TotalAmount = 7500.00m,
                    TransactionDate = DateTime.UtcNow.AddDays(-28).AddHours(10).AddMinutes(30)
                },
                // Satış işlemleri
                new TransactionHistory
                {
                    PortfolioId = 1,
                    CoinId = "solana",
                    TransactionType = TransactionType.Sell,
                    Quantity = 25m,
                    Price = 110.00m,
                    TotalAmount = 2750.00m,
                    TransactionDate = DateTime.UtcNow.AddDays(-25).AddHours(14).AddMinutes(20)
                },
                new TransactionHistory
                {
                    PortfolioId = 1,
                    CoinId = "bitcoin",
                    TransactionType = TransactionType.Sell,
                    Quantity = 0.1m,
                    Price = 45000.00m,
                    TotalAmount = 4500.00m,
                    TransactionDate = DateTime.UtcNow.AddDays(-20).AddHours(9).AddMinutes(15)
                },
                // Son işlemler
                new TransactionHistory
                {
                    PortfolioId = 1,
                    CoinId = "cardano",
                    TransactionType = TransactionType.Buy,
                    Quantity = 200m,
                    Price = 0.45m,
                    TotalAmount = 90.00m,
                    TransactionDate = DateTime.UtcNow.AddDays(-15).AddHours(12).AddMinutes(30)
                },
                new TransactionHistory
                {
                    PortfolioId = 1,
                    CoinId = "binancecoin",
                    TransactionType = TransactionType.Buy,
                    Quantity = 5m,
                    Price = 300.00m,
                    TotalAmount = 1500.00m,
                    TransactionDate = DateTime.UtcNow.AddDays(-10).AddHours(16).AddMinutes(45)
                },
                new TransactionHistory
                {
                    PortfolioId = 1,
                    CoinId = "ethereum",
                    TransactionType = TransactionType.Sell,
                    Quantity = 0.5m,
                    Price = 2700.00m,
                    TotalAmount = 1350.00m,
                    TransactionDate = DateTime.UtcNow.AddDays(-5).AddHours(11).AddMinutes(20)
                }
            };

            // 1 Haziran 2025'ten itibaren her güne 5 işlem ekle
            var startDate = new DateTime(2025, 6, 1);
            var endDate = DateTime.Today;
            var currentDate = startDate;
            var random = new Random(123); // Sabit seed için

            var coinIds = new[] { "bitcoin", "ethereum", "solana", "cardano", "binancecoin" }; // Sadece mevcut coinler
            var transactionTypes = new[] { TransactionType.Buy, TransactionType.Sell, TransactionType.Edit };

            while (currentDate <= endDate)
            {
                for (int i = 0; i < 5; i++)
                {
                    var coinId = coinIds[random.Next(coinIds.Length)];
                    var transactionType = transactionTypes[random.Next(transactionTypes.Length)];
                    
                    // Coin bazlı gerçekçi fiyatlar
                    decimal price = coinId switch
                    {
                        "bitcoin" => random.Next(40000, 50000),
                        "ethereum" => random.Next(2500, 3500),
                        "solana" => random.Next(80, 120),
                        "cardano" => random.Next(40, 60) / 100m,
                        "binancecoin" => random.Next(250, 400),
                        _ => random.Next(1, 100)
                    };

                    // Coin bazlı gerçekçi miktarlar
                    decimal quantity = coinId switch
                    {
                        "bitcoin" => random.Next(1, 10) / 100m,
                        "ethereum" => random.Next(1, 20) / 10m,
                        "solana" => random.Next(10, 100),
                        "cardano" => random.Next(100, 1000),
                        "binancecoin" => random.Next(1, 20),
                        _ => random.Next(1, 100)
                    };

                    var totalAmount = price * quantity;
                    
                    // TotalAmount'u sınırla (çok büyük olmasın)
                    if (totalAmount > 1000000m)
                    {
                        totalAmount = 1000000m;
                    }

                    var hour = random.Next(9, 18); // 09:00-18:00 arası
                    var minute = random.Next(0, 60);
                    var second = random.Next(0, 60);

                    transactionHistory.Add(new TransactionHistory
                    {
                        PortfolioId = 1, // Burak'ın portföyü
                        CoinId = coinId,
                        TransactionType = transactionType,
                        Quantity = quantity,
                        Price = price,
                        TotalAmount = totalAmount,
                        TransactionDate = currentDate.AddHours(hour).AddMinutes(minute).AddSeconds(second)
                    });
                }
                currentDate = currentDate.AddDays(1);
            }

            await context.TransactionHistories.AddRangeAsync(transactionHistory);
            await context.SaveChangesAsync();

            Console.WriteLine("✅ Örnek veriler başarıyla eklendi!");
            Console.WriteLine($"📊 Toplam kayıt sayıları:");
            Console.WriteLine($"   - Kullanıcılar: {await context.Users.CountAsync()}");
            Console.WriteLine($"   - Portföyler: {await context.Portfolios.CountAsync()}");
            Console.WriteLine($"   - Portföy Kalemleri: {await context.PortfolioItems.CountAsync()}");
            Console.WriteLine($"   - İşlem Geçmişi: {await context.TransactionHistories.CountAsync()}");
            Console.WriteLine($"   - Favori Coinler: {await context.FavoriteCoins.CountAsync()}");
            Console.WriteLine($"   - Fiyat Alarmları: {await context.PriceAlerts.CountAsync()}");
            Console.WriteLine($"   - Feed Gönderileri: {await context.FeedPosts.CountAsync()}");
            Console.WriteLine($"   - Yorumlar: {await context.Comments.CountAsync()}");
            Console.WriteLine($"   - Beğeniler: {await context.Likes.CountAsync()}");
            Console.WriteLine($"   - Takip İlişkileri: {await context.Follows.CountAsync()}");
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
} 