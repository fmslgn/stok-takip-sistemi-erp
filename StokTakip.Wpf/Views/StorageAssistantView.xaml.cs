using System.Windows;
using System.Windows.Controls;
using StokTakip.Business;

namespace StokTakip.Wpf.Views;

/// <summary>
/// Saklama koşulu önerisi ve sohbet cevaplarını Business katmanındaki manager üzerinden üreten WPF ekrandır.
/// </summary>
public partial class StorageAssistantView : UserControl
{
    private readonly SaklamaKosuluManager _saklamaKosuluManager = new();
    private string _mevcutOneri = string.Empty;

    public StorageAssistantView()
    {
        InitializeComponent();
        SohbeteEkle("Asistan", "Merhaba, ürün adı ve kategori girerek saklama önerisi alabilirsiniz.");
    }

    private async void BtnOneriAl_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(TxtUrunAdi.Text))
            {
                MessageBox.Show("Ürün adı boş bırakılamaz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Web destekli öneri alınamazsa manager kural tabanlı yedek öneriye düşer.
            _mevcutOneri = await _saklamaKosuluManager.SaklamaKosuluOnerWebDestekliAsync(TxtUrunAdi.Text, TxtKategori.Text, TxtBarkod.Text);
            SohbeteEkle("Asistan", _mevcutOneri);
        }
        catch
        {
            _mevcutOneri = _saklamaKosuluManager.SaklamaKosuluOner(TxtUrunAdi.Text, TxtKategori.Text);
            SohbeteEkle("Asistan", $"Şu anda detaylı yanıt üretilemedi, kural tabanlı öneri üzerinden devam ediyorum. {_mevcutOneri}");
        }
    }

    private void BtnGonder_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string mesaj = TxtKullaniciMesaji.Text.Trim();
            if (string.IsNullOrWhiteSpace(mesaj))
            {
                MessageBox.Show("Mesaj boş bırakılamaz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SohbeteEkle("Kullanıcı", mesaj);

            // Chatbot cevabı WPF içinde üretilmez; kural tabanlı mantık Business katmanında çalışır.
            string yanit = _saklamaKosuluManager.SaklamaChatbotYanitiUret(TxtUrunAdi.Text, TxtKategori.Text, TxtBarkod.Text, _mevcutOneri, mesaj);
            _mevcutOneri = string.IsNullOrWhiteSpace(_mevcutOneri) ? yanit : _mevcutOneri;
            SohbeteEkle("Asistan", yanit);
            TxtKullaniciMesaji.Clear();
        }
        catch
        {
            SohbeteEkle("Asistan", "Şu anda detaylı yanıt üretilemedi, kural tabanlı öneri üzerinden devam ediyorum.");
        }
    }

    private void BtnTemizle_Click(object sender, RoutedEventArgs e)
    {
        TxtUrunAdi.Clear();
        TxtKategori.Clear();
        TxtBarkod.Clear();
        TxtKullaniciMesaji.Clear();
        TxtChatGecmisi.Clear();
        _mevcutOneri = string.Empty;
        SohbeteEkle("Asistan", "Merhaba, ürün adı ve kategori girerek saklama önerisi alabilirsiniz.");
    }

    private void SohbeteEkle(string kim, string mesaj)
    {
        TxtChatGecmisi.AppendText($"{kim}: {mesaj}{Environment.NewLine}{Environment.NewLine}");
        TxtChatGecmisi.ScrollToEnd();
    }
}
