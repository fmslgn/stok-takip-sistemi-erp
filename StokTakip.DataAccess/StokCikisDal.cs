using StokTakip.Entities;

namespace StokTakip.DataAccess;

/// <summary>
/// Stok cikis hareketleri ile ilgili veritabani islemlerinin yazilacagi DAL sinifidir.
/// </summary>
public class StokCikisDal
{
    /// <summary>
    /// Ileride urun stok miktarini azaltan cikis kayitlari burada yonetilecek.
    /// </summary>
    public void Hazirla(StokCikis stokCikis)
    {
        // Stok yetersizligi kontrolleri Business katmanindan sonra burada veriye uygulanacak.
    }
}
