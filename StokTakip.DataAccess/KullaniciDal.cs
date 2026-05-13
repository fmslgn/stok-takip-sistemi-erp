using StokTakip.Entities;

namespace StokTakip.DataAccess;

/// <summary>
/// Kullanici tablosu ile ilgili veritabani islemlerinin yazilacagi DAL sinifidir.
/// </summary>
public class KullaniciDal
{
    /// <summary>
    /// Tum kullanici kayitlarini listeler.
    /// </summary>
    public List<Kullanici> GetAll()
    {
        var kullanicilar = new List<Kullanici>();
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            SELECT id, ad_soyad, kullanici_adi, sifre, rol, aktif_mi, olusturma_tarihi
            FROM kullanicilar
            ORDER BY id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            kullanicilar.Add(MapKullanici(reader));
        }

        return kullanicilar;
    }

    /// <summary>
    /// Id bilgisine gore tek kullanici kaydi getirir.
    /// </summary>
    public Kullanici? GetById(int id)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            SELECT id, ad_soyad, kullanici_adi, sifre, rol, aktif_mi, olusturma_tarihi
            FROM kullanicilar
            WHERE id = @id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapKullanici(reader) : null;
    }

    /// <summary>
    /// Yeni kullanici kaydi ekler ve olusan id degerini dondurur.
    /// </summary>
    public int Add(Kullanici kullanici)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            INSERT INTO kullanicilar (ad_soyad, kullanici_adi, sifre, rol, aktif_mi)
            VALUES (@ad_soyad, @kullanici_adi, @sifre, @rol, @aktif_mi)
            RETURNING id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@ad_soyad", kullanici.AdSoyad);
        command.Parameters.AddWithValue("@kullanici_adi", kullanici.KullaniciAdi);
        command.Parameters.AddWithValue("@sifre", kullanici.Sifre);
        command.Parameters.AddWithValue("@rol", kullanici.Rol);
        command.Parameters.AddWithValue("@aktif_mi", kullanici.AktifMi);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    /// <summary>
    /// Var olan kullanici kaydini gunceller.
    /// </summary>
    public void Update(Kullanici kullanici)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            UPDATE kullanicilar
            SET ad_soyad = @ad_soyad,
                kullanici_adi = @kullanici_adi,
                sifre = @sifre,
                rol = @rol,
                aktif_mi = @aktif_mi
            WHERE id = @id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", kullanici.Id);
        command.Parameters.AddWithValue("@ad_soyad", kullanici.AdSoyad);
        command.Parameters.AddWithValue("@kullanici_adi", kullanici.KullaniciAdi);
        command.Parameters.AddWithValue("@sifre", kullanici.Sifre);
        command.Parameters.AddWithValue("@rol", kullanici.Rol);
        command.Parameters.AddWithValue("@aktif_mi", kullanici.AktifMi);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Kullanici kaydini veritabanindan siler.
    /// </summary>
    public void Delete(int id)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = "DELETE FROM kullanicilar WHERE id = @id";
        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Kullanici adi ve sifreye gore aktif kullanici girisini kontrol eder.
    /// </summary>
    public Kullanici? LoginKontrol(string kullaniciAdi, string sifre)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            SELECT id, ad_soyad, kullanici_adi, sifre, rol, aktif_mi, olusturma_tarihi
            FROM kullanicilar
            WHERE kullanici_adi = @kullanici_adi
              AND sifre = @sifre
              AND aktif_mi = TRUE
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@kullanici_adi", kullaniciAdi);
        command.Parameters.AddWithValue("@sifre", sifre);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapKullanici(reader) : null;
    }

    /// <summary>
    /// Eski iskelet kodun derlenmesi icin tutulur; gercek kayit islemi Add metodudur.
    /// </summary>
    public void Hazirla(Kullanici kullanici)
    {
        // Bu metot UI iskeletinin yanlislikla veritabani islemi yapmamasi icin bos birakildi.
    }

    private static Kullanici MapKullanici(Npgsql.NpgsqlDataReader reader)
    {
        return new Kullanici
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            AdSoyad = reader.GetString(reader.GetOrdinal("ad_soyad")),
            KullaniciAdi = reader.GetString(reader.GetOrdinal("kullanici_adi")),
            Sifre = reader.GetString(reader.GetOrdinal("sifre")),
            Rol = reader.GetString(reader.GetOrdinal("rol")),
            AktifMi = reader.GetBoolean(reader.GetOrdinal("aktif_mi")),
            OlusturmaTarihi = reader.GetDateTime(reader.GetOrdinal("olusturma_tarihi"))
        };
    }
}
