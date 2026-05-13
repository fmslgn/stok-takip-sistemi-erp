using StokTakip.Entities;

namespace StokTakip.DataAccess;

/// <summary>
/// Stok giris hareketleri ile ilgili veritabani islemlerinin yazilacagi DAL sinifidir.
/// </summary>
public class StokGirisDal
{
    /// <summary>
    /// Tum stok giris kayitlarini listeler.
    /// </summary>
    public List<StokGiris> GetAll()
    {
        var girisler = new List<StokGiris>();
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = "SELECT id, urun_id, kullanici_id, miktar, aciklama, giris_tarihi FROM stok_girisler ORDER BY id";
        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            girisler.Add(MapStokGiris(reader));
        }

        return girisler;
    }

    /// <summary>
    /// Id bilgisine gore tek stok giris kaydi getirir.
    /// </summary>
    public StokGiris? GetById(int id)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = "SELECT id, urun_id, kullanici_id, miktar, aciklama, giris_tarihi FROM stok_girisler WHERE id = @id";
        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapStokGiris(reader) : null;
    }

    /// <summary>
    /// Yeni stok giris kaydi ekler. Urun stok miktarini bu metot degistirmez.
    /// </summary>
    public int Add(StokGiris stokGiris)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            INSERT INTO stok_girisler (urun_id, kullanici_id, miktar, aciklama, giris_tarihi)
            VALUES (@urun_id, @kullanici_id, @miktar, @aciklama, @giris_tarihi)
            RETURNING id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@urun_id", stokGiris.UrunId);
        command.Parameters.AddWithValue("@kullanici_id", stokGiris.KullaniciId);
        command.Parameters.AddWithValue("@miktar", stokGiris.Miktar);
        command.Parameters.AddWithValue("@aciklama", stokGiris.Aciklama);
        command.Parameters.AddWithValue("@giris_tarihi", stokGiris.GirisTarihi);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    /// <summary>
    /// Var olan stok giris kaydini gunceller. Urun stok miktarini bu metot degistirmez.
    /// </summary>
    public void Update(StokGiris stokGiris)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            UPDATE stok_girisler
            SET urun_id = @urun_id,
                kullanici_id = @kullanici_id,
                miktar = @miktar,
                aciklama = @aciklama,
                giris_tarihi = @giris_tarihi
            WHERE id = @id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", stokGiris.Id);
        command.Parameters.AddWithValue("@urun_id", stokGiris.UrunId);
        command.Parameters.AddWithValue("@kullanici_id", stokGiris.KullaniciId);
        command.Parameters.AddWithValue("@miktar", stokGiris.Miktar);
        command.Parameters.AddWithValue("@aciklama", stokGiris.Aciklama);
        command.Parameters.AddWithValue("@giris_tarihi", stokGiris.GirisTarihi);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Stok giris kaydini veritabanindan siler.
    /// </summary>
    public void Delete(int id)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = "DELETE FROM stok_girisler WHERE id = @id";
        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Eski iskelet kodun derlenmesi icin tutulur; gercek kayit islemi Add metodudur.
    /// </summary>
    public void Hazirla(StokGiris stokGiris)
    {
        // Bu metot UI iskeletinin yanlislikla veritabani islemi yapmamasi icin bos birakildi.
    }

    private static StokGiris MapStokGiris(Npgsql.NpgsqlDataReader reader)
    {
        return new StokGiris
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            UrunId = reader.GetInt32(reader.GetOrdinal("urun_id")),
            KullaniciId = reader.GetInt32(reader.GetOrdinal("kullanici_id")),
            Miktar = reader.GetInt32(reader.GetOrdinal("miktar")),
            Aciklama = reader.IsDBNull(reader.GetOrdinal("aciklama")) ? string.Empty : reader.GetString(reader.GetOrdinal("aciklama")),
            GirisTarihi = reader.GetDateTime(reader.GetOrdinal("giris_tarihi"))
        };
    }
}
