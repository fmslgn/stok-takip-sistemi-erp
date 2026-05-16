using System.Windows;
using System.Windows.Controls;
using StokTakip.Wpf.Helpers;
using StokTakip.Wpf.ViewModels;

namespace StokTakip.Wpf.Views;

/// <summary>
/// WPF raporlama ekranında sayısal kartları, durum mesajını ve görsel rapor alanını gösterir.
/// </summary>
public partial class ReportsView : UserControl
{
    private readonly ReportsViewModel _viewModel = new();

    public ReportsView()
    {
        InitializeComponent();
        DataContext = _viewModel;
        Loaded += ReportsView_Loaded;
    }

    /// <summary>
    /// Rapor verilerini UI thread'i kilitlemeden arka planda yukler.
    /// </summary>
    private async void ReportsView_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= ReportsView_Loaded;

        try
        {
            await _viewModel.RaporlariYukleAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            DialogHelper.ShowWarning(
                $"Raporlama ekranı açılırken bir hata oluştu: {ex.Message}",
                "Raporlama",
                Window.GetWindow(this));
        }
    }

    /// <summary>Rapor kartlarındaki özet sayıları ve oranları PDF olarak dışa aktarır.</summary>
    private void BtnRaporOzetiPdfKaydet_Click(object sender, RoutedEventArgs e)
    {
        var owner = Window.GetWindow(this);
        PdfExportHelper.RaporOzetiPdfKaydet(owner!, _viewModel);
    }
}
