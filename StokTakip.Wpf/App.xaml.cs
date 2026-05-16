using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using StokTakip.Wpf.Helpers;
using QuestPDF.Infrastructure;

namespace StokTakip.Wpf;

/// <summary>
/// WPF uygulamasının başlangıç kaynaklarını, pencere akışını ve genel hata yakalamayı yöneten uygulama sınıfıdır.
/// </summary>
public partial class App : Application
{
    public App()
    {
        DispatcherUnhandledException += App_DispatcherUnhandledException;
    }

    /// <summary>
    /// QuestPDF topluluk lisansını uygulama başında ayarlar; PDF üretimi öncesinde zorunludur.
    /// </summary>
    protected override void OnStartup(StartupEventArgs e)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        base.OnStartup(e);
        // Styles yuklendikten sonra kayitli tema sozlugu eklenir.
        ThemeManager.Initialize();
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // Global hata yakalama, beklenmeyen WPF hatalarında uygulamanın kapanmasını engeller.
        string hataDetayi = e.Exception.ToString();
        Debug.WriteLine(hataDetayi);
        Console.WriteLine(hataDetayi);
        HataLogunaYaz(hataDetayi);

        DialogHelper.ShowError("Beklenmeyen bir hata oluştu.");
        e.Handled = true;
    }

    private static void HataLogunaYaz(string hataDetayi)
    {
        try
        {
            string logsKlasoru = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logsKlasoru);
            string logDosyasi = Path.Combine(logsKlasoru, "app-error.log");
            File.AppendAllText(logDosyasi, $"{DateTime.Now:dd.MM.yyyy HH:mm:ss}{Environment.NewLine}{hataDetayi}{Environment.NewLine}{new string('-', 80)}{Environment.NewLine}");
        }
        catch
        {
            // Log yazılamazsa kullanıcı akışı bozulmasın.
        }
    }
}
