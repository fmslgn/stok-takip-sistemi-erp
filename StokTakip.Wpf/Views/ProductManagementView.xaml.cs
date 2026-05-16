using System.Windows;
using System.Windows.Controls;
using StokTakip.Wpf.Helpers;
using StokTakip.Wpf.ViewModels;

namespace StokTakip.Wpf.Views;

/// <summary>
/// WPF Ürün Yönetimi ekranında ViewModel bağlaması ve veri yükleme işlemlerini yürütür.
/// </summary>
public partial class ProductManagementView : UserControl
{
    private readonly ProductManagementViewModel _viewModel = new();

    public ProductManagementView()
    {
        InitializeComponent();
        DataContext = _viewModel;
        Loaded += ProductManagementView_Loaded;
    }

    /// <summary>
    /// Ürün, kategori ve saklama koşulu listelerini UI thread'i kilitlemeden yükler.
    /// </summary>
    private async void ProductManagementView_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= ProductManagementView_Loaded;

        try
        {
            await _viewModel.YukleAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            DialogHelper.ShowWarning(
                $"Ürün yönetimi verileri yüklenirken beklenmeyen bir hata oluştu: {ex.Message}",
                "Ürün Yönetimi",
                Window.GetWindow(this));
        }
    }

    /// <summary>Filtrelenmiş ürün listesini ayrı salt okunur pencerede büyütür.</summary>
    private void BtnUrunListesiBuyut_Click(object sender, RoutedEventArgs e)
    {
        var owner = Window.GetWindow(this);
        var pencere = new ProductListPreviewWindow(_viewModel.Urunler)
        {
            Owner = owner
        };
        pencere.Show();
    }

    /// <summary>Ürün yönetiminde görünen (filtrelenmiş) listeyi PDF olarak kaydeder.</summary>
    private void BtnUrunListesiPdfKaydet_Click(object sender, RoutedEventArgs e)
    {
        var owner = Window.GetWindow(this);
        PdfExportHelper.UrunListesiniPdfKaydet(owner!, _viewModel.Urunler, "Ürün yönetimi ekranındaki mevcut ürün listesi.");
    }
}
