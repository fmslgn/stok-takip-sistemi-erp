using System.Windows;
using System.Windows.Controls;
using StokTakip.Wpf.ViewModels;

namespace StokTakip.Wpf.Views;

/// <summary>
/// WPF ürün yönetimi ekranının modern form ve liste iskeletini gösterir.
/// </summary>
public partial class ProductManagementView : UserControl
{
    private readonly ProductManagementViewModel _viewModel = new();

    public ProductManagementView()
    {
        InitializeComponent();
        DataContext = _viewModel;

        // Ürün, kategori ve saklama koşulu listeleri Business katmanından yüklenir.
        _viewModel.Yukle();
    }

    private void BtnListeYenile_Click(object sender, RoutedEventArgs e)
    {
        // WPF liste yenileme işlemi UI içinde SQL yazmadan manager üzerinden yapılır.
        _viewModel.Yukle();
    }

    private void BtnIslemIskeleti_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Bu işlem WPF tarafında sonraki aşamada Business katmanına bağlanacaktır.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void BtnSaklamaAsistani_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Saklama Asistanı WPF tarafında sonraki aşamada detaylandırılacaktır.", "Saklama Asistanı", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
