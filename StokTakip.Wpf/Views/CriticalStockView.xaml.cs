using System.Windows;
using System.Windows.Controls;
using StokTakip.Wpf.Helpers;
using StokTakip.Wpf.ViewModels;

namespace StokTakip.Wpf.Views;

/// <summary>
/// Kritik stok seviyesine düşen ürünleri WPF DataGrid üzerinde gösteren ekrandır.
/// </summary>
public partial class CriticalStockView : UserControl
{
    private readonly CriticalStockViewModel _viewModel = new();

    public CriticalStockView()
    {
        InitializeComponent();
        DataContext = _viewModel;
        Loaded += CriticalStockView_Loaded;
    }

    private async void CriticalStockView_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= CriticalStockView_Loaded;

        try
        {
            await _viewModel.YukleAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            DialogHelper.ShowWarning($"Kritik stok listesi yüklenirken hata: {ex.Message}", "Kritik Stok", Window.GetWindow(this));
        }
    }

    /// <summary>Kritik stok listesindeki ürünleri PDF olarak dışa aktarır.</summary>
    private void BtnKritikStokPdfKaydet_Click(object sender, RoutedEventArgs e)
    {
        var owner = Window.GetWindow(this);
        PdfExportHelper.KritikStokListesiniPdfKaydet(owner!, _viewModel.KritikUrunler, "Kritik stok ekranındaki ürün listesi.");
    }
}
