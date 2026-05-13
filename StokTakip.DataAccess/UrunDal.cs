using StokTakip.Entities;

namespace StokTakip.DataAccess;

/// <summary>
/// Urun tablosu ile ilgili veritabani islemlerinin yazilacagi DAL sinifidir.
/// </summary>
public class UrunDal
{
    /// <summary>
    /// Tum urun kayitlarini listeler.
    /// </summary>
    public List<Urun> GetAll()
    {
        var urunler = new List<Urun>();
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            SELECT id, kategori_id, saklama_kosulu_id, urun_adi, barkod, birim,
                   stok_miktari, kritik_stok_seviyesi, alis_fiyati, satis_fiyati,
                   aciklama, aktif_mi, olusturma_tarihi
            FROM urunler
            ORDER BY id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            urunler.Add(MapUrun(reader));
        }

        return urunler;
    }

    /// <summary>
    /// Id bilgisine gore tek urun kaydi getirir.
    /// </summary>
    public Urun? GetById(int id)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            SELECT id, kategori_id, saklama_kosulu_id, urun_adi, barkod, birim,
                   stok_miktari, kritik_stok_seviyesi, alis_fiyati, satis_fiyati,
                   aciklama, aktif_mi, olusturma_tarihi
            FROM urunler
            WHERE id = @id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapUrun(reader) : null;
    }

    /// <summary>
    /// Barkod bilgisine gore urun kaydi getirir.
    /// </summary>
    public Urun? GetByBarkod(string barkod)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            SELECT id, kategori_id, saklama_kosulu_id, urun_adi, barkod, birim,
                   stok_miktari, kritik_stok_seviyesi, alis_fiyati, satis_fiyati,
                   aciklama, aktif_mi, olusturma_tarihi
            FROM urunler
            WHERE barkod = @barkod
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@barkod", barkod);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapUrun(reader) : null;
    }

    /// <summary>
    /// Yeni urun kaydi ekler ve olusan id degerini dondurur.
    /// </summary>
    public int Add(Urun urun)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            INSERT INTO urunler
                (kategori_id, saklama_kosulu_id, urun_adi, barkod, birim, stok_miktari,
                 kritik_stok_seviyesi, alis_fiyati, satis_fiyati, aciklama, aktif_mi)
            VALUES
                (@kategori_id, @saklama_kosulu_id, @urun_adi, @barkod, @birim, @stok_miktari,
                 @kritik_stok_seviyesi, @alis_fiyati, @satis_fiyati, @aciklama, @aktif_mi)
            RETURNING id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        AddUrunParameters(command, urun);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    /// <summary>
    /// Var olan urun kaydini gunceller.
    /// </summary>
    public void Update(Urun urun)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            UPDATE urunler
            SET kategori_id = @kategori_id,
                saklama_kosulu_id = @saklama_kosulu_id,
                urun_adi = @urun_adi,
                barkod = @barkod,
                birim = @birim,
                stok_miktari = @stok_miktari,
                kritik_stok_seviyesi = @kritik_stok_seviyesi,
                alis_fiyati = @alis_fiyati,
                satis_fiyati = @satis_fiyati,
                aciklama = @aciklama,
                aktif_mi = @aktif_mi
            WHERE id = @id
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", urun.Id);
        AddUrunParameters(command, urun);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Urun kaydini veritabanindan siler.
    /// </summary>
    public void Delete(int id)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = "DELETE FROM urunler WHERE id = @id";
        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Urunun stok miktarini gunceller.
    /// </summary>
    public void StokMiktariGuncelle(int urunId, int yeniStokMiktari)
    {
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = "UPDATE urunler SET stok_miktari = @stok_miktari WHERE id = @id";
        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", urunId);
        command.Parameters.AddWithValue("@stok_miktari", yeniStokMiktari);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Mevcut stogu kritik stok seviyesine esit veya altinda olan urunleri listeler.
    /// </summary>
    public List<Urun> GetKritikStoktakiler()
    {
        var urunler = new List<Urun>();
        using var connection = DbHelper.GetConnection();
        connection.Open();

        const string sql = """
            SELECT id, kategori_id, saklama_kosulu_id, urun_adi, barkod, birim,
                   stok_miktari, kritik_stok_seviyesi, alis_fiyati, satis_fiyati,
                   aciklama, aktif_mi, olusturma_tarihi
            FROM urunler
            WHERE stok_miktari <= kritik_stok_seviyesi
            ORDER BY stok_miktari, urun_adi
            """;

        using var command = new Npgsql.NpgsqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            urunler.Add(MapUrun(reader));
        }

        return urunler;
    }

    /// <summary>
    /// Eski iskelet kodun derlenmesi icin tutulur; gercek kayit islemi Add metodudur.
    /// </summary>
    public void Hazirla(Urun urun)
    {
        // Bu metot UI iskeletinin yanlislikla veritabani islemi yapmamasi icin bos birakildi.
    }

    private static void AddUrunParameters(Npgsql.NpgsqlCommand command, Urun urun)
    {
        command.Parameters.AddWithValue("@kategori_id", urun.KategoriId);
        command.Parameters.AddWithValue("@saklama_kosulu_id", urun.SaklamaKosuluId.HasValue ? urun.SaklamaKosuluId.Value : DBNull.Value);
        command.Parameters.AddWithValue("@urun_adi", urun.UrunAdi);
        command.Parameters.AddWithValue("@barkod", urun.Barkod);
        command.Parameters.AddWithValue("@birim", urun.Birim);
        command.Parameters.AddWithValue("@stok_miktari", urun.StokMiktari);
        command.Parameters.AddWithValue("@kritik_stok_seviyesi", urun.KritikStokSeviyesi);
        command.Parameters.AddWithValue("@alis_fiyati", urun.AlisFiyati);
        command.Parameters.AddWithValue("@satis_fiyati", urun.SatisFiyati);
        command.Parameters.AddWithValue("@aciklama", urun.Aciklama);
        command.Parameters.AddWithValue("@aktif_mi", urun.AktifMi);
    }

    private static Urun MapUrun(Npgsql.NpgsqlDataReader reader)
    {
        return new Urun
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            KategoriId = reader.GetInt32(reader.GetOrdinal("kategori_id")),
            SaklamaKosuluId = reader.IsDBNull(reader.GetOrdinal("saklama_kosulu_id")) ? null : reader.GetInt32(reader.GetOrdinal("saklama_kosulu_id")),
            UrunAdi = reader.GetString(reader.GetOrdinal("urun_adi")),
            Barkod = reader.GetString(reader.GetOrdinal("barkod")),
            Birim = reader.GetString(reader.GetOrdinal("birim")),
            StokMiktari = reader.GetInt32(reader.GetOrdinal("stok_miktari")),
            KritikStokSeviyesi = reader.GetInt32(reader.GetOrdinal("kritik_stok_seviyesi")),
            AlisFiyati = reader.GetDecimal(reader.GetOrdinal("alis_fiyati")),
            SatisFiyati = reader.GetDecimal(reader.GetOrdinal("satis_fiyati")),
            Aciklama = reader.IsDBNull(reader.GetOrdinal("aciklama")) ? string.Empty : reader.GetString(reader.GetOrdinal("aciklama")),
            AktifMi = reader.GetBoolean(reader.GetOrdinal("aktif_mi")),
            OlusturmaTarihi = reader.GetDateTime(reader.GetOrdinal("olusturma_tarihi"))
        };
    }
}
