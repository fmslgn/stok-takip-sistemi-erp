using StokTakip.DataAccess;
using StokTakip.Entities;

namespace StokTakip.Business;

/// <summary>
/// Kullanici islemlerinde UI ile DataAccess katmani arasindaki is kurallarini yonetir.
/// </summary>
public class KullaniciManager
{
    private readonly KullaniciDal _kullaniciDal = new();

    /// <summary>
    /// Tum kullanicilari listeler.
    /// </summary>
    public List<Kullanici> GetAll()
    {
        return _kullaniciDal.GetAll();
    }

    /// <summary>
    /// Id bilgisine gore kullanici getirir.
    /// </summary>
    public Kullanici? GetById(int id)
    {
        IdKontrol(id);
        return _kullaniciDal.GetById(id);
    }

    /// <summary>
    /// Kullanici bilgilerini kontrol edip yeni kayit ekler.
    /// </summary>
    public int Add(Kullanici kullanici)
    {
        KullaniciDogrula(kullanici);
        return _kullaniciDal.Add(kullanici);
    }

    /// <summary>
    /// Kullanici bilgilerini kontrol edip mevcut kaydi gunceller.
    /// </summary>
    public void Update(Kullanici kullanici)
    {
        IdKontrol(kullanici.Id);
        KullaniciDogrula(kullanici);
        _kullaniciDal.Update(kullanici);
    }

    /// <summary>
    /// Id kontrolu yapildiktan sonra kullanici kaydini siler.
    /// </summary>
    public void Delete(int id)
    {
        IdKontrol(id);
        _kullaniciDal.Delete(id);
    }

    /// <summary>
    /// Kullanici adi ve sifre ile aktif kullanici girisini kontrol eder.
    /// </summary>
    public Kullanici? LoginKontrol(string kullaniciAdi, string sifre)
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi))
        {
            throw new ArgumentException("Kullanici adi bos birakilamaz.");
        }

        if (string.IsNullOrWhiteSpace(sifre))
        {
            throw new ArgumentException("Sifre bos birakilamaz.");
        }

        // DAL tarafindaki sorgu sadece aktif kullanicilari getirir.
        return _kullaniciDal.LoginKontrol(kullaniciAdi.Trim(), sifre);
    }

    /// <summary>
    /// Eski UI iskeleti ile uyumluluk icin tutulur.
    /// </summary>
    public void Hazirla(Kullanici kullanici)
    {
        _kullaniciDal.Hazirla(kullanici);
    }

    private static void KullaniciDogrula(Kullanici kullanici)
    {
        if (string.IsNullOrWhiteSpace(kullanici.KullaniciAdi))
        {
            throw new ArgumentException("Kullanici adi bos birakilamaz.");
        }

        if (string.IsNullOrWhiteSpace(kullanici.Sifre))
        {
            throw new ArgumentException("Sifre bos birakilamaz.");
        }

        if (string.IsNullOrWhiteSpace(kullanici.Rol))
        {
            throw new ArgumentException("Rol bos birakilamaz.");
        }
    }

    private static void IdKontrol(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Gecerli bir kullanici secilmelidir.");
        }
    }
}
