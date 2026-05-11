using StokTakip.DataAccess;
using StokTakip.Entities;

namespace StokTakip.Business;

/// <summary>
/// Kategori islemlerindeki is kurallarini yoneten Business sinifidir.
/// </summary>
public class KategoriManager
{
    private readonly KategoriDal _kategoriDal = new();

    /// <summary>
    /// Ileride kategori kaydi icin zorunlu alan kontrolleri burada yapilacak.
    /// </summary>
    public void Hazirla(Kategori kategori)
    {
        // Is kurallari tamamlandiktan sonra veri islemi DataAccess katmanina aktarilir.
        _kategoriDal.Hazirla(kategori);
    }
}
