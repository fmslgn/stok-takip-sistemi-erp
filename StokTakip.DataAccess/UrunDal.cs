using StokTakip.Entities;

namespace StokTakip.DataAccess;

/// <summary>
/// Urun tablosu ile ilgili veritabani islemlerinin yazilacagi DAL sinifidir.
/// </summary>
public class UrunDal
{
    /// <summary>
    /// Ileride urun yonetimi ve kritik stok sorgulari burada yazilacak.
    /// </summary>
    public void Hazirla(Urun urun)
    {
        // UI katmani SQL yazmayacagi icin urun sorgulari bu sinifta toplanacak.
    }
}
