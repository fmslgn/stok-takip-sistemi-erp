using System.Windows;
using System.Windows.Controls;
using StokTakip.Wpf.ViewModels;

namespace StokTakip.Wpf.Views;

/// <summary>
/// Kategori kayıtlarını listeleyen ve kategori yönetimi için WPF form iskeleti sunan ekrandır.
/// </summary>
public partial class CategoryManagementView : UserControl
{
    private readonly CategoryManagementViewModel _viewModel = new();

    public CategoryManagementView()
    {
        InitializeComponent();
        DataContext = _viewModel;
        _viewModel.Yukle();
    }

    private void BtnIslemIskeleti_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Kategori işlemleri WPF tarafında sonraki aşamada Business katmanına bağlanacaktır.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
