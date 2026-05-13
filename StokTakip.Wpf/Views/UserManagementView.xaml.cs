using System.Windows;
using System.Windows.Controls;
using StokTakip.Wpf.ViewModels;

namespace StokTakip.Wpf.Views;

/// <summary>
/// Kullanıcı kayıtlarının WPF tarafında listelendiği ve yönetim iskeletinin sunulduğu ekrandır.
/// </summary>
public partial class UserManagementView : UserControl
{
    private readonly UserManagementViewModel _viewModel = new();

    public UserManagementView()
    {
        InitializeComponent();
        DataContext = _viewModel;
        _viewModel.Yukle();
    }

    private void BtnIslemIskeleti_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Kullanıcı işlemleri WPF tarafında sonraki aşamada Business katmanına bağlanacaktır.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
