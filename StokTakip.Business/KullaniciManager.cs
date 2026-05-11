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
    /// Ileride kullanici dogrulama kurallari burada uygulanacak.
    /// </summary>
    public void Hazirla(Kullanici kullanici)
    {
        // WinForms katmani sadece Manager sinifini cagirir; SQL islemleri DAL katmaninda kalir.
        _kullaniciDal.Hazirla(kullanici);
    }
}
