using StokTakip.DataAccess;
using StokTakip.Entities;

namespace StokTakip.Business;

/// <summary>
/// Stok giris surecinin is kurallarini yoneten Business sinifidir.
/// </summary>
public class StokGirisManager
{
    private readonly StokGirisDal _stokGirisDal = new();

    /// <summary>
    /// Ileride giris miktari ve urun secimi kontrolleri burada yapilacak.
    /// </summary>
    public void Hazirla(StokGiris stokGiris)
    {
        // Uygun bulunan stok giris islemi DAL katmanina aktarilir.
        _stokGirisDal.Hazirla(stokGiris);
    }
}
