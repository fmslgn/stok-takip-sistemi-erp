using StokTakip.Entities;

namespace StokTakip.DataAccess;

/// <summary>
/// Stok giris hareketleri ile ilgili veritabani islemlerinin yazilacagi DAL sinifidir.
/// </summary>
public class StokGirisDal
{
    /// <summary>
    /// Ileride urun stok miktarini artiran giris kayitlari burada yonetilecek.
    /// </summary>
    public void Hazirla(StokGiris stokGiris)
    {
        // Transaction gerektiren stok giris islemleri bu katmanda yazilacak.
    }
}
