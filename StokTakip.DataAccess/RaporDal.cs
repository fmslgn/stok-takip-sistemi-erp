using StokTakip.Entities;

namespace StokTakip.DataAccess;

/// <summary>
/// Raporlama verileri ile ilgili veritabani islemlerinin yazilacagi DAL sinifidir.
/// </summary>
public class RaporDal
{
    /// <summary>
    /// Tum rapor kayitlarini listeler.
    /// </summary>
    public List<Rapor> GetAll()
    {
        var raporlar = new List<Rapor>();
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            SELECT id, kullanici_id, rapor_turu, rapor_basligi, rapor_aciklama, olusturma_tarihi
            FROM raporlar
            ORDER BY id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            raporlar.Add(MapRapor(reader));
        }

        return raporlar;
    }

    /// <summary>
    /// Id bilgisine gore tek rapor kaydi getirir.
    /// </summary>
    public Rapor? GetById(int id)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            SELECT id, kullanici_id, rapor_turu, rapor_basligi, rapor_aciklama, olusturma_tarihi
            FROM raporlar
            WHERE id = @id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapRapor(reader) : null;
    }

    /// <summary>
    /// Yeni rapor kaydi ekler ve olusan id degerini dondurur.
    /// </summary>
    public int Add(Rapor rapor)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            INSERT INTO raporlar (kullanici_id, rapor_turu, rapor_basligi, rapor_aciklama)
            VALUES (@kullanici_id, @rapor_turu, @rapor_basligi, @rapor_aciklama)
            RETURNING id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@kullanici_id", rapor.KullaniciId);
        command.Parameters.AddWithValue("@rapor_turu", rapor.RaporTuru);
        command.Parameters.AddWithValue("@rapor_basligi", rapor.RaporBasligi);
        command.Parameters.AddWithValue("@rapor_aciklama", rapor.RaporAciklama);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    /// <summary>
    /// Var olan rapor kaydini gunceller.
    /// </summary>
    public void Update(Rapor rapor)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            UPDATE raporlar
            SET kullanici_id = @kullanici_id,
                rapor_turu = @rapor_turu,
                rapor_basligi = @rapor_basligi,
                rapor_aciklama = @rapor_aciklama
            WHERE id = @id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", rapor.Id);
        command.Parameters.AddWithValue("@kullanici_id", rapor.KullaniciId);
        command.Parameters.AddWithValue("@rapor_turu", rapor.RaporTuru);
        command.Parameters.AddWithValue("@rapor_basligi", rapor.RaporBasligi);
        command.Parameters.AddWithValue("@rapor_aciklama", rapor.RaporAciklama);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Rapor kaydini veritabanindan siler.
    /// </summary>
    public void Delete(int id)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = "DELETE FROM raporlar WHERE id = @id";
        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Eski iskelet kodun derlenmesi icin tutulur; gercek kayit islemi Add metodudur.
    /// </summary>
    public void Hazirla(Rapor rapor)
    {
        // Bu metot UI iskeletinin yanlislikla veritabani islemi yapmamasi icin bos birakildi.
    }

    private static Rapor MapRapor(Npgsql.NpgsqlDataReader reader)
    {
        return new Rapor
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            KullaniciId = reader.GetInt32(reader.GetOrdinal("kullanici_id")),
            RaporTuru = reader.GetString(reader.GetOrdinal("rapor_turu")),
            RaporBasligi = reader.GetString(reader.GetOrdinal("rapor_basligi")),
            RaporAciklama = reader.IsDBNull(reader.GetOrdinal("rapor_aciklama")) ? string.Empty : reader.GetString(reader.GetOrdinal("rapor_aciklama")),
            OlusturmaTarihi = reader.GetDateTime(reader.GetOrdinal("olusturma_tarihi"))
        };
    }
}
