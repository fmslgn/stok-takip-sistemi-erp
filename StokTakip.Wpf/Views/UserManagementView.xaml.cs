using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using StokTakip.Wpf.Helpers;
using StokTakip.Wpf.ViewModels;

namespace StokTakip.Wpf.Views;

/// <summary>
/// Kullanıcı kayıtlarını Business katmanı üzerinden yöneten WPF ekranıdır.
/// </summary>
public partial class UserManagementView : UserControl
{
    private readonly UserManagementViewModel _viewModel = new();
    private bool _sifreSenkronizeEdiliyor;

    public UserManagementView()
    {
        InitializeComponent();
        DataContext = _viewModel;
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        Loaded += UserManagementView_Loaded;
    }

    private async void UserManagementView_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= UserManagementView_Loaded;

        try
        {
            await _viewModel.YukleAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            DialogHelper.ShowWarning($"Kullanıcı listesi yüklenirken hata: {ex.Message}", "Kullanıcı Yönetimi", Window.GetWindow(this));
        }
    }

    /// <summary>PasswordBox değerini ViewModel ile senkronize eder.</summary>
    private void PwdSifre_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (_sifreSenkronizeEdiliyor)
        {
            return;
        }

        _viewModel.Sifre = PwdSifre.Password;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not (nameof(UserManagementViewModel.Sifre) or nameof(UserManagementViewModel.SeciliKullanici)))
        {
            return;
        }

        _sifreSenkronizeEdiliyor = true;
        PwdSifre.Password = _viewModel.Sifre;
        _sifreSenkronizeEdiliyor = false;
    }

    /// <summary>Kullanıcı listesini PDF olarak dışa aktarır.</summary>
    private void BtnKullaniciListesiPdfKaydet_Click(object sender, RoutedEventArgs e)
    {
        var owner = Window.GetWindow(this);
        PdfExportHelper.KullaniciListesiniPdfKaydet(owner!, _viewModel.Kullanicilar, "Kullanıcı yönetimi ekranındaki kullanıcı listesi.");
    }
}
