using StokTakip.Entities;

namespace StokTakip.DataAccess;

/// <summary>
/// Raporlama verileri ile ilgili veritabani islemlerinin yazilacagi DAL sinifidir.
/// </summary>
public class RaporDal
{
    /// <summary>
    /// Ileride stok, giris-cikis ve kritik stok raporlari burada sorgulanacak.
    /// </summary>
    public void Hazirla(Rapor rapor)
    {
        // Rapor ekranlari SQL'e dogrudan erismeden bu sinifi kullanacak.
    }
}
