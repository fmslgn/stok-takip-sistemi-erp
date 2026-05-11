using StokTakip.Entities;

namespace StokTakip.DataAccess;

/// <summary>
/// Saklama kosullari ile ilgili veritabani islemlerinin yazilacagi DAL sinifidir.
/// </summary>
public class SaklamaKosuluDal
{
    /// <summary>
    /// Tum saklama kosulu kayitlarini listeler.
    /// </summary>
    public List<SaklamaKosulu> GetAll()
    {
        var kosullar = new List<SaklamaKosulu>();
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            SELECT id, kosul_adi, kategori_anahtar_kelime, onerilen_sicaklik,
                   nem_orani, saklama_aciklamasi, aktif_mi, olusturma_tarihi
            FROM saklama_kosullari
            ORDER BY id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            kosullar.Add(MapSaklamaKosulu(reader));
        }

        return kosullar;
    }

    /// <summary>
    /// Id bilgisine gore tek saklama kosulu kaydi getirir.
    /// </summary>
    public SaklamaKosulu? GetById(int id)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            SELECT id, kosul_adi, kategori_anahtar_kelime, onerilen_sicaklik,
                   nem_orani, saklama_aciklamasi, aktif_mi, olusturma_tarihi
            FROM saklama_kosullari
            WHERE id = @id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapSaklamaKosulu(reader) : null;
    }

    /// <summary>
    /// Yeni saklama kosulu kaydi ekler ve olusan id degerini dondurur.
    /// </summary>
    public int Add(SaklamaKosulu saklamaKosulu)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            INSERT INTO saklama_kosullari
                (kosul_adi, kategori_anahtar_kelime, onerilen_sicaklik, nem_orani, saklama_aciklamasi, aktif_mi)
            VALUES
                (@kosul_adi, @kategori_anahtar_kelime, @onerilen_sicaklik, @nem_orani, @saklama_aciklamasi, @aktif_mi)
            RETURNING id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@kosul_adi", saklamaKosulu.KosulAdi);
        command.Parameters.AddWithValue("@kategori_anahtar_kelime", saklamaKosulu.KategoriAnahtarKelime);
        command.Parameters.AddWithValue("@onerilen_sicaklik", saklamaKosulu.OnerilenSicaklik);
        command.Parameters.AddWithValue("@nem_orani", saklamaKosulu.NemOrani);
        command.Parameters.AddWithValue("@saklama_aciklamasi", saklamaKosulu.SaklamaAciklamasi);
        command.Parameters.AddWithValue("@aktif_mi", saklamaKosulu.AktifMi);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    /// <summary>
    /// Var olan saklama kosulu kaydini gunceller.
    /// </summary>
    public void Update(SaklamaKosulu saklamaKosulu)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            UPDATE saklama_kosullari
            SET kosul_adi = @kosul_adi,
                kategori_anahtar_kelime = @kategori_anahtar_kelime,
                onerilen_sicaklik = @onerilen_sicaklik,
                nem_orani = @nem_orani,
                saklama_aciklamasi = @saklama_aciklamasi,
                aktif_mi = @aktif_mi
            WHERE id = @id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", saklamaKosulu.Id);
        command.Parameters.AddWithValue("@kosul_adi", saklamaKosulu.KosulAdi);
        command.Parameters.AddWithValue("@kategori_anahtar_kelime", saklamaKosulu.KategoriAnahtarKelime);
        command.Parameters.AddWithValue("@onerilen_sicaklik", saklamaKosulu.OnerilenSicaklik);
        command.Parameters.AddWithValue("@nem_orani", saklamaKosulu.NemOrani);
        command.Parameters.AddWithValue("@saklama_aciklamasi", saklamaKosulu.SaklamaAciklamasi);
        command.Parameters.AddWithValue("@aktif_mi", saklamaKosulu.AktifMi);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Saklama kosulu kaydini veritabanindan siler.
    /// </summary>
    public void Delete(int id)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = "DELETE FROM saklama_kosullari WHERE id = @id";
        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Eski iskelet kodun derlenmesi icin tutulur; gercek kayit islemi Add metodudur.
    /// </summary>
    public void Hazirla(SaklamaKosulu saklamaKosulu)
    {
        // Bu metot UI iskeletinin yanlislikla veritabani islemi yapmamasi icin bos birakildi.
    }

    private static SaklamaKosulu MapSaklamaKosulu(Npgsql.NpgsqlDataReader reader)
    {
        return new SaklamaKosulu
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            KosulAdi = reader.GetString(reader.GetOrdinal("kosul_adi")),
            KategoriAnahtarKelime = reader.GetString(reader.GetOrdinal("kategori_anahtar_kelime")),
            OnerilenSicaklik = reader.GetString(reader.GetOrdinal("onerilen_sicaklik")),
            NemOrani = reader.GetString(reader.GetOrdinal("nem_orani")),
            SaklamaAciklamasi = reader.IsDBNull(reader.GetOrdinal("saklama_aciklamasi")) ? string.Empty : reader.GetString(reader.GetOrdinal("saklama_aciklamasi")),
            AktifMi = reader.GetBoolean(reader.GetOrdinal("aktif_mi")),
            OlusturmaTarihi = reader.GetDateTime(reader.GetOrdinal("olusturma_tarihi"))
        };
    }
}
