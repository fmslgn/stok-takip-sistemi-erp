using StokTakip.DataAccess;
using StokTakip.Entities;

namespace StokTakip.Business;

/// <summary>
/// Stok giris surecinin is kurallarini yoneten Business sinifidir.
/// </summary>
public class StokGirisManager
{
    private readonly StokGirisDal _stokGirisDal = new();
    private readonly UrunDal _urunDal = new();

    /// <summary>
    /// Tum stok giris hareketlerini listeler.
    /// </summary>
    public List<StokGiris> GetAll()
    {
        return _stokGirisDal.GetAll();
    }

    /// <summary>
    /// Id bilgisine gore stok giris hareketi getirir.
    /// </summary>
    public StokGiris? GetById(int id)
    {
        IdKontrol(id, "Gecerli bir stok giris kaydi secilmelidir.");
        return _stokGirisDal.GetById(id);
    }

    /// <summary>
    /// Stok giris kaydini ekler ve urunun stok miktarini artirir.
    /// </summary>
    public int Add(StokGiris stokGiris)
    {
        StokGirisDogrula(stokGiris);

        var urun = _urunDal.GetById(stokGiris.UrunId);
        if (urun is null)
        {
            throw new InvalidOperationException("Urun bulunamadi.");
        }

        // Once hareket kaydi eklenir, sonra urunun mevcut stogu artirilir.
        var yeniKayitId = _stokGirisDal.Add(stokGiris);
        var yeniStokMiktari = urun.StokMiktari + stokGiris.Miktar;
        _urunDal.StokMiktariGuncelle(stokGiris.UrunId, yeniStokMiktari);

        return yeniKayitId;
    }

    /// <summary>
    /// Id kontrolu yapildiktan sonra stok giris kaydini siler.
    /// </summary>
    public void Delete(int id)
    {
        IdKontrol(id, "Gecerli bir stok giris kaydi secilmelidir.");
        _stokGirisDal.Delete(id);
    }

    /// <summary>
    /// Eski UI iskeleti ile uyumluluk icin tutulur.
    /// </summary>
    public void Hazirla(StokGiris stokGiris)
    {
        _stokGirisDal.Hazirla(stokGiris);
    }

    private static void StokGirisDogrula(StokGiris stokGiris)
    {
        if (stokGiris.UrunId <= 0)
        {
            throw new ArgumentException("Urun secilmeden stok girisi yapilamaz.");
        }

        if (stokGiris.KullaniciId <= 0)
        {
            throw new ArgumentException("Kullanici secilmeden stok girisi yapilamaz.");
        }

        if (stokGiris.Miktar <= 0)
        {
            throw new ArgumentException("Stok giris miktari 0 veya negatif olamaz.");
        }
    }

    private static void IdKontrol(int id, string mesaj)
    {
        if (id <= 0)
        {
            throw new ArgumentException(mesaj);
        }
    }
}
