using StokTakip.Entities;

namespace StokTakip.DataAccess;

/// <summary>
/// Kategori tablosu ile ilgili veritabani islemlerinin yazilacagi DAL sinifidir.
/// </summary>
public class KategoriDal
{
    /// <summary>
    /// Tum kategori kayitlarini listeler.
    /// </summary>
    public List<Kategori> GetAll()
    {
        var kategoriler = new List<Kategori>();
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = "SELECT id, kategori_adi, aciklama, aktif_mi, olusturma_tarihi FROM kategoriler ORDER BY id";
        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            kategoriler.Add(MapKategori(reader));
        }

        return kategoriler;
    }

    /// <summary>
    /// Id bilgisine gore tek kategori kaydi getirir.
    /// </summary>
    public Kategori? GetById(int id)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = "SELECT id, kategori_adi, aciklama, aktif_mi, olusturma_tarihi FROM kategoriler WHERE id = @id";
        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapKategori(reader) : null;
    }

    /// <summary>
    /// Yeni kategori kaydi ekler ve olusan id degerini dondurur.
    /// </summary>
    public int Add(Kategori kategori)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            INSERT INTO kategoriler (kategori_adi, aciklama, aktif_mi)
            VALUES (@kategori_adi, @aciklama, @aktif_mi)
            RETURNING id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@kategori_adi", kategori.KategoriAdi);
        command.Parameters.AddWithValue("@aciklama", kategori.Aciklama);
        command.Parameters.AddWithValue("@aktif_mi", kategori.AktifMi);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    /// <summary>
    /// Var olan kategori kaydini gunceller.
    /// </summary>
    public void Update(Kategori kategori)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            UPDATE kategoriler
            SET kategori_adi = @kategori_adi,
                aciklama = @aciklama,
                aktif_mi = @aktif_mi
            WHERE id = @id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", kategori.Id);
        command.Parameters.AddWithValue("@kategori_adi", kategori.KategoriAdi);
        command.Parameters.AddWithValue("@aciklama", kategori.Aciklama);
        command.Parameters.AddWithValue("@aktif_mi", kategori.AktifMi);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Kategori kaydini veritabanindan siler.
    /// </summary>
    public void Delete(int id)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = "DELETE FROM kategoriler WHERE id = @id";
        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Eski iskelet kodun derlenmesi icin tutulur; gercek kayit islemi Add metodudur.
    /// </summary>
    public void Hazirla(Kategori kategori)
    {
        // Bu metot UI iskeletinin yanlislikla veritabani islemi yapmamasi icin bos birakildi.
    }

    private static Kategori MapKategori(Npgsql.NpgsqlDataReader reader)
    {
        return new Kategori
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            KategoriAdi = reader.GetString(reader.GetOrdinal("kategori_adi")),
            Aciklama = reader.IsDBNull(reader.GetOrdinal("aciklama")) ? string.Empty : reader.GetString(reader.GetOrdinal("aciklama")),
            AktifMi = reader.GetBoolean(reader.GetOrdinal("aktif_mi")),
            OlusturmaTarihi = reader.GetDateTime(reader.GetOrdinal("olusturma_tarihi"))
        };
    }
}
