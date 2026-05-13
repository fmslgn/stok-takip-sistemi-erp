using System.Windows;
using System.Windows.Controls;
using StokTakip.Entities;
using StokTakip.Wpf.ViewModels;

namespace StokTakip.Wpf.Views;

/// <summary>
/// Stok çıkış hareketlerinin Business katmanı üzerinden kaydedildiği WPF ekrandır.
/// </summary>
public partial class StockExitView : UserControl
{
    private readonly StockExitViewModel _viewModel = new();

    public StockExitView()
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
                MessageBox.Show("Çıkış miktarı sayısal olmalıdır.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Yetersiz stok kontrolü StokCikisManager içinde yapılır.
            _viewModel.StokCikisiKaydet(CmbUrun.SelectedItem as Urun, miktar, TxtAciklama.Text, DpTarih.SelectedDate ?? DateTime.Now);
            MessageBox.Show("Stok çıkışı kaydedildi.", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
            Temizle();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Stok Çıkış Hatası", MessageBoxButton.OK, MessageBoxImage.Warning);
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
