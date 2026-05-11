using StokTakip.DataAccess;
using StokTakip.Entities;

namespace StokTakip.Business;

/// <summary>
/// Raporlama ekranlarinin ihtiyac duydugu is kurallarini yoneten Business sinifidir.
/// </summary>
public class RaporManager
{
    private readonly RaporDal _raporDal = new();

    /// <summary>
    /// Ileride rapor turu, tarih araligi ve filtre kontrolleri burada yapilacak.
    /// </summary>
    public void Hazirla(Rapor rapor)
    {
        // Rapor icin gereken veriler DAL katmanindan alinacak.
        _raporDal.Hazirla(rapor);
    }
}
