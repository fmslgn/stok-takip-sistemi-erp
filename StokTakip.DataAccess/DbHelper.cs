using Npgsql;

namespace StokTakip.DataAccess;

/// <summary>
/// PostgreSQL veritabani baglantisini merkezi olarak yoneten yardimci siniftir.
/// </summary>
public static class DbHelper
{
    // Baglanti bilgisi tek noktada tutulur. Sifre degisirse sadece bu satir guncellenir.
    private const string ConnectionString =
        "Host=127.0.0.1;Port=5432;Database=stok_takip_db;Username=postgres;Password=postgres";

    /// <summary>
    /// DAL siniflarinin kullanacagi yeni PostgreSQL baglantisini olusturur.
    /// </summary>
    public static NpgsqlConnection GetConnection()
    {
        // Baglantiyi acma ve kapatma sorumlulugu using blogu ile DAL metodlarindadir.
        return new NpgsqlConnection(ConnectionString);
    }

    /// <summary>
    /// Eski kodlarla uyumluluk icin GetConnection metoduna yonlendirme yapar.
    /// </summary>
    public static NpgsqlConnection CreateConnection()
    {
        return GetConnection();
    }
}
