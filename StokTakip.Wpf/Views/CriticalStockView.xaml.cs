using System.Windows;
using System.Windows.Controls;
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
        _viewModel.Yukle();
    }

    private void BtnYenile_Click(object sender, RoutedEventArgs e)
    {
        // Kritik stok listesi Business katmanındaki UrunManager ile yenilenir.
        _viewModel.Yukle();
    }
}
