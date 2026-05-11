using StokTakip.DataAccess;
using StokTakip.Entities;

namespace StokTakip.Business;

/// <summary>
/// Saklama kosullari ve kural tabanli akilli oneri ekraninin is kurallarini yonetir.
/// </summary>
public class SaklamaKosuluManager
{
    private readonly SaklamaKosuluDal _saklamaKosuluDal = new();

    /// <summary>
    /// Tum saklama kosullarini listeler.
    /// </summary>
    public List<SaklamaKosulu> GetAll()
    {
        return _saklamaKosuluDal.GetAll();
    }

    /// <summary>
    /// Id bilgisine gore saklama kosulu getirir.
    /// </summary>
    public SaklamaKosulu? GetById(int id)
    {
        IdKontrol(id);
        return _saklamaKosuluDal.GetById(id);
    }

    /// <summary>
    /// Saklama kosulu bilgilerini kontrol edip yeni kayit ekler.
    /// </summary>
    public int Add(SaklamaKosulu saklamaKosulu)
    {
        SaklamaKosuluDogrula(saklamaKosulu);
        return _saklamaKosuluDal.Add(saklamaKosulu);
    }

    /// <summary>
    /// Saklama kosulu bilgilerini kontrol edip mevcut kaydi gunceller.
    /// </summary>
    public void Update(SaklamaKosulu saklamaKosulu)
    {
        IdKontrol(saklamaKosulu.Id);
        SaklamaKosuluDogrula(saklamaKosulu);
        _saklamaKosuluDal.Update(saklamaKosulu);
    }

    /// <summary>
    /// Id kontrolu yapildiktan sonra saklama kosulu kaydini siler.
    /// </summary>
    public void Delete(int id)
    {
        IdKontrol(id);
        _saklamaKosuluDal.Delete(id);
    }

    /// <summary>
    /// Urun adi ve kategoriye gore kural tabanli saklama onerisi uretir.
    /// </summary>
    public string SaklamaKosuluOner(string urunAdi, string kategoriAdi)
    {
        var urun = (urunAdi ?? string.Empty).Trim().ToLowerInvariant();
        var kategori = (kategoriAdi ?? string.Empty).Trim().ToLowerInvariant();
        var metin = $"{urun} {kategori}";

        // Sut ve benzeri hassas gidalarda soguk zincir onerilir.
        if (Icerir(metin, "sut", "yogurt", "peynir"))
        {
            return "Soguk zincir onerilir. Urun 0-4 C araliginda, buzdolabi kosullarinda saklanmalidir.";
        }

        if (Icerir(metin, "et", "tavuk", "balik"))
        {
            return "Dondurucu veya soguk zincir onerilir. Urun cozundurulmeden ve hijyenik kosullarda saklanmalidir.";
        }

        if (Icerir(metin, "kuru gida", "gida", "biskuvi", "un", "makarna", "pirinc"))
        {
            return "Serin, kuru ve gunes almayan ortam onerilir. Ambalaj kapali tutulmalidir.";
        }

        if (Icerir(metin, "temizlik", "deterjan", "camasir", "kimyasal"))
        {
            return "Cocuklardan uzak, serin ve kuru ortam onerilir. Gida urunleriyle ayni alanda saklanmamalidir.";
        }

        if (Icerir(metin, "elektronik", "sarj", "telefon", "kablo"))
        {
            return "Nemden, sudan ve yuksek sicakliktan uzak ortam onerilir.";
        }

        if (Icerir(metin, "kirtasiye", "kalem", "kagit", "defter"))
        {
            return "Kuru ve temiz ortam onerilir. Urunler duzenli raf sisteminde saklanmalidir.";
        }

        return "Genel saklama onerisi: Urun kuru, temiz, havalandirilmis ve dogrudan gunes almayan bir ortamda saklanmalidir.";
    }

    /// <summary>
    /// Eski UI iskeleti ile uyumluluk icin tutulur.
    /// </summary>
    public void Hazirla(SaklamaKosulu saklamaKosulu)
    {
        _saklamaKosuluDal.Hazirla(saklamaKosulu);
    }

    private static void SaklamaKosuluDogrula(SaklamaKosulu saklamaKosulu)
    {
        if (string.IsNullOrWhiteSpace(saklamaKosulu.KosulAdi))
        {
            throw new ArgumentException("Saklama kosulu adi bos birakilamaz.");
        }

        if (string.IsNullOrWhiteSpace(saklamaKosulu.KategoriAnahtarKelime))
        {
            throw new ArgumentException("Kategori anahtar kelimesi bos birakilamaz.");
        }

        if (string.IsNullOrWhiteSpace(saklamaKosulu.OnerilenSicaklik))
        {
            throw new ArgumentException("Onerilen sicaklik bos birakilamaz.");
        }

        if (string.IsNullOrWhiteSpace(saklamaKosulu.NemOrani))
        {
            throw new ArgumentException("Nem orani bos birakilamaz.");
        }
    }

    private static bool Icerir(string metin, params string[] anahtarKelimeler)
    {
        return anahtarKelimeler.Any(metin.Contains);
    }

    private static void IdKontrol(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Gecerli bir saklama kosulu secilmelidir.");
        }
    }
}
