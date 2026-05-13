using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

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

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // Global hata yakalama, beklenmeyen WPF hatalarında uygulamanın kapanmasını engeller.
        string hataDetayi = e.Exception.ToString();
        Debug.WriteLine(hataDetayi);
        Console.WriteLine(hataDetayi);
        HataLogunaYaz(hataDetayi);

        MessageBox.Show("Beklenmeyen bir hata oluştu.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
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
