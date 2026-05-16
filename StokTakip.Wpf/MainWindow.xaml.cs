using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using StokTakip.Entities;
using StokTakip.Wpf.Helpers;
using StokTakip.Wpf.Views;

namespace StokTakip.Wpf;

/// <summary>
/// WPF ERP ana penceresinde sol menü ve orta içerik alanını yöneten penceredir.
/// </summary>
public partial class MainWindow : Window
{
    private const string SidebarAktifEtiket = "Active";

    private readonly Kullanici? _aktifKullanici;
    private readonly IReadOnlyList<Button> _sidebarNavButonlari;

    public MainWindow()
        : this(null)
    {
    }

    public MainWindow(Kullanici? aktifKullanici)
    {
        _aktifKullanici = aktifKullanici;
        InitializeComponent();
        DataContext = this;

        _sidebarNavButonlari = new List<Button>
        {
            BtnNavDashboard,
            BtnNavUsers,
            BtnNavCategories,
            BtnNavProducts,
            BtnNavStockEntry,
            BtnNavStockExit,
            BtnNavCriticalStock,
            BtnNavReports
        };

        ShowDashboard();
    }

    public string AktifKullaniciBilgisi =>
        _aktifKullanici is null
            ? "Giriş yapan kullanıcı: admin - Yönetici"
            : $"Giriş yapan kullanıcı: {_aktifKullanici.KullaniciAdi} - {_aktifKullanici.Rol}";

    private void BtnDashboard_Click(object sender, RoutedEventArgs e)
    {
        ShowDashboard();
    }

    private void BtnUsers_Click(object sender, RoutedEventArgs e) =>
        ModulAc(BtnNavUsers, "Kullanıcı Yönetimi", () => new UserManagementView(), "Kullanıcı yönetimi ekranı açılırken bir hata oluştu.");

    private void BtnCategories_Click(object sender, RoutedEventArgs e) =>
        ModulAc(BtnNavCategories, "Kategori Yönetimi", () => new CategoryManagementView(), "Kategori yönetimi ekranı açılırken bir hata oluştu.");

    private void BtnProducts_Click(object sender, RoutedEventArgs e) =>
        ModulAc(BtnNavProducts, "Ürün Yönetimi", () => new ProductManagementView(), "Ürün yönetimi ekranı açılırken bir hata oluştu.");

    private void BtnStockEntry_Click(object sender, RoutedEventArgs e) =>
        ModulAc(BtnNavStockEntry, "Stok Giriş", () => new StockEntryView(), "Stok giriş ekranı açılırken bir hata oluştu.");

    private void BtnStockExit_Click(object sender, RoutedEventArgs e) =>
        ModulAc(BtnNavStockExit, "Stok Çıkış", () => new StockExitView(), "Stok çıkış ekranı açılırken bir hata oluştu.");

    private void BtnCriticalStock_Click(object sender, RoutedEventArgs e) =>
        ModulAc(BtnNavCriticalStock, "Kritik Stok", () => new CriticalStockView(), "Kritik stok ekranı açılırken bir hata oluştu.");

    private void BtnReports_Click(object sender, RoutedEventArgs e) =>
        ModulAc(BtnNavReports, "Raporlama", () => new ReportsView(), "Raporlama ekranı açılırken bir hata oluştu.");

    /// <summary>Çıkış öncesi tema uyumlu onay alır; yalnızca Evet seçilirse uygulama kapanır.</summary>
    private void BtnExit_Click(object sender, RoutedEventArgs e)
    {
        if (!DialogHelper.ShowConfirm(
                "Uygulamadan çıkış yapmak istediğinize emin misiniz?",
                "Çıkış Onayı",
                this))
        {
            return;
        }

        Application.Current.Shutdown();
    }

    /// <summary>
    /// Ana menu ekranini acar ve sol menude Ana Menu secimini vurgular.
    /// </summary>
    private void ShowDashboard()
    {
        SidebarAktifMenudeSec(BtnNavDashboard);
        GuvenliSayfaGoster("Ana Menü", OlusturDashboardView, "Ana Menü ekranı açılırken bir hata oluştu.");
    }

    /// <summary>
    /// Dashboard olusturur ve hızlı erişim taleplerini ana penceredeki modül geçişine bağlar.
    /// </summary>
    private object OlusturDashboardView()
    {
        var view = new DashboardView();
        view.HizliErisimTalepEdildi += DashboardView_HizliErisimTalepEdildi;
        return view;
    }

