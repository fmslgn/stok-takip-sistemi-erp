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
    /// Tum kategorileri listeler.
    /// </summary>
    public List<Kategori> GetAll()
    {
        return _kategoriDal.GetAll();
    }

    /// <summary>
    /// Id bilgisine gore kategori getirir.
    /// </summary>
    public Kategori? GetById(int id)
    {
        IdKontrol(id);
        return _kategoriDal.GetById(id);
    }

    /// <summary>
    /// Kategori adini kontrol edip yeni kategori ekler.
    /// </summary>
    public int Add(Kategori kategori)
    {
        KategoriDogrula(kategori);
        return _kategoriDal.Add(kategori);
    }

    /// <summary>
    /// Kategori adini kontrol edip mevcut kategori kaydini gunceller.
    /// </summary>
    public void Update(Kategori kategori)
    {
        IdKontrol(kategori.Id);
        KategoriDogrula(kategori);
        _kategoriDal.Update(kategori);
    }

    /// <summary>
    /// Id kontrolu yapildiktan sonra kategori kaydini siler.
    /// </summary>
    public void Delete(int id)
    {
        IdKontrol(id);
        _kategoriDal.Delete(id);
    }

    /// <summary>
    /// Eski UI iskeleti ile uyumluluk icin tutulur.
    /// </summary>
    public void Hazirla(Kategori kategori)
    {
        _kategoriDal.Hazirla(kategori);
    }

    private static void KategoriDogrula(Kategori kategori)
    {
        if (string.IsNullOrWhiteSpace(kategori.KategoriAdi))
        {
            throw new ArgumentException("Kategori adi bos birakilamaz.");
        }
    }

    private static void IdKontrol(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Gecerli bir kategori secilmelidir.");
        }
    }
}
