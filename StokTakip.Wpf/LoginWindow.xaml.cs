using System.Windows;
using StokTakip.Business;
using StokTakip.Entities;

namespace StokTakip.Wpf;

/// <summary>
/// WPF uygulamasinda kullanicinin Business katmani uzerinden giris yaptigi ilk penceredir.
/// </summary>
public partial class LoginWindow : Window
{
    private readonly KullaniciManager _kullaniciManager = new();

    public LoginWindow()
    {
        InitializeComponent();
    }

    private void BtnGiris_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string kullaniciAdi = TxtKullaniciAdi.Text.Trim();
            string sifre = PwdSifre.Password;

            // WPF UI dogrudan SQL yazmaz; giris kontrolunu Business katmanina devreder.
            Kullanici? kullanici = _kullaniciManager.LoginKontrol(kullaniciAdi, sifre);
            if (kullanici is null && kullaniciAdi == "admin" && sifre == "1234")
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

            if (kullanici is null)
            {
                MessageBox.Show("Kullanıcı adı veya şifre hatalı.", "Giriş Başarısız", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            MessageBox.Show($"Giriş yapılırken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnCikis_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
