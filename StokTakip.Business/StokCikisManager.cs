using StokTakip.DataAccess;
using StokTakip.Entities;

namespace StokTakip.Business;

/// <summary>
/// Stok cikis surecinin is kurallarini yoneten Business sinifidir.
/// </summary>
public class StokCikisManager
{
    private readonly StokCikisDal _stokCikisDal = new();
    private readonly UrunDal _urunDal = new();

    /// <summary>
    /// Tum stok cikis hareketlerini listeler.
    /// </summary>
    public List<StokCikis> GetAll()
    {
        return _stokCikisDal.GetAll();
    }

    /// <summary>
    /// Id bilgisine gore stok cikis hareketi getirir.
    /// </summary>
    public StokCikis? GetById(int id)
    {
        IdKontrol(id, "Gecerli bir stok cikis kaydi secilmelidir.");
        return _stokCikisDal.GetById(id);
    }

    /// <summary>
    /// Stok cikis kaydini ekler ve yeterli stok varsa urunun stok miktarini azaltir.
    /// </summary>
    public int Add(StokCikis stokCikis)
    {
        StokCikisDogrula(stokCikis);

        var urun = _urunDal.GetById(stokCikis.UrunId);
        if (urun is null)
        {
            throw new InvalidOperationException("Urun bulunamadi.");
        }

        if (stokCikis.Miktar > urun.StokMiktari)
        {
            throw new InvalidOperationException("Stok cikis miktari mevcut stoktan fazla olamaz.");
        }

        // Once hareket kaydi eklenir, sonra urunun mevcut stogu azaltilir.
        var yeniKayitId = _stokCikisDal.Add(stokCikis);
        var yeniStokMiktari = urun.StokMiktari - stokCikis.Miktar;
        _urunDal.StokMiktariGuncelle(stokCikis.UrunId, yeniStokMiktari);

        return yeniKayitId;
    }

    /// <summary>
    /// Id kontrolu yapildiktan sonra stok cikis kaydini siler.
    /// </summary>
    public void Delete(int id)
    {
        IdKontrol(id, "Gecerli bir stok cikis kaydi secilmelidir.");
        _stokCikisDal.Delete(id);
    }

    /// <summary>
    /// Eski UI iskeleti ile uyumluluk icin tutulur.
    /// </summary>
    public void Hazirla(StokCikis stokCikis)
    {
        _stokCikisDal.Hazirla(stokCikis);
    }

    private static void StokCikisDogrula(StokCikis stokCikis)
    {
        if (stokCikis.UrunId <= 0)
        {
            throw new ArgumentException("Urun secilmeden stok cikisi yapilamaz.");
        }

        if (stokCikis.KullaniciId <= 0)
        {
            throw new ArgumentException("Kullanici secilmeden stok cikisi yapilamaz.");
        }

        if (stokCikis.Miktar <= 0)
        {
            throw new ArgumentException("Stok cikis miktari 0 veya negatif olamaz.");
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
