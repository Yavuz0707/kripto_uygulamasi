using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryptoGuard.Core.Interfaces;
using CryptoGuard.Core.Models;
using System.Threading.Tasks;

namespace CryptoGuard.MAUI.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IPortfolioService _portfolioService;

        private string? username;
        public string? Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        private string? email;
        public string? Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        private string? password;
        public string? Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private string? confirmPassword;
        public string? ConfirmPassword
        {
            get => confirmPassword;
            set => SetProperty(ref confirmPassword, value);
        }

        private string? errorMessage;
        public string? ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        public bool IsNotBusy => !IsBusy;

        public RegisterViewModel(IUserService userService, IPortfolioService portfolioService)
        {
            _userService = userService;
            _portfolioService = portfolioService;
            Title = "Register";
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || 
                string.IsNullOrWhiteSpace(Email) || 
                string.IsNullOrWhiteSpace(Password) || 
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "Lütfen tüm alanları doldurunuz";
                return;
            }

            if (!Email.Contains("@"))
            {
                ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz (@ işareti gerekli)";
                return;
            }

            if (Password.Length < 8)
            {
                ErrorMessage = "Şifre en az 8 karakter olmalıdır";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Şifreler uyuşmuyor";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var user = new User
                {
                    Username = Username,
                    Email = Email,
                    PasswordHash = "temp"
                };

                var createdUser = await _userService.CreateUserAsync(user, Password);
                await _portfolioService.CreatePortfolioAsync(new Portfolio
                {
                    UserId = createdUser.Id,
                    Name = "Varsayılan Portföy"
                });
                
                await Shell.Current.DisplayAlert("Başarılı", "Hesabınız başarıyla oluşturuldu! Şimdi giriş yapabilirsiniz.", "Tamam");
                
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Kayıt hatası: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("LoginPage");
        }
    }
} 