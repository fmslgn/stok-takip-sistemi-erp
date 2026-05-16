using System.Windows;
using StokTakip.Wpf.Dialogs;

namespace StokTakip.Wpf.Helpers;

/// <summary>
/// WPF uygulamasinda tema uyumlu bilgi, uyari, hata ve onay mesajlarini AppDialogWindow uzerinden gosterir.
/// </summary>
public static class DialogHelper
{
    public static void ShowInfo(string message, string title = "Bilgi", Window? owner = null) =>
        Goster(AppDialogKind.Info, message, title, owner);

    public static void ShowSuccess(string message, string title = "Başarılı", Window? owner = null) =>
        Goster(AppDialogKind.Success, message, title, owner);

    public static void ShowWarning(string message, string title = "Uyarı", Window? owner = null) =>
        Goster(AppDialogKind.Warning, message, title, owner);

    public static void ShowError(string message, string title = "Hata", Window? owner = null) =>
        Goster(AppDialogKind.Error, message, title, owner);

    /// <summary>Onay dialogu; kullanici Evet derse true, Hayir veya kapatma false doner.</summary>
    public static bool ShowConfirm(string message, string title = "Onay", Window? owner = null)
    {
        var pencere = new AppDialogWindow(title, message, AppDialogKind.Confirm)
        {
            Owner = owner ?? AktifSahipPencere()
        };
        pencere.ShowDialog();
        return pencere.DialogResultValue;
    }

    private static void Goster(AppDialogKind kind, string message, string title, Window? owner)
    {
        var pencere = new AppDialogWindow(title, message, kind)
        {
            Owner = owner ?? AktifSahipPencere()
        };
        pencere.ShowDialog();
    }

    private static Window? AktifSahipPencere()
    {
        if (Application.Current is null)
        {
            return null;
        }

        var aktif = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
        if (aktif is not null)
        {
            return aktif;
        }

        return Application.Current.MainWindow is { IsLoaded: true } main ? main : null;
    }
}
