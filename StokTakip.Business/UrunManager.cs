using StokTakip.DataAccess;
using StokTakip.Entities;

namespace StokTakip.Business;

/// <summary>
/// Urun yonetimi ve kritik stok kontrollerine ait is kurallarini yonetir.
/// </summary>
public class UrunManager
{
    private readonly UrunDal _urunDal = new();

    /// <summary>
    /// Tum urunleri listeler.
    /// </summary>
    public List<Urun> GetAll()
    {
        return _urunDal.GetAll();
    }

    /// <summary>
    /// Id bilgisine gore urun getirir.
    /// </summary>
    public Urun? GetById(int id)
    {
        IdKontrol(id);
        return _urunDal.GetById(id);
    }

    /// <summary>
    /// Barkod bilgisine gore urun arar.
    /// </summary>
    public Urun? GetByBarkod(string barkod)
    {
        if (string.IsNullOrWhiteSpace(barkod))
        {
            throw new ArgumentException("Barkod bos birakilamaz.");
        }

        return _urunDal.GetByBarkod(barkod.Trim());
    }

    /// <summary>
    /// Urun bilgilerini kontrol edip yeni urun ekler.
    /// </summary>
    public int Add(Urun urun)
    {
        UrunDogrula(urun);

        // Ayni barkod ile ikinci urun eklenmesini engeller.
        if (_urunDal.GetByBarkod(urun.Barkod.Trim()) is not null)
        {
            throw new InvalidOperationException("Bu barkod ile kayitli baska bir urun bulunmaktadir.");
        }

        return _urunDal.Add(urun);
    }

    /// <summary>
    /// Urun bilgilerini kontrol edip mevcut urun kaydini gunceller.
    /// </summary>
    public void Update(Urun urun)
    {
        IdKontrol(urun.Id);
        UrunDogrula(urun);

        var barkodluUrun = _urunDal.GetByBarkod(urun.Barkod.Trim());
        if (barkodluUrun is not null && barkodluUrun.Id != urun.Id)
        {
            throw new InvalidOperationException("Bu barkod ile kayitli baska bir urun bulunmaktadir.");
        }

        _urunDal.Update(urun);
    }

    /// <summary>
    /// Id kontrolu yapildiktan sonra urun kaydini siler.
    /// </summary>
    public void Delete(int id)
    {
        IdKontrol(id);
        _urunDal.Delete(id);
    }

    /// <summary>
    /// Kritik stok seviyesine esit veya altinda kalan urunleri getirir.
    /// </summary>
    public List<Urun> GetKritikStoktakiler()
    {
        return _urunDal.GetKritikStoktakiler();
    }

    /// <summary>
    /// Urunun stok miktarini is kurali kontrolunden sonra gunceller.
    /// </summary>
    public void StokMiktariGuncelle(int urunId, int yeniStokMiktari)
    {
        IdKontrol(urunId);

        if (yeniStokMiktari < 0)
        {
            throw new ArgumentException("Stok miktari negatif olamaz.");
        }

        if (_urunDal.GetById(urunId) is null)
        {
            throw new InvalidOperationException("Urun bulunamadi.");
        }

        _urunDal.StokMiktariGuncelle(urunId, yeniStokMiktari);
    }

    /// <summary>
    /// Eski UI iskeleti ile uyumluluk icin tutulur.
    /// </summary>
    public void Hazirla(Urun urun)
    {
        _urunDal.Hazirla(urun);
    }

    private static void UrunDogrula(Urun urun)
    {
        if (string.IsNullOrWhiteSpace(urun.UrunAdi))
        {
            throw new ArgumentException("Urun adi bos birakilamaz.");
        }

        if (urun.KategoriId <= 0)
        {
            throw new ArgumentException("Kategori secilmeden urun eklenemez.");
        }

        if (string.IsNullOrWhiteSpace(urun.Barkod))
        {
            throw new ArgumentException("Barkod bos birakilamaz.");
        }

        if (string.IsNullOrWhiteSpace(urun.Birim))
        {
            throw new ArgumentException("Birim bos birakilamaz.");
        }

        if (urun.StokMiktari < 0)
        {
            throw new ArgumentException("Stok miktari negatif olamaz.");
        }

        if (urun.KritikStokSeviyesi < 0)
        {
            throw new ArgumentException("Kritik stok seviyesi negatif olamaz.");
        }

        if (urun.AlisFiyati < 0)
        {
            throw new ArgumentException("Alis fiyati negatif olamaz.");
        }

        if (urun.SatisFiyati < 0)
        {
            throw new ArgumentException("Satis fiyati negatif olamaz.");
        }

        if (urun.SatisFiyati < urun.AlisFiyati)
        {
            throw new ArgumentException("Satis fiyati alis fiyatindan dusuk olamaz.");
        }
    }

    private static void IdKontrol(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Gecerli bir urun secilmelidir.");
        }
    }
}
