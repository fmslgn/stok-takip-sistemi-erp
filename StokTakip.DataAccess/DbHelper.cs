using Npgsql;

namespace StokTakip.DataAccess;

/// <summary>
/// PostgreSQL veritabani baglantisini merkezi olarak yoneten yardimci siniftir.
/// </summary>
public static class DbHelper
{
    // Final asamasinda bu bilgi appsettings veya guvenli bir ayar dosyasindan okunabilir.
    private const string ConnectionString =
        "Host=localhost;Port=5432;Database=StokTakipSistemi;Username=postgres;Password=postgres";

    /// <summary>
    /// DAL siniflarinin kullanacagi yeni PostgreSQL baglantisini olusturur.
    /// </summary>
    public static NpgsqlConnection CreateConnection()
    {
        // Baglantiyi acma/kapama sorumlulugu islemi yapan DAL metoduna birakilir.
        return new NpgsqlConnection(ConnectionString);
    }
}
