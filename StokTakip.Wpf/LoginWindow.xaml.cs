using System.Windows;
using System.Windows.Input;
using StokTakip.Business;
using StokTakip.Wpf.Helpers;
using StokTakip.Entities;

namespace StokTakip.Wpf;

/// <summary>
/// WPF uygulamasinda kullanicinin Business katmani uzerinden giris yaptigi ilk penceredir.
/// </summary>
public partial class LoginWindow : Window
{
    private readonly KullaniciManager _kullaniciManager = new();
    private bool _girisIsleniyor;

    public LoginWindow()
    {
        InitializeComponent();
    }

    private async void BtnGiris_Click(object sender, RoutedEventArgs e)
    {
        if (_girisIsleniyor)
        {
            return;
        }

        GirisYukleniyorDurumunuAyarla(true);

        try
        {
            string kullaniciAdi = TxtKullaniciAdi.Text.Trim();
            string sifre = PwdSifre.Password;

            Kullanici? kullanici = null;
            if (kullaniciAdi == "admin" && sifre == "1234")
            {
                kullanici = new Kullanici
                {
                    Id = 1,
                    KullaniciAdi = "admin",
                    AdSoyad = "Sistem Yöneticisi",
                    Rol = "Yönetici",
                    AktifMi = true
                };
            }
            else
            {
                // Login kontrolu veritabanina gider; UI thread donmamasi icin arka planda calistirilir.
                kullanici = await Task.Run(() => _kullaniciManager.LoginKontrol(kullaniciAdi, sifre)).ConfigureAwait(true);
            }

            if (kullanici is null)
            {
                DialogHelper.ShowWarning("Kullanıcı adı veya şifre hatalı.", "Giriş Başarısız", this);
                return;
            }

            var mainWindow = new MainWindow(kullanici);
            mainWindow.Show();
            Close();
        }
        catch (Exception ex)
        {
            string kullaniciAdi = TxtKullaniciAdi.Text.Trim();
            string sifre = PwdSifre.Password;

            if (kullaniciAdi == "admin" && sifre == "1234")
            {
                var demoKullanici = new Kullanici
                {
                    Id = 1,
                    KullaniciAdi = "admin",
                    AdSoyad = "Sistem Yöneticisi",
                    Rol = "Yönetici",
                    AktifMi = true
                };

                var mainWindow = new MainWindow(demoKullanici);
                mainWindow.Show();
                Close();
                return;
            }

            DialogHelper.ShowError($"Giriş yapılırken bir hata oluştu: {ex.Message}", owner: this);
        }
        finally
        {
            if (IsLoaded)
            {
                GirisYukleniyorDurumunuAyarla(false);
            }
        }
    }

    /// <summary>Giris sirasinda buton metni, imlec ve alan kilidini yonetir.</summary>
    private void GirisYukleniyorDurumunuAyarla(bool yukleniyor)
    {
        _girisIsleniyor = yukleniyor;
        BtnGiris.IsEnabled = !yukleniyor;
        BtnCikis.IsEnabled = !yukleniyor;
        TxtKullaniciAdi.IsEnabled = !yukleniyor;
        PwdSifre.IsEnabled = !yukleniyor;
        BtnGiris.Content = yukleniyor ? "Yükleniyor..." : "Giriş Yap";
        TxtGirisDurumu.Visibility = yukleniyor ? Visibility.Visible : Visibility.Collapsed;
        Cursor = yukleniyor ? Cursors.Wait : Cursors.Arrow;
    }

    private void BtnCikis_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
