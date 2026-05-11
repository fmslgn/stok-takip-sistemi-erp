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
    /// Ileride urun adi, barkod ve kritik stok seviyesi kontrolleri burada yapilacak.
    /// </summary>
    public void Hazirla(Urun urun)
    {
        // UI katmani urun islemleri icin bu sinifi kullanir.
        _urunDal.Hazirla(urun);
    }
}
