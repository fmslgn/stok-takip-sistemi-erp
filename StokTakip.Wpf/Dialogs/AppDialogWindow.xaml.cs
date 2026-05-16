using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace StokTakip.Wpf.Dialogs;

/// <summary>
/// Uygulama temasiyla uyumlu bilgi, uyari, hata ve onay mesajlarini gosteren ozel dialog penceresidir.
/// Owner uzerinde karartma/overlay kullanilmaz; yalnizca ortalanmis beyaz kart gosterilir.
/// </summary>
public partial class AppDialogWindow : Window
{
    private readonly bool _onayModu;

    /// <summary>Onay dialogunda Evet=true, Hayir/Kapat=false; tek butonlu dialoglarda Tamam=true.</summary>
    public bool DialogResultValue { get; private set; }

    public AppDialogWindow(string title, string message, AppDialogKind kind)
    {
        InitializeComponent();
        TxtTitle.Text = title;
        TxtMessage.Text = message;
        _onayModu = kind == AppDialogKind.Confirm;
        UygulaTurStili(kind);
        ButonlariAyarla(kind);
    }

    private void UygulaTurStili(AppDialogKind kind)
    {
        switch (kind)
        {
            case AppDialogKind.Success:
                IconBadge.Background = new SolidColorBrush(Color.FromRgb(220, 252, 231));
                IconText.Foreground = new SolidColorBrush(Color.FromRgb(22, 163, 74));
                IconText.Text = "✓";
                break;
            case AppDialogKind.Warning:
                IconBadge.Background = new SolidColorBrush(Color.FromRgb(255, 237, 213));
                IconText.Foreground = new SolidColorBrush(Color.FromRgb(234, 88, 12));
                IconText.Text = "!";
                break;
            case AppDialogKind.Error:
                IconBadge.Background = new SolidColorBrush(Color.FromRgb(254, 226, 226));
                IconText.Foreground = new SolidColorBrush(Color.FromRgb(220, 38, 38));
                IconText.Text = "✕";
                break;
            case AppDialogKind.Confirm:
                IconBadge.Background = (Brush)FindResource("PrimarySoftBrush");
                IconText.Foreground = (Brush)FindResource("PrimaryBrush");
                IconText.Text = "?";
                break;
            default:
                IconBadge.Background = (Brush)FindResource("PrimarySoftBrush");
                IconText.Foreground = (Brush)FindResource("PrimaryBrush");
                IconText.Text = "i";
                break;
        }
    }

    private void ButonlariAyarla(AppDialogKind kind)
    {
        if (kind == AppDialogKind.Confirm)
        {
            BtnOk.Visibility = Visibility.Collapsed;
            BtnYes.Visibility = Visibility.Visible;
            BtnNo.Visibility = Visibility.Visible;
            BtnYes.IsDefault = true;
            BtnOk.IsDefault = false;
            return;
        }

        BtnOk.Visibility = Visibility.Visible;
        BtnYes.Visibility = Visibility.Collapsed;
        BtnNo.Visibility = Visibility.Collapsed;
        BtnOk.IsDefault = true;
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        DialogResultValue = true;
        Close();
    }

    private void BtnYes_Click(object sender, RoutedEventArgs e)
    {
        DialogResultValue = true;
        Close();
    }

    private void BtnNo_Click(object sender, RoutedEventArgs e)
    {
        DialogResultValue = false;
        Close();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        DialogResultValue = !_onayModu;
        Close();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            DialogResultValue = false;
            Close();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter && _onayModu && BtnYes.IsVisible)
        {
            DialogResultValue = true;
            Close();
            e.Handled = true;
        }
    }
}

/// <summary>Dialog penceresinin gosterim turunu belirler.</summary>
public enum AppDialogKind
{
    Info,
    Success,
    Warning,
    Error,
    Confirm
}
