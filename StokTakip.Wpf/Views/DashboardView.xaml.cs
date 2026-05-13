using System.Windows.Controls;
using StokTakip.Wpf.ViewModels;

namespace StokTakip.Wpf.Views;

/// <summary>
/// Ana menü ekranında temel stok özetlerini kart düzeninde gösterir.
/// </summary>
public partial class DashboardView : UserControl
{
    private readonly DashboardViewModel _viewModel = new();

    public DashboardView()
    {
        InitializeComponent();
        DataContext = _viewModel;

        // Dashboard kartları Business katmanındaki rapor metotlarından beslenir.
        _viewModel.Yukle();
    }
}
