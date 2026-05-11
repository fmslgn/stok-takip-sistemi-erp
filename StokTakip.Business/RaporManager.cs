using StokTakip.DataAccess;
using StokTakip.Entities;

namespace StokTakip.Business;

/// <summary>
/// Raporlama ekranlarinin ihtiyac duydugu is kurallarini yoneten Business sinifidir.
/// </summary>
public class RaporManager
{
    private readonly RaporDal _raporDal = new();
    private readonly UrunDal _urunDal = new();

    /// <summary>
    /// Tum rapor kayitlarini listeler.
    /// </summary>
    public List<Rapor> GetAll()
    {
        return _raporDal.GetAll();
    }

    /// <summary>
    /// Id bilgisine gore rapor getirir.
    /// </summary>
    public Rapor? GetById(int id)
    {
        IdKontrol(id);
        return _raporDal.GetById(id);
    }

    /// <summary>
    /// Rapor bilgilerini kontrol edip yeni rapor kaydi ekler.
    /// </summary>
    public int Add(Rapor rapor)
    {
        RaporDogrula(rapor);
        return _raporDal.Add(rapor);
    }

    /// <summary>
    /// Id kontrolu yapildiktan sonra rapor kaydini siler.
    /// </summary>
    public void Delete(int id)
    {
        IdKontrol(id);
        _raporDal.Delete(id);
    }

    /// <summary>
    /// Sistemdeki toplam urun sayisini getirir.
    /// </summary>
    public int ToplamUrunSayisiGetir()
    {
        return _urunDal.GetAll().Count;
    }

    /// <summary>
    /// Kritik stok seviyesinde bulunan urun sayisini getirir.
    /// </summary>
    public int KritikStokUrunSayisiGetir()
    {
        return _urunDal.GetKritikStoktakiler().Count;
    }

    /// <summary>
    /// Tum urunlerin toplam stok miktarini hesaplar.
    /// </summary>
    public int ToplamStokMiktariGetir()
    {
        return _urunDal.GetAll().Sum(urun => urun.StokMiktari);
    }

    /// <summary>
    /// Eski UI iskeleti ile uyumluluk icin tutulur.
    /// </summary>
    public void Hazirla(Rapor rapor)
    {
        _raporDal.Hazirla(rapor);
    }

    private static void RaporDogrula(Rapor rapor)
    {
        if (rapor.KullaniciId <= 0)
        {
            throw new ArgumentException("Rapor icin kullanici secilmelidir.");
        }

        if (string.IsNullOrWhiteSpace(rapor.RaporTuru))
        {
            throw new ArgumentException("Rapor turu bos birakilamaz.");
        }

        if (string.IsNullOrWhiteSpace(rapor.RaporBasligi))
        {
            throw new ArgumentException("Rapor basligi bos birakilamaz.");
        }
    }

    private static void IdKontrol(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Gecerli bir rapor secilmelidir.");
        }
    }
}