    /// <summary>
    /// Ana menüden gelen hızlı erişim isteğini sol menü vurgusu ile birlikte ilgili modüle yönlendirir.
    /// </summary>
    private void DashboardView_HizliErisimTalepEdildi(object? sender, DashboardHizliErisimEventArgs e)
    {
        switch (e.Hedef)
        {
            case DashboardHizliErisimHedef.UrunYonetimi:
                ModulAc(BtnNavProducts, "Ürün Yönetimi", () => new ProductManagementView(), "Ürün yönetimi ekranı açılırken bir hata oluştu.");
                break;
            case DashboardHizliErisimHedef.KategoriYonetimi:
                ModulAc(BtnNavCategories, "Kategori Yönetimi", () => new CategoryManagementView(), "Kategori yönetimi ekranı açılırken bir hata oluştu.");
                break;
            case DashboardHizliErisimHedef.StokGiris:
                ModulAc(BtnNavStockEntry, "Stok Giriş", () => new StockEntryView(), "Stok giriş ekranı açılırken bir hata oluştu.");
                break;
            case DashboardHizliErisimHedef.StokCikis:
                ModulAc(BtnNavStockExit, "Stok Çıkış", () => new StockExitView(), "Stok çıkış ekranı açılırken bir hata oluştu.");
                break;
            case DashboardHizliErisimHedef.KritikStok:
                ModulAc(BtnNavCriticalStock, "Kritik Stok", () => new CriticalStockView(), "Kritik stok ekranı açılırken bir hata oluştu.");
                break;
            case DashboardHizliErisimHedef.Raporlama:
                ModulAc(BtnNavReports, "Raporlama", () => new ReportsView(), "Raporlama ekranı açılırken bir hata oluştu.");
                break;
        }
    }

    /// <summary>
    /// Sol menüde ilgili öğeyi aktif gösterir ve içerik alanında modül ekranını açar (menü tıklamaları ile aynı mantık).
    /// </summary>
    private void ModulAc(Button sidebarButonu, string baslik, Func<object> viewFactory, string hataMesaji)
    {
        SidebarAktifMenudeSec(sidebarButonu);
        GuvenliSayfaGoster(baslik, viewFactory, hataMesaji);
    }

    /// <summary>
    /// Sol menüde yalnızca navigasyon butonlarından birini mor-mavi aktif görünüme alır; Çıkış bu listeye dahil değildir.
    /// </summary>
    private void SidebarAktifMenudeSec(object sender)
    {
        if (sender is not Button aktifButon)
        {
            return;
        }

        foreach (var buton in _sidebarNavButonlari)
        {
            buton.Tag = ReferenceEquals(buton, aktifButon) ? SidebarAktifEtiket : null;
        }
    }

    /// <summary>
    /// Ust baslik altindaki aciklama metnini acilan sayfaya gore gunceller.
    /// </summary>
    private void AciklamayiBasligaGoreAyarla(string baslik)
    {
        TxtSayfaAciklamasi.Text = baslik switch
        {
            "Ana Menü" => "Stok durumunuzu ve modüllere hızlı erişimi buradan takip edin.",
            "Kullanıcı Yönetimi" => "Sistem kullanıcılarını ve rollerini yönetin.",
            "Kategori Yönetimi" => "Ürün kategorilerini düzenleyin ve takip edin.",
            "Ürün Yönetimi" => "Ürün ekleme, güncelleme, silme ve listeleme işlemleri.",
            "Stok Giriş" => "Depoya giren ürün miktarlarını kaydedin.",
            "Stok Çıkış" => "Depodan çıkan ürün miktarlarını kaydedin.",
            "Kritik Stok" => "Kritik seviyeye düşen ürünleri görüntüleyin.",
            "Raporlama" => "Stok durumunu sayısal ve görsel raporlarla takip edin.",
            _ => "Seçili modül içeriği aşağıda gösterilir."
        };
    }

    /// <summary>
    /// Sol menuden secilen WPF ekranını ortak içerik alanında güvenli şekilde gösterir.
    /// </summary>
    private void GuvenliSayfaGoster(string baslik, Func<object> viewFactory, string hataMesaji)
    {
        try
        {
            TxtSayfaBasligi.Text = baslik;
            AciklamayiBasligaGoreAyarla(baslik);
            ContentHost.Content = viewFactory();
        }
        catch
        {
            // View oluşturulurken hata olursa uygulama kapanmaz; kullanıcıya anlaşılır bilgi verilir.
            DialogHelper.ShowWarning(hataMesaji, baslik, this);
            AciklamayiBasligaGoreAyarla(baslik);
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
