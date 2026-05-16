using System.Windows;
using System.Windows.Controls;
using StokTakip.Entities;
using StokTakip.Wpf.Helpers;
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
        Loaded += StockExitView_Loaded;
    }

    private async void StockExitView_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= StockExitView_Loaded;

        try
        {
            await _viewModel.YukleAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            DialogHelper.ShowWarning($"Stok çıkış verileri yüklenirken hata: {ex.Message}", "Stok Çıkış", Window.GetWindow(this));
        }
    }

    private async void BtnKaydet_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var owner = Window.GetWindow(this);

            if (CmbUrun.SelectedItem is not Urun seciliUrun)
            {
                DialogHelper.ShowWarning("Lütfen ürün seçiniz.", owner: owner);
                return;
            }

            if (!int.TryParse(TxtMiktar.Text, out int miktar))
            {
                DialogHelper.ShowWarning("Çıkış miktarı sayısal olmalıdır.", owner: owner);
                return;
            }

            await _viewModel.StokCikisiKaydetAsync(seciliUrun, miktar, TxtAciklama.Text, DpTarih.SelectedDate ?? DateTime.Now).ConfigureAwait(true);
            DialogHelper.ShowSuccess("Stok çıkışı kaydedildi.", owner: Window.GetWindow(this));
            Temizle();
        }
        catch (Exception ex)
        {
            DialogHelper.ShowWarning(ex.Message, "Stok Çıkış Hatası", Window.GetWindow(this));
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

    /// <summary>Stok çıkış hareketleri listesini (DataGrid kaynağı) PDF olarak dışa aktarır.</summary>
    private void BtnStokCikisHareketleriPdfKaydet_Click(object sender, RoutedEventArgs e)
    {
        var owner = Window.GetWindow(this);
        PdfExportHelper.StokCikisHareketleriniPdfKaydet(owner!, _viewModel.StokCikislari);
    }
}
