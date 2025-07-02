﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using CryptoGuard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using CryptoGuard.Core.Interfaces;
using CryptoGuard.Infrastructure.Repositories;
using CryptoGuard.Infrastructure.Services;
using CryptoGuard.Services.Services;
using CryptoGuard.MAUI.ViewModels;
using CryptoGuard.MAUI.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using LiveChartsCore.SkiaSharpView.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace CryptoGuard.MAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseLiveCharts()
			.UseSkiaSharp()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// appsettings.json'dan connection string oku
		var config = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: true)
			.Build();

		var connectionString = config.GetConnectionString("DefaultConnection");
		builder.Services.AddDbContextFactory<CryptoGuardDbContext>(options =>
			options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));

		// Repository
		builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

		// API Servisleri
		builder.Services.AddSingleton<ICoinLoreService, CoinLoreService>();
		builder.Services.AddScoped<IVoiceRecognitionService, VoiceRecognitionService>();

		// Uygulama Servisleri
		builder.Services.AddScoped<IUserService, UserService>();
		builder.Services.AddScoped<IPortfolioService, PortfolioService>();
		builder.Services.AddScoped<IPriceAlertService, PriceAlertService>();
		builder.Services.AddScoped<IFeedPostService, FeedPostService>();

		// ViewModel'ler
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<RegisterViewModel>();
		builder.Services.AddTransient<MainViewModel>();
		builder.Services.AddTransient<AllCoinsViewModel>();
		builder.Services.AddTransient<PortfolioViewModel>();
		builder.Services.AddTransient<AddCoinPopupViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();
		builder.Services.AddTransient<FeedViewModel>();
		builder.Services.AddTransient<ProfileViewModel>();
		builder.Services.AddTransient<ForgotPasswordViewModel>();
		builder.Services.AddTransient<ResetPasswordViewModel>();
		builder.Services.AddTransient<CryptoGuardAIViewModel>();
		builder.Services.AddTransient<TransactionHistoryViewModel>();

		// Sayfalar
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RegisterPage>();
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<AllCoinsPage>();
		builder.Services.AddTransient<PortfolioPage>();
		builder.Services.AddTransient<AddCoinPopup>();
		builder.Services.AddTransient<ForgotPasswordPage>();
		builder.Services.AddTransient<ResetPasswordPage>();
		builder.Services.AddTransient<CryptoGuardAIPage>();
		builder.Services.AddTransient<TransactionHistoryPage>();

		builder.Configuration.AddConfiguration(config);

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();
		App.ServiceProvider = app.Services;

		// Örnek verileri ekle - Arka planda asenkron
		Task.Run(async () =>
		{
			try
			{
				using (var scope = app.Services.CreateScope())
				{
					var db = scope.ServiceProvider.GetRequiredService<CryptoGuardDbContext>();
					
					// Veritabanını silip yeniden oluşturma kodunu kaldırdım
					// await db.Database.EnsureDeletedAsync();
					// await db.Database.EnsureCreatedAsync();
					// System.Diagnostics.Debug.WriteLine("🗑️ Veritabanı sıfırlandı ve yeniden oluşturuldu");
					
					// Sadece migration uygula
					await db.Database.MigrateAsync();
					
					await DbInitializer.SeedDataAsync(db);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Veritabanı seed hatası: " + ex.ToString());
			}
		});

		return app;
	}
}
