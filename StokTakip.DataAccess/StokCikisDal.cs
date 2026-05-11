using StokTakip.Entities;

namespace StokTakip.DataAccess;

/// <summary>
/// Stok cikis hareketleri ile ilgili veritabani islemlerinin yazilacagi DAL sinifidir.
/// </summary>
public class StokCikisDal
{
    /// <summary>
    /// Tum stok cikis kayitlarini listeler.
    /// </summary>
    public List<StokCikis> GetAll()
    {
        var cikislar = new List<StokCikis>();
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = "SELECT id, urun_id, kullanici_id, miktar, aciklama, cikis_tarihi FROM stok_cikislar ORDER BY id";
        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            cikislar.Add(MapStokCikis(reader));
        }

        return cikislar;
    }

    /// <summary>
    /// Id bilgisine gore tek stok cikis kaydi getirir.
    /// </summary>
    public StokCikis? GetById(int id)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = "SELECT id, urun_id, kullanici_id, miktar, aciklama, cikis_tarihi FROM stok_cikislar WHERE id = @id";
        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapStokCikis(reader) : null;
    }

    /// <summary>
    /// Yeni stok cikis kaydi ekler. Urun stok miktarini bu metot degistirmez.
    /// </summary>
    public int Add(StokCikis stokCikis)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            INSERT INTO stok_cikislar (urun_id, kullanici_id, miktar, aciklama, cikis_tarihi)
            VALUES (@urun_id, @kullanici_id, @miktar, @aciklama, @cikis_tarihi)
            RETURNING id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@urun_id", stokCikis.UrunId);
        command.Parameters.AddWithValue("@kullanici_id", stokCikis.KullaniciId);
        command.Parameters.AddWithValue("@miktar", stokCikis.Miktar);
        command.Parameters.AddWithValue("@aciklama", stokCikis.Aciklama);
        command.Parameters.AddWithValue("@cikis_tarihi", stokCikis.CikisTarihi);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    /// <summary>
    /// Var olan stok cikis kaydini gunceller. Urun stok miktarini bu metot degistirmez.
    /// </summary>
    public void Update(StokCikis stokCikis)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            UPDATE stok_cikislar
            SET urun_id = @urun_id,
                kullanici_id = @kullanici_id,
                miktar = @miktar,
                aciklama = @aciklama,
                cikis_tarihi = @cikis_tarihi
            WHERE id = @id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", stokCikis.Id);
        command.Parameters.AddWithValue("@urun_id", stokCikis.UrunId);
        command.Parameters.AddWithValue("@kullanici_id", stokCikis.KullaniciId);
        command.Parameters.AddWithValue("@miktar", stokCikis.Miktar);
        command.Parameters.AddWithValue("@aciklama", stokCikis.Aciklama);
        command.Parameters.AddWithValue("@cikis_tarihi", stokCikis.CikisTarihi);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Stok cikis kaydini veritabanindan siler.
    /// </summary>
    public void Delete(int id)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = "DELETE FROM stok_cikislar WHERE id = @id";
        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Eski iskelet kodun derlenmesi icin tutulur; gercek kayit islemi Add metodudur.
    /// </summary>
    public void Hazirla(StokCikis stokCikis)
    {
        // Bu metot UI iskeletinin yanlislikla veritabani islemi yapmamasi icin bos birakildi.
    }

    private static StokCikis MapStokCikis(Npgsql.NpgsqlDataReader reader)
    {
        return new StokCikis
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            UrunId = reader.GetInt32(reader.GetOrdinal("urun_id")),
            KullaniciId = reader.GetInt32(reader.GetOrdinal("kullanici_id")),
            Miktar = reader.GetInt32(reader.GetOrdinal("miktar")),
            Aciklama = reader.IsDBNull(reader.GetOrdinal("aciklama")) ? string.Empty : reader.GetString(reader.GetOrdinal("aciklama")),
            CikisTarihi = reader.GetDateTime(reader.GetOrdinal("cikis_tarihi"))
        };
    }
}
