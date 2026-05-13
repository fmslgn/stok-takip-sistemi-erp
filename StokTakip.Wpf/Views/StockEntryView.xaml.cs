using System.Windows;
using System.Windows.Controls;
using StokTakip.Entities;
using StokTakip.Wpf.ViewModels;

namespace StokTakip.Wpf.Views;

/// <summary>
/// Stok giriş hareketlerinin Business katmanı üzerinden kaydedildiği WPF ekrandır.
/// </summary>
public partial class StockEntryView : UserControl
{
    private readonly StockEntryViewModel _viewModel = new();

    public StockEntryView()
    {
        InitializeComponent();
        DataContext = _viewModel;
        DpTarih.SelectedDate = DateTime.Today;
        _viewModel.Yukle();
    }

    private void BtnKaydet_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!int.TryParse(TxtMiktar.Text, out int miktar))
            {
                MessageBox.Show("Giriş miktarı sayısal olmalıdır.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kayıt işlemi WPF içinde SQL yazmadan StokGirisManager üzerinden yürür.
            _viewModel.StokGirisiKaydet(CmbUrun.SelectedItem as Urun, miktar, TxtAciklama.Text, DpTarih.SelectedDate ?? DateTime.Now);
            MessageBox.Show("Stok girişi kaydedildi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
            Temizle();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Stok Giriş Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void BtnTemizle_Click(object sender, RoutedEventArgs e)
    {
        Temizle();
    }

    private void Temizle()
    {
        CmbUrun.SelectedIndex = -1;
        TxtMiktar.Clear();
        TxtAciklama.Clear();
        DpTarih.SelectedDate = DateTime.Today;
    }
}
