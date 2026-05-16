using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;

namespace StokTakip.Wpf.Helpers;

/// <summary>
/// Acik/koyu tema secimini uygular; tema ResourceDictionary App.Resources.MergedDictionaries icinde degistirilir.
/// Renkler Styles.xaml icinde DynamicResource ile baglandigi icin gecis aninda yansir.
/// Tercih: %AppData%/SYAStokTakip/theme.json
/// </summary>
public static class ThemeManager
{
    private const string UygulamaKlasorAdi = "SYAStokTakip";
    private const string TemaDosyaAdi = "theme.json";
    private const string LightThemeDosyasi = "LightTheme.xaml";
    private const string DarkThemeDosyasi = "DarkTheme.xaml";

    private static ResourceDictionary? _aktifTemaSozlugu;

    public static AppTheme CurrentTheme { get; private set; } = AppTheme.Light;

    public static event EventHandler? ThemeChanged;

    /// <summary>Uygulama acilisinda kayitli temayi yukler; okunamazsa acik tema kullanilir.</summary>
    public static void Initialize()
    {
        ApplyTheme(LoadSavedTheme(), persist: false);
    }

    public static void ToggleTheme() => ApplyTheme(CurrentTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light);

    /// <summary>Tum tema sozluklerini kaldirip yenisini ekler; persist true ise tercih dosyaya yazilir.</summary>
    public static void ApplyTheme(AppTheme theme, bool persist = true)
    {
        if (Application.Current is null)
        {
            return;
        }

        var birlesik = Application.Current.Resources.MergedDictionaries;
        TemaSozlukleriniKaldir(birlesik);

        var yeniSozluk = TemaSozluguYukle(theme);
        birlesik.Insert(0, yeniSozluk);
        _aktifTemaSozlugu = yeniSozluk;
        CurrentTheme = theme;

        if (persist)
        {
            SaveTheme(theme);
        }

        Debug.WriteLine($"[ThemeManager] Aktif tema: {theme}");

        ThemeChanged?.Invoke(null, EventArgs.Empty);

        // Acik pencerelerde DynamicResource baglantilarini tazele
        Application.Current.Dispatcher.BeginInvoke(TemaGorunumunuYenile, DispatcherPriority.Loaded);
    }

    public static string ToggleButtonText =>
        CurrentTheme == AppTheme.Light ? "Koyu Tema" : "Açık Tema";

    private static ResourceDictionary TemaSozluguYukle(AppTheme theme)
    {
        string dosya = theme == AppTheme.Dark ? DarkThemeDosyasi : LightThemeDosyasi;
        return new ResourceDictionary
        {
            Source = TemaPackUri(dosya)
        };
    }

    /// <summary>Pack URI ile tema XAML yuklenir; goreli yol hatalarini onler.</summary>
    private static Uri TemaPackUri(string dosyaAdi) =>
        new($"pack://application:,,,/Resources/Themes/{dosyaAdi}", UriKind.Absolute);

    /// <summary>App.xaml ve onceki gecislerden kalan tum tema sozluklerini temizler.</summary>
    private static void TemaSozlukleriniKaldir(IList<ResourceDictionary> birlesik)
    {
        for (int i = birlesik.Count - 1; i >= 0; i--)
        {
            if (TemaSozluguMu(birlesik[i]))
            {
                birlesik.RemoveAt(i);
            }
        }

        _aktifTemaSozlugu = null;
    }

    private static bool TemaSozluguMu(ResourceDictionary sozluk)
    {
        if (sozluk.Source is null)
        {
            return ReferenceEquals(sozluk, _aktifTemaSozlugu);
        }

        string kaynak = sozluk.Source.OriginalString;
        return kaynak.Contains(LightThemeDosyasi, StringComparison.OrdinalIgnoreCase)
               || kaynak.Contains(DarkThemeDosyasi, StringComparison.OrdinalIgnoreCase);
    }

    private static void TemaGorunumunuYenile()
    {
        if (Application.Current is null)
        {
            return;
        }

        foreach (Window pencere in Application.Current.Windows.OfType<Window>())
        {
            if (pencere.Background is not null)
            {
                var arkaPlan = pencere.Background;
                pencere.Background = null;
                pencere.Background = arkaPlan;
            }

            pencere.InvalidateVisual();
        }
    }

    private static string TemaDosyaYolu =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            UygulamaKlasorAdi,
            TemaDosyaAdi);

    private static AppTheme LoadSavedTheme()
    {
        try
        {
            string yol = TemaDosyaYolu;
            if (!File.Exists(yol))
            {
                return AppTheme.Light;
            }

            string json = File.ReadAllText(yol);
            var model = JsonSerializer.Deserialize<ThemeSettingsModel>(json);
            return model?.Theme?.Equals("dark", StringComparison.OrdinalIgnoreCase) == true
                ? AppTheme.Dark
                : AppTheme.Light;
        }
        catch
        {
            return AppTheme.Light;
        }
    }

    private static void SaveTheme(AppTheme theme)
    {
        try
        {
            string yol = TemaDosyaYolu;
            Directory.CreateDirectory(Path.GetDirectoryName(yol)!);
            var model = new ThemeSettingsModel
            {
                Theme = theme == AppTheme.Dark ? "dark" : "light"
            };
            File.WriteAllText(yol, JsonSerializer.Serialize(model));
        }
        catch
        {
            // Tema kaydi yazilamazsa uygulama akisi devam eder.
        }
    }

    private sealed class ThemeSettingsModel
    {
        public string? Theme { get; set; }
    }
}

public enum AppTheme
{
    Light,
    Dark
}
