using CryptoGuard.MAUI.ViewModels;
using CryptoGuard.Core.Interfaces;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace CryptoGuard.MAUI.Views;

public partial class TransactionHistoryPage : ContentPage
{
    public TransactionHistoryPage(TransactionHistoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        Debug.WriteLine("TransactionHistoryPage: Page loaded successfully");
    }
} 