using System.Windows;
using System.Windows.Controls;
using StokTakip.Entities;
using StokTakip.Wpf.Helpers;
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
        Loaded += StockEntryView_Loaded;
    }

    private async void StockEntryView_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= StockEntryView_Loaded;

        try
        {
            await _viewModel.YukleAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            DialogHelper.ShowWarning($"Stok giriş verileri yüklenirken hata: {ex.Message}", "Stok Giriş", Window.GetWindow(this));
        }
    }

    private async void BtnKaydet_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var owner = Window.GetWindow(this);

            // Placeholder secili degilken kayit engellenir.
            if (CmbUrun.SelectedItem is not Urun seciliUrun)
            {
                DialogHelper.ShowWarning("Lütfen ürün seçiniz.", owner: owner);
                return;
            }

            if (!int.TryParse(TxtMiktar.Text, out int miktar))
            {
                DialogHelper.ShowWarning("Giriş miktarı sayısal olmalıdır.", owner: owner);
                return;
            }

            await _viewModel.StokGirisiKaydetAsync(seciliUrun, miktar, TxtAciklama.Text, DpTarih.SelectedDate ?? DateTime.Now).ConfigureAwait(true);
            DialogHelper.ShowSuccess("Stok girişi kaydedildi.", owner: Window.GetWindow(this));
            Temizle();
        }
        catch (Exception ex)
        {
            DialogHelper.ShowWarning(ex.Message, "Stok Giriş Hatası", Window.GetWindow(this));
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

    /// <summary>Stok giriş hareketleri listesini (DataGrid kaynağı) PDF olarak dışa aktarır.</summary>
    private void BtnStokGirisHareketleriPdfKaydet_Click(object sender, RoutedEventArgs e)
    {
        var owner = Window.GetWindow(this);
        PdfExportHelper.StokGirisHareketleriniPdfKaydet(owner!, _viewModel.StokGirisleri);
    }
}
