using StokTakip.DataAccess;
using StokTakip.Entities;

namespace StokTakip.Business;

/// <summary>
/// Saklama kosullari ve AI destekli oneri ekraninin is kurallarini yonetir.
/// </summary>
public class SaklamaKosuluManager
{
    private readonly SaklamaKosuluDal _saklamaKosuluDal = new();

    /// <summary>
    /// Ileride saklama kosulu bilgilerinin dogrulamalari burada yapilacak.
    /// </summary>
    public void Hazirla(SaklamaKosulu saklamaKosulu)
    {
        // Saklama kosulu verisi DataAccess katmani uzerinden veritabanina tasinir.
        _saklamaKosuluDal.Hazirla(saklamaKosulu);
    }
}
