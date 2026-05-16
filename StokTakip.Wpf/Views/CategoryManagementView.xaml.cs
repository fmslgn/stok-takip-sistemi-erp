using System.Windows;
using System.Windows.Controls;
using StokTakip.Wpf.Helpers;
using StokTakip.Wpf.ViewModels;

namespace StokTakip.Wpf.Views;

/// <summary>
/// Kategori kayıtlarını Business katmanı üzerinden yöneten WPF ekranıdır.
/// </summary>
public partial class CategoryManagementView : UserControl
{
    private readonly CategoryManagementViewModel _viewModel = new();

    public CategoryManagementView()
    {
        InitializeComponent();
        DataContext = _viewModel;
        Loaded += CategoryManagementView_Loaded;
    }

    private async void CategoryManagementView_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= CategoryManagementView_Loaded;

        try
        {
            await _viewModel.YukleAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            DialogHelper.ShowWarning($"Kategori listesi yüklenirken hata: {ex.Message}", "Kategori Yönetimi", Window.GetWindow(this));
        }
    }

    /// <summary>Kategori listesini PDF olarak dışa aktarır.</summary>
    private void BtnKategoriListesiPdfKaydet_Click(object sender, RoutedEventArgs e)
    {
        var owner = Window.GetWindow(this);
        PdfExportHelper.KategoriListesiniPdfKaydet(owner!, _viewModel.Kategoriler, "Kategori yönetimi ekranındaki kategori listesi.");
    }
}
