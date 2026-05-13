using System.Windows;
using System.Windows.Controls;
using StokTakip.Entities;
using StokTakip.Wpf.Views;

namespace StokTakip.Wpf;

/// <summary>
/// WPF ERP ana penceresinde sol menü ve orta içerik alanını yöneten penceredir.
/// </summary>
public partial class MainWindow : Window
{
    private readonly Kullanici? _aktifKullanici;

    public MainWindow()
        : this(null)
    {
    }

    public MainWindow(Kullanici? aktifKullanici)
    {
        _aktifKullanici = aktifKullanici;
        InitializeComponent();
        DataContext = this;
        ShowDashboard();
    }

    public string AktifKullaniciBilgisi =>
        _aktifKullanici is null
            ? "Giriş yapan kullanıcı: admin - yönetici"
            : $"Giriş yapan kullanıcı: {_aktifKullanici.KullaniciAdi} - {_aktifKullanici.Rol}";

    private void BtnDashboard_Click(object sender, RoutedEventArgs e)
    {
        ShowDashboard();
    }

    private void BtnUsers_Click(object sender, RoutedEventArgs e)
    {
        GuvenliSayfaGoster("Kullanıcı Yönetimi", () => new UserManagementView(), "Kullanıcı yönetimi ekranı açılırken bir hata oluştu.");
    }

    private void BtnCategories_Click(object sender, RoutedEventArgs e)
    {
        GuvenliSayfaGoster("Kategori Yönetimi", () => new CategoryManagementView(), "Kategori yönetimi ekranı açılırken bir hata oluştu.");
    }

    private void BtnProducts_Click(object sender, RoutedEventArgs e)
    {
        GuvenliSayfaGoster("Ürün Yönetimi", () => new ProductManagementView(), "Ürün yönetimi ekranı açılırken bir hata oluştu.");
    }

    private void BtnStockEntry_Click(object sender, RoutedEventArgs e)
    {
        GuvenliSayfaGoster("Stok Giriş", () => new StockEntryView(), "Stok giriş ekranı açılırken bir hata oluştu.");
    }

    private void BtnStockExit_Click(object sender, RoutedEventArgs e)
    {
        GuvenliSayfaGoster("Stok Çıkış", () => new StockExitView(), "Stok çıkış ekranı açılırken bir hata oluştu.");
    }

    private void BtnCriticalStock_Click(object sender, RoutedEventArgs e)
    {
        GuvenliSayfaGoster("Kritik Stok", () => new CriticalStockView(), "Kritik stok ekranı açılırken bir hata oluştu.");
    }

    private void BtnReports_Click(object sender, RoutedEventArgs e)
    {
        GuvenliSayfaGoster("Raporlama", () => new ReportsView(), "Raporlama ekranı açılırken bir hata oluştu.");
    }

    private void BtnStorageAssistant_Click(object sender, RoutedEventArgs e)
    {
        GuvenliSayfaGoster("Saklama Asistanı", () => new StorageAssistantView(), "Saklama Asistanı ekranı açılırken bir hata oluştu.");
    }

    private void BtnExit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void ShowDashboard()
    {
        GuvenliSayfaGoster("Ana Menü", () => new DashboardView(), "Ana Menü ekranı açılırken bir hata oluştu.");
    }

    /// <summary>
    /// Sol menüden seçilen WPF ekranını ortak içerik alanında güvenli şekilde gösterir.
    /// </summary>
    private void GuvenliSayfaGoster(string baslik, Func<object> viewFactory, string hataMesaji)
    {
        try
        {
            TxtSayfaBasligi.Text = baslik;
            ContentHost.Content = viewFactory();
        }
        catch
        {
            // View oluşturulurken hata olursa uygulama kapanmaz; kullanıcıya anlaşılır bilgi verilir.
            MessageBox.Show(hataMesaji, baslik, MessageBoxButton.OK, MessageBoxImage.Warning);
            ContentHost.Content = HataIcerigiOlustur(baslik, baslik == "Raporlama" ? "Rapor verileri yüklenemedi." : hataMesaji);
        }
    }

    private static object HataIcerigiOlustur(string baslik, string mesaj)
    {
        return new Border
        {
            Padding = new Thickness(24),
            Child = new StackPanel
            {
                Children =
                {
                    new TextBlock
                    {
                        Text = baslik,
                        FontSize = 24,
                        FontWeight = FontWeights.Bold
                    },
                    new TextBlock
                    {
                        Text = mesaj,
                        Margin = new Thickness(0, 10, 0, 0),
                        TextWrapping = TextWrapping.Wrap
                    }
                }
            }
        };
    }
}
